(function() {
    'use strict';

    var serviceId = 'controller.abstract';
    angular.module('app').factory(serviceId,
        ['common', '$window', 'commonConfig', 'breeze', 'datacontext',AbstractRepository]);

    function AbstractRepository(common, $window, commonConfig, breeze, datacontext) {
        var confirmDiscardMsg = 'Are you sure you want to discard changes without saving?';

        //var provider = entityManagerFactory.manager;

        function Ctor(argObj /* controllerId, $scope, watchedEntityNames*/) {
            var vm = this;
            var hasAddedEntityPropertyChanged = false;

            var $on = argObj.$scope.$on.bind(argObj.$scope);
            var manager = datacontext.provider;
            var unwatchers = [$on('$destroy', removeListeners)];
            var breezeWatcher;
            var errorEntities = [];
            var isSaving = false;

            var watchedEntityNames;
            if (argObj.watchedEntityNames) {
                watchedEntityNames = Array.isArray(argObj.watchedEntityNames)
                    ? argObj.watchedEntityNames
                    : argObj.watchedEntityNames.split(',');//assuming string if not array

                for (var i = 0; i < watchedEntityNames.length; i++) {
                    watchedEntityNames[i] = watchedEntityNames[i].split('.');
                }
            }

            unwatchers.push($on('$routeChangeStart', beforeRouteChange));
            if (argObj.$scope.$hide) {
                vm.close = modalClose;
            }

            if (argObj.$watchers) {unwatchers = unwatchers.concat(argObj.$watchers);}
            $window.addEventListener("beforeunload", beforeUnload);

            vm.log = common.logger.getLogFn(argObj.controllerId);
            vm.disableSave = disableSave;
            vm.notifyViewModelLoaded = notifyViewModelLoaded;
            vm.save = save;

            var _watched;
            function getWatched(force) {
                if (!watchedEntityNames) {
                    return manager.getEntities();
                }
                if (!_watched || force) {
                    //var oldWatched = _watched ? _watched.slice() : []; // DEBUG
                    _watched = (_watched || []).filter(filterDeletedEnts); //keep a hold of these
                    watchedEntityNames.forEach(function (wen) {
                        var ent = vm[wen[0]];
                        var currentLevel;
                        if (!ent) { return; }
                        if (!Array.isArray(ent)) {
                            ent = [ent];
                        }

                        for (var i = 1; i < wen.length; i++) {
                            currentLevel = [];
                            ent.forEach(function (el) {
                                var child = el[wen[i]];
                                if (child === undefined) {
                                    throw new Error('watched entity child is undefined - ' + wen[i] + ' (' + wen.join(',') + ') entity ' +
                                        el.entityType.shortName + ' [available options =(' + el.entityType.navigationProperties.map(function (dp) { return dp.name }).join(',') + ')]');
                                }
                                if (Array.isArray(child)) {
                                    currentLevel = currentLevel.concat(child);
                                } else {
                                    currentLevel.push(child);
                                }
                            });
                            ent = currentLevel;
                        }

                        _watched = _watched.concat(ent.filter(filterEnts));
                    });

                    //var dif = _watched.filter(function (el) { return oldWatched.indexOf(el) === -1; })
                    //vm.log.debug({ msg: "+" + dif.length + " to watch", data: dif }) //DEBUG
                }
                return _watched;
            }

            function filterEnts(ent) {
                return !!(ent && ent.entityAspect);
            }

            function filterDeletedEnts(ent) {
                return !!(ent && ent.entityAspect && ent.entityAspect.entityState === breeze.EntityState.Deleted);
            }

            function filterHasValidationErrors(el) { return el.entityAspect.hasValidationErrors && el.entityAspect.entityState !== breeze.EntityState.Deleted; }

            function notifyViewModelLoaded() {
                var watched = getWatched(true);
                vm.isEntityStateChanged = watched.some(isUserChanged);
                errorEntities = watched.filter(filterHasValidationErrors);
                //binding the watcher here as no point binding earlier - propertychanged will fire for every property as the entity is being hydrated
                if (!breezeWatcher) {
                    breezeWatcher = manager.entityChanged.subscribe(entityChanged);
                }
            }

            function entityChanged(changeArgs) {
                var ent = changeArgs.entity;
                //note, when creating entities, this will be called before the entity has been assigned to the viewmodel property.
                //this can probaby be used to advantage
                //console.log(changeArgs.entityAction.name + '\t-\t' + ent.entityType.shortName + '\t-\t' + ent.entityAspect.entityState.name);
                switch (changeArgs.entityAction) {
                    case breeze.EntityAction.EntityStateChange:
                        if (getWatched().indexOf(ent) !== -1) {
                            if (!vm.isEntityStateChanged) {
                                vm.isEntityStateChanged = isUserChanged(ent);
                            }
                            if (ent.entityAspect.entityState === breeze.EntityState.Deleted) {
                                var indx = errorEntities.indexOf(ent);
                                if (indx !== -1) {
                                    errorEntities.splice(indx, 1);
                                }
                            }
                        }
                        break;
                    case breeze.EntityAction.PropertyChange:
                        if (changeArgs.args.property.isNavigationProperty) {
                            notifyViewModelLoaded();
                        }
                        if (getWatched().indexOf(ent) === -1) {
                            return;
                        }
                        var indx = errorEntities.indexOf(ent);
                        if (ent.entityAspect.entityState !== breeze.EntityState.Deleted  && ent.entityAspect.hasValidationErrors) { //?detached
                            if (indx === -1) {
                                errorEntities.push(ent);
                            }
   
                        } else {
                            if (indx !== -1) {
                                errorEntities.splice(indx, 1);
                            }
                        }
                        if (!vm.isEntityStateChanged) {
                            vm.isEntityStateChanged = isUserChanged(ent);
                        }
                        break;
                    //case breeze.EntityAction.MergeOnSave: //TODO check this works - we should be adding to watched collection on add, not on mergeOnSave
                    case breeze.EntityAction.Attach:
                    case breeze.EntityAction.Detach:
                        notifyViewModelLoaded();
                        break;
                    //default:
                        //console.log(changeArgs.entityAction.name + '\t-\t' + ent.entityType.shortName + '\t-\t' + ent.entityAspect.entityState.name);
                }
            }

            function isUserChanged(ent) {
                switch (ent.entityAspect.entityState) {
                    case breeze.EntityState.Modified:
                    case breeze.EntityState.Deleted:
                        return true;
                    case breeze.EntityState.Added:
                        return !ent.entityType.dataProperties.every(function (dp) {
                            //if (dp.isPartOfKey) {return true;}
                            var propVal = ent[dp.name];
                            return propVal === dp.defaultValue || propVal === ent.entityAspect.instantiationValues[dp.name];
                        });
                    default:
                        return false;
                }
            }

            function beforeUnload(e){
                if (vm.isEntityStateChanged) {
                    e.returnValue = confirmDiscardMsg; // Gecko, Trident, Chrome 34+
                    return confirmDiscardMsg;          // Gecko, WebKit, Chrome <34
                }
            }

            function beforeRouteChange(e) {
                vm.log.debug("changed entity count: " + manager.getChanges().length);
                if (!e.defaultPrevented) {
                    if (vm.isEntityStateChanged && !confirm(confirmDiscardMsg)) {
                        e.preventDefault();
                        common.$broadcast(commonConfig.config.controllerActivateSuccessEvent); //switch the spinner off
                    } else {
                        removeListeners({});//remove before performing next step

                        getWatched().forEach(function (ent) {
                            if (ent.entityAspect.entityState.isAddedModifiedOrDeleted()) {
                                ent.entityAspect.rejectChanges();
                            }
                        });
                    }
                }
            }

            function removeListeners(e) {
                if (unwatchers && !e.defaultPrevented) {
                    $window.removeEventListener("beforeunload", beforeUnload);
                    unwatchers.forEach(function (unwatch) { unwatch(); });
                    if (breezeWatcher) {
                        manager.entityChanged.unsubscribe(breezeWatcher);
                        breezeWatcher = null;
                    }
                    unwatchers = null;
                }
            }
            
            function disableSave() {
                return isSaving || errorEntities.length || !vm.isEntityStateChanged;
            }

            function save() {
                isSaving = true;
                var toSave;
                if (arguments.length) {
                    toSave = [];
                    for (var i = 0; i < arguments.length; i++) {
                        if (Array.isArray(arguments[i])) {
                            toSave = toSave.concat(arguments[i]);
                        } else {
                            toSave.push(arguments[i]);
                        }
                    }
                } else {
                    toSave = watchedEntityNames
                        ?getWatched()
                        :[];
                }
                toSave = toSave.filter(function (el) { return !!(el && el.entityAspect && el.entityAspect.entityState.isAddedModifiedOrDeleted()); })
                return datacontext.save.apply(null, toSave).then(function () {
                    vm.isEntityStateChanged = false;
                }).finally(function() {
                    isSaving = false;
                });
            }

            

            function modalClose() {
                var evtArg = {
                    defaultPrevented: false,
                    preventDefault: function () {
                        this.defaultPrevented = true;
                    }
                };
                beforeRouteChange(evtArg);
                if (!evtArg.defaultPrevented) {
                    argObj.$scope.$hide();
                    removeListeners(evtArg);
                }
            }
        }

        //no point instantiating above (as true factory method) as will only extend other methods
        Ctor /* .prototype */ = { 
            constructor: Ctor,
        };

        return Ctor;

        /* Implementation */
        /*
        function setIsPartialTrue(entities) {
            // call for all "partial queries"
            for (var i = entities.length; i--;) { entities[i].isPartial = true; }
            return entities;
        }
        */
    }
})();
﻿(function () {
    'use strict';
    var controllerId = 'courseType';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$scope', 'breeze', '$aside'];

    function controller(abstractController, $routeParams, common, datacontext, $scope, breeze, $aside) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: ['courseType', 'courseType.courseFormats', 'courseType.courseFormats.courseSlots', 'courseType.courseFormats.courseSlots.activity', 'courseType.courseTypeDepartments'],
            $scope: $scope,
            $watchers: [$scope.$on('$destroy',removeSelectedSlots)]
        })
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.activeFormatIndex = -1;
        vm.activitySelected = activitySelected;
        vm.clone = clone;
        vm.courseType = {};
        vm.createSlot = createSlot;
        vm.createNewFormat = createNewFormat;
        vm.deleteFormat = deleteFormat;
        vm.departments = [];
        vm.editSlot = editSlot;
        vm.editChoices = editChoices;
        vm.emersionCategories = common.getEnumValues().emersion;
        vm.getCourseActivityNames = getCourseActivityNames;
        vm.instructorCourses = [];
        vm.isScenarioChanged = isScenarioChanged;
        vm.removeSlot = removeSlot;

        vm.selectedDepartments = [];
        vm.title = 'Course Format';

        vm.sortableOptions = {
            handle: '.handle',
            stop: function (e, ui) {
                // this callback has the changed model
                vm.courseFormat.courseSlots.forEach(function (el,indx) {
                    if (el.order !== indx) {
                        el.order = indx;
                    }
                });
            }
        };

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var departments;
                var promises = [
                    datacontext.courseTypes.find({where: breeze.Predicate.create('instructorCourseId','==',null).and('id','!=',id)}).then(function (data) {
                        vm.instructorCourses = data;
                    }),
                    datacontext.institutions.all({expand:'departments'}).then(function(data){
                        departments = data;
                    })
                ];
                if (isNew) {
                    vm.courseType = datacontext.courseTypes.create();
                    datacontext.courseFormats.create({ courseType: vm.courseType });
                    vm.notifyViewModelLoaded();
                } else {
                    //promises.push(datacontext.courseTypes.fetchByKey(id, { expand: 'courseFormats.courseSlots' }).then(function (data) { - if the courseFormats were not already loaded from the server
                    promises.push(datacontext.courseTypes.fetchByKey(id,
                    {
                        expand: ["courseFormats.courseSlots.activity", "courseTypeDepartments"]
                    }).then(function (data) {
                        vm.courseType = data;
                        vm.courseType.courseFormats.forEach(function (el) {
                            el.courseSlots.sort(common.sortOnPropertyName('order'));
                        });
                        vm.activeFormatIndex = vm.courseType.courseFormats.findIndex(function (el) {
                            return el.id === $routeParams.formatId;
                        });
                    }));
                }
                common.activateController(promises, controllerId)
                    .then(function () {
                        var ctds = vm.courseType.courseTypeDepartments;
                        departments.forEach(function (el) {
                            vm.departments.push({ name: '<strong>' + el.name + '</strong>', dptGroup: true });
                            el.departments.forEach(iterateDpt);
                            vm.departments.push({ dptGroup: false });
                        });
                        function iterateDpt(d) {
                            var checked = ctds.some(function(c){return c.departmentId === d.id;});
                            vm.departments.push({ name: d.name, abbreviation: d.abbreviation, dptId:d.id, checked:checked });
                        }
                        $scope.$watchCollection(function () { return vm.selectedDepartments }, common.manageCollectionChange(datacontext.courseTypeDepartments, 'dptId',
                            function (member) {
                                return {
                                    departmentId: member.dptId,
                                    courseTypeId: vm.courseType.id
                                };
                            }));
                        if (vm.courseType.courseFormats.length === 1) {
                            vm.activeFormatIndex = 0;
                        }
                        vm.log('Activated Course Format View');
                        vm.notifyViewModelLoaded();
                    });
            });
        }

        //logic is - can't delete slots after course has been run, as they may have associated tables such as participants
        //-altering the name of a slot - don't allow reassignment of activity, as course participants etc will be all mucked up
        //-only show typeahead if a new slot (otherwise simple input)

        //-if a new slot/activity and activity selected, delete new activity, replace with selected
        function activitySelected(activityName, slot) {
            removeActivity(slot);
            slot.activity = vm.courseType.courseActivities.find(function (el) {
                return el.name === activityName;
            });
            reinstateInactive(slot.courseFormat);
        }

        function createActivity(slot) {
            slot.activity = datacontext.courseActivities.create({
                courseTypeId: vm.courseType.id
            });
        }

        function removeSelectedSlots() {
            if (vm.courseType) {
                vm.courseType.courseFormats.forEach(function (el) {
                    delete el.selectedSlot;
                });
            }
        }

        function removeActivity(slot) {
            var ca = slot.activity
            if (ca && ca.entityAspect.entityState.isAdded()) {
                ca.entityAspect.setDeleted();
            }
            slot.activity = null;
        }

        function getCourseActivityNames(name) {
            name = name.toLowerCase();
            var returnVar =[];
            if (vm.courseType.courseActivities) {
                vm.courseType.courseActivities.forEach(function (el) {
                    if (el.name.toLowerCase().indexOf(name) !== -1) {
                        returnVar.push(el.name);
                }
            });
            }
            return returnVar;
        }
        
        /*
        function saveOverride() {
            //vm.log.debug($event);
            baseSave().then(removeSelectedSlots);
        }//;
        */

        function createSlot(courseFormat) {
            courseFormat.selectedSlot = datacontext.courseSlots.create({
                courseFormatId: courseFormat.id,
                order: (courseFormat.courseSlots || []).length
            });
            createActivity(courseFormat.selectedSlot);
        }

        function removeSlot(courseSlot) {
            removeActivity(courseSlot);
            delete courseSlot.courseFormat.selectedSlot;
            if (courseSlot.entityAspect.entityState.isAdded()) {
                courseSlot.entityAspect.setDeleted();
            } else {
                courseSlot.isActive = false;
            }
        }

        function editChoices(slot) {
            var modal = getModalInstance();
            modal.$scope.courseActivity = slot.activity;
            modal.$promise.then(modal.show);
        }

        function editSlot(courseSlot) {
            courseSlot.courseFormat.selectedSlot = courseSlot;
            courseSlot.isScenario = !courseSlot.activity;
        }

        function isScenarioChanged(slot) {
            if (slot.isScenario) {
                removeActivity(slot);
                reinstateInactive(slot.courseFormat);
            } else {
                createActivity(slot);
            }
        }

        function reinstateInactive(courseFormat) {
            var selectedSlot = courseFormat.selectedSlot;
            if (!selectedSlot.entityAspect.entityState.isAdded()) {
                return;
            }
            var emptyScenarioSlot = courseFormat.courseSlots.some(function (el) {
                return !el.isActive && el !== selectedSlot && el.activity === selectedSlot.activity
            });
            if (emptyScenarioSlot) { 
                selectedSlot.setDeleted();
                courseFormat.selectedSlot = emptyScenarioSlot;
                courseFormat.selectedSlot.isActive = true;
            }
        }

        var _modalInstance;
        function getModalInstance() {
            if (!_modalInstance) {
                var scope = $scope.$new();
                _modalInstance = $aside({
                        templateUrl: 'app/activityResources/activityResource.html',
                        controller: 'activityResource',
                    show: false,
                    id: 'cpModal',
                    placement: 'left',
                        animation: 'am-slide-left',
                    scope: scope,
                    controllerAs: 'ar'
                });
            }
            return _modalInstance;
        }

        function clone(cf) {
            var newFormat = datacontext.cloneItem(cf, ['courseSlots']);
            newFormat.description += " - copy";
            vm.activeFormatIndex = vm.courseType.courseFormats.length -1;
        }

        function createNewFormat() {
            datacontext.courseFormats.create({ courseTypeId: vm.courseType.id });
            vm.activeFormatIndex = vm.courseType.courseFormats.length - 1;
        }

        function deleteFormat(cf) {
            cf.courseSlots.forEach(function (el) {
                el.entityAspect.setDeleted();
            });
            cf.entityAspect.setDeleted();
        }
    }
})();

﻿(function () {
    'use strict';
    var controllerId = 'dashboard';
    angular.module('app').controller(controllerId, ['common', 'errorhandler', 'datacontext','breeze', 'tokenStorageService','$rootScope','AUTH_EVENTS','$q','$location',dashboard]);

    function dashboard(common, errorhandler, datacontext, breeze, tokenStorageService, $rootScope, AUTH_EVENTS,$q,$location) {
        var log = common.logger.getLogFn(controllerId);
        var FilterQueryOp = breeze.FilterQueryOp;
        var vm = this;
        vm.currentUserId = null;
        vm.userCourses = [];
        vm.upcomingCourses = [];
        vm.userSummaryData = {};
        vm.title = 'Dashboard';
        vm.loginState = '';

        activate();

        function activate() {
            if (!tokenStorageService.isLoggedIn()) {
                $rootScope.$on(AUTH_EVENTS.loginConfirmed, updateDash);
            }

            common.activateController( getPromises(), controllerId) //notification of login after dashboard activated
                .then(function () {
                    log('Activated Dashboard View');
                });
        }

        function getPromises() {
            if (tokenStorageService.isLoggedIn()) {
                vm.currentUserId = tokenStorageService.getUserId();
                //todo uow.ready promise - nested
                return [getCourses() /*, getUserSummaryData(), getUserSummaryData */];
            }
            vm.loginState = 'notLoggedIn';
            vm.currentUserId = null;
            return [];
        }

        function getCourses() {
            var now = new Date();
            return $q.all([
                datacontext.ready(),
                datacontext.courses.find({
                    where: breeze.Predicate.create('startTime', '>', now),
                    orderBy: 'startTime',
                    take: 5,
                    expand: 'courseParticipants',
                    //select: 'id,startTime,courseParticipants,roomId'
                }).then(function (data) { vm.upcomingCourses = data; })])
           .then(function () {
                vm.upcomingCourses.forEach(function (course) {
                    course.includesCurrentUser = course.includesUser(vm.currentUserId);
                    if (!vm.nextUserCourse && course.includesCurrentUser) {
                        vm.nextUserCourse = course;
                    }
                });
                vm.loginState = (vm.nextUserCourse ? 'active' : 'inactive');
            });
        }

        function getUserSummaryData() {
            return datacontext.getUserCourses().then(function (data) {
                return vm.userSummaryData = data;
            });
        }

        function getUpcomingCourses() {
            return uow.staffingResourceListItems.find().then(function (data) {
                return vm.userCourses = data;
            });
        }

        function updateDash() {
            $q.all([getPromises()]).then(function (eventArgs) {
                log.info({ msg: 'Dashboard updated' });
            });
        }

    }
})();
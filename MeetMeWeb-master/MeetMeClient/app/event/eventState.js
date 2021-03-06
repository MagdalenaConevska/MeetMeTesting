﻿(function (angular) {
    'use strict';

    angular
      .module('meet-me')
      .config(registerState);


    registerState.$inject = ['$stateProvider'];

    function registerState($stateProvider) {

        $stateProvider.state('event', {
            url: '/event',
            authenticate: true,
            templateUrl: 'app/event/eventView.html',
            controller: 'EventController',
            controllerAs: 'vm'
        });
    }

})(angular);
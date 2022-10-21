(function () {
    'use strict';

    function dashboardController(
        $scope, $timeout, navigationService, notificationsService, YuzuDeliveryExamplesResources) {

        var vm = this;
        vm.loading = false;
        vm.dashboard = {};
        vm.running = [];
        vm.completed = [];
        vm.errors = [];

        vm.page = {
            title: 'Yuzu Delivery Examples'
        };

        vm.actionClass = function (name) {
            if (vm.running.includes(name)) return 'running';
            if (vm.completed.includes(name)) return 'completed';
            if (vm.errors.includes(name)) return 'error';
            return '';
        }

        vm.runAction = function (name) {
            if (!vm.running.includes(name)) {
                vm.running.push(name);
                YuzuDeliveryExamplesResources[name]()
                    .then(() => {
                        vm.running = vm.running.filter((item) => { item != name; })
                        if (!vm.completed.includes(name)) vm.completed.push(name);
                    }, () => {
                        vm.running = vm.running.filter((item) => { item != name; })
                        vm.errors.push(name);
                    })
            }
        }

        vm.loading = false;

    }

    function YuzuDeliveryExamplesResources($http, $timeout) {
        return {
            groups: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/CreateGroups');
            },
            global: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/CreateGlobal');
            },
            viewModels: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/GenerateViewModels');
            },
            docTypes: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/GenerateDocumentTypes');
            },
            templates: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/AddTemplates');
            },
            content: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryExamples/ExampleBuild/CreateContent');
            }
        };
    }

    angular.module('umbraco').controller('YuzuDeliveryExamplesDashboardController', dashboardController);
    angular.module("umbraco").factory('YuzuDeliveryExamplesResources', YuzuDeliveryExamplesResources);
})();
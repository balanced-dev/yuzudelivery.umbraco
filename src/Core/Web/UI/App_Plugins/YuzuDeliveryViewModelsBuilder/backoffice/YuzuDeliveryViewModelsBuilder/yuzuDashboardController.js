(function () {
    'use strict';

    function dashboardController(
        $scope, $timeout, navigationService, notificationsService, YuzuDeliveryCoreResources) {

        var vm = this;
        vm.loading = false;
        vm.dashboard = {};

        vm.page = {
            title: 'Yuzu Delivery Dashboards'
        };

        $timeout(function () {
            navigationService.syncTree({ tree: "YuzuDeliveryViewModelsBuilder", path: "-1" });
        });

        vm.generate = function () {
            vm.generating = true;
            YuzuDeliveryCoreResources.build()
                .then(function (response) {
                    vm.generating = false;
                    if (response.data) {
                        vm.dashboard.lastError = response.data;
                    }
                });
        };

        vm.refreshTemplates = function () {

            YuzuDeliveryCoreResources.refreshTemplates()
                .then(function (response) {

                    if (response.data === true) {
                        notificationsService.success("Templates Refreshed", "Happy Templating!");
                    } else {
                        notificationsService.error("Error", "Something went wrong resfreshing templates.");
                    }

                });
        };

        YuzuDeliveryCoreResources.dashboard()
            .then(function (response) {
                vm.enabled = response.data.Item1;
                vm.dashboard.text = response.data.Item2;
            });

        vm.loading = false;

    }

    function YuzuDeliveryCoreResources($http, $timeout) {
        return {
            build: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryViewModelsBuilder/Generate/Build');
            },
            refreshTemplates: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryViewModelsBuilder/Generate/RefreshTemplates');
            },
            dashboard: function () {
                return $http.get('/umbraco/backoffice/YuzuDeliveryViewModelsBuilder/Generate/GetDashboard');
            }
        };
    }

    angular.module('umbraco').controller('YuzuSettingsDashboardController', dashboardController);
    angular.module("umbraco").factory('YuzuDeliveryCoreResources', YuzuDeliveryCoreResources);
})();
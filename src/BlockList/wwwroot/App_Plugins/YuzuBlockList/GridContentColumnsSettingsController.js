angular.module('umbraco')
    .controller('GridContentColumnsSettings', function ($scope) {

        var store = $scope.$parent.$parent.$parent.$parent.$parent.vm.availableBlockTypes;

        $scope.$parent.$parent.$parent.$parent.$parent.vm.availableBlockTypes = [];

        $scope.$on("$destroy", function () {
            $scope.$parent.$parent.$parent.$parent.$parent.vm.availableBlockTypes = store;
        });

    });


angular.module('umbraco')
    .controller('GridContentSectionController', function ($scope, $rootScope) {

        $scope.getGridSettings = function (property) {
            if (property.alias === 'l50')
                return { "grid-column": "1 / span 6" };
            else if (property.alias === 'r50')
                return { "grid-column": "7 / span 6" };
            else
                return { "grid-column": "1 / span 12" };
        }

    });
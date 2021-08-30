angular.module('umbraco')
    .controller('GridContentItem', function ($scope, $element, $window, yuzuDeliveryBlockListResources) {

        var loadPreview = function () {

            yuzuDeliveryBlockListResources.getPreview($scope.block.data)
                .then(function (response) {
                    var data = response.data;
                    if (data.error) {
                        var errorContainer = $element.find('.preview-error');
                        errorContainer.html(data.error);
                    }
                    else if (data.preview) {
                        var previewContainer = $element.find('.preview-container');
                        previewContainer.html(data.preview);
                    }
                });
        }

        $scope.$watch('block.data', function (newValue, oldValue) {
            loadPreview();
        }, true);

        loadPreview();

    });


angular.module('umbraco.resources').factory('yuzuDeliveryBlockListResources',
    function ($q, $http, $routeParams, umbRequestHelper) {
        return {
            getPreview: function (blockData) {

                var url = "/umbraco/backoffice/YuzuDeliveryUmbracoImport/BlockListPreview/GetPartialData";
                var resultParameters = { content: JSON.stringify(blockData, false), contentTypeKey: blockData.contentTypeKey };

                return $http.post(url, resultParameters, {
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
                    transformRequest: function (result) {
                        return $.param(result);
                    }
                });
            }
        };
    }
);
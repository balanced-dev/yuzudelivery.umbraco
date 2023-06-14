angular.module('umbraco')
    .controller('GridContentItem', function ($scope, $element, $window, $http, yuzuDeliveryBlockListResources, editorState) {

        var vm = this;

        if (editorState.getCurrent().parentId > 0) {
             vm.nodeId = editorState.getCurrent().parentId;
        } else {
            vm.nodeId = editorState.getCurrent().id;
        }

        var loadPreview = function () {

            var colSettings = $scope.$parent.$parent.$parent.vm.parentBlock.settingsData;
            var rowSettings = $scope.$parent.$parent.$parent.$parent.$parent.$parent.vm.parentBlock.settingsData;

            yuzuDeliveryBlockListResources.getPreview(vm.nodeId, $scope.block.data, $scope.block.settingsData, colSettings, rowSettings)
                .then(function (response) {
                    var data = response.data;
                    if (data.error) {
                        var errorContainer = $element.find('.preview-error');
                        errorContainer.html(data.error);
                    }
                    else if (data.preview) {
                        //Add Markup
                        var previewContainer = $element.find('.preview-container');
                        previewContainer.html(data.preview);
                        
                        //Add Theme
                        var previewWrapper = $element.find('.preview-wrapper');
                        previewWrapper[0].className = "";
                        previewWrapper[0].classList.add('preview-wrapper');
                        data.classes.forEach(function (className) {
                            previewWrapper[0].classList.add(className);
                        });
                    }
                });
        }

        $scope.$watch('block.data', function (newValue, oldValue) {
            if (newValue != oldValue) {
                loadPreview();
            }
        }, true);

        //watch for column settings change
        $scope.$parent.$parent.$parent.$watch('vm.parentBlock.settingsData', function (newValue, oldValue) {
            if (newValue != oldValue) {
                loadPreview();
            }
        }, true);

        //watch for row settings change
        $scope.$parent.$parent.$parent.$parent.$parent.$parent.$watch('vm.parentBlock.settingsData', function (newValue, oldValue) {
            if (newValue != oldValue) {
                loadPreview();
            }
        }, true);

        loadPreview();

    });

function getCircularReplacer() {
    const seen = new WeakSet();
    return (key, value) => {
        if (key === "$block")
            return null;
        if (typeof value === "object" && value !== null) {
            if (seen.has(value)) {
                return;
            }
            seen.add(value);
        }
        return value;
    };
};

angular.module('umbraco.resources').factory('yuzuDeliveryBlockListResources',
    function ($q, $http, $routeParams, umbRequestHelper) {
        return {
            getPreview: function (nodeId, blockData, itemSettingsData, colSettingsData, rowSettingsData) {

                var url = "/umbraco/backoffice/YuzuDeliveryUmbracoImport/BlockListPreview/GetPartialData";
                var json = JSON.stringify(blockData, getCircularReplacer());

                var resultParameters = {
                    nodeId: nodeId,
                    content: json,
                    contentTypeKey: blockData.contentTypeKey,
                    itemSettings: JSON.stringify(itemSettingsData),
                    colSettings: JSON.stringify(colSettingsData),
                    rowSettings: JSON.stringify(rowSettingsData)
                };

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

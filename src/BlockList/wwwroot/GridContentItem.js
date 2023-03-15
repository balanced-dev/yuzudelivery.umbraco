angular.module('umbraco')
    .controller('GridContentItem', function ($scope, $element, $window, yuzuDeliveryBlockListResources, editorState) {

        var loadPreview = function () {

            console.log($scope.block.data, 'parent id', editorState.getCurrent().parentId);
            $scope.parentId = editorState.getCurrent().parentId;
            console.log('settings', $scope.$parent.$parent.$parent.vm.parentBlock.settingsData);

            $scope.nodeId = editorState.getCurrent().parentId;

            yuzuDeliveryBlockListResources.getPreview($scope.block.data, $scope.$parent.$parent.$parent.vm.parentBlock.settingsData)
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
                        previewWrapper[0].classList.add(data.theme);

                        var styleUrl = '/theme-css?nodeId=' + editorState.getCurrent().parentId;;
                        $http.get(styleUrl).then(function(response) {
                            var style = document.createElement('link');
                            style.rel = 'stylesheet';
                            style.type = 'text/css';
                            style.href = styleUrl;
                            $element.appendChild(style);
                        });

                    }
                });
        }

        $scope.$watch('block.data', function (newValue, oldValue) {
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
            getPreview: function (blockData, settingsData) {

                var url = "/umbraco/backoffice/YuzuDeliveryUmbracoImport/BlockListPreview/GetPartialData";
                var json = JSON.stringify(blockData, getCircularReplacer());
                var settingsJson = JSON.stringify(settingsData);
                var resultParameters = { content: json, contentTypeKey: blockData.contentTypeKey, colSettings:  settingsJson};

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

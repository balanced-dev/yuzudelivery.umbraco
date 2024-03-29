﻿angular.module('umbraco')
    .controller('GridContentSectionController', function ($scope, $rootScope, $element, $sce) {

        var findBlockListWrapper = function (element) {
            var parent = element.parent()[0];
            if (parent.classList.contains('umb-block-list__wrapper')) {
                return parent;
            }
            else {
                return findBlockListWrapper($(parent));
            }
        };

        const wrapper = findBlockListWrapper($element);
        const firstChild = $(wrapper).children()[0];
        if (!firstChild.classList.contains('yuzu-grid-reorder')) {

            $(wrapper).addClass('sections-wrapper');
            const button = document.createElement('button');
            $(button).addClass('yuzu-grid-reorder btn umb-button__button btn-outline umb-button--xs umb-outline');
            $(button).text('Reorder');
            $(button).on('click', function (e) {
                e.preventDefault();
                if ($(button).text() == 'Reorder') {
                    $(button).text("I am done reordering");
                    $(wrapper).find('.section-view').addClass('section-reorder');
                    $(wrapper).addClass('is-reordering');
                }
                else {
                    $(button).text('Reorder');
                    $(wrapper).find('.section-view').removeClass('section-reorder');
                    $(wrapper).removeClass('is-reordering');
                }
            });
            wrapper.prepend(button);
        }

        $scope.getGridSettings = function (property) {
            console.log(property.alias);
            if (property.alias.includes('50'))
                return { "grid-column": "span 6 / span 6" };
            else if (property.alias.includes('33'))
                return { "grid-column": "span 4 / span 4" };
            else
                return { "grid-column": "span 12 / span 12" };
        }

    });
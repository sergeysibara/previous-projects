(function () {
    angular
        .module('mindMapApp')
        .factory('TreeNodeViewFactory', ['$compile', viewCreator]);

    function viewCreator($compile) {
        return {
            createView: function ($scope, tree, viewClass) {
                let template = viewClass.getViewTemplate();
                let htmlElem = angular.element($compile(template)($scope))[0];
                return new viewClass(tree.drawSettings.drawArea, htmlElem);
            }
        }
    }
}());
(function () {
    angular
        .module('mindMapApp')
        .directive('jqDraggable', draggableAttr);

    draggableAttr.$inject = ["$parse"];

    function draggableAttr($parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var fn = $parse(attrs.onDragAction);
                $(element).draggable({
                    drag: function (jqevent, ui) {
                        fn(scope, {$event: jqevent});
                    }
                });
            }
        };
    }

}());
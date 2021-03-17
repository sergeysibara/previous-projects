'use strict';

(function () {
    angular.module('mindMapApp', ['ngRoute', 'mindMapApp.about', 'mindMapApp.mindmap']).config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/about', {
            templateUrl: 'views/about/about.html',
            controller: 'AboutCtrl',
            title: 'About'
        }).when('/mindmap', {
            templateUrl: 'views/mindmap/mindmap.html',
            controller: 'MindMapCtrl',
            title: 'MindMap'
        });
        // .otherwise({redirectTo: '/about'});
    }]).run(['$rootScope', function ($rootScope) {
        $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
            $rootScope.title = current.$$route !== undefined ? current.$$route.title : 'Application';
        });
    }]);
})();

//# sourceMappingURL=app-compiled.js.map
'use strict';

(function () {
    angular.module('mindMapApp').directive('jqDraggable', draggableAttr);

    draggableAttr.$inject = ["$parse"];

    function draggableAttr($parse) {
        return {
            restrict: 'A',
            link: function link(scope, element, attrs) {
                var fn = $parse(attrs.onDragAction);
                $(element).draggable({
                    drag: function drag(jqevent, ui) {
                        fn(scope, { $event: jqevent });
                    }
                });
            }
        };
    }
})();

//# sourceMappingURL=jq-draggable-compiled.js.map
/** Базовый класс для отрисовки соединений узлов */
"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var BaseTreeConnectionsController = (function () {
    function BaseTreeConnectionsController(elementId) {
        _classCallCheck(this, BaseTreeConnectionsController);
    }

    _createClass(BaseTreeConnectionsController, [{
        key: "resize",
        value: function resize() {}
    }, {
        key: "removeAllConnections",
        value: function removeAllConnections() {}
    }, {
        key: "addConnection",
        value: function addConnection(startNode, targetNode) {}
    }, {
        key: "updateConnection",
        value: function updateConnection() {}
    }, {
        key: "addConnectionsRecursively",
        value: function addConnectionsRecursively(startNode) {
            for (var i = 0; i < startNode.childs.length; i++) {
                this.addConnection(startNode, startNode.childs[i]);
                this.addConnectionsRecursively(startNode.childs[i]);
            }
        }

        /** Получение коордиант 4-х точек безье для дальнейшей отрисовки кривой безье */
    }], [{
        key: "_getBezierPoints",
        value: function _getBezierPoints(startNode, targetNode) {
            var p1 = startNode.view.rightCenter;
            var p4 = targetNode.view.leftCenter;
            var p2 = { x: p1.x + startNode.tree.nodeOffsetX, y: p1.y };
            var p3 = { x: p2.x - startNode.tree.nodeOffsetX, y: p4.y };
            return { p1: p1, p2: p2, p3: p3, p4: p4 };
        }
    }]);

    return BaseTreeConnectionsController;
})();

//# sourceMappingURL=tree-connections-controller-compiled.js.map
/** Класс для отрисовки соединений узлов с помощью SVG */
'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x2, _x3, _x4) { var _again = true; _function: while (_again) { var object = _x2, property = _x3, receiver = _x4; desc = parent = getter = undefined; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x2 = parent; _x3 = property; _x4 = receiver; _again = true; continue _function; } } else if ('value' in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

function _inherits(subClass, superClass) { if (typeof superClass !== 'function' && superClass !== null) { throw new TypeError('Super expression must either be null or a function, not ' + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var TreeSVGConnectionsController = (function (_BaseTreeConnectionsController) {
    _inherits(TreeSVGConnectionsController, _BaseTreeConnectionsController);

    function TreeSVGConnectionsController(elementId) {
        _classCallCheck(this, TreeSVGConnectionsController);

        _get(Object.getPrototypeOf(TreeSVGConnectionsController.prototype), 'constructor', this).call(this, elementId);
        this.context = SVG(elementId);
        this._drawArea = this.context.node.parentNode;
        this.resize();
    }

    _createClass(TreeSVGConnectionsController, [{
        key: 'resize',
        value: function resize() {
            this.context.size(0, 0);
            this.context.size(this._drawArea.scrollWidth, this._drawArea.scrollHeight - 4);
        }

        /** Удалить все линии дерева */
    }, {
        key: 'removeAllConnections',
        value: function removeAllConnections() {
            while (this.context.node.childNodes.length > 0) {
                this.context.node.removeChild(this.context.node.childNodes[0]);
            }
        }
    }, {
        key: 'addConnection',
        value: function addConnection(startNode, targetNode) {
            var points = _get(Object.getPrototypeOf(TreeSVGConnectionsController.prototype), 'constructor', this)._getBezierPoints(startNode, targetNode);
            var path = this.context.path().M(points.p1.x, points.p1.y).C(points.p2.x, points.p2.y, points.p3.x, points.p3.y, points.p4.x, points.p4.y);
            targetNode.parentConnection = path.node;

            targetNode.subscribeOnRemove(function () {
                if (path.node.parentNode !== null) path.node.parentNode.removeChild(path.node);
            });
        }

        /** Обновить все соединения данного узла без рекурсии
         * @excludeChilds {boolean} - исключить обновление соединений с дочерними узлами
         * */
    }, {
        key: 'updateLinkedConnections',
        value: function updateLinkedConnections(targetNode) {
            var excludeChilds = arguments.length <= 1 || arguments[1] === undefined ? false : arguments[1];

            if (targetNode.parentConnection == undefined) {
                if (targetNode.parent != undefined) this.addConnection(targetNode.parent, targetNode);
            }

            if (targetNode.parent != undefined) {
                this.constructor._updateConnection(targetNode.parent, targetNode);
            }

            if (excludeChilds) return;

            for (var i = 0; i < targetNode.childs.length; i++) {
                var child = targetNode.childs[i];
                if (child.parentConnection == undefined) continue;
                this.constructor._updateConnection(targetNode, child);
            }
        }
    }], [{
        key: '_updateConnection',
        value: function _updateConnection(targetNode, child) {
            var points = _get(Object.getPrototypeOf(TreeSVGConnectionsController), '_getBezierPoints', this).call(this, targetNode, child);
            child.parentConnection.setAttribute('d', ' M' + points.p1.x + ' ' + points.p1.y + ' C' + points.p2.x + ' ' + points.p2.y + ' ' + points.p3.x + ' ' + points.p3.y + ' ' + points.p4.x + ' ' + points.p4.y);
        }
    }]);

    return TreeSVGConnectionsController;
})(BaseTreeConnectionsController);

//# sourceMappingURL=tree-svg-connections-controller-compiled.js.map
/** Базовый класс модели узла дерева */
'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

var NodeModel = (function () {
    function NodeModel(value, debugText) {
        _classCallCheck(this, NodeModel);

        this.value = value;
        this.debugText = debugText;
    }

    _createClass(NodeModel, [{
        key: 'clone',
        value: function clone() {
            return new NodeModel(this.value, '');
        }
    }, {
        key: 'compareByValue',
        value: function compareByValue(nodeModel) {
            if (nodeModel === this.value) //для простых типов
                return true;

            //if (nodeModel == undefined || nodeModel.value == undefined)
            //    throw new Error('nodeModel or nodeModel.value is undefined');
            return this.value === nodeModel.value;
        }
    }]);

    return NodeModel;
})();

//# sourceMappingURL=node-model-compiled.js.map
/** Базовый класс для предствления узла дерева */
"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var BaseNodeView = (function () {
    function BaseNodeView(drawArea, htmlElement) {
        _classCallCheck(this, BaseNodeView);

        this.htmlElement = drawArea.appendChild(htmlElement);
    }

    _createClass(BaseNodeView, [{
        key: "init",
        value: function init(node) {
            this.htmlElement.getTreeNode = function () {
                return node;
            };
            this.update(node.model);
        }
    }, {
        key: "update",
        value: function update(model) {
            $(this.htmlElement).find('.model-value').html(model.value);
        }
    }, {
        key: "setPosition",
        value: function setPosition(x, y) {
            this.htmlElement.style.left = x + "px";
            this.htmlElement.style.top = y + "px";
        }
    }, {
        key: "rightCenter",
        get: function get() {
            var elem = this.htmlElement;
            return {
                x: this.right,
                y: this.top + elem.offsetHeight / 2
            };
        }
    }, {
        key: "leftCenter",
        get: function get() {
            var elem = this.htmlElement;
            var offset = $(elem).position();

            return {
                x: offset.left + $(elem).parent().scrollLeft(),
                y: this.top + elem.offsetHeight / 2
            };
        }
    }, {
        key: "top",
        get: function get() {
            var elem = this.htmlElement;
            var offset = $(elem).position();

            return offset.top + $(elem).parent().scrollTop();
        }
    }, {
        key: "right",
        get: function get() {
            var elem = this.htmlElement;
            var offset = $(elem).position();

            return offset.left + elem.offsetWidth + $(elem).parent().scrollLeft();
        }
    }, {
        key: "left",
        get: function get() {
            var elem = this.htmlElement;
            var offset = $(elem).position();

            return offset.left + $(elem).parent().scrollLeft();
        }
    }], [{
        key: "getViewTemplate",
        value: function getViewTemplate() {
            return '';
        }
    }, {
        key: "getInnerViewTemplate",
        value: function getInnerViewTemplate() {
            return '';
        }
    }, {
        key: "getEditableTemplate",
        value: function getEditableTemplate(model) {
            return '';
        }
    }]);

    return BaseNodeView;
})();

//# sourceMappingURL=base-node-view-compiled.js.map
'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; desc = parent = getter = undefined; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; continue _function; } } else if ('value' in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

function _inherits(subClass, superClass) { if (typeof superClass !== 'function' && superClass !== null) { throw new TypeError('Super expression must either be null or a function, not ' + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var TextNodeView = (function (_BaseNodeView) {
    _inherits(TextNodeView, _BaseNodeView);

    function TextNodeView(drawArea, htmlElement) {
        _classCallCheck(this, TextNodeView);

        _get(Object.getPrototypeOf(TextNodeView.prototype), 'constructor', this).call(this, drawArea, htmlElement);
    }

    _createClass(TextNodeView, null, [{
        key: 'getViewTemplate',
        value: function getViewTemplate() {
            var template = '<div class="tree-node" jq-draggable on-drag-action="updateConnection($event)"' + ('ng-dblclick="switchNodeToEditable($event)">' + TextNodeView.getInnerViewTemplate() + '</div>');
            return template;
        }
    }, {
        key: 'getInnerViewTemplate',
        value: function getInnerViewTemplate() {
            return '<p class="model-value"></p>';
        }
    }, {
        key: 'getEditableTemplate',
        value: function getEditableTemplate(model) {
            var template = '<div><input value="' + model.value + '"/><button ng-click="switchNodeToView($event)">end</button></div>';
            return template;
        }
    }]);

    return TextNodeView;
})(BaseNodeView);

//# sourceMappingURL=text-node-view-compiled.js.map
"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; desc = parent = getter = undefined; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var TextWithImageNodeView = (function (_BaseNodeView) {
    _inherits(TextWithImageNodeView, _BaseNodeView);

    function TextWithImageNodeView(drawArea, htmlElement) {
        _classCallCheck(this, TextWithImageNodeView);

        _get(Object.getPrototypeOf(TextWithImageNodeView.prototype), "constructor", this).call(this, drawArea, htmlElement);
    }

    _createClass(TextWithImageNodeView, null, [{
        key: "getViewTemplate",
        value: function getViewTemplate() {
            var template = "<div class=\"tree-node\" jq-draggable on-drag-action=\"updateConnection($event)\" ng-dblclick=\"switchNodeToEditable($event)\">" + TextWithImageNodeView.getInnerViewTemplate() + "</div>";
            return template;
        }
    }, {
        key: "getInnerViewTemplate",
        value: function getInnerViewTemplate() {
            var template = '<div><img src="../app/content/images/icon1.png" ><p class="model-value"></p></div>';
            return template;
        }
    }, {
        key: "getEditableTemplate",
        value: function getEditableTemplate(model) {
            var template = "<div><input value=\"" + model.value + "\"/><button ng-click=\"switchNodeToView($event)\">end</button></div>";
            return template;
        }
    }]);

    return TextWithImageNodeView;
})(BaseNodeView);

//# sourceMappingURL=text-with-image-node-view-compiled.js.map
/**
 * Основной класс для работы с деревом.
 * Содержит методы для рекурсивного клонирования, перемещения, удаления поддеревьев, поиска в дереве, выравнивание дерева по высоте при отрисовке.
 */
'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

var Tree = (function () {
    /**
     * @drawElem {HTMLElement} - html элемент, в котором дерево будет отрисовываться
     * @connectionsController {BaseTreeConnectionsController}
     * @nodeViewConstructorFunc {function} - функция, возвращающая экземпляр класса BaseNodeView
     */

    function Tree(drawElem, connectionsController, nodeViewConstructorFunc) {
        _classCallCheck(this, Tree);

        this._root = undefined;
        this._drawSettings = {
            drawArea: drawElem
        };

        this.nodeOffsetX = 20;
        this.nodeOffsetY = 20;
        this.connectionsController = connectionsController;

        this._onCreateNodeActions = []; //функции, вызываемые при создании узла
        this._nodeViewConstructorFunc = nodeViewConstructorFunc;
    }

    _createClass(Tree, [{
        key: 'addRootNode',
        value: function addRootNode(model, view) {
            this._root = new TreeNode(model, undefined, this, view);
        }
    }, {
        key: 'addChildNodeTo',
        value: function addChildNodeTo(model, parentNode, view) {
            var childNode = parentNode.addChild(model, view);
            childNode.view.setPosition(parentNode.view.right + this.nodeOffsetX, parentNode.view.top);

            this.connectionsController.resize();
            this.connectionsController.addConnection(parentNode, childNode);

            for (var i = 0; i < this._onCreateNodeActions.length; i++) {
                this._onCreateNodeActions[i](childNode);
            }return childNode;
        }

        /**
         * Рекурсивный обход дерева, начиная с указанного узла.
         * В каждом узле вызывается переданная функция: boolean nodeAction(currentNode, currentLevel)
         * @currentLevel {number} - текущий уровень узел. Указывать не нужно, используется при обходе
         */
    }, {
        key: 'removeNode',
        value: function removeNode(node) {
            var removeHtmlElements = function removeHtmlElements(node, currentLevel) {
                node.remove();
                return false;
            };

            if (node == this._root) {
                Tree.traversal(node, removeHtmlElements);
                delete this._root;
                return;
            }

            var index = node.parent.childs.indexOf(node);
            if (index != -1) {
                Tree.traversal(node, removeHtmlElements);
                node.parent.childs.splice(index, 1);
            }
        }
    }, {
        key: 'moveNode',
        value: function moveNode(node, toNode) {
            if (node == this._root) {
                console.warn('root node can not be moved!');
                return;
            }

            if (Tree._hasNodeInHierarchy(node, toNode)) {
                console.warn(node.model.value + ' can not be moved to self hierarchy!');
                return;
            }

            //move
            var index = node.parent.childs.indexOf(node);
            if (index != -1) {
                node.parent.childs.splice(index, 1);
                toNode.insertNode(node);
            }

            //update position
            Tree._shiftNodesPositionRelativeNode(node, toNode);
            node.tree.connectionsController.resize();
        }
    }, {
        key: 'deepCloneNode',
        value: function deepCloneNode(node, toNode) {
            if (Tree._hasNodeInHierarchy(node, toNode)) {
                console.warn(node.model.value + ' can not do clone to self hierarchy!');
                return;
            }

            var firstNewNode = this._cloneNodeRecursively(node, toNode);
            Tree._shiftNodesPositionRelativeNode(firstNewNode, toNode);
            node.tree.connectionsController.resize();
        }
    }, {
        key: '_cloneNodeRecursively',
        value: function _cloneNodeRecursively(node, toNode) {
            var newNode = node.cloneTo(toNode, this._nodeViewConstructorFunc(node.view.constructor));
            newNode.view.setPosition(node.view.left, node.view.top);

            for (var i = 0; i < node.childs.length; i++) {
                this._cloneNodeRecursively(node.childs[i], newNode);
            }for (var i = 0; i < this._onCreateNodeActions.length; i++) {
                this._onCreateNodeActions[i](newNode);
            }return newNode;
        }

        /**
         * Поиск модели в дереве, начиная с указанного узла
         * @findFirstOnly {boolean} - искать только до первого найденной модели
         * @returns {TreeNode[]}
         */
    }, {
        key: 'findModel',
        value: function findModel(startNode, model) {
            var findFirstOnly = arguments.length <= 2 || arguments[2] === undefined ? false : arguments[2];

            var results = [];
            var nodeAction = function nodeAction(node) {
                for (var i = 0; i < node.childs.length; i++) {
                    if (node.childs[i].model.compareByValue(model)) {
                        results.push(node.childs[i]);
                        if (findFirstOnly) return true;
                    }
                }
                return false;
            };

            Tree.traversal(startNode, nodeAction);
            return results;
        }
    }, {
        key: 'subscribeOnCreateNode',
        value: function subscribeOnCreateNode(action) {
            this._onCreateNodeActions.push(action);
        }

        /** Перерисовка дерева */
    }, {
        key: 'redraw',
        value: function redraw() {
            this.horizontalAlign();
            this.connectionsController.removeAllConnections();
            this.connectionsController.resize();
            this.connectionsController.addConnectionsRecursively(this.root);
        }

        /** Выравнивание дерева по горизонтали в зависимости от дочерних узлов так, чтобы узлы не пересекались */
    }, {
        key: 'horizontalAlign',
        value: function horizontalAlign() {
            if (this._root == undefined) {
                console.log('root is undefined');
                return;
            }

            Tree._calcBranchCount(this._root);
            Tree._setBranchIndex(this._root, 0);

            var nodeActionData = {
                nodeWidth: this._root.htmlElement.offsetWidth,
                nodeHeight: this._root.htmlElement.offsetHeight
            };

            var setNodePosition = function setNodePosition(node, currentLevel) {
                node.view.setPosition(10 + currentLevel * (nodeActionData.nodeWidth + node.tree.nodeOffsetX), 10 + (node.branchIndex - 1) * (nodeActionData.nodeHeight + node.tree.nodeOffsetY));
                return false;
            };

            Tree.traversal(this._root, setNodePosition);
        }

        /** Проверяет, есть ли указанный узел в иерархии узла subRoot */
    }, {
        key: 'root',
        get: function get() {
            return this._root;
        }
    }, {
        key: 'drawSettings',
        get: function get() {
            return this._drawSettings;
        }
    }, {
        key: 'drawArea',
        get: function get() {
            return this._drawSettings.drawArea;
        }
    }], [{
        key: 'traversal',
        value: function traversal(node, nodeAction) {
            var currentLevel = arguments.length <= 2 || arguments[2] === undefined ? 0 : arguments[2];

            var doStopTraversal = nodeAction(node, currentLevel);
            if (doStopTraversal) return true;

            //цикл обязательно должен быть for, а не for in, т.к. в некоторых nodeAction может быть важна очередность прохода
            for (var i = 0; i < node.childs.length; i++) {
                doStopTraversal = Tree.traversal(node.childs[i], nodeAction, currentLevel + 1);
                if (doStopTraversal) return true;
            }
            return false;
        }
    }, {
        key: '_hasNodeInHierarchy',
        value: function _hasNodeInHierarchy(subRoot, desiredNode) {
            var compareNodes = function compareNodes(node, currentLevel) {
                return desiredNode == node;
            };

            return Tree.traversal(subRoot, compareNodes);
        }

        /** Сдвигает узел и его дочернии узлы относительно другого узла */
    }, {
        key: '_shiftNodesPositionRelativeNode',
        value: function _shiftNodesPositionRelativeNode(node, relativeNode) {
            var diff = {
                x: relativeNode.view.left - node.view.left,
                y: relativeNode.view.top - node.view.top
            };
            var nodeOffsetX = node.tree.nodeOffsetX;

            var updatePosition = function updatePosition(node, currentLevel) {
                node.view.setPosition(node.view.left + diff.x + node.htmlElement.offsetWidth + nodeOffsetX, node.view.top + diff.y);
                node.tree.connectionsController.updateLinkedConnections(node, false);
                return false;
            };
            Tree.traversal(node, updatePosition);
        }

        /**
         * Рассчет и установка, к какой ветки дерева относится данный узел.
         * Используется при вычислении количества ветвлений дерева.
         */
    }, {
        key: '_setBranchIndex',
        value: function _setBranchIndex(node, prevBranchIndex) {
            node.branchIndex = Math.ceil(node.branchCount / 2) + prevBranchIndex;

            for (var i = 0; i < node.childs.length; i++) {
                //установка промежуточного branchIndex для текущего родительского узла. т.е. происходит расширение дерева.
                if (prevBranchIndex == node.branchIndex) {
                    if (node.branchCount % 2 == 0) {
                        node.branchIndex++;
                        node.branchCount++;
                        prevBranchIndex++;
                    }
                }

                prevBranchIndex = Tree._setBranchIndex(node.childs[i], prevBranchIndex);
            }

            //node.model.debugText = ` prevIndex: ${Math.max(prevBranchIndex, 1)} <br> max: ${node.maxLevelWidth}  index: ${node.branchIndex}`;
            //node.updateView();

            if (prevBranchIndex >= node.branchIndex) return prevBranchIndex;
            return prevBranchIndex + 1;
        }

        /**
         * Рассчет и установка количества ветвлений у каждого узла в иерархии, начиная с указанного
         * Используется при вычислении количества ветвлений дерева.
         */
    }, {
        key: '_calcBranchCount',
        value: function _calcBranchCount(node) {

            var currentBranchCount = 0;
            for (var i = 0; i < node.childs.length; i++) {
                currentBranchCount += Tree._calcBranchCount(node.childs[i]);
            }if (currentBranchCount < 1) currentBranchCount = 1;

            node.branchCount = currentBranchCount;
            return currentBranchCount;
        }
    }]);

    return Tree;
})();

//# sourceMappingURL=tree-compiled.js.map
/** Узел дерева. Содержит ссылки на NodeModel, NodeView и на соединение с родительским узлом */
'use strict';

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ('value' in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError('Cannot call a class as a function'); } }

var TreeNode = (function () {
    /**
     * @model {NodeModel} - модель конкретного типа
     * @parent {TreeNode} - ссылка на родительский узел
     * @tree {Tree} - ссылка на дерева
     * @tree {BaseNodeView} - представление конкретного типа для модели
     */

    function TreeNode(model, parent, tree, view) {
        _classCallCheck(this, TreeNode);

        this.parentConnection = undefined;
        this.branchCount = 0; //Количество ветвлений узла при рекурсивный обходе до самого нижнего подуровня.
        this.branchIndex = 0; //Индекс ветви дерева. Поле нужно для рассчетов количетсва ветвлений дерева.

        this.parent = parent;
        if (parent != undefined) parent.childs.push(this);

        this._childs = [];
        this._model = model instanceof Object ? model : new NodeModel(model, '');
        this._tree = tree;

        this._view = view;
        this._view.init(this);

        this._onRemoveActions = []; //функции, вызываемые при удалении узла
    }

    _createClass(TreeNode, [{
        key: 'updateView',
        value: function updateView() {
            this._view.update(this._model);
        }
    }, {
        key: 'cloneTo',
        value: function cloneTo(newParent, view) {
            return new TreeNode(this._model.clone(), newParent, this._tree, view);
        }
    }, {
        key: 'addChild',
        value: function addChild(model, view) {
            return new TreeNode(model, this, this._tree, view);
        }
    }, {
        key: 'insertNode',
        value: function insertNode(node) {
            this.childs.push(node);
            node.parent = this;
        }
    }, {
        key: 'remove',
        value: function remove() {
            for (var i = 0; i < this._onRemoveActions.length; i++) {
                this._onRemoveActions[i]();
            } //this._onRemoveActions.length=0;

            if (this._view.htmlElement.parentNode === null) {
                console.log(this._view.htmlElement);
                return;
            }
            this._view.htmlElement.parentNode.removeChild(this._view.htmlElement);
        }

        /** Подписка на событие удаления узла */
    }, {
        key: 'subscribeOnRemove',
        value: function subscribeOnRemove(action) {
            this._onRemoveActions.push(action);
        }
    }, {
        key: 'model',
        get: function get() {
            return this._model;
        },
        set: function set(newModel) {
            this._model = newModel;
            this._view.updateView(newModel);
        }
    }, {
        key: 'childs',
        get: function get() {
            return this._childs;
        }
    }, {
        key: 'view',
        get: function get() {
            return this._view;
        }
    }, {
        key: 'htmlElement',
        get: function get() {
            return this._view.htmlElement;
        }
    }, {
        key: 'tree',
        get: function get() {
            return this._tree;
        }
    }]);

    return TreeNode;
})();

//# sourceMappingURL=tree-node-compiled.js.map
'use strict';

(function () {
    angular.module('mindMapApp').factory('TreeNodeViewFactory', ['$compile', viewCreator]);

    function viewCreator($compile) {
        return {
            createView: function createView($scope, tree, viewClass) {
                var template = viewClass.getViewTemplate();
                var htmlElem = angular.element($compile(template)($scope))[0];
                return new viewClass(tree.drawSettings.drawArea, htmlElem);
            }
        };
    }
})();

//# sourceMappingURL=tree-node-view-factory-compiled.js.map
'use strict';

(function () {
    angular.module('mindMapApp').service('DOMUtils', [utils]);

    function utils() {
        /** Поиск элемента с указанным свойством в родительских элементах */
        this.findElementWithPropertyInParents = function (currentElem, property) {
            if (currentElem == null || currentElem == document.body || currentElem == document.head || currentElem == document.documentElement) return undefined;

            var elem = currentElem;
            for (var i = 0; i < 100; i++) {
                if (elem[property] != undefined) return elem;

                if (elem.parentNode == document.body) break;
                elem = elem.parentNode;
            }
            return undefined;
        };
    }
})();

//# sourceMappingURL=dom-utils-service-compiled.js.map
'use strict';

(function () {
    angular.module('mindMapApp.about', []).controller('AboutCtrl', ['$scope', AboutCtrl]);

    function AboutCtrl($scope) {
        $scope.features = ['Базовые алгоритмы работы с деревом: поиск, перемещение поддерева, удаление поддерева, клонирование поддерева', 'Расчет высоты дерева, чтобы узлы не пересекались при отрисовке', 'Редактирование узлов при двойном клике мыши по узлу', 'Контекстное меню при нажатии ПКМ для узлов и области отрисовки дерева', 'Поддержка разных типов узлов', 'Перемещение узлов мышью', 'Отрисовка соединений без использования специальных библиотек вроде jsPlumb. Использовались только svgjs и svgjs.path', 'Директива, фабрика, сервис на AngularJS'];
    }
})();

//# sourceMappingURL=about-ctrl-compiled.js.map
'use strict';

(function () {
    angular.module('mindMapApp.mindmap', []).controller('MindMapCtrl', ['$scope', '$compile', 'TreeNodeViewFactory', 'DOMUtils', MindMapCtrl]);

    MindMapCtrl.$inject = ['$scope', '$element', 'TreeNodeViewFactory', 'DOMUtils'];

    function MindMapCtrl($scope, $compile, TreeNodeViewFactory, DOMUtils) {
        $scope.$on("$destroy", function () {
            $.contextMenu('destroy');
            tree = null;
        });

        $scope.updateConnection = function () {
            var elem = $('.ui-draggable-dragging').get(0);
            if (elem.getTreeNode == undefined) return;

            var treeNode = elem.getTreeNode();
            treeNode.tree.connectionsController.updateLinkedConnections(treeNode);
            treeNode.tree.connectionsController.resize();
        };

        $scope.switchNodeToEditable = function (event) {
            var elem = DOMUtils.findElementWithPropertyInParents(event.target, 'getTreeNode');
            if (elem == undefined) return;

            var treeNode = elem.getTreeNode();
            var template = treeNode.view.constructor.getEditableTemplate(treeNode.model);
            var innerElem = angular.element($compile(template)($scope))[0];
            $(elem).html(innerElem);
        };

        $scope.switchNodeToView = function (event) {
            var elem = DOMUtils.findElementWithPropertyInParents(event.target, 'getTreeNode');
            if (elem == undefined) return;

            var treeNode = elem.getTreeNode();
            treeNode.model.value = $(elem).find('input').val();
            var template = treeNode.view.constructor.getInnerViewTemplate();
            var innerElem = angular.element($compile(template)($scope))[0];
            $(elem).html(innerElem);
            treeNode.updateView();
        };

        var drawArea = $("#draw-area");

        var createNodeView = function createNodeView(viewClass) {
            return TreeNodeViewFactory.createView($scope, tree, viewClass);
        };

        //Код ниже (до строчки tree.redraw()), отвечающий за создание и заполнение дерева данными логичней вынести в фабрику,
        //но, т.к. это пример и в реальном проекте данные бы откуда-нибудь загружались, а не создавались вручную, то код оставлен тут.
        var tree = new Tree(drawArea.get(0), new TreeSVGConnectionsController('draw-area'), createNodeView);

        tree.addRootNode('root', createNodeView(TextNodeView));
        tree.root.addChild('0', createNodeView(TextNodeView));
        tree.root.addChild('1', createNodeView(TextWithImageNodeView));
        tree.root.addChild('2', createNodeView(TextWithImageNodeView));
        tree.root.addChild('3', createNodeView(TextWithImageNodeView));
        tree.root.childs[1].addChild('1_0', createNodeView(TextNodeView));
        tree.root.childs[1].childs[0].addChild('1_0_0', createNodeView(TextNodeView));
        tree.root.childs[1].childs[0].childs[0].addChild('1_0_0_0', createNodeView(TextNodeView));
        tree.root.childs[1].addChild('1_1', createNodeView(TextNodeView));
        tree.root.childs[2].addChild('2_0', createNodeView(TextNodeView));

        tree.root.childs[3].addChild('3_0', createNodeView(TextNodeView));
        tree.root.childs[3].addChild('3_1', createNodeView(TextNodeView));
        tree.root.childs[3].childs[1].addChild('3_1_0', createNodeView(TextNodeView));
        tree.root.childs[3].childs[1].addChild('3_1_1', createNodeView(TextNodeView));
        tree.root.childs[3].childs[1].childs[1].addChild('3_1_1_0', createNodeView(TextNodeView));
        tree.root.childs[3].childs[1].childs[1].addChild('3_1_1_1', createNodeView(TextNodeView));
        tree.root.childs[3].childs[1].childs[1].addChild('3_1_1_2', createNodeView(TextNodeView));
        tree.root.childs[3].childs[1].childs[1].addChild('3_1_1_3', createNodeView(TextNodeView));
        tree.root.childs[3].childs[1].addChild('3_1_2', createNodeView(TextNodeView));

        //for testing
        tree.removeNode(tree.root.childs[3].childs[1].childs[1].childs[1]);
        tree.moveNode(tree.root.childs[3].childs[1], tree.root.childs[0]);
        tree.moveNode(tree.root.childs[0], tree.root.childs[2]);
        tree.deepCloneNode(tree.root.childs[2], tree.root.childs[0]);

        var results = tree.findModel(tree.root, '3');
        results.forEach(function (x) {
            return x.htmlElement.style.backgroundColor = 'gray';
        });

        tree.redraw();

        $.contextMenu({
            selector: '#draw-area',
            callback: function callback(key, options) {
                switch (key) {
                    case 'createRootNode':
                        window.alert('not implemented');
                        break;

                    case 'horizontalAlign':
                        tree.redraw();
                        break;
                }
            },
            items: {
                createRootNode: { name: "Create Root Node" },
                horizontalAlign: { name: "Tree horizontal Align" }
            }
        });

        var selectedNode = undefined;
        drawArea.contextMenu({
            selector: '.tree-node',
            events: {
                show: function show(options) {
                    this.data('cloneDisabled', !(selectedNode == undefined));
                    this.data('moveDisabled', !(selectedNode == undefined));
                }
            },

            callback: function callback(key, options) {
                var currentTreeNode = this.get(0).getTreeNode();

                switch (key) {
                    case 'addTextNode':
                        currentTreeNode.tree.addChildNodeTo('noname', currentTreeNode, createNodeView(TextNodeView));
                        break;

                    case 'addTextWithImageNode':
                        currentTreeNode.tree.addChildNodeTo('noname', currentTreeNode, createNodeView(TextWithImageNodeView));
                        break;

                    case 'selectNode':
                        selectedNode = currentTreeNode;
                        break;

                    case 'moveNode':
                        if (selectedNode != undefined) tree.moveNode(selectedNode, currentTreeNode);
                        break;

                    case 'cloneNode':
                        if (selectedNode != undefined) tree.deepCloneNode(selectedNode, currentTreeNode);
                        break;

                    case 'deleteNode':
                        currentTreeNode.tree.removeNode(currentTreeNode);
                        break;
                }

                if (key != 'selectNode') selectedNode = undefined;
            },
            items: {
                addTextNode: { name: "Add text node" },
                addTextWithImageNode: { name: "Add text with image node" },
                selectNode: { name: "Select" },
                cloneNode: {
                    name: "CloneTo", disabled: function disabled(key, opt) {
                        return !this.data('cloneDisabled');
                    }
                },
                moveNode: {
                    name: "MoveTo", disabled: function disabled(key, opt) {
                        return !this.data('moveDisabled');
                    }
                },
                deleteNode: { name: "Delete" }
            }
        });
    }
})();

//# sourceMappingURL=mindmap-ctrl-compiled.js.map
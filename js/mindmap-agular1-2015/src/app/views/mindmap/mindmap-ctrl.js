(function () {
    angular
        .module('mindMapApp.mindmap', [])
        .controller('MindMapCtrl',  ['$scope', '$compile', 'TreeNodeViewFactory', 'DOMUtils', MindMapCtrl]);

    MindMapCtrl.$inject = ['$scope', '$element', 'TreeNodeViewFactory', 'DOMUtils'];

    function MindMapCtrl($scope, $compile, TreeNodeViewFactory, DOMUtils) {
        $scope.$on("$destroy", function () {
            $.contextMenu('destroy');
            tree = null;
        });

        $scope.updateConnection = function () {
            let elem = $('.ui-draggable-dragging').get(0);
            if (elem.getTreeNode == undefined)
                return;

            let treeNode = elem.getTreeNode();
            treeNode.tree.connectionsController.updateLinkedConnections(treeNode);
            treeNode.tree.connectionsController.resize();
        };

        $scope.switchNodeToEditable = function (event) {
            let elem = DOMUtils.findElementWithPropertyInParents(event.target, 'getTreeNode');
            if (elem == undefined)
                return;

            let treeNode = elem.getTreeNode();
            let template = treeNode.view.constructor.getEditableTemplate(treeNode.model);
            let innerElem = angular.element($compile(template)($scope))[0];
            $(elem).html(innerElem);
        };

        $scope.switchNodeToView = function (event) {
            let elem = DOMUtils.findElementWithPropertyInParents(event.target, 'getTreeNode');
            if (elem == undefined)
                return;

            let treeNode = elem.getTreeNode();
            treeNode.model.value = $(elem).find('input').val();
            let template = treeNode.view.constructor.getInnerViewTemplate();
            let innerElem = angular.element($compile(template)($scope))[0];
            $(elem).html(innerElem);
            treeNode.updateView();
        };

        let drawArea = $("#draw-area");

        let createNodeView = (viewClass)=> {
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

        let results = tree.findModel(tree.root, '3');
        results.forEach(x=>x.htmlElement.style.backgroundColor = 'gray');


        tree.redraw();

        $.contextMenu({
            selector: '#draw-area',
            callback: function (key, options) {
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
                createRootNode: {name: "Create Root Node"},
                horizontalAlign: {name: "Tree horizontal Align"}
            }
        });

        let selectedNode = undefined;
        drawArea.contextMenu({
            selector: '.tree-node',
            events: {
                show: function (options) {
                    this.data('cloneDisabled', !(selectedNode == undefined));
                    this.data('moveDisabled', !(selectedNode == undefined));
                }
            },

            callback: function (key, options) {
                let currentTreeNode = this.get(0).getTreeNode();

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
                        if (selectedNode != undefined)
                            tree.moveNode(selectedNode, currentTreeNode);
                        break;

                    case 'cloneNode':
                        if (selectedNode != undefined)
                            tree.deepCloneNode(selectedNode, currentTreeNode);
                        break;

                    case 'deleteNode':
                        currentTreeNode.tree.removeNode(currentTreeNode);
                        break;
                }

                if (key != 'selectNode')
                    selectedNode = undefined;
            },
            items: {
                addTextNode: {name: "Add text node"},
                addTextWithImageNode: {name: "Add text with image node"},
                selectNode: {name: "Select"},
                cloneNode: {
                    name: "CloneTo", disabled: function (key, opt) {
                        return !this.data('cloneDisabled');
                    }
                },
                moveNode: {
                    name: "MoveTo", disabled: function (key, opt) {
                        return !this.data('moveDisabled');
                    }
                },
                deleteNode: {name: "Delete"}
            }
        });
    }

})();
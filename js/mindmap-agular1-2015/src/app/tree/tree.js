/**
 * Основной класс для работы с деревом.
 * Содержит методы для рекурсивного клонирования, перемещения, удаления поддеревьев, поиска в дереве, выравнивание дерева по высоте при отрисовке.
 */
class Tree {
    /**
     * @drawElem {HTMLElement} - html элемент, в котором дерево будет отрисовываться
     * @connectionsController {BaseTreeConnectionsController}
     * @nodeViewConstructorFunc {function} - функция, возвращающая экземпляр класса BaseNodeView
     */
    constructor(drawElem, connectionsController, nodeViewConstructorFunc) {
        this._root = undefined;
        this._drawSettings = {
            drawArea: drawElem
        };

        this.nodeOffsetX = 20;
        this.nodeOffsetY = 20;
        this.connectionsController = connectionsController;

        this._onCreateNodeActions = [];//функции, вызываемые при создании узла
        this._nodeViewConstructorFunc = nodeViewConstructorFunc;
    }

    get root() {
        return this._root;
    }

    get drawSettings() {
        return this._drawSettings;
    }

    get drawArea() {
        return this._drawSettings.drawArea;
    }


    addRootNode(model, view) {
        this._root = new TreeNode(model, undefined, this, view);
    }

    addChildNodeTo(model, parentNode, view) {
        let childNode = parentNode.addChild(model, view);
        childNode.view.setPosition(parentNode.view.right + this.nodeOffsetX, parentNode.view.top);

        this.connectionsController.resize();
        this.connectionsController.addConnection(parentNode, childNode);

        for (let i = 0; i < this._onCreateNodeActions.length; i++)
            this._onCreateNodeActions[i](childNode);

        return childNode;
    }

    /**
     * Рекурсивный обход дерева, начиная с указанного узла.
     * В каждом узле вызывается переданная функция: boolean nodeAction(currentNode, currentLevel)
     * @currentLevel {number} - текущий уровень узел. Указывать не нужно, используется при обходе
     */
    static traversal(node, nodeAction, currentLevel = 0) {
        let doStopTraversal = nodeAction(node, currentLevel);
        if (doStopTraversal)
            return true;

        //цикл обязательно должен быть for, а не for in, т.к. в некоторых nodeAction может быть важна очередность прохода
        for (let i = 0; i < node.childs.length; i++) {
            doStopTraversal = Tree.traversal(node.childs[i], nodeAction, currentLevel + 1);
            if (doStopTraversal)
                return true;
        }
        return false;
    }

    removeNode(node) {
        let removeHtmlElements = function (node, currentLevel) {
            node.remove();
            return false;
        };

        if (node == this._root) {
            Tree.traversal(node, removeHtmlElements);
            delete this._root;
            return;
        }

        let index = node.parent.childs.indexOf(node);
        if (index != -1) {
            Tree.traversal(node, removeHtmlElements);
            node.parent.childs.splice(index, 1);
        }
    }

    moveNode(node, toNode) {
        if (node == this._root) {
            console.warn('root node can not be moved!');
            return;
        }

        if (Tree._hasNodeInHierarchy(node, toNode)) {
            console.warn(`${node.model.value} can not be moved to self hierarchy!`);
            return;
        }

        //move
        let index = node.parent.childs.indexOf(node);
        if (index != -1) {
            node.parent.childs.splice(index, 1);
            toNode.insertNode(node);
        }

        //update position
        Tree._shiftNodesPositionRelativeNode(node, toNode);
        node.tree.connectionsController.resize();
    }

    deepCloneNode(node, toNode) {
        if (Tree._hasNodeInHierarchy(node, toNode)) {
            console.warn(`${node.model.value} can not do clone to self hierarchy!`);
            return;
        }

        let firstNewNode = this._cloneNodeRecursively(node, toNode);
        Tree._shiftNodesPositionRelativeNode(firstNewNode, toNode);
        node.tree.connectionsController.resize();
    }

    _cloneNodeRecursively(node, toNode) {
        let newNode = node.cloneTo(toNode, this._nodeViewConstructorFunc(node.view.constructor));
        newNode.view.setPosition(node.view.left, node.view.top);

        for (let i = 0; i < node.childs.length; i++)
            this._cloneNodeRecursively(node.childs[i], newNode);

        for (let i = 0; i < this._onCreateNodeActions.length; i++)
            this._onCreateNodeActions[i](newNode);
        return newNode;
    }


    /**
     * Поиск модели в дереве, начиная с указанного узла
     * @findFirstOnly {boolean} - искать только до первого найденной модели
     * @returns {TreeNode[]}
     */
    findModel(startNode, model, findFirstOnly = false) {
        let results = [];
        let nodeAction = function (node) {
            for (let i = 0; i < node.childs.length; i++) {
                if (node.childs[i].model.compareByValue(model)) {
                    results.push(node.childs[i]);
                    if (findFirstOnly)
                        return true;
                }
            }
            return false;
        };

        Tree.traversal(startNode, nodeAction);
        return results;
    }

    subscribeOnCreateNode(action) {
        this._onCreateNodeActions.push(action);
    }

    /** Перерисовка дерева */
    redraw() {
        this.horizontalAlign();
        this.connectionsController.removeAllConnections();
        this.connectionsController.resize();
        this.connectionsController.addConnectionsRecursively(this.root);
    }

    /** Выравнивание дерева по горизонтали в зависимости от дочерних узлов так, чтобы узлы не пересекались */
    horizontalAlign() {
        if (this._root == undefined) {
            console.log('root is undefined');
            return;
        }

        Tree._calcBranchCount(this._root);
        Tree._setBranchIndex(this._root, 0);

        let nodeActionData = {
            nodeWidth: this._root.htmlElement.offsetWidth,
            nodeHeight: this._root.htmlElement.offsetHeight
        };

        let setNodePosition = function (node, currentLevel) {
            node.view.setPosition(10 + currentLevel * (nodeActionData.nodeWidth + node.tree.nodeOffsetX),
                10 + ((node.branchIndex - 1) * (nodeActionData.nodeHeight + node.tree.nodeOffsetY))
            );
            return false;
        };

        Tree.traversal(this._root, setNodePosition);
    }

    /** Проверяет, есть ли указанный узел в иерархии узла subRoot */
    static _hasNodeInHierarchy(subRoot, desiredNode) {
        let compareNodes = function (node, currentLevel) {
            return desiredNode == node;
        };

        return Tree.traversal(subRoot, compareNodes);
    }

    /** Сдвигает узел и его дочернии узлы относительно другого узла */
    static _shiftNodesPositionRelativeNode(node, relativeNode) {
        let diff = {
            x: relativeNode.view.left - node.view.left,
            y: relativeNode.view.top - node.view.top
        };
        let nodeOffsetX = node.tree.nodeOffsetX;

        let updatePosition = function (node, currentLevel) {
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
    static _setBranchIndex(node, prevBranchIndex) {
        node.branchIndex = Math.ceil(node.branchCount / 2) + prevBranchIndex;

        for (let i = 0; i < node.childs.length; i++) {
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

        if (prevBranchIndex >= node.branchIndex)
            return prevBranchIndex;
        return prevBranchIndex + 1;
    }

    /**
     * Рассчет и установка количества ветвлений у каждого узла в иерархии, начиная с указанного
     * Используется при вычислении количества ветвлений дерева.
     */
    static _calcBranchCount(node) {

        let currentBranchCount = 0;
        for (let i = 0; i < node.childs.length; i++)
            currentBranchCount += Tree._calcBranchCount(node.childs[i]);

        if (currentBranchCount < 1)
            currentBranchCount = 1;

        node.branchCount = currentBranchCount;
        return currentBranchCount;
    }

}
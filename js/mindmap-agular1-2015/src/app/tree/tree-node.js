/** Узел дерева. Содержит ссылки на NodeModel, NodeView и на соединение с родительским узлом */
class TreeNode {
    /**
     * @model {NodeModel} - модель конкретного типа
     * @parent {TreeNode} - ссылка на родительский узел
     * @tree {Tree} - ссылка на дерева
     * @tree {BaseNodeView} - представление конкретного типа для модели
     */
    constructor(model, parent, tree, view) {
        this.parentConnection = undefined;
        this.branchCount = 0; //Количество ветвлений узла при рекурсивный обходе до самого нижнего подуровня.
        this.branchIndex = 0; //Индекс ветви дерева. Поле нужно для рассчетов количетсва ветвлений дерева.

        this.parent = parent;
        if (parent != undefined)
            parent.childs.push(this);

        this._childs = [];
        this._model = (model instanceof Object ) ? model : new NodeModel(model, '');
        this._tree = tree;

        this._view = view;
        this._view.init(this);

        this._onRemoveActions = []; //функции, вызываемые при удалении узла
    }

    get model() {
        return this._model;
    }

    set model(newModel) {
        this._model = newModel;
        this._view.updateView(newModel);
    }

    get childs() {
        return this._childs;
    }

    get view() {
        return this._view;
    }

    get htmlElement() {
        return this._view.htmlElement;
    }

    get tree() {
        return this._tree;
    }

    updateView() {
        this._view.update(this._model);
    }

    cloneTo(newParent, view) {
        return new TreeNode(this._model.clone(), newParent, this._tree, view);
    }

    addChild(model, view) {
        return new TreeNode(model, this, this._tree, view);
    }

    insertNode(node) {
        this.childs.push(node);
        node.parent = this;
    }

    remove() {
        for (let i = 0; i < this._onRemoveActions.length; i++)
            this._onRemoveActions[i]();
        //this._onRemoveActions.length=0;

        if (this._view.htmlElement.parentNode === null) {
            console.log(this._view.htmlElement);
            return;
        }
        this._view.htmlElement.parentNode.removeChild(this._view.htmlElement);

    }

    /** Подписка на событие удаления узла */
    subscribeOnRemove(action) {
        this._onRemoveActions.push(action);
    }
}
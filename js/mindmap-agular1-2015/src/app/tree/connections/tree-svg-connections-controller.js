/** Класс для отрисовки соединений узлов с помощью SVG */
class TreeSVGConnectionsController extends BaseTreeConnectionsController {
    constructor(elementId) {
        super(elementId);
        this.context = SVG(elementId);
        this._drawArea = this.context.node.parentNode;
        this.resize();
    }

    resize() {
        this.context.size(0, 0);
        this.context.size(this._drawArea.scrollWidth, this._drawArea.scrollHeight - 4);
    }

    /** Удалить все линии дерева */
    removeAllConnections() {
        while (this.context.node.childNodes.length > 0) {
            this.context.node.removeChild(this.context.node.childNodes[0]);
        }
    }

    addConnection(startNode, targetNode) {
        let points = super.constructor._getBezierPoints(startNode, targetNode);
        let path = this.context.path().M(points.p1.x, points.p1.y).C(points.p2.x, points.p2.y, points.p3.x, points.p3.y, points.p4.x, points.p4.y);
        targetNode.parentConnection = path.node;

        targetNode.subscribeOnRemove(()=> {
            if (path.node.parentNode !== null)
                path.node.parentNode.removeChild(path.node);
        });
    }

    /** Обновить все соединения данного узла без рекурсии
     * @excludeChilds {boolean} - исключить обновление соединений с дочерними узлами
     * */
    updateLinkedConnections(targetNode, excludeChilds = false) {
        if (targetNode.parentConnection == undefined) {
            if (targetNode.parent != undefined)
                this.addConnection(targetNode.parent, targetNode)
        }

        if (targetNode.parent != undefined) {
            this.constructor._updateConnection(targetNode.parent, targetNode);
        }

        if (excludeChilds)
            return;

        for (let i = 0; i < targetNode.childs.length; i++) {
            let child = targetNode.childs[i];
            if (child.parentConnection == undefined)
                continue;
            this.constructor._updateConnection(targetNode, child);
        }
    }

    static _updateConnection(targetNode, child) {
        let points = super._getBezierPoints(targetNode, child);
        child.parentConnection.setAttribute('d', ` M${points.p1.x} ${points.p1.y} C${points.p2.x} ${points.p2.y} ${points.p3.x} ${points.p3.y} ${points.p4.x} ${points.p4.y}`);
    }
}
/** Базовый класс для отрисовки соединений узлов */
class BaseTreeConnectionsController {
    constructor(elementId) {
    }

    resize() {

    }

    removeAllConnections() {

    }


    addConnection(startNode, targetNode) {

    }

    updateConnection() {

    }

    addConnectionsRecursively(startNode) {
        for (let i = 0; i < startNode.childs.length; i++) {
            this.addConnection(startNode, startNode.childs[i]);
            this.addConnectionsRecursively(startNode.childs[i]);
        }
    }

    /** Получение коордиант 4-х точек безье для дальнейшей отрисовки кривой безье */
    static _getBezierPoints(startNode, targetNode) {
        let p1 = startNode.view.rightCenter;
        let p4 = targetNode.view.leftCenter;
        let p2 = {x: p1.x + startNode.tree.nodeOffsetX, y: p1.y};
        let p3 = {x: p2.x - startNode.tree.nodeOffsetX, y: p4.y};
        return {p1, p2, p3, p4};
    }
}
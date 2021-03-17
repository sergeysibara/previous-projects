/** Базовый класс модели узла дерева */
class NodeModel {
    constructor(value, debugText) {
        this.value = value;
        this.debugText = debugText;
    }

    clone(){
        return new NodeModel(this.value,'');
    }

    compareByValue(nodeModel) {
        if (nodeModel === this.value) //для простых типов
            return true;

        //if (nodeModel == undefined || nodeModel.value == undefined)
        //    throw new Error('nodeModel or nodeModel.value is undefined');
        return (this.value === nodeModel.value);
    }
}
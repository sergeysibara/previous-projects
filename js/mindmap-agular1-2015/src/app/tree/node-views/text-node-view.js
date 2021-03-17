class TextNodeView extends BaseNodeView {
    constructor(drawArea, htmlElement) {
        super(drawArea, htmlElement);
    }

    static getViewTemplate() {
        let template = `<div class="tree-node" jq-draggable on-drag-action="updateConnection($event)"` +
            `ng-dblclick="switchNodeToEditable($event)">${TextNodeView.getInnerViewTemplate()}</div>`;
        return template;
    }

    static getInnerViewTemplate() {
        return '<p class="model-value"></p>';
    }

    static getEditableTemplate(model) {
        let template = `<div><input value="${model.value}"/><button ng-click="switchNodeToView($event)">end</button></div>`;
        return template;
    }
}
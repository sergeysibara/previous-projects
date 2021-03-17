class TextWithImageNodeView extends BaseNodeView {
    constructor(drawArea, htmlElement) {
        super(drawArea, htmlElement);
    }

    static getViewTemplate() {
        let template = `<div class="tree-node" jq-draggable on-drag-action="updateConnection($event)" ng-dblclick="switchNodeToEditable($event)">` +
            TextWithImageNodeView.getInnerViewTemplate() + "</div>";
        return template;
    }

    static getInnerViewTemplate() {
        let template = '<div><img src="../app/content/images/icon1.png" ><p class="model-value"></p></div>';
        return template;
    }

    static getEditableTemplate(model) {
        let template = `<div><input value="${model.value}"/><button ng-click="switchNodeToView($event)">end</button></div>`;
        return template;
    }
}
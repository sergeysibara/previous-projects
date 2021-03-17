/** Базовый класс для предствления узла дерева */
class BaseNodeView {
    constructor(drawArea, htmlElement) {
        this.htmlElement = drawArea.appendChild(htmlElement);
    }

    init(node) {
        this.htmlElement.getTreeNode = ()=> {
            return node;
        };
        this.update(node.model);
    }

    update(model) {
        $(this.htmlElement).find('.model-value').html(model.value);
    }

    setPosition(x, y) {
        this.htmlElement.style.left = x + "px";
        this.htmlElement.style.top = y + "px";
    }

    get rightCenter() {
        let elem = this.htmlElement;
        return {
            x: this.right,
            y: this.top + elem.offsetHeight / 2
        }
    }

    get leftCenter() {
        let elem = this.htmlElement;
        let offset = $(elem).position();

        return {
            x: offset.left + $(elem).parent().scrollLeft(),
            y: this.top + elem.offsetHeight / 2
        }
    }

    get top() {
        let elem = this.htmlElement;
        let offset = $(elem).position();

        return offset.top + $(elem).parent().scrollTop();
    }

    get right() {
        let elem = this.htmlElement;
        let offset = $(elem).position();

        return offset.left + elem.offsetWidth + $(elem).parent().scrollLeft();
    }

    get left() {
        let elem = this.htmlElement;
        let offset = $(elem).position();

        return offset.left + $(elem).parent().scrollLeft();
    }

    static getViewTemplate() {
        return '';
    }

    static getInnerViewTemplate() {
        return '';
    }

    static getEditableTemplate(model) {
        return '';
    }

}
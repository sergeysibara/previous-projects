(function () {
    angular
        .module('mindMapApp')
        .service('DOMUtils', [utils]);

    function utils() {
        /** Поиск элемента с указанным свойством в родительских элементах */
        this.findElementWithPropertyInParents = function (currentElem, property) {
            if (currentElem == null || currentElem == document.body || currentElem == document.head || currentElem == document.documentElement)
                return undefined;

            let elem = currentElem;
            for (let i = 0; i < 100; i++) {
                if (elem[property] != undefined)
                    return elem;

                if (elem.parentNode == document.body)
                    break;
                elem = elem.parentNode;
            }
            return undefined;
        }
    }

}());
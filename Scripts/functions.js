function wndsize() {
    var w = 0; var h = 0;
    //IE
    if (!window.innerWidth) {
        if (!(document.documentElement.clientWidth == 0)) {
            //strict mode
            w = document.documentElement.clientWidth; h = document.documentElement.clientHeight;
        } else {
            //quirks mode
            w = document.body.clientWidth; h = document.body.clientHeight;
        }
    } else {
        //w3c
        w = window.innerWidth; h = window.innerHeight;
    }
    return { width: w, height: h };
}

kendo.ui.Grid.fn.options.columnMenuInit = function (e) {
    var menu = e.container.find(".k-menu").data("kendoMenu");
    menu.bind('activate', function (e) {
        if (e.item.is(':last-child')) {
            // if an element in the submenu is focused first, the issue is not observed
            e.item.find('span.k-dropdown.k-header').first().focus();
            // e.item.find('input').first().focus();
        }
    });
}

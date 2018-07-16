var kendoCustom = {};

kendoCustom.contextMenu = function (options) {
    var ctxMenuId = "contextMenu_" + Math.floor(Math.random() * 1000000);
    if (!options) { alert('You have not specified any options!'); return null; }
    if (!options.trigger) { alert('You must supply a trigger element selector!'); return null; }
    if (!options.menu) { alert('You must specify a menu element selector!'); return null; }
    if (!options.click) { alert('You must specify a "click" function!'); return null; }

    options.zindex = options.zindex || 999000;

    var menu = $(options.menu);
    if (!menu.parent().hasClass('k-context-menu')) menu.wrap(document.createElement('div'));
    menu = menu.parent().addClass('k-content k-widget k-header k-reset k-menu-vertical').hide();
    menu.find("li").addClass('k-item k-state-default').wrapInner("<span class='k-link'/>");
    menu.find("img").css("margin-right", "5px");

    $.each(menu.find("separator"), function (e) {
        $(this).prev().css("border-bottom", "1px solid silver");
        $(this).remove();
    });

    var panel = $('#' + ctxMenuId);
    if (!panel[0]) panel = $(document.createElement('div')).attr('id', ctxMenuId);

    menu.visible = false;
    menu.css({ position: 'absolute', display: 'none', 'z-index': options.zindex + 1, padding: 0 });

    menu.state = function (elementCollection, state) {
        if (state) $(elementCollection).addClass("disabled").find("span").css({ color: "silver", cursor: "default" });
        else $(elementCollection).removeClass("disabled").find("span").css({ color: "black", cursor: "pointer" });
        return $(elementCollection);
    };

    menu.close = function () {
        menu.fadeOut(0);
        panel.hide();
        menu.visible = false;
        menu.point = null;
    };

    menu.show = function (e) {
        if (menu.visible) menu.close();
        else {
            menu.point = e;
            var pos = $(this).offset();
            pos.top = e.pageY - 170;
            pos.left = e.pageX - 12;

            if (options.popup) {
                try{
                    if (!options.popup(e.target, pos)) return false;
                }
                catch(e){
                    return false;
                }
            }
            panel.css({ top: e.pageY - 170, left: e.pageX - 12, width: 'auto', height: 'auto', 'background-color': 'transparent' }).show();
            menu.data('trigger', e.target);
            menu.css(pos).fadeIn(0);
            menu.visible = true;
        }
        return false;
    };

    panel.css({ position: 'absolute', display: 'none', 'z-index': options.zindex });
    $(document).click(menu.close);

    var list = menu.find('ul').css({ 'list-style-type': 'none', padding: 0, margin: 0 });
    list.find('li').click(function () {
        if ($(this).hasClass("disabled")) return;
        var pnt = menu.point;
        menu.close();
        options.click($(this), menu.data('trigger'), pnt);
    }).mouseenter(function () { if (!$(this).hasClass("disabled")) $(this).addClass('k-state-hover'); }).
        mouseleave(function () { $(this).removeClass('k-state-hover'); }).
        mousedown(function () { if (!$(this).hasClass("disabled")) $(this).addClass('k-state-selected'); }).
        mouseup(function () { $(this).removeClass('k-state-selected'); }).
        css({ margin: 0, padding: '3px 7px 3px 7px' });

    $(options.trigger).each(function () { $(this).bind("contextmenu", menu.show); });
    return menu;
};
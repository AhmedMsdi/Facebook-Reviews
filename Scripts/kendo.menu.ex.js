/*
 *# Kendo UI MenuEx by CZ
 *
 * MenuEx allows you to create:
 *  - context menus
 *  - hook to click event of menu node
 *  - extra data assigned to menu node
 *  - addnode callback (envoked when html of node generated)
 *
 *## Auto data-data insertion.
 *
 * {text: 'text', data: {id:5}}
 *
 * will be
 *
 * <li class='...' data-data='{id:5}'>text</li>
 *
 *## Node click callback
 *
 * {text: 'text', click: 'alert(this)' }
 *
 *## addnode callback
 *
 *  addnode: function ( e ) {
 *      console.log(e);
 *  }
 *
 *## All extra options
 *
 * - anchor - selector to elements menu will be linked
 * - event  - jQuery event triggers context menu (default: 'contexmenu')
 * - delay  - delay before menu will hide
 *
 *## Example
 *
 * $(document).ready(function()  {
 *      $('#testMenu').kendoMenuEx({
 *      dataSource: [
 *            {
 *                text: 'item #1',
 *                imageUrl: "../../content/shared/icons/sports/baseball.png",
 *                data: {id: 5, extra: 'extradata'},
 *                click: 'showalert(this)'
 *            }
 *        ],
 *        select: function(el) {
 *            console.log(el);
 *        },
 *        orientation: 'vertical',
 *        anchor: '#mySpan, #myButton',
 *        delay: 1500,
 *        addnode: function(el) { console.log(el); }
 *    });
 * });
 *
 *
 * Kendo UI Complete v2012.1.322 (http://kendoui.com)
 * Copyright 2012 Telerik AD. All rights reserved.
 *
 */

(function ($, undefined) {

    var parent = window.kendo.ui.Menu.renderItem;

    $.extend(window.kendo.ui.Menu, {

        renderItem: function (options) {

            var r = parent(options);
            var injects = "";

            if (options.item.data) {

                injects = " data-data='" + JSON.stringify(options.item.data) + "'";
            }

            if (injects.length > 0) {

                var re = /(<li)([^>]*>.*)/;
                var m = re.exec(r);

                r = m[1] + injects + m[2];
            }

            return r;
        }
    });

    var hiding = false;
    var showing = false;

    var MenuEx = window.kendo.ui.Menu.extend({/** @lends kendo.ui.Menu.prototype */

        /**
         * target object which was clicked
         */
        target: {},
        /**
         * menu item which was clicked
         */
        item: {},

        options: {
            name: "MenuEx",
            delay: 1000,
            event: 'contextmenu',
            orientation: 'vertical'
        },

        init: function (element, options) {

            var that = this;

            window.kendo.ui.Menu.fn.init.call(that, element, options);

            that.element.addClass('k-context-menu');

            if (options.anchor) {

                event = options.event || that.options.event;

                $(options.anchor).bind(event, function (e) {

                    that.show(options.anchor, e);
                    return false;
                });

                this.bind('mouseleave', function () {

                    delay = options.delay || that.options.delay;
                    setTimeout(function () { that.hide() }, delay);
                });
            }

            $(document).click($.proxy(that._documentClick, that));

            var items = that.element.find('.k-item');

            $.each(options.dataSource, function (i, el) {

                if (el.click != undefined) {

                    jQuery(items[i]).click(function (e) {

                        //el.click.call($(e.target).parents('li'), e);
                        that.item = $(e.target).parents('li');
                        el.click.call(that, e);
                    });
                }
            });
        },

        hide: function () {

            if (showing) {

                hiding = true;
                var $target = $(this.target);
                if ($target.hasClass('k-item')) {

                    $target.find('.k-in').removeClass('k-state-focused');
                }
                this.element.fadeOut(function () {

                    hiding = false;
                    showing = false;
                });
            }
        },

        show: function (anchor, e) {

            if (hiding == false) {

                this.target = e.currentTarget;
                var $target = $(this.target);
                if ($target.hasClass('k-item')) {

                    $target.find('.k-in').addClass('k-state-focused');
                }
                this.element.css({ 'top': e.pageY, 'left': e.pageX });
                this.element.fadeIn(function () { showing = true; });
            }
        },

        _documentClick: function (e) {

            var that = this;
            that.hide();
        }
    });

    window.kendo.ui.plugin(MenuEx);

})(jQuery);
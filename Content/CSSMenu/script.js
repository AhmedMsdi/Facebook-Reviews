(function ($) {

    $.fn.menumaker = function (options) {

        var cssmenu = $(this), settings = $.extend({
            title: "Menu",
            format: "dropdown",
            sticky: false
        }, options);

        return this.each(function () {
            cssmenu.prepend('<div id="menu-button">' + settings.title + '</div>');
            $(this).find("#menu-button").on('click', function () {
                $(this).toggleClass('menu-opened');
                var mainmenu = $(this).next('ul');
                if (mainmenu.hasClass('open')) {
                    mainmenu.hide().removeClass('open');
                }
                else {
                    mainmenu.show().addClass('open');
                    if (settings.format === "dropdown") {
                        mainmenu.find('ul').show();
                    }
                }
            });

            cssmenu.find('li ul').parent().addClass('has-sub');

            multiTg = function () {
                cssmenu.find(".has-sub").prepend('<span class="submenu-button"></span>');
                cssmenu.find('.submenu-button').on('click', function () {
                    $(this).toggleClass('submenu-opened');
                    if ($(this).siblings('ul').hasClass('open')) {
                        $(this).siblings('ul').removeClass('open').hide();
                    }
                    else {
                        $(this).siblings('ul').addClass('open').show();
                    }
                });
            };

            if (settings.format === 'multitoggle') multiTg();
            else cssmenu.addClass('dropdown');

            if (settings.sticky === true) cssmenu.css('position', 'fixed');

            /**
             * Store the window width */
            var windowWidth = $(window).width();
            resizeFix = function () {
                /**
                * Check window width has actually changed and it's not just iOS triggering a resize event on scroll */

                if ($(window).width() != windowWidth) {
                    /**
                    * Update the window width for next time */

                    windowWidth = $(window).width();
                    setTimeout(function () {
                        if ($(window).width() > 1040) {
                            cssmenu.find('ul').show();
                        }
                        if ($(window).width() <= 1040) {
                            cssmenu.find('ul').hide().removeClass('open');
                            /**
                            * Make sure to change the icons for opened menus */
                            $('.submenu-button').removeClass('submenu-opened');
                            $('.submenu-button').addClass('submenu-closed');
                        }
                    }, 500);

                }
            };

            resizeFix();
            return $(window).on('resize', resizeFix);

        });
    };
})(jQuery);


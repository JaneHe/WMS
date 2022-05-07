
(function($) {
    var opt;
    $.fn.jqprint = function (options) {
        opt = $.extend({}, $.fn.jqprint.defaults, options);

        var $element = (this instanceof jQuery) ? this : $(this);
        
        if (opt.operaSupport && $.browser.opera) 
        { 
            var tab = window.open("","jqPrint-preview");
            tab.document.open();

            var doc = tab.document;
        }
        else 
        {
            var $iframe = $("<iframe  />");
        
            if (!opt.debug) { $iframe.css({ position: "absolute", width: "0px", height: "0px", left: "-600px", top: "-600px" }); }

            $iframe.appendTo($(this));
            var doc = $iframe[0].contentWindow.document;
        }
        
        if (opt.importCSS)
        {
            if ($("link[media=print]").length > 0) 
            {
                $("link[media=print]").each( function() {
                    doc.write("<link type='text/css' rel='stylesheet' href='" + $(this).attr("href") + "' media='print' />");
                });
            }
            else 
            {
                $("link").each( function() {
                    doc.write("<link type='text/css' rel='stylesheet' href='" + $(this).attr("href") + "' />");
                });
            }
        }
        
        if (opt.printContainer) { doc.write($element.outer()); }
        else { $element.each( function() { doc.write($(this).html()); }); }
        
        doc.close();
        
        (opt.operaSupport && $.browser.opera ? tab : $iframe[0].contentWindow).focus();
        setTimeout(function () {
            (opt.operaSupport && $.browser.opera ? tab : $iframe[0].contentWindow).print();
            if (tab) { tab.close(); }
            $(".cxy_print_div").show();//打开关闭需要的层（因为手机端打印本页面的内容，电脑可以指定区域）
            $("#print_div").hide();//隐藏打印的div层
        }, 10);
    }
    
    $.fn.jqprint.defaults = {
		debug: false,
		importCSS: true, 
		printContainer: true,
		operaSupport: true
	};

    jQuery.fn.outer = function() {
      return $($('<div></div>').html(this.clone())).html();
    } 
})(jQuery);
/**
*  Version 2.0
*      -Contributors: "mindinquiring" : 除了打印使用过滤器来排除任何样式表.
*  测试只有在IE 8和火狐.
*  示例:
*      Print Button: <div id="print_button">打印</div>
*      Print Area  : <div class="PrintArea"> ... html ... </div>
*      Javascript  : <script>
*                       $("div#print_button").click(function(){
*                           $("div.PrintArea").printArea( [OPTIONS] );
*                       });
*                     </script>
*  选项是通过json (json示例: {mode: "popup", popClose: false})
*
*  {OPTIONS} | [type]    | (default), values    | Explanation
*  --------- | --------- | -------------------- | -----------
*  @mode     | [string]  | ("iframe"), "popup"  | 可打印窗口要么是iframe或浏览器弹出
*  @popHt    | [number]  | (500)                | 弹出窗口的高度
*  @popWd    | [number]  | (400)                | 弹出窗口的宽度
*  @popX     | [number]  | (500)                | 弹出窗口屏幕X的位置
*  @popY     | [number]  | (500)                | 弹出窗口屏幕Y的位置
*  @popTitle | [string]  | ('')                 | 弹出窗口标题元素
*  @popClose | [boolean] | (false), true        | 印刷后弹出窗口关闭
*/
(function ($) {
    var counter = 0;
    var modes = { iframe: "iframe", popup: "popup" };
    var defaults = {
        mode:
            modes.iframe,
        popHt: 500,
        popWd: 400,
        popX: 200,
        popY: 200,
        popTitle: '',
        popClose: false
    };

    var settings = {}; //global settings

    $.fn.printArea = function (options) {
        $.extend(settings, defaults, options);

        counter++;
        var idPrefix = "printArea_";
        $("[id^=" + idPrefix + "]").remove();
        var ele = $(this);

        settings.id = idPrefix + counter;

        var writeDoc;
        var printWindow;

        switch (settings.mode) {
            case modes.iframe:
                var f = new Iframe();
                writeDoc = f.doc;
                printWindow = f.contentWindow || f;
                break;
            case modes.popup:
                printWindow = new Popup();
                writeDoc = printWindow.doc;
        }

        writeDoc.open();
        writeDoc.write("<html>" + getHead() + getBody(ele) + "</html>");
        writeDoc.close();

        printWindow.focus();
        printWindow.print();

        if (settings.mode == modes.popup && settings.popClose)
            printWindow.close();
    }

    function getHead() {
        var head = "<head><title>" + settings.popTitle + "</title>";
        $(document).find("link")
            .filter(function () {
                return $(this).attr("rel").toLowerCase() == "stylesheet";
            })
            .filter(function () { // 这个过滤器提供的 "mindinquiring"
                var media = $(this).attr("media");
                return (media.toLowerCase() == "" || media.toLowerCase() == "print")
            })
            .each(function () {
                head += '<link type="text/css" rel="stylesheet" href="' + $(this).attr("href") + '" >';
            });
        var style = $(document).find("style").html();
        head += '<style type="text/css">' + style + "</style>";
        head += "</head>";
        return head;
    }

    function getBody(printElement) {
        var body = "<body>";
        body += '<div class="' + $(printElement).attr("class") + '">' + $(printElement).html() + '</div>';
        body += "</body>";
        return body;
    }

    function Iframe() {
        var frameId = settings.id;

        var iframeStyle = 'border:0;position:absolute;width:0px;height:0px;left:0px;top:0px;';
        var iframe;

        try {
            iframe = document.createElement('iframe');
            document.body.appendChild(iframe);
            $(iframe).attr({ style: iframeStyle, id: frameId, src: "" });
            iframe.doc = null;
            iframe.doc = iframe.contentDocument ? iframe.contentDocument : (iframe.contentWindow ? iframe.contentWindow.document : iframe.document);
        }
        catch (e) { throw e + ". iframes may not be supported in this browser."; }

        if (iframe.doc == null) throw "Cannot find document.";

        return iframe;
    }

    function Popup() {
        var windowAttr = "location=yes,statusbar=no,directories=no,menubar=no,titlebar=no,toolbar=no,dependent=no";
        windowAttr += ",width=" + settings.popWd + ",height=" + settings.popHt;
        windowAttr += ",resizable=yes,screenX=" + settings.popX + ",screenY=" + settings.popY + ",personalbar=no,scrollbars=no";

        var newWin = window.open("", "_blank", windowAttr);

        newWin.doc = newWin.document;

        return newWin;
    }
})(jQuery);
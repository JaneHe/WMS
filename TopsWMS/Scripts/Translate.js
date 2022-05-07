///使用方法
//引入两个js文件
//    <script src="../../Scripts/Common/Mask.js?@DateTime.Now" type="text/javascript"></script>
//    <script src="../../TopsLib/Scripts/LoadTranslationData.js?@DateTime.Now" type="text/javascript"></script>
//执行
//    $.Translate({ moduleText: "焊接", language: "eng" });
//HTML标签
//    <div class="PressClickRefresh" clickeffect="true" language>
//        <span>刷新</span>
//    </div>

//或者

//执行
//    $.Translate({ moduleText: "焊接" });
//HTML标签
//    <div class="PressClickRefresh" clickeffect="true" language="eng" languageText="刷新">
//        <span></span>
//    </div>
 
$.Translate = function (option) {

    var settings = {
        moduleText: "", //模块名称
        language: "" //语言简写
    };

    $.extend(settings, option);

    var trans = new Object();
    //模块名称
    //trans.moduleText = arguments[0] || {};
    trans.moduleText = settings.moduleText;
    trans.language = settings.language;
    trans.translationData = new Array();

    //trans.mask = new Mask();
    //trans.mask.show();

    //加载该模块底下的翻译数据
    trans.f_loadTranslationData = function () {
        $.ajax({
            type: "GET",
            url: "/tools/AjaxGet/SelDictionaryByMoudle",
            data: { ModuleName: trans.moduleText },
            success: function (data) {
                data = JSON.parse(data);
                trans.f_spaceTranslationData(data);
                trans.loadingTranslate();
            }
        })
    }

    //整合模块下的翻译数据
    trans.f_spaceTranslationData = function (data) {
        if (data.Data[0].length > 0) {
            data.Data[0].forEach(function (v, i) {
                trans.translationData.push(v);
            })
        }
    }

    //翻译方法
    trans.loadingTranslate = function () {

        //遍历所有文本元素，使其进行翻译
        $(document.body).find("[languageText]").each(function (index, obj) {
      
            var text = $(obj).attr("languageText");
            trans.translationData.forEach(function (v, i) {

                if ((trans.language != "" && v.Field == text && v.Location == trans.language)
                    || (trans.language == "" && v.Field == text && v.Location == $(obj).attr("language"))) {
                    $(obj).text(v.Caption);
                    return false;
                }
            })

        })

        //trans.mask.hide();

    }
    trans.f_loadTranslationData();

    return trans;
}

$(document).ready($.Translate);
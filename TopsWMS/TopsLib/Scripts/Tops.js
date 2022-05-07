


//封装收的Ajax请求方法
$.topsAjax = function (params) {
    var settings = {
        type: "GET",
        data: null,
        url: "",
        dataType:"json",
        dealError: true,
        before: function (settings) { },
        success: function (data) { }
    };
    $.extend(settings, params);
    //如果执行before函数返回false就return;
    if (settings.before(settings) == false) return;
    $.ajax({
        type: settings.type,
        url: settings.url,
        data: settings.data,
        dataType: settings.dataType,
        success: function (data) {
            if (settings.dealError)
                $.topsErrorDeal(data);
            settings.success(data);
        },
        complete: function (jqXHR, textStatus) {

        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("请求失败:" + textStatus + "|" + errorThrown);
        }
    });
};
//系统部分异常处理
$.topsErrorDeal = function (data) {
    if (!data.IsSuccess) {
        if (data.Msgs[0].indexOf("[Error]") == 0)
            alert(data.Msgs[0]); //异常过滤器报错
        else if (data.Msgs[0].indexOf("[F001]") == 0)
            //alert(data.Msgs[0]); //Session过期
            window.top.f_minLogin();
        else
            alert(data.Msgs[0]);
        return false;
    }
};

//工具条
$.toolbar = function (option)
{
    var defaults = {
        data: null,
        labelWidth: 100,
        moduleConfig: null,
        onBeforeRenderGrid: function () { }
    },
    settings = $.extend(defaults, option);
    var p = new Object();
    p.settings = settings;
    p.map = new Object();
    $.each(this, function (index) {

    });
}

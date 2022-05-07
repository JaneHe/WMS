//正常调用
//$.OffLineAjax({
//    type: "POST",
//    url: "/AjaxPost/AddProducttest",
//    data: { id: 1 },
//    success: function (data) {
//        data = JSON.parse(data);
//        if (data.IsSuccess) {
//            alert("保存成功");
//        } else {
//            alert(data.Msgs[0]);
//        }
//    }
//})

$.OffLineAjax = function (options) {
    var settings = {
        type: "POST",
        data: null,
        url: "",
        success: function (data) { }
    }
    $.extend(settings, options);
    //如果执行before函数返回false就return;
    //if (settings.before(settings) == false) return;

    //替换成特有的请求
    settings.url = settings.url.replace("AjaxPost", "AjaxOffLinePost");

    var ajax_Time = $.OffLine_getNowTime();
    settings.data.OffLine_Param = JSON.stringify(settings.data);
    settings.data.OffLine_AjaxName = settings.url.substring(settings.url.lastIndexOf("/") + 1, settings.url.length);
    settings.data.OffLine_Key = settings.url.substring(settings.url.lastIndexOf("/") + 1, settings.url.length) + "_OffLine_" + ajax_Time;

    var obj = new Object();
    obj.url = settings.url;
    obj.Time = ajax_Time;
    obj.param = settings.data;
    obj.success = settings.success.toString();
    $.OffLine_InsertOffLineData(obj);

    $.ajax({
        type: settings.type,
        url: settings.url,
        data: settings.data,
        success: settings.success,
        complete: function (jqXHR, textStatus) {
            //判断网络是否断开
            //            if (!window.navigator.onLine) {
            //                var response = JSON.parse(jqXHR.responseText);
            //                var msg = response.Msgs[0] || "";
            //                var obj = new Object();
            //                obj.Time = ajax_Time;
            //                obj.url = settings.url;
            //                obj.param = settings.data;
            //                obj.success = settings.success;
            //                $.OffLine_InsertOffLineData(obj);
            //            }
            if (window.navigator.onLine) {
                //返回数据则说明该请求成功执行，则删除对应本地的存储数据
                $.OffLine_RemoveOffLineData(settings.url.substring(settings.url.lastIndexOf("/") + 1, settings.url.length) + "_OffLine_" + ajax_Time);
            }
        }
    })

}



//断网状态下Ajax请求数组
var OffLine_List = new Array();


//记录每一个发送的请求到数据库
$.OffLine_InsertOffLineData = function (options) {
    var settings = {
        url: "",
        param: "",
        success: function (data) { }
    }
    $.extend(settings, options);
    //如果执行before函数返回false就return;
    //if (settings.before(settings) == false) return;


    var param = new Object();
    param.url = settings.url;
    param.param = settings.param;
    param.success = settings.success.toString();
    param.ajaxtime = settings.Time;

    //存储数据到本地
    localStorage[settings.url.substring(settings.url.lastIndexOf("/") + 1, settings.url.length) + "_OffLine_" + settings.Time] = JSON.stringify(param);

    //请求状态,如果之前处于离线现在已经连接网络则为false，反之为true
    if (typeof (localStorage["OL_IntervalPostOffLineState"]) == "undefined")
        localStorage["OL_IntervalPostOffLineState"] = true;


}

//删除存放到本地硬盘的数据
$.OffLine_RemoveOffLineData = function (name) {
    localStorage.removeItem(name);
}



//如果网络恢复正常，则自动开始重新发送因为离线而未保存的请求
$.OffLine_SetIntervalPostOffLineData = function () {
    setInterval(function () {
        if (window.navigator.onLine && !(localStorage["OL_IntervalPostOffLineState"] == "true")) {

            if (localStorage.length > 0) {
                //OffLine_List = OffLine_List.filter(function (item) { return item.hasOwnProperty("url") && item.hasOwnProperty("param") && item.hasOwnProperty("success") });
                for (var index = 0; index < localStorage.length; index++) {
                    if (localStorage.key(index).indexOf("OffLine") != -1) {
                        var object = JSON.parse(localStorage.getItem(localStorage.key(index)));

                        var param = new Object();
                        param.key = localStorage.key(index);
                        param.url = object.url;
                        param.param = object.param;

                        param.success = new Function('return ' + object.success)();

                        OffLine_List.push(param);
                    }
                }
            } else {
                localStorage["OL_IntervalPostOffLineState"] = true;
            }

            OffLine_List = OffLine_List.filter(function (item) { return item.hasOwnProperty("url") && item.hasOwnProperty("param") && item.hasOwnProperty("success") });

            //            if (OffLine_List.length == 0) {
            //                clearInterval(OffLine_Timer);
            //                OffLine_Timer = null;
            //            }

            if (OffLine_List.length > 0) {
                for (var index = 0; index < OffLine_List.length; index++) {
                    $.OffLine_AutoPostAjax({
                        list_index: index,
                        url: OffLine_List[index].url,
                        data: OffLine_List[index].param,
                        success: OffLine_List[index].success
                    })
                }
            }

        }

        if (!window.navigator.onLine) {  //断网状态
            localStorage["OffLine_IntervalPostOffLineState"] = false;
        }
    }, 1000);

}

//重新发送因为离线未保存的请求
$.OffLine_AutoPostAjax = function (options) {
    var settings = {
        type: "POST",
        data: null,
        url: "",
        success: function (data) { }
    }
    $.extend(settings, options);
    //如果执行before函数返回false就return;
    //if (settings.before(settings) == false) return;

    var ajax_Time = $.OffLine_getNowTime();

    $.ajax({
        type: settings.type,
        url: settings.url,
        data: settings.data,
        success: settings.success,
        complete: function (jqXHR, textStatus) {
            //判断网络是否断开
            if (!window.navigator.onLine) {
                var response = JSON.parse(jqXHR.response);
                var msg = response.Msgs[0] || "";
                if (msg.indexOf("网络") != -1) {
                    return;
                }

                var obj = new Object();
                obj.Time = settings.ajaxtime;
                obj.url = settings.url;
                obj.param = settings.data;
                obj.success = settings.success;
                $.OffLine_InsertOffLineData(obj);
            }
        }
    })

    delete OffLine_List[settings.list_index].url;
    delete OffLine_List[settings.list_index].param;
    delete OffLine_List[settings.list_index].success;
    delete localStorage.removeItem(OffLine_List[settings.list_index].key);
    delete OffLine_List[settings.list_index].key;
}

$.OffLine_getNowTime = function () {
    //当前时间
    return new Date().getFullYear().toString() + (new Date().getMonth() + 1 < 10 ? "0" + (new Date().getMonth() + 1) : (new Date().getMonth() + 1).toString())
        + (new Date().getDate() < 10 ? "0" + new Date().getDate() : new Date().getDate().toString()) + (new Date().getHours() < 10 ? "0" + new Date().getHours() : new Date().getHours().toString())
        + (new Date().getMinutes() < 10 ? "0" + new Date().getMinutes() : new Date().getMinutes().toString()) + (new Date().getSeconds() < 10 ? "0" + new Date().getSeconds() : new Date().getSeconds().toString())
        + (new Date().getSeconds() < 10 ? "0" + new Date().getSeconds() : new Date().getSeconds().toString())

}

$(function () {
    $.OffLine_SetIntervalPostOffLineData();
});

 
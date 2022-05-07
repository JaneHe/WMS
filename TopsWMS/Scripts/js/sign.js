
var signCount = 1;//页面加载时调用信号强度
var dsRefreshSignal = ''; //定时刷新信号变量
var signDateTime = 15000; //设置信号定时刷新时间
var dsRefreshWorkList = '';  //定时刷新未出站工作机台列表
var workListDateTime = 30000;//设置未出站工作机台列表定时刷新时间


/******************** 定时刷新信号 ********************/
//开始定时刷新信号
function f_signalStart() {
    dsRefreshSignal = setInterval(f_signalIntensity, signDateTime);
}
//结束定时刷新
function f_signalEnd() {
    clearInterval(dsRefreshSignal);
}
//定时刷新调用
function f_signalIntensity() {
    var args = new Object();
    args.seltype = "SelSignalIntensity";
    args.ts = f_signalIntensitytsCallback;
    args.fc = f_signalIntensityfcCallback;
    f_ommonAjaxSign(args);
}
//定时刷新信号强度不好回调函数
function f_signalIntensityfcCallback(reuls) {
    if (reuls.Data.length > 0) {

        $('.sign').css("background-color", "#00ff21");
        if (signCount == 0) {
            f_commonShowMsg("网络重新连接了!");
        }
        signCount = 1;
    }
}
//定时刷新信号强度良好回调函数
function f_signalIntensitytsCallback() {
    $('.sign').css("background-color", "white");
    if (signCount == 1) {
        f_commonShowMsg("网络状态不好,请注意!");
    }
    signCount = 0;
}
//点击时调用
function f_signalBtnClick() {
    var args = new Object();
    args.seltype = "SelSignalIntensity";
    args.ts = f_signalBtnClicktsCallback;
    args.fc = f_signalBtnClickfcCallback;
    f_ommonAjaxSign(args);
}
//点击网络中断时回调函数
function f_signalBtnClicktsCallback() {
    $('.sign').css("background-color", "white");
    f_commonShowMsg("网络状态不好,请注意!");
}
//点击网络通畅时回调函数
function f_signalBtnClickfcCallback() {
    $('.sign').css("background-color", "#00ff21");
    f_commonShowMsg("网络状态良好!");
}
//查询查询信号对象执行方法
function f_ommonAjaxSign(args) {
    var url = '/AjaxGet/' + args.seltype;
    $.ajax({
        type: "Get",
        url: url,
        timeout: 1500, //1秒内返回数据
        data: args.param,
        success: function (data) {
            var data = $.parseJSON(data);
            if (data.IsSuccess) {
                //查询成功回调方法
                args.fc(data, args);
            } else {
                f_commonShowMsg(data.Msgs[0]);
            }
        },
        error: function (XMLHttpRequest, textStatus) {
            if (textStatus == 'timeout') {
                args.ts();
            }

        }
    });
}

/******************** 定时未出站工单列表 ********************/
//开始定时刷新未出站工作机台列表
function f_timingListStart() {
    dsRefreshWork = setInterval(f_openWorkStands, workListDateTime);
}
//结束定时刷新未出站工作机台列表
function f_timingListEnd() {
    clearInterval(dsRefreshWork);
}

//刷新打开折叠的工作站
function f_openWorkStands() {
    openWS.length = 0;
    for (var i = 0; i < WSCount.length; i++) {
        if (!$('#workDetil' + WSCount[i].Id).hasClass('ui-collapsible-collapsed')) {
            openWS.push({ "Id": WSCount[i].Id });
        }
    }

    f_selAllFinishProgram();

    f_commonShowMsg('刷新成功!');

}
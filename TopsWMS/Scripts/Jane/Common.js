////使用方法 
//var d = new dateHelper();
//var chazhi = d.datediff('2015-07-28', '2015-07-29');


///日期帮助类
function dateHelper() {
    this.pdays = [0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    this.rdays = [0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
}

$.extend(dateHelper.prototype, {
    nextday: function () {
        var array = arguments[0].split("-");
        var days = this.pdays;
        if (array[0] % 4 == 0 && array[0] % 400 != 0)
            days = this.rdays;
        if (array[1] == 12 && array[2] == days[12]) {
            array[0] = parseInt(array[0]) + 1;
            array[1] = "01";
            array[2] = "01";
        } else if (array[2] == days[parseInt(array[1])]) {
            array[1] = parseInt(array[1]) + 1 < 10 ? "0" + (parseInt(array[1]) + 1) : parseInt(array[1]) + 1;
            array[2] = "01";
        } else {
            array[2] = parseInt(array[2]) + 1 < 10 ? "0" + (parseInt(array[2]) + 1) : parseInt(array[2]) + 1;
        }
        return array.join("-");
    },
    prevday: function () {
        var array = arguments[0].split("-");
        var days = this.pdays;
        if (array[0] % 4 == 0 && array[0] % 400 != 0)
            days = this.rdays;
        if (array[1] == 1 && array[2] == 1) {
            array[0] = parseInt(array[0]) - 1;
            array[1] = "12";
            array[2] = "31";
        } else if (array[2] == 1) {
            array[1] = parseInt(array[1]) - 1 < 10 ? "0" + (parseInt(array[1]) - 1) : parseInt(array[1]) - 1;
            array[2] = days[parseInt(array[1])];
        } else {
            array[2] = parseInt(array[2]) - 1 < 10 ? "0" + (parseInt(array[2]) - 1) : parseInt(array[2]) - 1;
        }
        return array.join("-");
    },
    nextdays: function () {
        var result = arguments[0];
        for (var index = arguments[1]; arguments[1] > 0; arguments[1]--) {
            result = this.nextday(result);
        }
        return result;
    },
    prevdays: function () {
        var result = arguments[0];
        for (var index = arguments[1]; arguments[1] > 0; arguments[1]--) {
            result = this.prevday(result);
        }
        return result;
    },
    datediff: function () {
        var startdate = new Date(arguments[0]);
        var enddate = new Date(arguments[1]);
        return (enddate - startdate) / 1000 / 60;
    },
    getnowdate: function () {
        return new Date().getFullYear().toString() + "-" + (new Date().getMonth() + 1 < 10 ? "0" + (new Date().getMonth() + 1) : (new Date().getMonth() + 1).toString())
        + "-" + (new Date().getDate() < 10 ? "0" + new Date().getDate() : new Date().getDate().toString())
    },
    getwhichweek: function () {
        //参数没有传递，那么更具当前时间进行查询
        var seldate = arguments[0] || new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
        //第一周周末(星期日)
        var firstweekday;
        //一天的毫秒数
        var onedaytime = 86400000;
        //返回的周数
        var week;

        //获取本年度的第一天
        var firstdayofyear = new Date(seldate.getFullYear(), 0, 1, seldate.getHours(), seldate.getMinutes(), seldate.getSeconds());
        //第一天是星期几
        var firstdayforweek = firstdayofyear.getDay();
        //如果为0=周日
        if (firstdayforweek == 0) firstdayforweek = 7;
        firstweekday = firstdayofyear.getTime() + (7 - firstdayforweek) * onedaytime;
        if (firstdayforweek > 4) {
            //本年度的第一天大于周四，即这周的有4天以上的时间是属于上一年的，那么
            //这一周是属于上一年的，所以要加上一周的时间，才能算出第一周的周末来
            firstweekday += 7 * onedaytime;
        }
        //当前天跟第一周周末相差的天数
        var diffday = Math.round((seldate.getTime() - firstweekday) / onedaytime);

        if (diffday <= 0) {
            //当前查询天与第一周周末相差天数
            if (diffday <= -7) {
                //相差超过7天，那么就当前天是属于上一年的最后一周
                //查询上一年最后一周
                var lastyear = new Date(new Date(seldate.getFullYear(), 0, 1).getTime() - onedaytime);
                week = f_GetWeek(lastyear);
            } else {
                week = 1;
            }
        } else {
            week = Math.ceil(diffday / 7);
            week = week + 1;
        }

        //最后一周判断
        if (week == 53) {
            //获取年度的最后一周
            var lastday = new Date(new Date(seldate.getFullYear() + 1, 0, 1).getTime() - onedaytime);
            if (lastday.getDay() < 4) {
                //如果一年中的最后周所占天数少于4天，减少一周
                week = 1
            }
        }
        return week;
    },
    getUTCDayText: function () {
 
        switch (new Date(arguments[0]).getUTCDay()) {
            case 1:
                return "周一";
                break;
            case 2:
                return "周二";
                break;
            case 3:
                return "周三";
                break;
            case 4:
                return "周四";
                break;
            case 5:
                return "周五";
                break;
            case 6:
                return "周六";
                break;
            case 0:
                return "周日";
                break;
        }
    }
})
///日期帮助类

///Ajax请求帮助类
function ajaxHelper() {

}

$.extend(ajaxHelper.prototype, {
    //查询请求
    ajax_Get: function () {
        //请求对象名称
        var url = arguments[0];
        //请求对象参数
        var data = arguments[1];

        var obj = arguments;
        var ajax = $.ajax({
            type: "GET",
            url: "/AjaxGet/" + url,
            data: data,
            success: function (result) {
                result = JSON.parse(result);
                if (result.IsSuccess) {
                    //成功事件
                    obj[2](result);
                } else {
                    //失败事件
                    obj[3](result);
                }
            }
        })
        return ajax;
    },
    //新增修改请求
    ajax_Post: function () {
        //请求对象名称
        var url = arguments[0];
        //请求对象参数
        var data = arguments[1];

        var obj = arguments;
        var ajax = $.ajax({
            type: "POST",
            url: "/AjaxPost/" + url,
            data: data,
            success: function (result) {
                result = JSON.parse(result);
                if (result.IsSuccess) {
                    //成功事件
                    obj[2](result);
                } else {
                    //失败事件
                    obj[3](result);
                }
            }
        })
        return ajax;
    }
});
///Ajax请求帮助类


///手持终端使用,禁止弹出手机键盘
function f_preventKeyBoard() {
    $(document.body).find("input[type=text]").live("focus",function () {
        $(this).attr("readonly", "readonly");
        var nowfocus = $(this);
        setTimeout(function () {
            nowfocus.removeAttr("readonly");
        }, 100);
    });

    $(document.body).find("input[type=text]").live("click",function () {
        $(this).attr("readonly", "readonly");
        var nowfocus = $(this);
        setTimeout(function () {
            nowfocus.removeAttr("readonly");
        }, 100);
    }); 
}
///手持终端使用,禁止弹出手机键盘


///手持终端使用 提示消息
var msgtimer;
function f_commonShowMsg(msg, container) {
    setTimeout(function () {
        if (msgtimer) {
            clearTimeout(msgtimer);
            $(".msgdiv").hide().remove();
        }
        var msgdiv = $('<div id="msgdiv" class="msgdiv">' + msg + '</div>');
       
        //移动端调整
        msgdiv.css({
            "display": "none",
            "position": "fixed",
            "min-width": "calc(100%)",
            "width": "calc(100%)",
            "min-height": "35px",
            "line-height": "35px",
            "background": "#E5EFFE",
            "padding": "0px 0px",
            "color": "white",
            "background-color": "black",
            "top": "0px", 
            "z-index": "1000",
            "text-align": "center",
            "font-weight": "bold",
            "border-radius": "0px",
            "box-shadow": "0px 2px 5px 2px gray",
            "font-size": "14px",
            "font-family": "微软雅黑",
            "opacity": "0.9"
        });
        //end

        if (typeof (container) != 'undefined') {
            if ($("#" + container).length > 0) {
                $("#" + container).append(msgdiv);
            } else if ($("." + container).length > 0) {
                $("." + container).append(msgdiv);
            } else {
                $(document).append(msgdiv);
            }
        } else {
            $("body").append(msgdiv);
        }
        //msgdiv.css("margin-left", -Math.floor(msgdiv.width() / 2) + "px");
        // if (msgdiv.width() == 600) {
        //     msgdiv.css("line-height", "25px");
        // }

        msgdiv.fadeIn(function () {
            msgtimer = setTimeout(function () {
                msgdiv.fadeOut(function () {
                    msgdiv.remove();
                });
            }, 1500);
        });
    }, 900);
}

///手持终端使用 提示消息


///验证帮助类 
function redisHelper() {
}

$.extend(redisHelper.prototype, {
    //验证方式(适用于email,手机号码,正小数,IP地址,电话号码)
    redis_format: function () {

        var redis; //验证方式
        var format = $(this).attr("format");
        switch (format) {
            case "telephone":
                redis = /^0?(13[0-9]|15[012356789]|18[0236789]|14[57])[0-9]{8}$/
                break;
            case "smallcou":
                redis = /^[0-9]+(.[0-9]{1,3})?$ /
                break;
            case "IP":
                redis = /^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])(\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])){3}$/;
                break;
            case "phone":
                redis = /^((\(\d{2,3}\))|(\d{3}\-))?13\d{9}$/;
                break;
        }

        var zhi;

        //获取控件类型
        if (this.nodeName.toLowerCase() == "input") {
            zhi = $(this).val();
        } else if (this.nodeName.toLowerCase() == "select") {
            zhi = $(this).find("option:selected").text();
        }

        if (!redis.test(zhi)) {
            $(this).css("border", "1.5px solid red");
        } else {
            $(this).css("border", "");
        }
    },
    //空值判断验证
    redis_isnull: function () {
        var zhi;

        //获取控件类型
        if (this.nodeName.toLowerCase() == "input") {
            zhi = $(this).val();
        } else if (this.nodeName.toLowerCase() == "select") {
            zhi = $(this).find("option:selected").text();
        }

        if (zhi.trim() == "") {
            $(this).css("border", "1px solid red");
        } else {
            $(this).css("border", "");
        }
    }
})

//页面加载完成后执行的方法
//window.onload = function () {
//    
//    $(document.body).find("input[format]").blur(redis.redis_format);
//    $(document.body).find("input").blur(redis.redis_isnull);
// 
//}

///验证帮助类



///数字帮助类

function MathHelper() {

}

$.extend(MathHelper.prototype, {
    //转换成千分位的数字
    math_ChangebyQF: function (num) {
        var arr = new Array();
        var number = "";
        if (num.toString().substring(0, 1) == "-") {
            number = "-";
            num = num.toString().replace("-", "");
        }
        var len = num.toString().length;
        while (len > 3) {
            arr.push(num.toString().substring(len - 3, len) + ",");
            len -= 3;
        }
        if (len != 0) {
            arr.push(num.toString().substring(0, len) + ",");
        }
        var result = "";
        for (var a = arr.length - 1; a >= 0; a--) {
            result += arr[a];
        }
        return number + result.substring(0, result.length - 1);
    }
})


///数字帮助类



///获取某个div里的控件值
$.fn.has_formdatas = function () {
    var data = arguments[0];  //数组  data
    $(this).find("input[name]").each(function (index, value) {
        data[$(value).attr("name")] = $(value).val();
    })
    $(this).find("select[name]").each(function (index, value) {
        data[$(value).attr("name")] = $(value).find("option:selected").text();
    })
    return data;
}

//js获取项目根路径，如： http://localhost:8083/uimcardprj  
function f_getRootPath() {
    //获取当前网址，如： http://localhost:8083/uimcardprj/share/meun.jsp  
    var curWwwPath = window.document.location.href;
    //获取主机地址之后的目录，如： uimcardprj/share/meun.jsp  
    var pathName = window.document.location.pathname;
    var pos = curWwwPath.indexOf(pathName);
    //获取主机地址，如： http://localhost:8083  
    var localhostPaht = curWwwPath.substring(0, pos);
    //获取带"/"的项目名，如：/uimcardprj  
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    return (localhostPaht + projectName);
}


/*
*功能:获取当前周
*    这里是以周一为一周的开始，一年的第一周和最后一周的判断，以所属年的天数多的为准，如2014年的最后一周有3天
*    是属于2014年的，小于4天，所以这一周是就是2015年的第一周，如此类推
*参考效果:http://www.jb51.net/w3school/tiy/t.asp-f=html5_form_week.htm
*参数:nullldate,不传默认为当前时间/根据传入的时间查询周
*返回:查询出来的周数
*/
function f_GetWeek(seldate) {
    //参数没有传递，那么更具当前时间进行查询
    var seldate = seldate || new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    //第一周周末(星期日)
    var firstweekday;
    //一天的毫秒数
    var onedaytime = 86400000;
    //返回的周数
    var week;

    //获取本年度的第一天
    var firstdayofyear = new Date(seldate.getFullYear(), 0, 1, seldate.getHours(), seldate.getMinutes(), seldate.getSeconds());
    //第一天是星期几
    var firstdayforweek = firstdayofyear.getDay();
    //如果为0=周日
    if (firstdayforweek == 0) firstdayforweek = 7;
    firstweekday = firstdayofyear.getTime() + (7 - firstdayforweek) * onedaytime;
    if (firstdayforweek > 4) {
        //本年度的第一天大于周四，即这周的有4天以上的时间是属于上一年的，那么
        //这一周是属于上一年的，所以要加上一周的时间，才能算出第一周的周末来
        firstweekday += 7 * onedaytime;
    }
    //当前天跟第一周周末相差的天数
    var diffday = Math.round((seldate.getTime() - firstweekday) / onedaytime);

    if (diffday <= 0) {
        //当前查询天与第一周周末相差天数
        if (diffday <= -7) {
            //相差超过7天，那么就当前天是属于上一年的最后一周
            //查询上一年最后一周
            var lastyear = new Date(new Date(seldate.getFullYear(), 0, 1).getTime() - onedaytime);
            week = f_GetWeek(lastyear);
        } else {
            week = 1;
        }
    } else {
        week = Math.ceil(diffday / 7);
        week = week + 1;
    }

    //最后一周判断
    if (week == 53) {
        //获取年度的最后一周
        var lastday = new Date(new Date(seldate.getFullYear() + 1, 0, 1).getTime() - onedaytime);
        if (lastday.getDay() < 4) {
            //如果一年中的最后周所占天数少于4天，减少一周
            week = 1
        }
    }
    return week;
}

 
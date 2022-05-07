

function WarmMsg() {
    var msg_warm_div = document.createElement("div");
    msg_warm_div.id = "msg_warming";
    var msg_alert_div = document.createElement("div");
    msg_alert_div.id = "msg_alert";
    var msg_warm_a = document.createElement("a");
    msg_warm_a.id = "msg_close";
    msg_warm_a.text = "X";
    msg_warm_div.appendChild(msg_alert_div);
    msg_warm_div.appendChild(msg_warm_a);

    $(msg_warm_div).css({ "width": "auto",
        "background": "#00a2d4",
        "min-height": "50px",
        "max-height": "50px",
        "position": "absolute",
        "top": "20%",
        "left": "30%",
        "height": "auto",
        "display": "table",
        "box-shadow": "8px 8px 20px #888888"
    })
    $(msg_warm_a).css({ "position": "absolute",
        "top": "-5px",
        "right": "0",
        "font-family": "微软雅黑",
        "font-size": "15px",
        "font-weight": "bold",
        "cursor": "pointer"
    })
    $(msg_alert_div).css({ "margin": "20px 50px",
        "font-family": "微软雅黑",
        "font-size": "20px",
        "color": "white",
        "font-weight": "bold"
    })

    //关闭时间
    $(msg_warm_a).click(function () {
        $(msg_warm_div).fadeOut();
    })

    this.msg_warm_div = msg_warm_div;

}

//显示弹出框
WarmMsg.prototype.show = function (text) { 
    if ($(document.body).find(this.msg_warm_div).html() == null) {
        $(document.body).append(this.msg_warm_div);
    }

    $(this.msg_warm_div).children("div").text(text);
    $(this.msg_warm_div).delay("slow").fadeIn("slow");
    $(this.msg_warm_div).delay(3000).fadeOut("slow");
}
     

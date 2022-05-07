//调用方法 在Html标签中加入 onclick="launchFullscreen(document.documentElement);

//全屏
function launchFullscreen(element) {
    if (typeof ($(document.body).attr("fullscreenstate")) == "undefined") {
        $(document.body).attr("fullscreenstate", "no");
    }
    if ($(document.body).attr("fullscreenstate") == "no") {//变量判断 yes：进入全屏  no：退出全屏
        //进入全屏
        if (element.requestFullscreen) {
            element.requestFullscreen();
        } else if (element.mozRequestFullScreen) {
            element.mozRequestFullScreen();
        } else if (element.webkitRequestFullscreen) {
            element.webkitRequestFullscreen();
        } else if (element.msRequestFullscreen) {
            element.msRequestFullscreen();
        }
        $(document.body).attr("fullscreenstate", "yes");
    } else {
        //退出全屏
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
        else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        }
        else if (document.webkitCancelFullScreen) {
            document.webkitCancelFullScreen();
        }
        else if (document.msExitFullscreen) {
            document.msExitFullscreen();
        }
        $(document.body).attr("fullscreenstate", "no");
    }
}
////使用方法
//var m = new Mask();
//m.show();

//遮罩层
function Mask() {
    //遮罩层DIV
    var maskdiv = document.createElement("div");
    maskdiv.setAttribute("id", "auto_mask")
    $(maskdiv).css("z-index", "9999").css("left", "0px").css("position", "absolute").css("top", "0px").css("display", "none");
    $(maskdiv).css("background-color", "#000").css("-moz-opacity", "0.3").css("opacity", "0.3").css("filter", "alpha(opacity=30)");
    //loading
    var loadings = document.createElement("img");
    loadings.setAttribute("src", "/TopsLib/Content/loading.gif");
    //页面宽高
    var pageWidth = 0;
    var pageHeight = 0;
    //计算适应性宽高
    pageWidth = document.body.offsetWidth;
    if (document.body.offsetHeight < document.documentElement.scrollTop + document.documentElement.clientHeight)
        pageHeight = document.documentElement.scrollTop + document.documentElement.clientHeight;
    else
        pageHeight = document.body.offsetHeight;
    //遮罩层
    maskdiv.style.width = pageWidth + "px";
    maskdiv.style.height = pageHeight + "px";
    //loading
    loadings.style.position = "absolute";
    loadings.style.left = (pageWidth / 2) + "px";
    loadings.style.top = (pageHeight / 2) + "px";


    //loading
    maskdiv.appendChild(loadings)
    document.body.appendChild(maskdiv);
    //指定属性
    this.mask = maskdiv;
}

//显示遮罩层
Mask.prototype.show = function () {
    $(this.mask).show();
}

//隐藏遮罩层
Mask.prototype.hide = function () {
    $(this.mask).hide();
}

//显示遮罩层多少秒
Mask.prototype.showtime = function (second) {
    var o = this;
    this.show();
    setTimeout(function () {
        o.hide();
    }, second);

}

//隐藏loading，显示遮罩层
Mask.prototype.show_noloading = function () {
    $(this.mask).find("img").hide();
    $(this.mask).show();
}

//隐藏遮罩层
Mask.prototype.hide_noloading = function () {
    $(this.mask).find("img").show();
    $(this.mask).hide();
}
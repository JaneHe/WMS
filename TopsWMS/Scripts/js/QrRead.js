var QrRead = new Object();
QrRead.Mx = "";
    clearFlag = null;
    vcount = 0;
    context = "";
    vid = "";
    func = null;
    interval_fun = function ()
    {
        var tx = $("#" +vid).val();
        if (tx != "") {
            if (vcount == 0) {
                vcount++;
                context = tx;
            }
            else {
                if (tx != context) {
                    context = tx;
                    vcount++;
                }
                else {
                    clearInterval(clearFlag);
                    var v= context;
                    var arr = v.split("*");
                    $("#" + vid).val(arr[0]);
                    context = arr[0];
                    if (arr.length > 1) {
                        func(v);
                        QrRead.Mx = v;
                    }
                    vcount = 0;
                    clearFlag = setInterval(interval_fun, 200);
                }
            }
        }
    }
    QrRead.Bind = function (id,fun)
    {
        vid = id;
        func = fun;
        clearFlag = setInterval(interval_fun, 200);
    }
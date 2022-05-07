
//判定是否有存在空的标准值
function f_IsExistsNullForOrderPrint() {

    //判定是否有存在空的标准值，有的话为true,没有的话为false
    var IsExists = false;
    //成型工程 成型方式所在横向单元格的位置
    var tdindex = -1;
    //成型工程 成型方式所在纵向单元格的位置
    var trindex = -1;

    //空项目 所在横向单元格的位置
    var nullobjecttdindex = "";

    //空项目 所在纵向单元格的位置
    var nullobjecttrindex = "";


    //遍历打印Table的每一行，如果有标准值，则检查是否存在空的标准值
    $(document.body).find("table:visible").each(function (index, value) {
        $(value).find("tr").each(function (i, v) {
            //获取第一个td单元格的值
            var str = $(v).find("td:eq(0)").text();

            //识别项目一行的字段
            if (str.indexOf("项目") != -1) {

                $(v).find("td").each(function (i1, v1) {
                    var s = $(v1).text();
                    //匹配上成型方式时，直接获取相应的位置
                    if (s.indexOf("编带") != -1 || s.indexOf("散装") != -1 || s.indexOf("长脚") != -1 || s.indexOf("座板") != -1) {
                        tdindex = i1;
                        trindex = i + 1;
                    } else if (s.trim() == "") {
                        nullobjecttdindex += i1 + ",";
                        nullobjecttrindex += (i + 1) + ",";
                    }
                })
            }

            //通常标准列第一列都是标准字样
            if (str.indexOf("标准") != -1 || str.indexOf("℃") != -1) {

                $(v).find("td").each(function (i1, v1) {

                    //判定该单元格是否是空标准值(除成型方式（编带，散装长脚，座板允许为空,空项目）之外)
                    if ($(v1).text() == "" && !((tdindex == i1 && trindex == i) || (nullobjecttdindex.indexOf(i1 + ",") > -1 && nullobjecttrindex.indexOf(i + ",") > -1))) {
                        IsExists = true;
                        return false;
                    }
                })
            }

            if (IsExists) return false;
        })

        if (IsExists) return false;
    })

    //存在空值时给予提示
    if (IsExists) {
        alert("该订单打印界面存在空的标准值，请填写后再进行打印!");
        return false;
    }

    return true;
}
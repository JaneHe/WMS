  window.onload = function () {
        document.getElementById('text').focus();
    }
    document.onkeydown = function (e) {
        if (!e) e = window.event;//»ðºüÖÐÊÇ window.event
        if (e.keyCode == 13) {
            document.getElementById("text").click();
            var v = document.getElementById("text").value;
           // window.location.href = "../JQ-M?id=" + v;
            if (v != "") {
                document.location = "../JQ-M?id=" + v;
            }
        }
    }
//调用方式
//<script src="../../Scripts/JSobject/SpeechRadio.js" type="text/javascript"></script>
//var speech = new SpeechRadio({ name: "test", format: "mp3", text: "电镀项目测试", language: "chs" });
//speech.listen();

function SpeechRadio() {
    var speech = new Object();
    speech.options = {
        name: "",
        format: "mp3",
        text: "",
        language: ""
    }

    $.extend(speech.options, arguments[0]);

    speech.create = function () {
        var $audio = $("<audio></audio>");
        var $source = $("<source></source>");
        $audio.attr("controls", "controls").attr("preload", "load").hide();
        switch (speech.options.format) {
            case "wav":
                speech.options.formation = "audio/wav";
                break;
            case "mp3":
                speech.options.formation = "audio/mpeg";
                break;
        }
        $source.attr("type", speech.options.formation);
        $audio.append($source);
        speech.targets = $audio;
        $(document.body).append($audio);

        $audio.find("source").attr("src", "../../Scripts/Radio/" + speech.options.name + "." + speech.options.format);
    }

    speech.loadSrc = function () {
        $.ajax({
            type: "GET",
            url: "/Speech/Save",
            data: speech.options,
            async: false, //不异步处理
            success: function (data) {
                if (data.IsSuccess) {
                } else {
                    //alert(data.Msgs[0]);
                }
            }
        })
    }
    speech.listen = function () {
        debugger;
        var audio = speech.targets[0];
        //audio.pause();
        audio.play();
    }


    speech.create();
    speech.loadSrc();

    return speech;
}

 
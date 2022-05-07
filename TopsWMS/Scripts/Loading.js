function ShowLoading(LoadingObj) {
    if (LoadingObj.Count == 0) {
        $("#loading,.coverDiv").show();
    }
    LoadingObj.Count++;
}
function HideLoading(LoadingObj) {
    LoadingObj.Count--;
    if (LoadingObj.Count <= 0) {
        $("#loading,.coverDiv").hide();
        LoadingObj.Count = 0;
    }
}
function showAlert(msg, isAutoClose) {
    isAutoClose = isAutoClose || false;
    $(".coverDiv,.dialog,#msgEnter,#msgClose").show();
    //$(".dialog,#msgEnter,#msgClose").show();
    $("#msgCancel,#msgEnter1").hide();
    $("#msgContent").html(msg);
    $(".dialog").position({
        of: $("body"),
        at: "center center",
        my: "center center"
    });
    if (isAutoClose) {
        setTimeout("$('.coverDiv,.dialog').hide();", 2000);
    }
}

function showConfirm(msg) {
    $(".coverDiv,.dialog,#msgCancel,#msgEnter1").show();
    $("#msgEnter,#msgClose").hide();
    $("#msgContent").html(msg);
}

function closeAlert() {
    $(".coverDiv,.dialog").hide();
}
$(function () {
    setInterval(loginDisplay, 300000);
});

function loginDisplay() {
    $.ajax({
        type: "post",
        url: "/Account/GetLoginUser",

        success: function (response) {
            $("#onlineUsers").html(response);
        }
    });
}

//Should be followed by <div id="onlineUsers"></div>  
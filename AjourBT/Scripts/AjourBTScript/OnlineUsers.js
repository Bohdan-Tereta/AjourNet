$(document).on("click", "#onlineUsersDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Online Users";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "700px",
            resizable: false,
            title: 'Online Users',
            position: {
                my: "center",
                at: "center"
            },

        });
    });

    return false;
});
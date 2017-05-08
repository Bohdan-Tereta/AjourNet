$(document).on("click", "a#BTPerEmployee", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Last BT Data";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var updateID = element.attr('data-updateid');
    $(dialogDiv).load(url, function () {
            $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Last BT",
            position: {
                my: "center",
                at: "center"
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
    });
    return false;
});


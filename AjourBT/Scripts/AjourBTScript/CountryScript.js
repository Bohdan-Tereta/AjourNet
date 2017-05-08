var country;
var coun;
$(document).on('click', '#CreateCountry', function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = 'Create Country';
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');

    country =  $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            rasizable: false,
            title: 'Create Country',
            position: {
                my: 'center',
                at: 'center'
            },
            buttons: {
                "Save": function () {
                    $('#createCountryForm').submit();
                }
            },
            close: function (event, ui) {
                $(this).dialog('destroy');
                $(this).remove();
            }
        })
        $.validator.unobtrusive.parse(this);
    })
    return false;
})

$(document).on("click", ".countryEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Country"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    coun = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Country",
            position: {
                my: "center",
                at: "center"
            },
            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveCountry, #btnDeleteCountry").button();

        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeleteCountry", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteCountry-Confirm").load(url, function () {

        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: title,
            position: {
                my: "center",
                at: "center"
            },
            buttons:{
                "OK": {
                    text: 'OK',
                    id: 'OKDelete',
                    click: function () {
                            $('#deleteConfirmedForm').submit();

                            $(this).dialog("destroy");
                            $(this).remove();

                            $(coun).dialog("close");
                            $(coun).remove();
                    }
                },

                "Cancel": function ()
                {
                    $(this).dialog("destroy");
                    $(this).remove();

                    $(coun).dialog("close");
                    $(coun).remove();
                }
            }
        });
    });
    return false;
});

$(document).on("click", "#btnCancelDeleteCountry", function (event) {
    $("#deleteCountry-Confirm").dialog("close");
});


AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

function CountryCreateOnSuccess(data) {
    if (data.search("CountryData") != -1) {
        $('#CountryData').replaceWith($(data))

        $(country).dialog("close");
        $(country).remove();
    }
    else {
        $(country).html(data);
    }
}

function CountryEditOnSuccess(data) {
    if (data.error) {
        $("#CountryModelError").html(data.error);
    }
    else if (data.search("CountryData") != -1) {
        $('#CountryData').replaceWith($(data))

        $(coun).dialog("close");
        $(coun).remove();
    }
    else {
        $(coun).find("table").html($(data).find("table"));
    }
}
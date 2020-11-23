var JSHelper = {
    //model: AJAXModel_Modal
    Modal_Open: function (model) {
        var box = $("#" + model.idboxrender);

        $.ajax({
            type: "POST",
            data: model,
            url: model.url,
            success: function (response) {
                box.html(response);
                $("#" + model.idmodal).modal("show");

                JSLoader.init();
            }
        });
    },

    //model: AJAXModel_Modal
    Modal_Close: function (model) {
        $("#" + model.idmodal).modal("hide");
    },

    //submit async
    AJAX_SubmitAsync: function (url, model) {
        return $.ajax({
            type: "POST",
            datatype: "json",
            data: model,
            url: url,
            enctype: 'multipart/form-data',
            processData: false,
            contentType: false
        });
    },

    //submit sync
    AJAX_SubmitSync: function (url, model) {
        return $.ajax({
            type: "POST",
            datatype: "json",
            data: model,
            url: url,
            enctype: 'multipart/form-data',
            processData: false,
            contentType: false,
            async: false
        });
    },

    //delete
    AJAX_Delete: function (url, id) {
        return $.ajax({
            type: "POST",
            datatype: "json",
            data: { id: id },
            url: url
        });
    },

    //request
    AJAX_SendRequest: function (url, model) {
        return $.ajax({
            type: "POST",
            datatype: "json",
            data: { model: model },
            url: url
        });
    },

    //load html
    AJAX_LoadData: function (url, model) {
        return $.ajax({
            url: url,
            type: 'GET',
            data: model
        });
    }
}


$(function () {
    $('body').on('click', '.btnCreateNewCustomer', function () {
        bootbox.prompt({
            title: "Tên khách hàng?",
            centerVertical: true,
            callback: function (result) {
                MN_CustomerController.Create(result);
            }
        });
    });

    $('body').on('click', '.btnNewContact', function () {
        MN_CustomerController.Contact_New();
    });

    $('body').on('click', '.btnEditContact', function () {
        var cmd = $(this);
        var id = cmd.attr('idata');
        var customerid = cmd.attr('idata1');
        var contactType = cmd.parent().parent().find('#ContactType').val();
        var contactValue = cmd.parent().parent().find('input[name=txtContactValue]').val();
        
        MN_CustomerController.Contact_Save(id, customerid, contactType, contactValue);
    });

    $('body').on('click', '.btnDeleteContact', function () {
        var cmd = $(this);
        var id = cmd.attr('idata');

        MN_CustomerController.Contact_Delete(id);
    });
});

var MN_CustomerController = {
    init() {
        MN_CustomerController.Contact_LoadData();
    },
    Create(name) {
        var model = {
            Name: name
        };

        JSHelper.AJAX_SendRequest('/MN_Customer/CreateNewCustomer', model)
            .success(function (response) {
                if (response.isSuccess) {
                    window.location.href = '/MN_Customer/Update/' + response.Message;
                } else {
                    toastr.error(response.Message);
                }
            })
    },
    Contact_LoadData() {
        var id = $("#hidCustomerId").val();

        JSHelper.AJAX_LoadData('/MN_Customer/CustomerContactPartial', { customerid: id })
            .success(function (response) {
                $('#tblContact tbody').html('');
                $('#tblContact tbody').html(response);
            });
    },
    Contact_New() {
        var model = {
            ContactType: 0,
            Value: "",
            CustomerId: $("#hidCustomerId").val()
        };

        JSHelper.AJAX_SendRequest('/MN_Customer/NewContactPartial', model)
            .success(function (response) {
                $('#tblContact tbody').append(response);
            });
    },
    Contact_Save(id, customerId, contactType, contactValue) {
        var model = {
            Id: id,
            CustomerId: customerId,
            ContactType: contactType,
            Value: contactValue
        };

        JSHelper.AJAX_SendRequest('/MN_Customer/ContactEdit', model)
            .success(function (response) {
                if (response.isSuccess) {
                    toastr.success(response.Message);

                    MN_CustomerController.Contact_LoadData();
                } else {
                    toastr.success(response.Message);
                }
            });
    },
    Contact_Delete(id) {
        JSHelper.AJAX_Delete('/MN_Customer/ContactDelete', id)
            .success(function (response) {
                if (response.isSuccess) {
                    toastr.success(response.Message);

                    MN_CustomerController.Contact_LoadData();
                } else {
                    toastr.success(response.Message);
                }
            });
    }
}
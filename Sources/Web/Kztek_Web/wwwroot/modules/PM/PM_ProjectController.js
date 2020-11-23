$(function () {
    //Click thêm mới

    $('body').on('click', '.btnCreateNewProject', function () {
        bootbox.prompt({
            title: "Tên dự án mới là gì?",
            centerVertical: true,
            callback: function (result) {
                PM_ProjectController.Create(result);
            }
        });
    });

    //Mở modal component

    $('body').on('click', '.btnNewComponent', function () {
        PM_ProjectController.Component_Modal('', $('#hidProjectId').val());
    });

    //Tiến hành thêm

    $('body').on('click', '#btnConfirmComponent', function () {
        PM_ProjectController.Component_Submit();
    });

    //Click chọn nhóm componet cấp cha

    $('body').on('change', '#ModalComponent #ParentId', function () {
        PM_ProjectController.Component_LoadParentComponent($(this).val());
    });

    //Click edit

    $('body').on('click', '.btnEditComponent', function () {
        PM_ProjectController.Component_Modal($(this).attr('idata'), $('#hidProjectId').val());
    });

    //Click tag users

    $('body').on('click', '.btnTagUser', function () {
        PM_ProjectController.Work_Modal($(this).attr('idata'), $('#hidProjectId').val());
    });

    //Click xác nhận tag người

    $('body').on('click', '#btnConfirmWork', function () {
        PM_ProjectController.Work_Submit();
    });

    //Click xóa component

    $('body').on('click', '.btnDeleteComponent', function () {
        var id = $(this).attr('idata');
        bootbox.confirm('Bạn có chắc chắn muốn xóa?', function (result) {
            if (result) {
                PM_ProjectController.Component_Delete(id);
            }
        });
        
    });

    //

    $('body').on('click', '.btnCompleteMComponent', function () {
        var id = $(this).attr('idata');

        bootbox.confirm('Bạn có chắc chắn muốn check component hoàn thành?', function (result) {
            if (result) {
                PM_ProjectController.Component_MComplete(id);
            }
        });
    });

    //Save change

    $('body').on('click', '.btnSaveChangeProject', function () {
        bootbox.confirm('Bạn có chắc chắn lưu thay đổi thông tin của dự án', function (result) {
            if (result) {
                PM_ProjectController.Project_Save();
            }
        });
    });

    //Open mess modal

    $('body').on('click', '.btnSendMessage', function () {
        PM_ProjectController.Message_Modal($('#hidProjectId').val());
    });

    //Send message

    $('body').on('click', '#btnConfirmMessage', function () {
        bootbox.confirm('Bạn có chắc chắn muốn gửi thông báo này tới những người này?', function (result) {
            if (result) {
                PM_ProjectController.Message_Submit();
            }
        });
    });


});

var PM_ProjectController = {
    init() {
        PM_ProjectController.Component_LoadData($('#hidProjectId').val());
    },
    Project_Save() {
        var form = $('#frmProject');

        var formData = new FormData();
        formData.append('Id', form.find('#hidProjectId').val());
        formData.append('Title', form.find('#Title').val());
        formData.append('Description', form.find('#Description').val());
        formData.append('Note', form.find('#Note').val());
        formData.append('DateStart', form.find('#dtpDateStart').val());
        formData.append('DateEnd', form.find('#dtpDateEnd').val());
        formData.append('Status', form.find('#Status').val());

        JSHelper.AJAX_SubmitAsync('/PM_Project/ProjectSubmit', formData)
            .success(function (response) {
                if (response.isSuccess) {
                    toastr.success(response.Message);
                } else {
                    toastr.error(response.Message);
                }
            });
    },
    Component_LoadData(projectid) {
        JSHelper.AJAX_LoadData('/PM_Project/ComponentListPartial', { projectid: projectid })
            .success(function (response) {
                $('#tblComponent tbody').html(response);
            });
    },
    Component_Modal(id, projectid) {
        var model = new AJAXModel_Modal('/PM_Project/ComponentModal', id, 'ModalComponent', 'boxComponentModal', false, id != "" ? "Cập nhật component" : "Thêm mới component", projectid);

        JSHelper.Modal_Open(model);
    },
    Component_Submit() {
        var modal = $("#ModalComponent");

        var formData = new FormData();
        formData.append('Code', modal.find("input[name=componentCode]").val());
        formData.append('Title', modal.find("input[name=componentTitle]").val());
        formData.append('Description', modal.find("textarea[name=componentDescription]").val());
        formData.append('Note', modal.find("textarea[name=componentNote]").val());
        formData.append('DateStart', modal.find("input[name=dtpComponentDateStart]").val());
        formData.append('DateEnd', modal.find("input[name=dtpComponentDateEnd]").val());
        formData.append('ParentId', modal.find("#ParentId").val());
        formData.append('Status', modal.find("#Status").val());
        formData.append('ProjectId', modal.find("#projectid").val());
        formData.append('ComponentId', modal.find("#componentid").val());

        JSHelper.AJAX_SubmitAsync('/PM_Project/ComponentSubmit', formData)
            .success(function (response) {
                if (response.isSuccess) {
                    toastr.success(response.Message);

                    var model = new AJAXModel_Modal('/PM_Project/ComponentModal', modal.find("#componentid").val(), 'ModalComponent', 'boxComponentModal', false, modal.find("#componentid").val() != "" ? "Cập nhật component" : "Thêm mới component", modal.find("#projectid").val());

                    JSHelper.Modal_Close(model);

                    PM_ProjectController.Component_LoadData(modal.find("#projectid").val());
                } else {
                    toastr.error(response.Message);
                }
            })
    },
    Component_LoadParentComponent(id) {
        JSHelper.AJAX_SendRequest('/PM_Project/ComponentParent', { ComponentId: id })
            .success(function (response) {
                if (response != null) {
                    var modal = $("#ModalComponent");
                    modal.find("input[name=componentCode]").val(response.Code);
                }
            })
    },
    Component_Delete(id) {
        JSHelper.AJAX_SendRequest('/PM_Project/ComponentDelete', { ComponentId: id })
            .success(function (response) {
                if (response.isSuccess) {
                    toastr.success(response.Message);

                    PM_ProjectController.Component_LoadData($('#hidProjectId').val());
                } else {
                    toastr.error(response.Message);
                }
            })
    },
    Component_MComplete(id) {
        JSHelper.AJAX_SendRequest('/PM_Project/ComponentMComplete', { ComponentId: id })
            .success(function (response) {
                if (response.isSuccess) {
                    toastr.success(response.Message);

                    PM_ProjectController.Component_LoadData($('#hidProjectId').val());
                } else {
                    toastr.error(response.Message);
                }
            })
    },
    Work_Modal(id, projectid) {
        var model = new AJAXModel_Modal('/PM_Project/WorkModal', id, 'ModalWork', 'boxWorkModal', false, "Tag người thực hiện", projectid);

        JSHelper.Modal_Open(model);
    },
    Work_Submit() {
        var modal = $('#ModalWork');

        var formData = new FormData();
        formData.append('UserIds', modal.find('#slUser').val());
        formData.append("ComponentId", modal.find('#hidComponentId').val());
        formData.append("ProjectId", modal.find('#hidProjectId').val());

        JSHelper.AJAX_SubmitAsync('/PM_Project/WorkSubmit', formData)
            .success(function (response) {
                if (response.isSuccess) {
                    toastr.success(response.Message);

                    var model = new AJAXModel_Modal('/PM_Project/WorkModal', modal.find('#hidComponentId').val(), 'ModalWork', 'boxWorkModal', false, "Tag người thực hiện", modal.find('#hidProjectId').val());

                    JSHelper.Modal_Close(model);

                    PM_ProjectController.Component_LoadData(modal.find("#hidProjectId").val());
                } else {
                    toastr.error(response.Message);
                }
            })
    },
    Message_Modal(projectid) {
        var model = new AJAXModel_Modal('/PM_Project/SendMessageModal', projectid, 'ModalMessage', 'boxMessageModal', false, "Gửi thông báo cho người thực hiện", "");

        JSHelper.Modal_Open(model);
    },
    Message_Submit() {
        var modal = $('#ModalMessage');

        var formData = new FormData();
        formData.append('Id', modal.find('#hidProjectId').val());
        formData.append('Title', modal.find('#messageTitle').val());
        formData.append("Description", modal.find('#messsageDescription').val());
        formData.append("UserIds", modal.find('#slUser').val());

        JSHelper.AJAX_SubmitAsync('/PM_Project/MessageSubmit', formData)
            .success(function (response) {
                if (response.isSuccess) {
                    toastr.success(response.Message);

                    var model = new AJAXModel_Modal('/PM_Project/SendMessageModal', projectid, 'ModalMessage', 'boxMessageModal', false, "Gửi thông báo cho người thực hiện", "");

                    JSHelper.Modal_Close(model);

                } else {
                    toastr.error(response.Message);
                }
            })
    },
    Create(title) {
        var model = {
            Title: title
        };

        JSHelper.AJAX_SendRequest('/PM_Project/CreateNewProject', model)
            .success(function (response) {
                if (response.isSuccess) {
                    window.location.href = '/PM_Project/Update/' + response.Message;
                } else {
                    toastr.error(response.Message);
                }
            })
    }
}
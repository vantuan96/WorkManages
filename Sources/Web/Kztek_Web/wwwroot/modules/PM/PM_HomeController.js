$(function() {
  $("body").on("click", ".btnProjectDetail", function() {
    PM_HomeController.Project_Detail($(this).attr("idata"));
  });

  $("body").on("click", ".btnBackToProjects", function() {
    PM_HomeController.Project_LoadData();
  });

    $("body").on("click", ".btnCompleteComponent", function () {
        var cmd = $(this);

    bootbox.confirm("Bạn chắc chắn là đã hoàn thành công việc?", function(
      result
    ) {
      if (result) {
        PM_HomeController.Component_Complete(
            cmd.attr("idata2"),
            cmd.attr("idata"),
            cmd.attr("idata1")
        );
      }
    });
  });

  $("body").on("click", ".btnAddDiary", function() {
    PM_HomeController.Diary_LoadModal("", $(this).attr("idata"));
  });

  $('body').on('click', '.btnEditDiary', function() {
    PM_HomeController.Diary_LoadModal($(this).attr("idata"), $(this).attr("idata1"));
  });

  $('body').on('click', '.btnRemoveDiary', function() {
    var id = $(this).attr("idata");

    bootbox.confirm('Bạn có chắc chắn muốn xóa', function (result) {
      if (result) {
        PM_HomeController.Diary_Delete(id);
      }
    });
    
  });

  $('body').on('click', '#btnConfirmDiary', function () {
    PM_HomeController.Diary_Submit();
  });

  $('body').on('click', '.btnCompleteTask', function () {
    var id = $(this).attr("idata");

    bootbox.confirm('Bạn có chắc chắn đã hoàn thành công việc này?', function (result) {
      if (result) {
        PM_HomeController.Task_Complete(id);
      }
    });
  });
});

var PM_HomeController = {
  init() {
    PM_HomeController.Project_LoadData();
    PM_HomeController.ScheduleCurrent();
    PM_HomeController.Notification_LoadData();
    PM_HomeController.Task_LoadData();
  },
  Project_LoadData() {
    JSHelper.AJAX_LoadData("/PM_Project/HomePartial", {}).success(function(
      response
    ) {
      $("#boxProjectRender").html(response);
    });
  },
  Project_Detail(projectid) {
    JSHelper.AJAX_LoadData("/PM_Project/HomeComponentPartial", {
      projectid: projectid
    }).success(function(response) {
      $("#boxProjectRender").html(response);
    });
  },
  Component_Complete(projectid, componentid, userid) {
    var model = {
      ProjectId: projectid,
      ComponentId: componentid,
      UserId: userid
    };

    JSHelper.AJAX_SendRequest("/PM_Project/ComponentComplete", model).success(
      function(response) {
        if (response.isSuccess) {
          toastr.success(response.Message);

          PM_HomeController.Project_Detail(projectid);
        } else {
          toastr.error(response.Message);
        }
      }
    );
  },
  OneSignalrRegister(playerid) {
    JSHelper.AJAX_SendRequest("/PM_Project/OneSignalrRegister", {
      PlayerId: playerid
    }).success(function(response) {
      console.log(response);
    });
  },
  ScheduleCurrent() {
    JSHelper.AJAX_LoadData("/WM_Schedule/HomeSchedulePartial", {}).success(
      function(response) {
        $("#boxScheduleRender").html(response);

        PM_HomeController.Diary_LoadData(
          $("#boxScheduleRender #hidScheduleId").val()
        );
      }
    );
  },
  Notification_LoadData() {
    JSHelper.AJAX_LoadData("/SY_Notification/HomeNotification", {}).success(
      function(response) {
        $("#boxNotificationRender").html(response);
      }
    );
  },
  Diary_LoadData(scheduleid) {
    JSHelper.AJAX_LoadData("/WM_Diary/HomeDiaryPartial", {
      scheduleid: scheduleid
    }).success(function(response) {
      $("#boxDiary #tblDiary").append(response);
    });
  },
  Diary_LoadModal(recordid, scheduleid) {
    var model = new AJAXModel_Modal(
      "/WM_Diary/HomeDiaryModal",
      recordid,
      "ModalDiary",
      "boxDiaryModal",
      false,
      recordid != "" ? "Cập nhật nhật ký" : "Thêm mới nhật ký",
      scheduleid
    );

    JSHelper.Modal_Open(model);
  },
  Diary_Submit() {
    var modal = $("#ModalDiary");

    var formData = new FormData();
    formData.append("Title", modal.find("input[name=diaryTitle]").val());
    formData.append(
      "Description",
      modal.find("textarea[name=diaryDescription]").val()
    );
    formData.append("Id", modal.find("#Id").val());
    formData.append("ScheduleId", modal.find("#ScheduleId").val());

    JSHelper.AJAX_SubmitAsync("/WM_Diary/HomeDiarySubmit", formData).success(
      function(response) {
        if (response.isSuccess) {
          toastr.success(response.Message);

          var model = new AJAXModel_Modal(
            "/WM_Diary/HomeDiaryModal",
            modal.find("#Id").val(),
            "ModalDiary",
            "boxDiaryModal",
            false,
            modal.find("#Id").val() != ""
              ? "Cập nhật nhật ký"
              : "Thêm mới nhật ký",
            modal.find("#ScheduleId").val()
          );

          JSHelper.Modal_Close(model);

          PM_HomeController.ScheduleCurrent();
        } else {
          toastr.error(response.Message);
        }
      }
    );
  },
  Diary_Delete(id) {
    JSHelper.AJAX_Delete('/WM_Diary/HomeDiaryDelete', id).success(function (response) {

      if (response.isSuccess) { 
        toastr.success(response.Message);
        PM_HomeController.ScheduleCurrent();
      } else {
        toastr.error(response.Message);
      }
    });
  },
  Task_LoadData() {
    JSHelper.AJAX_LoadData("/WM_Task/HomeTaskPartial", {}).success(function(response) {
      $("#boxTaskRender").html(response);
    });
  },
  Task_Complete(id) {
    JSHelper.AJAX_SendRequest("/WM_Task/HomeCompleteTask", {
      TaskId: id, UserId: ""
    }).success(function(response) {
      if (response.isSuccess) {
        toastr.success(response.Message);

        PM_HomeController.Task_LoadData();
      } else {
        toastr.error(response.Message);
      }
    });
  }
};

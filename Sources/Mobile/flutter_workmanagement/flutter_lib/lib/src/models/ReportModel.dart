class ReportPersonalModel {
  int projectTotal;
  int projectCompleteOnTime;
  int projectCompleteLate;
  int projectDoing;
  int taskTotal;
  int taskCompleteOnTime;
  int taskCompleteLate;
  int taskDoing;

  dynamic projectStatus;
  dynamic taskStatus;

  ReportPersonalModel(
      {this.projectTotal,
      this.projectCompleteOnTime,
      this.projectCompleteLate,
      this.projectDoing,
      this.taskTotal,
      this.taskCompleteOnTime,
      this.taskCompleteLate,
      this.taskDoing,
      this.projectStatus,
      this.taskStatus});

  factory ReportPersonalModel.fromJson(Map<String, dynamic> json) {
    return ReportPersonalModel(
      projectTotal: json["Project_Total"] as int,
      projectCompleteOnTime: json["Project_Completed_onTime"] as int,
      projectCompleteLate: json["Project_Completed_notOnTime"] as int,
      projectDoing: json["Project_NotComplete"] as int,
      taskTotal: json["Task_Total"] as int,
      taskCompleteOnTime: json["Task_Completed_onTime"] as int,
      taskCompleteLate: json["Task_Completed_notOnTime"] as int,
      taskDoing: json["Task_NotComplete"] as int,
      projectStatus: json["ProjectStatus"] as dynamic,
      taskStatus: json["TaskStatus"] as dynamic,
    );
  }
}

class ReportPersonalGrowModel {
  double projectCurrentPercent;
  double projectGrowPercent;
  double taskCurrentPercent;
  double taskGrowPercent;

  ReportPersonalGrowModel(
      {this.projectCurrentPercent, this.projectGrowPercent, this.taskCurrentPercent, this.taskGrowPercent});

  factory ReportPersonalGrowModel.fromJson(Map<String, dynamic> json) {
    return ReportPersonalGrowModel(
      projectCurrentPercent: json["Project_CurrentPercent"] as double,
      projectGrowPercent: json["Project_GrowPercent"] as double,
      taskCurrentPercent: json["Task_CurrentPercent"] as double,
      taskGrowPercent: json["Task_GrowPercent"] as double
    );
  }
}

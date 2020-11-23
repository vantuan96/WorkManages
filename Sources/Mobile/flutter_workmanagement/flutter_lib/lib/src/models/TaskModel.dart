class TaskModel {
  String id;
  String title;
  String description;
  String dateStart;
  String dateEnd;

  TaskModel({this.id, this.title, this.description, this.dateStart, this.dateEnd});

  factory TaskModel.fromJson(Map<String, dynamic> json) {
    return TaskModel(
        id: json['Id'] as String,
        title: json['Title'] as String,
        description: json['Description'] as String,
        dateStart: json['DateStart'] as String,
        dateEnd: json['DateEnd'] as String,
    );
  }
}
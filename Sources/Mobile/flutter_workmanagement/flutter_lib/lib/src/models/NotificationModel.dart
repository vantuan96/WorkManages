class NotificationModel {
  String id;
  String title;
  String description;
  String dateCreated;

  NotificationModel({this.id, this.title, this.description, this.dateCreated});

  factory NotificationModel.fromJson(Map<String, dynamic> json) {
    return NotificationModel(
      id: json['id'] as String,
      title: json['title'] as String,
      description: json['description'] as String,
      dateCreated: json['dateCreated'] as String,
    );
  }
}
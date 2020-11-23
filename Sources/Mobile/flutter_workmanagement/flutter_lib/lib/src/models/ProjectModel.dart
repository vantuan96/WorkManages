class ProjectModel {
  String id;
  String title;
  String description;
  String dateStart;
  String dateEnd;

  ProjectModel({this.id, this.title, this.description, this.dateStart, this.dateEnd});

  factory ProjectModel.fromJson(Map<String, dynamic> json) {
    return ProjectModel(
      id: json['id'] as String,
      title: json['title'] as String,
      description: json['description'] as String,
      dateStart: json['dateStart'] as String,
      dateEnd: json['dateEnd'] as String,
    );
  }
}
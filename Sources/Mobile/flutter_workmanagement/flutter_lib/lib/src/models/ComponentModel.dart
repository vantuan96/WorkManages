class ComponentModel {
  String id;
  String title;
  String code;
  String description;
  String dateStart;
  String dateEnd;
  String dateCreated;
  String note;

  ComponentModel({this.id, this.code, this.title, this.description, this.dateStart, this.dateEnd, this.dateCreated, this.note});

  factory ComponentModel.fromJson(Map<String, dynamic> json) {
    return ComponentModel(
      id: json['id'] as String,
      title: json['title'] as String,
      description: json['description'] as String,
      dateStart: json['dateStart'] as String,
      dateEnd: json['dateEnd'] as String,
      code: json['code'] as String,
      dateCreated: json['dateCreated'] as String,
      note: json['note'] as String
    );
  }
}
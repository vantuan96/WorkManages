class DiaryModel {
  String id;
  String title;
  String description;

  DiaryModel({this.id, this.title, this.description});

  factory DiaryModel.fromJson(Map<String, dynamic> json) {
    return DiaryModel(
      id: json['Id'] as String,
      title: json['Title'] as String,
      description: json['Description'] as String,
    );
  }
}
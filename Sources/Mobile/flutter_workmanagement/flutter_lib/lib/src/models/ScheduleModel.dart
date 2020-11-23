import 'DiaryModel.dart';

class ScheduleModel {
  String id;
  String title;
  String description;
  String dateStart;
  String dateEnd;
  List<dynamic> diaries;

  ScheduleModel({this.id, this.title, this.description, this.dateStart, this.dateEnd, this.diaries});

  factory ScheduleModel.fromJson(Map<String, dynamic> json) {
    return ScheduleModel(
      id: json['Id'] as String,
      title: json['Title'] as String,
      description: json['Description'] as String,
      dateStart: json['DateStart'] as String,
      dateEnd: json['DateEnd'] as String,
      diaries: json['Diaries'] as List<dynamic>
    );
  }
}
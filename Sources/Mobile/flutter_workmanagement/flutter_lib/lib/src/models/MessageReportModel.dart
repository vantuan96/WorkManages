class MessageReportModel {
  bool isSuccess;
  String message;

  MessageReportModel({this.isSuccess, this.message});

  factory MessageReportModel.fromJson(Map<String, dynamic> json) {
    return MessageReportModel(
      isSuccess: json['isSuccess'] as bool,
      message: json['Message'] as String,
    );
  }
}
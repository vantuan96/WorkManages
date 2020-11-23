import 'dart:convert';

import 'package:flutter_lib/flutter_lib.dart';
import 'package:http/http.dart' as http;

class PageScheduleService {
  Future<http.Response> getCurrentScheduleByFist(
      {String userId, String token = ""}) async {
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/schedule/getcurrentschedule/$userId',
        token: token);
  }

  Future<http.Response> addDiary(
      {String userId, String scheduleId, String title, String description, String token = ""}) async {
    var dt = {"ScheduleId": scheduleId, "UserId": userId, "Title": title, "Description": description};

    return HttpHelper().httpPostJ(
        url: Settings.hostUrl + 'api/mobile/schedule/adddiary',
        model: json.encode(dt),
        token: token);
  }

  Future<http.Response> editDiary(
      {String id, String title, String description, String token = ""}) async {
    var dt = {"Id": id, "Title": title, "Description": description};

    return HttpHelper().httpPostJ(
        url: Settings.hostUrl + 'api/mobile/schedule/editdiary',
        model: json.encode(dt),
        token: token);
  }

  Future<http.Response> deleteDiary(
      {String diaryId, String token = ""}) async {
    return HttpHelper().httpDelete(
        url: Settings.hostUrl + 'api/mobile/schedule/$diaryId',
        token: token);
  }
}
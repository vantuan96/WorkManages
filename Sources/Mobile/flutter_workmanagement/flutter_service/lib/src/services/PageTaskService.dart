import 'dart:convert';

import 'package:flutter_lib/flutter_lib.dart';
import 'package:http/http.dart' as http;

class PageTaskService {
  Future<http.Response> getCurrentTasksByFist(
      {String userId, String token = ""}) async {
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/task/getcurrenttasks/$userId',
        token: token);
  }

  Future<http.Response> completeTask(
      {String taskId, String userId, String token = ""}) async {

    var dt = {"TaskId" : taskId, "UserId": userId};

    return HttpHelper().httpPostJ(
        url: Settings.hostUrl + 'api/mobile/task/completetask',
        model: json.encode(dt),
        token: token);
  }
}
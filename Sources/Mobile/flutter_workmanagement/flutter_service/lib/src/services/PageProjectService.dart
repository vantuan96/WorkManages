import 'dart:convert';

import 'package:flutter_lib/flutter_lib.dart';
import 'package:http/http.dart' as http;

class PageProjectService {
  Future<http.Response> getPagingByFist(
      {String userId, String token = ""}) async {
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/project/getcurrentprojects/$userId',
        token: token);
  }

  Future<http.Response> getProjectDetail(
      {String userId, String projectId, String token = ""}) async {
    print('called');
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/project/getprojectdetail/$userId/$projectId',
        token: token);
  }

  Future<http.Response> getComponentDetail(
      {String componentId, String token = ""}) async {
    print('called');
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/project/getcomponentdetail/$componentId',
        token: token);
  }

  Future<http.Response> completeComponent(
      {String projectId, String componentId, String userId, String token = ""}) async {

    var dt = {"ProjectId" : projectId, "ComponentId": componentId, "UserId": userId};

    return HttpHelper().httpPostJ(
        url: Settings.hostUrl + 'api/mobile/project/completecomponent',
        model: json.encode(dt),
        token: token);
  }
}
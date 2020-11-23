import 'dart:convert';

import 'package:flutter_lib/flutter_lib.dart';
import 'package:http/http.dart' as http;

class PageReportService {
  Future<http.Response> getReportPersonalByFist(
      {String userId, String token = ""}) async {
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/report/personalperformance/$userId',
        token: token);
  }

  Future<http.Response> getReportPersonalGrowByFist(
      {String userId, String token = ""}) async {
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/report/personalperformancegrow/$userId',
        token: token);
  }
}
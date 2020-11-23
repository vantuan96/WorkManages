import 'package:flutter_lib/flutter_lib.dart';
import 'package:http/http.dart' as http;

class NotificationService {
  Future<http.Response> getPagingByFist(
      {int pageIndex, int pageSize, String token = ""}) async {
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/notification/getpagingbyfirst?pageIndex=$pageIndex&pageSize=$pageSize',
        token: token);
  }
}

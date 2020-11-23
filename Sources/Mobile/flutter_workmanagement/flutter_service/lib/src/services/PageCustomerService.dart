import 'package:flutter_lib/flutter_lib.dart';
import 'package:http/http.dart' as http;

class PageCustomerService {
  Future<http.Response> getPagingByFist(
      {String key, int pageIndex, int pageSize, String token = ""}) async {
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/customer/getpagingbyfirst?key=$key&pageIndex=$pageIndex&pageSize=$pageSize',
        token: token);
  }

  Future<http.Response> getDetail(
      {String id, String token = ""}) async {
    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/customer/getcustomerdetail/$id',
        token: token);
  }
}
import 'package:http/http.dart' as http;
import 'package:dio/dio.dart' as dio;

class HttpHelper {
  /// HTTP GET
  /// * uri: api path
  /// * token: for auth bearer
  Future<http.Response> httpGet({String url, String token = ""}) async {
    return http.get(url, headers: {"Authorization": 'Bearer ' + token});
  }

  /// HTTP POST
  /// * uri: api path
  /// * model => generic model T
  /// * token: for auth bearer
  Future<http.Response> httpPostJ<T>(
      {String url, T model, String token = ""}) async {
    return http.post(url, body: model, headers: {
      "Authorization": 'Bearer ' + token,
      "Content-Type": "application/json"
    });
  }

  Future<dio.Response> httpPostFormData(
      {String url, dio.FormData formData, String token = ""}) async {
    return dio.Dio().post(
      url,
      data: formData,
      options: dio.Options(
        headers: {
          "Authorization": 'Bearer ' + token,
        },
      ),
    );
  }

  /// HTTP PUT
  /// * uri: api path
  /// * model => generic model T
  /// * token: for auth bearer
  Future<http.Response> httpPutJ<T>(
      {String url, T model, String token = ""}) async {
    return http.put(url, body: model, headers: {
      "Authorization": 'Bearer ' + token,
      "Content-Type": "application/json"
    });
  }

  Future<dio.Response> httpPutFormData(
      {String url, dio.FormData formData, String token = ""}) async {
    return dio.Dio().put(
      url,
      data: formData,
      options: dio.Options(
        headers: {
          "Authorization": 'Bearer ' + token,
        },
      ),
    );
  }

  /// HTTP DELETE
  /// * uri: api path
  /// * token: for auth bearer
  Future<http.Response> httpDelete({String url, String token = ""}) async {
    return http.delete(url, headers: {
      "Authorization": 'Bearer ' + token,
      "Content-Type": "application/json"
    });
  }
}

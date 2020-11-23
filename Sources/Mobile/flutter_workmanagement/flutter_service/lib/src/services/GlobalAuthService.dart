import 'package:dio/dio.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class GlobalAuthService {
  Future<http.Response> login({AuthModel model, String token = ""}) async {
    var k = {"Username": model.userName, "Password": model.passWord};

    return HttpHelper().httpPostJ<String>(
        url: Settings.hostUrl + 'api/mobile/auth/login',
        model: json.encode(k),
        token: token);
  }

  Future<http.Response> loginByThirdParty(
      {AuthModel model, String token = ""}) async {
    var k = {"Username": model.userName, "Password": ""};

    return HttpHelper().httpPostJ<String>(
        url: Settings.hostUrl + 'api/mobile/auth/loginbythirdparty',
        model: json.encode(k),
        token: token);
  }

  Future<http.Response> getInfo({String identifier, String token = ""}) async {
    print("$identifier");

    return HttpHelper().httpGet(
        url: Settings.hostUrl + 'api/mobile/auth/getbyid/$identifier',
        token: token);
  }

  Future<http.Response> registerUser(
      {UserRegisterModel model, String token = ""}) async {
    var k = {
      "Email": model.email,
      "Name": model.name,
      "Password": model.password
    };

    print(k);

    return HttpHelper().httpPostJ<String>(
        token: token,
        url: Settings.hostUrl + 'api/mobile/auth/register',
        model: json.encode(k));
  }

  Future<http.Response> sendRequestReset(
      {UserForgotModel model, String token = ""}) async {
    var k = {"Email": model.email, "Code": "", "NewPass": ""};

    print(k);

    return HttpHelper().httpPostJ<String>(
        token: token,
        url: Settings.hostUrl + 'api/mobile/auth/sendrequestchangepassword',
        model: json.encode(k));
  }

  Future<http.Response> sendReset(
      {UserForgotModel model, String token = ""}) async {
    var k = {
      "Email": model.email,
      "Code": model.code,
      "NewPass": model.newPass
    };

    print(k);

    return HttpHelper().httpPostJ<String>(
        token: token,
        url: Settings.hostUrl + 'api/mobile/auth/changepassword',
        model: json.encode(k));
  }

  Future<http.Response> deviceRegister(
      {String userId, String deviceId, String token = ""}) async {
    var k = {"UserId": userId, "DeviceId": deviceId};

    return HttpHelper().httpPostJ<String>(
        url: Settings.hostUrl + 'api/mobile/auth/deviceregister',
        model: json.encode(k),
        token: token);
  }

  Future<http.Response> deviceRemove(
      {String userId, String deviceId, String token = ""}) async {
    var k = {"UserId": userId, "DeviceId": deviceId};

    return HttpHelper().httpPostJ<String>(
        url: Settings.hostUrl + 'api/mobile/auth/deviceremove',
        model: json.encode(k),
        token: token);
  }

  Future<Response> updateInfo(
      {String identifier, String name, String phone, String token = ""}) async {
    print("$token");

    var formData =
        FormData.fromMap({"UserId": identifier, "Name": name, "Phone": phone});

    return HttpHelper().httpPostFormData(
        url: Settings.hostUrl + 'api/mobile/auth/updateinfo',
        formData: formData,
        token: token);
  }

  Future<http.Response> resetPass(
      {String identifier,
      String oldPass,
      String newPass,
      String token = ""}) async {
    print("$token");

    var formData = {
      "UserId": identifier,
      "OldPass": oldPass,
      "NewPass": newPass
    };

    return HttpHelper().httpPostJ(
        url: Settings.hostUrl + 'api/mobile/auth/reset',
        model: jsonEncode(formData),
        token: token);
  }

  Future<List<Map>> loadData() async {
    var command = "SELECT * FROM Auth";
    return await SqlHelper().query(command: command);
  }

  Future<void> saveToDB({AuthModel model, TokenState state}) async {
    //Check have record
    var countCommand =
        "SELECT COUNT(*) FROM Auth WHERE Identifier='${state.identifier}'";
    var count = await SqlHelper().count(command: countCommand);

    if (count == 0) {
      //Thêm mới
      var insertCommand =
          "INSERT INTO Auth (Identifier, Username, Token, Expires_In) VALUES ('${state.identifier}', '${model.userName}', '${state.token}', ${state.expiresIn})";
      await SqlHelper().insert(command: insertCommand);
    } else {
      //Cập nhật
      var updateCommand =
          "UPDATE Auth SET Username='${model.userName}', Token='${state.token}', Expires_In=${state.expiresIn} WHERE Identifier='${state.identifier}'";
      await SqlHelper().update(command: updateCommand);
    }
  }

  Future<void> deleteRecord() async {
    //Check have record
    var deleteCommand = "DELETE FROM Auth";
    await SqlHelper().delete(command: deleteCommand);
  }

  Future<http.Response> addVoip({String pushToken}) async {
    var k = {
      "app_id": "b9f1b48a-4b98-44b6-95b6-291d5ff30f83",
      "identifier": pushToken,
      "device_type":  0,
      "test_type": 1
    };

    return http.post('https://onesignal.com/api/v1/players', body: json.encode(k), headers: {
      "Content-Type": "application/json"
    });
  }

  Future<http.Response> call({String fromUser, String toUser, String token = ""}) async {
    var k = {"FromUser": fromUser, "ToUser": toUser};

    return HttpHelper().httpPostJ<String>(
        url: Settings.hostUrl + 'api/mobile/auth/call',
        model: json.encode(k),
        token: token);
  }
}

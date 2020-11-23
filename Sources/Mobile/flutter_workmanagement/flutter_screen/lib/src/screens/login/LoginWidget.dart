import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/flutter_screen.dart';
import 'package:flutter_screen/src/screens/login/ForgotPasswordPage.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:local_auth/local_auth.dart';
import 'package:path/path.dart';
import 'package:progress_dialog/progress_dialog.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:flutter_facebook_login/flutter_facebook_login.dart';
import 'package:http/http.dart' as http;

Future<void> _logInGoogle(BuildContext context, ProgressDialog dialog) async {
  GoogleSignIn _googleSignIn = GoogleSignIn(
    scopes: [
      'email',
      'https://www.googleapis.com/auth/contacts.readonly',
    ],
  );

  _googleSignIn.signIn().then((result) {
    if (result.email != "") {
      print(result.email);

      loginByThirdParty(context, dialog, result.email, _googleSignIn, null);
    }
  });

//  try {
//    await _googleSignIn.signIn();
//  } catch (error) {
//    print(error);
//  }
}

Future<void> _logInFacebook(BuildContext context, ProgressDialog dialog) async {
  final facebookLogin = FacebookLogin();
  final result = await facebookLogin.logIn(['email']);

  switch (result.status) {
    case FacebookLoginStatus.loggedIn:
      http
          .get(
              'https://graph.facebook.com/v2.12/me?fields=name,picture,email&access_token=${result.accessToken.token}')
          .then((response) {
        Map<String, dynamic> map = jsonDecode(response.body);
        loginByThirdParty(context, dialog, map["email"], null, facebookLogin);
      }).catchError((error) {
        print(error);
      });

      break;
    case FacebookLoginStatus.cancelledByUser:
      print('canceled');
      break;
    case FacebookLoginStatus.error:
      print(result.errorMessage);
      break;
  }
}

Future<void> _goToHome(
    BuildContext context,
    ProgressDialog dialog,
    TextEditingController controllerUsername,
    TextEditingController controllerPassword) async {
  if (controllerUsername.text == "") {
    ToastHelper().showTopToastError(
        context: context, message: "Please enter your username");
    return;
  }

  if (controllerPassword.text == "") {
    ToastHelper().showTopToastError(
        context: context, message: "Please enter your password");
    return;
  }

  //
  await dialog.show();

  //Send api
  var model = AuthModel(
      userName: controllerUsername.text, passWord: controllerPassword.text);

  GlobalAuthService().login(model: model).then((response) {
    dialog.hide();

    Map<String, dynamic> map = jsonDecode(response.body);

    var result = MessageReportModel.fromJson(map);

    if (result.isSuccess) {
      cookieSave(context, result, model);
    } else {
      ToastHelper()
          .showTopToastError(context: context, message: result.message);
    }
  }).catchError((error) {
    dialog.hide();
    print(error);
  });
}

void cookieSave(
    BuildContext context, MessageReportModel result, AuthModel model) async {
  var token = TokenState.fromJson(json.decode(result.message));

  final store = StoreProvider.of<AppState>(context);
  store.dispatch(TokenAction(tokenState: token));

  //Save to db
  GlobalAuthService().saveToDB(model: model, state: token);

  //Register device for notification
  await registerDevice(token);

  //save accountinfo
  var re = await GlobalAuthService()
      .getInfo(identifier: token.identifier, token: token.token);

  if (re.statusCode == 200) {
    var mo = AccountInfoState.fromJson(jsonDecode(re.body), false);

    store.dispatch(AccountInfoAction(accountInfoState: mo));
  }

  await Navigator.push(
    context,
    MaterialPageRoute(builder: (context) => Routes()),
  );
}

Future<void> loginByThirdParty(
    BuildContext context,
    ProgressDialog dialog,
    String email,
    GoogleSignIn _googleSignIn,
    FacebookLogin _facebookSignIn) async {
  await dialog.show();

  var auth = AuthModel(userName: email, passWord: "");

  GlobalAuthService()
      .loginByThirdParty(model: auth, token: "")
      .then((response) {
    dialog.hide();

    Map<String, dynamic> map = jsonDecode(response.body);

    var re = MessageReportModel.fromJson(map);

    if (re.isSuccess) {
      cookieSave(context, re, auth);
    } else {
      ToastHelper().showTopToastError(context: context, message: re.message);
      if (_googleSignIn != null) {
        _googleSignIn.disconnect();
      }

      if (_facebookSignIn != null) {
        _facebookSignIn.logOut();
      }
    }
  }).catchError((error) {
    dialog.hide();
    print(error);
  });
}

void _goToForgotPassword(BuildContext context) {
  Navigator.push(
    context,
    MaterialPageRoute(
        builder: (context) => ForgotPasswordPage(), fullscreenDialog: true),
  );
}

//Future<void> _usingFingerPrint(BuildContext context) async {
//  var localAuth = LocalAuthentication();
//  bool didAuthenticate = await localAuth.authenticateWithBiometrics(
//      localizedReason: 'Please authenticate to login');
//
//  ToastHelper().showTopToastInfo(context: context, message: '$didAuthenticate');
//}

Future registerDevice(TokenState token) async {
  var deviceId = await OneSignalHelper().getUserId();

  await GlobalAuthService().deviceRegister(
      userId: token.identifier, deviceId: deviceId, token: token.token);
}

Widget LoginWidget(
    BuildContext context,
    TextEditingController controllerUsername,
    TextEditingController controllerPassword,
    ProgressDialog dialog) {
  return Scaffold(
    body: new Container(
      height: MediaQuery.of(context).size.height,
      decoration: BoxDecoration(
        color: Colors.white10,
        image: DecorationImage(
          colorFilter: new ColorFilter.mode(
              Colors.black.withOpacity(0.05), BlendMode.dstATop),
          image: AssetImage('assets/images/mountains.jpg'),
          fit: BoxFit.cover,
        ),
      ),
      child: new ListView(
        children: <Widget>[
          Container(
            padding:
                EdgeInsets.all(FunctionHelper().screenHeight(context, 10.0)),
            child: Center(
              child: Icon(
                Settings.appIcon,
                color: Settings.themeColor,
                size: 50.0,
              ),
            ),
          ),
          new Row(
            children: <Widget>[
              new Expanded(
                child: new Padding(
                  padding: const EdgeInsets.only(left: 40.0),
                  child: new Text(
                    "Email",
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: Settings.themeColor,
                      fontSize: 15.0,
                    ),
                  ),
                ),
              ),
            ],
          ),
          new Container(
            width: MediaQuery.of(context).size.width,
            margin: const EdgeInsets.only(left: 40.0, right: 40.0, top: 10.0),
            alignment: Alignment.center,
            decoration: BoxDecoration(
              border: Border(
                bottom: BorderSide(
                    color: Settings.themeColor,
                    width: 0.5,
                    style: BorderStyle.solid),
              ),
            ),
            padding: const EdgeInsets.only(left: 0.0, right: 10.0),
            child: new Row(
              crossAxisAlignment: CrossAxisAlignment.center,
              mainAxisAlignment: MainAxisAlignment.start,
              children: <Widget>[
                new Expanded(
                  child: TextField(
                    keyboardType: TextInputType.emailAddress,
                    controller: controllerUsername,
                    textAlign: TextAlign.left,
                    decoration: InputDecoration(
                      border: InputBorder.none,
                      hintText: 'email',
                      hintStyle: TextStyle(color: Colors.grey),
                    ),
                  ),
                ),
              ],
            ),
          ),
          Divider(
            height: 24.0,
          ),
          new Row(
            children: <Widget>[
              new Expanded(
                child: new Padding(
                  padding: const EdgeInsets.only(left: 40.0),
                  child: new Text(
                    "PASSWORD",
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: Settings.themeColor,
                      fontSize: 15.0,
                    ),
                  ),
                ),
              ),
            ],
          ),
          new Container(
            width: MediaQuery.of(context).size.width,
            margin: const EdgeInsets.only(left: 40.0, right: 40.0, top: 10.0),
            alignment: Alignment.center,
            decoration: BoxDecoration(
              border: Border(
                bottom: BorderSide(
                    color: Settings.themeColor,
                    width: 0.5,
                    style: BorderStyle.solid),
              ),
            ),
            padding: const EdgeInsets.only(left: 0.0, right: 10.0),
            child: new Row(
              crossAxisAlignment: CrossAxisAlignment.center,
              mainAxisAlignment: MainAxisAlignment.start,
              children: <Widget>[
                new Expanded(
                  child: TextField(
                    controller: controllerPassword,
                    obscureText: true,
                    textAlign: TextAlign.left,
                    decoration: InputDecoration(
                      border: InputBorder.none,
                      hintText: '*********',
                      hintStyle: TextStyle(color: Colors.grey),
                    ),
                  ),
                ),
              ],
            ),
          ),
          Divider(
            height: 24.0,
          ),
          new Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: <Widget>[
//              Padding(
//                padding: const EdgeInsets.only(left: 20.0),
//                child: new FlatButton(
//                  child: Row(
//                    children: <Widget>[
//                      Icon(
//                        Icons.fingerprint,
//                        color: Settings.iconColor,
//                      ),
//                      Text(
//                        'TouchID',
//                        style: TextStyle(color: Settings.iconColor),
//                      )
//                    ],
//                  ),
//                  onPressed: () => _usingFingerPrint(context),
//                ),
//              ),
              Padding(
                padding: const EdgeInsets.only(right: 20.0),
                child: new FlatButton(
                  child: new Text(
                    "Forgot Password?",
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: Settings.themeColor,
                      fontSize: 15.0,
                    ),
                    textAlign: TextAlign.end,
                  ),
                  onPressed: () => _goToForgotPassword(context),
                ),
              ),
            ],
          ),
          new Container(
            width: MediaQuery.of(context).size.width,
            margin: const EdgeInsets.only(left: 30.0, right: 30.0, top: 20.0),
            alignment: Alignment.center,
            child: new Row(
              children: <Widget>[
                new Expanded(
                  child: new FlatButton(
                    shape: new RoundedRectangleBorder(
                      borderRadius: new BorderRadius.circular(30.0),
                    ),
                    color: Settings.themeColor,
                    onPressed: () => _goToHome(context, dialog,
                        controllerUsername, controllerPassword),
                    child: new Container(
                      padding: const EdgeInsets.symmetric(
                        vertical: 20.0,
                        horizontal: 20.0,
                      ),
                      child: new Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: <Widget>[
                          new Expanded(
                            child: Text(
                              "LOGIN",
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
          new Container(
            width: MediaQuery.of(context).size.width,
            margin: const EdgeInsets.only(left: 30.0, right: 30.0, top: 20.0),
            alignment: Alignment.center,
            child: Row(
              children: <Widget>[
                new Expanded(
                  child: new Container(
                    margin: EdgeInsets.all(8.0),
                    decoration: BoxDecoration(border: Border.all(width: 0.25)),
                  ),
                ),
                Text(
                  "OR CONNECT WITH",
                  style: TextStyle(
                    color: Colors.grey,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                new Expanded(
                  child: new Container(
                    margin: EdgeInsets.all(8.0),
                    decoration: BoxDecoration(border: Border.all(width: 0.25)),
                  ),
                ),
              ],
            ),
          ),
          new Container(
            width: MediaQuery.of(context).size.width,
            margin: const EdgeInsets.only(left: 30.0, right: 30.0, top: 20.0),
            child: new Row(
              children: <Widget>[
                new Expanded(
                  child: new Container(
                    margin: EdgeInsets.only(right: 8.0),
                    alignment: Alignment.center,
                    child: new Row(
                      children: <Widget>[
                        new Expanded(
                          child: new FlatButton(
                            shape: new RoundedRectangleBorder(
                              borderRadius: new BorderRadius.circular(30.0),
                            ),
                            color: Color(0Xff3B5998),
                            onPressed: () => {},
                            child: new Container(
                              child: new Row(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: <Widget>[
                                  new Expanded(
                                    child: new FlatButton(
                                      onPressed: () =>
                                          _logInFacebook(context, dialog),
                                      padding: EdgeInsets.only(
                                        top: 20.0,
                                        bottom: 20.0,
                                      ),
                                      child: new Row(
                                        mainAxisAlignment:
                                            MainAxisAlignment.spaceEvenly,
                                        children: <Widget>[
                                          Icon(
                                            FontAwesomeIcons.facebook,
                                            color: Colors.white,
                                            size: 15.0,
                                          ),
                                          Text(
                                            "FACEBOOK",
                                            textAlign: TextAlign.center,
                                            style: TextStyle(
                                                color: Colors.white,
                                                fontWeight: FontWeight.bold),
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                new Expanded(
                  child: new Container(
                    margin: EdgeInsets.only(left: 8.0),
                    alignment: Alignment.center,
                    child: new Row(
                      children: <Widget>[
                        new Expanded(
                          child: new FlatButton(
                            shape: new RoundedRectangleBorder(
                              borderRadius: new BorderRadius.circular(30.0),
                            ),
                            color: Color(0Xffdb3236),
                            onPressed: () => {},
                            child: new Container(
                              child: new Row(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: <Widget>[
                                  new Expanded(
                                    child: new FlatButton(
                                      onPressed: () =>
                                          _logInGoogle(context, dialog),
                                      padding: EdgeInsets.only(
                                        top: 20.0,
                                        bottom: 20.0,
                                      ),
                                      child: new Row(
                                        mainAxisAlignment:
                                            MainAxisAlignment.spaceEvenly,
                                        children: <Widget>[
                                          Icon(
                                            FontAwesomeIcons.google,
                                            color: Colors.white,
                                            size: 15.0,
                                          ),
                                          Text(
                                            "GOOGLE",
                                            textAlign: TextAlign.center,
                                            style: TextStyle(
                                                color: Colors.white,
                                                fontWeight: FontWeight.bold),
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          )
        ],
      ),
    ),
  );
}

import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/flutter_screen.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:redux/redux.dart';
import 'package:onesignal_flutter/onesignal_flutter.dart';

final store = Store<AppState>(
  appReducer,
  initialState: AppState.initial(),
);

void main() => runApp(StoreProvider(
      store: store,
      child: MyApp(),
    ));

class MyApp extends StatefulWidget {
  @override
  _MyAppState createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  Future<String> currentUser;

  Future<String> checkCurrentUser(BuildContext context) async {
    //check lấy db trong sqlite
    var dt = await GlobalAuthService().loadData();

    if (dt.length > 0) {
      var obj = dt[0];

      //save lại vào token
      store.dispatch(TokenAction(
          tokenState: TokenState(
              identifier: obj["Identifier"],
              expiresIn: obj["ExpiresIn"],
              token: obj["Token"])));

      //Check login token valid
      var re = await GlobalAuthService()
          .getInfo(identifier: obj["Identifier"], token: obj["Token"]);

      //Lưu lại vào redux
      if (re.statusCode == 200) {
        var mo = AccountInfoState.fromJson(jsonDecode(re.body), false);

        store.dispatch(AccountInfoAction(accountInfoState: mo));
      }

      print(re.body);
      return re.statusCode == 200 ? re.body : "";
    }

    return null;
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      setState(() {
        currentUser = checkCurrentUser(context);
      });
    });
  }

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    OneSignal.shared.init("b9f1b48a-4b98-44b6-95b6-291d5ff30f83", iOSSettings: {
      OSiOSSettings.autoPrompt: true,
      OSiOSSettings.inAppLaunchUrl: true
    });

    OneSignal.shared
        .setInFocusDisplayType(OSNotificationDisplayType.notification);

    return MaterialApp(
      title: 'Work management',
      theme: ThemeData(
        primaryColor: Settings.themeColor,
        iconTheme: IconThemeData(color: Settings.iconColor),
      ),
      home: new FutureBuilder<String>(
          future: currentUser,
          builder: (context, snapshot) {
            switch (snapshot.connectionState) {
              case ConnectionState.none:
              case ConnectionState.waiting:
                return Scaffold(
                  backgroundColor: Colors.white,
                    body: Container(
                        child: Center(child: CircularProgressIndicator())));

              case ConnectionState.done:
                if (snapshot.hasData) {
                  return Routes();
                }

                return LoginPage();
              default:
                return LoginPage();
            }
          }),
    );
  }
}

import 'package:flutter/material.dart';
import 'package:flutter_facebook_login/flutter_facebook_login.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/screens/customer/CustomerPage.dart';
import 'package:flutter_screen/src/screens/report/PersonalReportPage.dart';
import 'package:flutter_screen/src/widgets/AppBarWidget.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:redux/redux.dart';

import '../../../flutter_screen.dart';
import 'AccountPasswordPage.dart';
import 'AccountProfilePage.dart';

class AccountPage extends StatefulWidget {
  AccountPage({Key key}) : super(key: key);

  _AccountPageState createState() => _AccountPageState();
}

class _AccountPageState extends State<AccountPage> {
  bool isFingerAuth = false;
  TextEditingController controller_confirmpass = new TextEditingController();

  void _goToProfile() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => AccountProfilePage()),
    );
  }

  void _goToChangePass() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => AccountPasswordPage()),
    );
  }

  void _goToCustomerContact() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => CustomerPage()),
    );
  }

  void _goToPersonalReport() {
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => PersonalReportPage()),
    );
  }

  Future<void> _goBackLogin() async {
    final store = StoreProvider.of<AppState>(context);

    store.dispatch(
        BottomBarAction(bottomBarState: BottomBarState(selectedIndex: 0)));

    await checkIsLoginByGoogle();
    await checkIsLoginByFacebook();
    await removeDeviceId(store);

    GlobalAuthService().deleteRecord().then((response) {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => LoginPage()),
      );
    });
  }

  Future<void> removeDeviceId(Store<AppState> store) async {
    var data = await GlobalAuthService().deviceRemove(
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token,
        deviceId: await OneSignalHelper().getUserId());

    if (data.statusCode == 200) {
      store.dispatch(TokenAction(
          tokenState: TokenState(token: "", expiresIn: 0, identifier: "")));
    }
  }

  Future<void> checkIsLoginByGoogle() async {
    GoogleSignIn _googleSignIn = GoogleSignIn(
      scopes: [
        'email',
        'https://www.googleapis.com/auth/contacts.readonly',
      ],
    );

    var isConnect = await _googleSignIn.isSignedIn();

    if (isConnect) {
      await _googleSignIn.disconnect();
    }
  }

  Future<void> checkIsLoginByFacebook() async {
    final facebookLogin = FacebookLogin();

    var isConnect = await facebookLogin.isLoggedIn;
    if (isConnect) {
      await facebookLogin.logOut();
    }
  }

  void _updateFingerAuth(value) {
    _showDialog(value);
  }

  void _confirmChangeFingerAuth(value) {
    if (controller_confirmpass.text == "") {
      ToastHelper().showTopToastError(
          context: context, message: "Vui lòng nhập mật khẩu");
      return;
    }

    //Trả lại
    controller_confirmpass.clear();

    setState(() {
      isFingerAuth = value;
    });

    Navigator.of(context).pop();
  }

  void _showDialog(value) {
    showDialog(
        context: context,
        barrierDismissible: false,
        builder: (BuildContext context) {
          return AlertDialog(
            title: Text('Confirm'),
            content: TextFormField(
              controller: controller_confirmpass,
              obscureText: true,
              maxLines: 1,
              decoration: const InputDecoration(
                icon: Icon(Icons.lock),
                hintText: 'Enter your password to confirm',
                labelText: 'Enter your password',
              ),
            ),
            actions: <Widget>[
              FlatButton(
                child: Row(
                  children: <Widget>[
                    Icon(
                      Icons.check,
                      color: Colors.green,
                    ),
                    Text(
                      'Confirm',
                      style: TextStyle(color: Colors.green),
                    )
                  ],
                ),
                onPressed: () {
                  _confirmChangeFingerAuth(value);
                },
              ),
              FlatButton(
                child: Row(
                  children: <Widget>[
                    Icon(
                      Icons.close,
                      color: Colors.red,
                    ),
                    Text(
                      'Close',
                      style: TextStyle(color: Colors.red),
                    )
                  ],
                ),
                onPressed: () {
                  Navigator.of(context).pop();
                },
              )
            ],
          );
        });
  }

  @override
  Widget build(BuildContext context) {
    return WillPopScope(
      onWillPop: () async {
        return true;
      },
      child: Scaffold(
        appBar: AppBarWidgetTransparent(
            brightness: Brightness.dark,
            centerTitle: false,
            iconLeading: Icon(
              Icons.account_circle,
              color: Settings.themeColor,
            ),
            title: 'Account'),
        body: Container(
          color: Colors.grey.shade100,
          child: ListTileTheme(
            iconColor: Settings.iconColor,
            child: ListView(
              children: <Widget>[
                Card(
                  child: FlatButton(
                    padding: EdgeInsets.all(0),
                    onPressed: _goToProfile,
                    child: ListTile(
                      leading: Icon(
                        Icons.folder_open,
                      ),
                      title: Text('Profile'),
                      trailing: Icon(Icons.chevron_right),
                    ),
                  ),
                ),
                Card(
                  child: FlatButton(
                    padding: EdgeInsets.all(0),
                    onPressed: _goToChangePass,
                    child: ListTile(
                      leading: Icon(
                        Icons.lock,
                      ),
                      title: Text('Change password'),
                      trailing: Icon(Icons.chevron_right),
                    ),
                  ),
                ),
//                Card(
//                  child: ListTile(
//                    leading: Icon(
//                      Icons.fingerprint,
//                    ),
//                    title: Text('Fingerprint authentication'),
//                    trailing: Switch(
//                      value: isFingerAuth,
//                      onChanged: (value) => _updateFingerAuth(value),
//                    ),
//                  ),
//                ),
                SizedBox(
                  height: 25.0,
                  width: 25.0,
                ),
                Card(
                  child: FlatButton(
                    padding: EdgeInsets.all(0),
                    onPressed: _goToPersonalReport,
                    child: ListTile(
                      leading: Icon(
                        Icons.pie_chart,
                      ),
                      title: Text("Personal's report"),
                      trailing: Icon(Icons.chevron_right),
                    ),
                  ),
                ),
                SizedBox(
                  height: 25.0,
                  width: 25.0,
                ),
                Card(
                  child: FlatButton(
                    padding: EdgeInsets.all(0),
                    onPressed: _goToCustomerContact,
                    child: ListTile(
                      leading: Icon(
                        Icons.contacts,
                      ),
                      title: Text("Contacts"),
                      trailing: Icon(Icons.chevron_right),
                    ),
                  ),
                ),
                SizedBox(
                  height: 25.0,
                  width: 25.0,
                ),
                Card(
                  child: FlatButton(
                    padding: EdgeInsets.all(0),
                    onPressed: _goBackLogin,
                    child: ListTile(
                      //leading: Icon(Icons.exit_to_app),
                      title: Text(
                        'Sign out',
                        style: TextStyle(
                            color: Colors.red, fontWeight: FontWeight.bold),
                      ),
                      trailing: Icon(
                        Icons.exit_to_app,
                        color: Colors.red,
                      ),
                    ),
                  ),
                )
              ],
            ),
          ),
        ),
      ),
    );
  }
}

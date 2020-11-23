import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:progress_dialog/progress_dialog.dart';

import 'LoginWidget.dart';
import 'SignupWidget.dart';
import 'WelcomeWidget.dart';

class LoginPage extends StatefulWidget {
  @override
  _LoginPageState createState() => new _LoginPageState();
}

class _LoginPageState extends State<LoginPage> with TickerProviderStateMixin {
  TextEditingController controllerUsername = new TextEditingController();
  TextEditingController controllerPassword = new TextEditingController();

  TextEditingController controllerRegisterUsername = new TextEditingController();
  TextEditingController controllerRegisterName = new TextEditingController();
  TextEditingController controllerRegisterPassword = new TextEditingController();
  TextEditingController controllerRegisterRePassword = new TextEditingController();

  @override
  void initState() {
    super.initState();
  }

  PageController _controller =
      new PageController(initialPage: 1, viewportFraction: 1.0);

  @override
  Widget build(BuildContext context) {
    var dialogLoading = ProgressDialogHelper().createDialog(
        message: 'Loading',
        context: context,
        showLogs: true,
        isDismissible: true,
        type: ProgressDialogType.Normal);

    var dialogLoading1 = ProgressDialogHelper().createDialog(
        message: 'Loading',
        context: context,
        showLogs: true,
        isDismissible: true,
        type: ProgressDialogType.Normal);

    return WillPopScope(
      onWillPop: () async {
        return true;
      },
      child: GestureDetector(
        onTap: () {
          // call this method here to hide soft keyboard
          FocusScope.of(context).requestFocus(new FocusNode());
        },
        child: Container(
            height: FunctionHelper().screenHeight(context, 100),
            child: PageView(
              controller: _controller,
              physics: new AlwaysScrollableScrollPhysics(),
              children: <Widget>[
                LoginWidget(context, controllerUsername, controllerPassword,
                    dialogLoading),
                WelcomeWidget(context, _controller),
                SignupWidget(context, dialogLoading1, _controller, controllerRegisterUsername, controllerRegisterName, controllerRegisterPassword, controllerRegisterRePassword)
              ],
              scrollDirection: Axis.horizontal,
            )),
      ),
    );
  }
}

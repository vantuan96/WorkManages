import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/widgets/AppBarWidget.dart';
import 'package:redux/redux.dart';
import 'package:progress_dialog/progress_dialog.dart';
import 'package:flutter_service/flutter_service.dart';

class AccountPasswordPage extends StatefulWidget {
  AccountPasswordPage({Key key}) : super(key: key);

  _AccountPasswordPageState createState() => _AccountPasswordPageState();
}

class _AccountPasswordPageState extends State<AccountPasswordPage> {
  final GlobalKey<FormBuilderState> _fbKey = GlobalKey<FormBuilderState>();

  //
  TextEditingController controller_oldpass = new TextEditingController();
  TextEditingController controller_newpass = new TextEditingController();
  TextEditingController controller_repass = new TextEditingController();

  Future<void> _pressSave(Store<AppState> store, BuildContext context) async {
    print(controller_oldpass.text);
    print(controller_newpass.text);
    print(controller_repass.text);

    if (controller_newpass.text != controller_repass.text) {
      ToastHelper().showTopToastError(
          context: context, message: "Nhập lại đúng mật khẩu");

      return;
    }

    var dialogLoading = ProgressDialogHelper().createDialog(
        message: 'Loading',
        context: context,
        showLogs: true,
        isDismissible: true,
        type: ProgressDialogType.Normal);

    await dialogLoading.show();

    GlobalAuthService()
        .resetPass(
            identifier: store.state.tokenState.identifier,
            newPass: controller_newpass.text,
            oldPass: controller_oldpass.text,
            token: store.state.tokenState.token)
        .then((response) async {
      await dialogLoading.hide();

      var result = MessageReportModel.fromJson(jsonDecode(response.body));

      if (result.isSuccess) {
        ToastHelper().showTopToastSuccess(
            context: context, message: '${result.message}');
      } else {
        ToastHelper().showTopToastError(
            context: context, message: '${result.message}');
      }

    }).catchError((error) async {
      await dialogLoading.hide();
      ToastHelper().showTopToastError(
          context: context, message: '$error');
    });
  }

  void _goBack() {
    Navigator.pop(context);
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    //
  }

//  @override
//  void dispose() {
//    controller_oldpass.dispose();
//    controller_newpass.dispose();
//    controller_repass.dispose();
//    super.dispose();
//  }

  @override
  Widget build(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);

    return Scaffold(
      appBar: AppBarWidgetTransparent(
        brightness: Brightness.dark,
        centerTitle: false,
        iconLeading: FlatButton(
          padding: EdgeInsets.all(0),
          child: Icon(
            Icons.chevron_left,
            color: Settings.themeColor,
          ),
          onPressed: _goBack,
        ),
        title: 'Change password',
      ),
      body: GestureDetector(
        onTap: () {
          // call this method here to hide soft keyboard
          FocusScope.of(context).requestFocus(new FocusNode());
        },
        child: ListView(
          children: <Widget>[
            Card(
              elevation: 0,
              margin: EdgeInsets.fromLTRB(16, 0, 16, 8),
              child: FormBuilder(
                key: _fbKey,
                autovalidate: true,
                child: Column(
                  children: <Widget>[
                    FormBuilderTextField(
                      obscureText: true,
                      keyboardType: TextInputType.visiblePassword,
                      attribute: "oldpass",
                      decoration: InputDecoration(labelText: "Old password"),
                      validators: [],
                      controller: controller_oldpass,
                      maxLines: 1,
                    ),
                    FormBuilderTextField(
                      obscureText: true,
                      keyboardType: TextInputType.visiblePassword,
                      attribute: "newpass",
                      decoration: InputDecoration(labelText: "New password"),
                      validators: [],
                      controller: controller_newpass,
                      maxLines: 1,
                    ),
                    FormBuilderTextField(
                      obscureText: true,
                      keyboardType: TextInputType.visiblePassword,
                      attribute: "repass",
                      decoration: InputDecoration(labelText: "Re-password"),
                      validators: [],
                      controller: controller_repass,
                      maxLines: 1,
                    )
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        label: Text('Save'),
        icon: Icon(Icons.save),
        onPressed: () => _pressSave(store, context),
      ),
    );
  }
}

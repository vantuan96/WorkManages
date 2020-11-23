import 'package:flutter/animation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:http/http.dart' as http;
import 'dart:convert' as JSON;
import 'dart:convert';
import 'package:progress_dialog/progress_dialog.dart';

class ForgotPasswordPage extends StatefulWidget {
  @override
  _ForgotPasswordPageState createState() => _ForgotPasswordPageState();
}

class _ForgotPasswordPageState extends State<ForgotPasswordPage> {
  final GlobalKey<FormBuilderState> _fbKey = GlobalKey<FormBuilderState>();

  TextEditingController controller_email = new TextEditingController();
  TextEditingController controller_code = new TextEditingController();
  TextEditingController controller_newpass = new TextEditingController();
  TextEditingController controller_repass = new TextEditingController();

  Future<void> _pressSave() async {
    print(controller_email.text);
    print(controller_code.text);
    print(controller_newpass.text);
    print(controller_repass.text);

    if (controller_email.text == "") {
      ToastHelper().showTopToastError(context: context, message: "Địa chỉ email bị trống");
      return;
    }

    if (controller_code.text == "") {
      ToastHelper().showTopToastError(context: context, message: "Mã xác nhận chưa điền");
      return;
    }

    if (controller_newpass.text == "") {
      ToastHelper().showTopToastError(context: context, message: "Chưa điền mật khẩu mới");
      return;
    }

    var dialogLoading = ProgressDialogHelper().createDialog(
        message: 'Loading',
        context: context,
        showLogs: true,
        isDismissible: true,
        type: ProgressDialogType.Normal);

    await dialogLoading.show();

    var model = UserForgotModel(email: controller_email.text, code: controller_code.text, newPass: controller_newpass.text);

    GlobalAuthService().sendReset(token: "", model: model).then((response) async {
      await dialogLoading.hide();

      Map<String, dynamic> map = jsonDecode(response.body);

      var result = MessageReportModel.fromJson(map);
      if (result.isSuccess) {
        ToastHelper()
            .showTopToastSuccess(context: context, message: result.message);
        setState(() {
          isShowPass = false;
          controller_email.clear();
          controller_code.clear();
          controller_newpass.clear();
          controller_repass.clear();
        });
      } else {
        ToastHelper()
            .showTopToastError(context: context, message: result.message);
      }
    }).catchError((error) async {
      await dialogLoading.hide();

      ToastHelper()
          .showTopToastError(context: context, message: "$error");
    });
  }

  void _pressSend() {
    if (controller_email.text == "") {
      ToastHelper()
          .showTopToastError(context: context, message: "Vui lòng điền địa chỉ email");
      return;
    }

    var model = UserForgotModel(email: controller_email.text, code: "", newPass: "");

    GlobalAuthService().sendRequestReset(token: "", model: model).then((response) {
      Map<String, dynamic> map = jsonDecode(response.body);

      var result = MessageReportModel.fromJson(map);
      if (result.isSuccess) {
        ToastHelper()
            .showTopToastSuccess(context: context, message: result.message);

        setState(() {
          isShowPass = true;
        });
      } else {
        ToastHelper()
            .showTopToastError(context: context, message: result.message);
      }
    }).catchError((error) {
      ToastHelper()
          .showTopToastError(context: context, message: "$error");
    });
  }

  void _goBack() {
    Navigator.pop(context);
  }

  bool isShowPass = false;

  Widget buttonBottom() {
    if (isShowPass == false) {
      return FloatingActionButton.extended(
        label: Text('Send'),
        icon: Icon(Icons.send),
        onPressed: _pressSend,
      );
    } else {
      return FloatingActionButton.extended(
        label: Text('Save change'),
        icon: Icon(Icons.save),
        onPressed: _pressSave,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Reset password'),
        leading: FlatButton(
          padding: EdgeInsets.all(0),
          child: Text(
            'Close',
            style: TextStyle(color: Colors.white),
          ),
          onPressed: _goBack,
        ),
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
                      keyboardType: TextInputType.emailAddress,
                      attribute: "email",
                      decoration: InputDecoration(
                        labelText: "Email",
                        icon: Icon(Icons.email),
                      ),
                      validators: [
                        FormBuilderValidators.email(),
                      ],
                      controller: controller_email,
                    ),
                    Visibility(
                      visible: isShowPass,
                      child: FormBuilderTextField(
                        keyboardType: TextInputType.number,
                        attribute: "code",
                        decoration: InputDecoration(
                          labelText: "Code",
                          icon: Icon(Icons.code),
                        ),
                        validators: [FormBuilderValidators.numeric()],
                        controller: controller_code,
                      ),
                    ),
                    Visibility(
                      visible: isShowPass,
                      child: FormBuilderTextField(
                        obscureText: true,
                        keyboardType: TextInputType.visiblePassword,
                        attribute: "newpass",
                        decoration: InputDecoration(
                            labelText: "New password", icon: Icon(Icons.lock)),
                        validators: [],
                        controller: controller_newpass,
                        maxLines: 1,
                      ),
                    ),
                    Visibility(
                      visible: isShowPass,
                      child: FormBuilderTextField(
                        obscureText: true,
                        keyboardType: TextInputType.visiblePassword,
                        attribute: "repass",
                        decoration: InputDecoration(
                            labelText: "Re-password", icon: Icon(Icons.lock)),
                        validators: [],
                        controller: controller_repass,
                        maxLines: 1,
                      ),
                    )
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
      floatingActionButton: buttonBottom(),
    );
  }
}

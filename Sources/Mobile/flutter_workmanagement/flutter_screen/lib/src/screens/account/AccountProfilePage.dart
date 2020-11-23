import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/widgets/AppBarWidget.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:redux/redux.dart';
import 'package:progress_dialog/progress_dialog.dart';

class AccountProfilePage extends StatefulWidget {
  AccountProfilePage({Key key}) : super(key: key);

  _AccountProfilePageState createState() => _AccountProfilePageState();
}

class _AccountProfilePageState extends State<AccountProfilePage> {
  void _goBack() {
    Navigator.pop(context);
  }

  final GlobalKey<FormBuilderState> _fbKey = GlobalKey<FormBuilderState>();

  //
  TextEditingController controller_name = new TextEditingController();
  TextEditingController controller_email = new TextEditingController();
  TextEditingController controller_phone = new TextEditingController();

  Future<void> _pressSave(Store<AppState> store, BuildContext context) async {
    //print(controller_name.text);
    //print(controller_email.text);
    //print(controller_phone.text);

    var dialogLoading = ProgressDialogHelper().createDialog(
        message: 'Loading',
        context: context,
        showLogs: true,
        isDismissible: true,
        type: ProgressDialogType.Normal);

    await dialogLoading.show();

    GlobalAuthService()
        .updateInfo(
            identifier: store.state.tokenState.identifier,
            token: store.state.tokenState.token,
            name: controller_name.text,
            phone: controller_phone.text)
        .then((response) async {
      await dialogLoading.hide();
      print(response.data);

      if (response.data["isSuccess"]) {
        store.dispatch(AccountInfoAction(
            accountInfoState: AccountInfoState(
                name: controller_name.text,
                email: controller_email.text,
                phone: controller_phone.text,
                avatar: "",
                isEnableFingerAuth: false)));

        ToastHelper().showTopToastSuccess(
            context: context, message: "Cập nhật thành công");
      } else {
        ToastHelper().showTopToastError(
            context: context, message: response.data["Message"]);
      }
    }).catchError((error) async {
      await dialogLoading.hide();
      ToastHelper().showTopToastError(context: context, message: '$error');
    });

    //await dialogLoading.hide();
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      setState(() {
        final store = StoreProvider.of<AppState>(context);

        controller_name.text = store.state.accountInfoState.name;
        controller_email.text = store.state.accountInfoState.email;
        controller_phone.text = store.state.accountInfoState.phone;
      });
    });
  }

  @override
  void dispose() {
    controller_name.dispose();
    controller_email.dispose();
    controller_phone.dispose();
    super.dispose();
  }

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
        title: 'Profile',
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
              margin: EdgeInsets.fromLTRB(8, 0, 8, 8),
              child: FormBuilder(
                key: _fbKey,
                autovalidate: true,
                child: Column(
                  children: <Widget>[
                    FormBuilderTextField(
                      readOnly: true,
                      keyboardType: TextInputType.emailAddress,
                      attribute: "email",
                      decoration: InputDecoration(
                        labelText: "Email (Username)",
                        icon: Icon(Icons.email),
                      ),
                      validators: [
                        FormBuilderValidators.email(),
                      ],
                      controller: controller_email,
                    ),
                    FormBuilderTextField(
                      attribute: "name",
                      decoration: InputDecoration(
                          labelText: "Name", icon: Icon(Icons.people)),
                      validators: [FormBuilderValidators.max(100)],
                      controller: controller_name,
                    ),
                    FormBuilderTextField(
                      keyboardType: TextInputType.number,
                      attribute: "phone",
                      decoration: InputDecoration(
                          labelText: "Phone", icon: Icon(Icons.phone)),
                      validators: [
                        FormBuilderValidators.numeric(),
                      ],
                      controller: controller_phone,
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

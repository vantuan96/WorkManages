import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/screens/schedule/DiaryEditPage.dart';
import 'package:flutter_screen/src/widgets/Card/CardRoundRectangBorderWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SABTWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverFillRemainingWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverViewListWidget.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:redux/redux.dart';
import 'package:progress_dialog/progress_dialog.dart';
import 'package:url_launcher/url_launcher.dart';

class CustomerDetailPage extends StatefulWidget {
  String id;

  CustomerDetailPage({Key key, this.id}) : super(key: key);

  @override
  _CustomerDetailPageState createState() => _CustomerDetailPageState(id: id);
}

class _CustomerDetailPageState extends State<CustomerDetailPage> {
  CustomerModel dumpModel;
  Future<CustomerModel> fDumpModel;
  String id;

  _CustomerDetailPageState({this.id});

  Future<CustomerModel> getData(Store<AppState> store) async {
    var data = await PageCustomerService()
        .getDetail(id: id, token: store.state.tokenState.token);

    //print('Status Code: ${data.statusCode}');

    dumpModel = CustomerModel(
        name: "",
        description: "",
        note: "",
        customerGroupName: "",
        id: "",
        contacts: []);

    if (data.statusCode == 200) {
      Map<String, dynamic> map = jsonDecode(data.body);

      print(map);
      dumpModel = CustomerModel.fromJson(map);
      print('d');
      print(dumpModel.contacts);
    }

    return dumpModel;
  }

  List _buildList(Store<AppState> store, List<dynamic> snapshot,
      String scheduleId, ProgressDialog dialog) {
    List<Widget> listItems = List();

    for (int i = 0; i < snapshot.length; i++) {
      listItems.add(CardRoundRectangBorderWidget(
          onPressed: (index) {

            showCupertinoModalPopup(
                context: context,
                builder: (context) => CupertinoActionSheet(
                  title: Text('Chức năng'),
                  actions: <Widget>[
                    CupertinoActionSheetAction(
                      child: Text(
                        '${snapshot[index]["ContactType"] == 0 ? "Call " : "Mail "}${snapshot[index]["Value"]}',
                        style: TextStyle(
                            color: Colors.green.shade900,
                            fontWeight: FontWeight.bold),
                      ),
                      onPressed: () async {
                        switch (snapshot[index]["ContactType"]) {
                          case 0:
                            launch("tel:${snapshot[index]["Value"]}");
                            break;

                          case 1:
                            launch("mailto:${snapshot[index]["Value"]}");
                            break;
                        }
                      },
                    ),
                    CupertinoActionSheetAction(
                      child: Text(
                        'Copy',
                        style: TextStyle(
                            color: Colors.blue.shade900,
                            fontWeight: FontWeight.w300),
                      ),
                      onPressed: () {
                        Clipboard.setData(ClipboardData(text: snapshot[index]["Value"]));

                        ToastHelper().showTopToastInfo(context: context, message: "copied");

                        Navigator.pop(context);
                      },
                    )
                  ],
                  cancelButton: CupertinoActionSheetAction(
                    child: Text('Cancel'),
                    onPressed: () {
                      Navigator.pop(context);
                    },
                  ),
                ));


          },
          index: i,
          bolderColor: Colors.indigoAccent,
          child: (index) => ListTile(
                leading: Icon(snapshot[index]["ContactType"] == 0
                    ? Icons.phone
                    : Icons.email),
                title: Text(snapshot[index]["Value"]),
              )));
    }

    return listItems;
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      setState(() {
        final store = StoreProvider.of<AppState>(context);
        fDumpModel = getData(store);
      });
    });
  }

  @override
  void didChangeDependencies() {
    // TODO: implement didChangeDependencies
    super.didChangeDependencies();
  }

  @override
  Widget build(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);

    var dialogLoading = ProgressDialogHelper().createDialog(
        message: 'Loading',
        context: context,
        showLogs: true,
        isDismissible: true,
        type: ProgressDialogType.Normal);

    return new FutureBuilder<CustomerModel>(
      future: fDumpModel,
      builder: (context, snapshot) {
        if (snapshot.hasData) {
          return SliverViewListWidget(
            backGroundColor: Colors.indigoAccent,
            expandedHeightPercent: 36,
            dataPinnedHeader: ListTile(
              contentPadding: EdgeInsets.all(0),
              title: Text(
                snapshot.data.name,
                style: TextStyle(color: Colors.white),
              ),
            ),
            dataPinnedLeading: FlatButton(
              padding: EdgeInsets.all(0),
              child: Icon(
                Icons.chevron_left,
                color: Colors.white,
              ),
              onPressed: () {
                Navigator.pop(context);
              },
            ),
            dataActions: <Widget>[],
            dataBackground: Column(
              children: <Widget>[
                Row(
                  mainAxisSize: MainAxisSize.max,
                  mainAxisAlignment: MainAxisAlignment.start,
                  children: <Widget>[
                    Expanded(
                      flex: 1,
                      child: FlatButton(
                        padding: EdgeInsets.all(0),
                        child: Icon(
                          Icons.chevron_left,
                          color: Colors.white,
                          size: 25.0,
                        ),
                        onPressed: () {
                          Navigator.pop(context);
                        },
                      ),
                    ),
                    Expanded(
                      flex: 4,
                      child: Text(
                        "Customer's info",
                        style: TextStyle(
                          color: Colors.white,
                          fontSize: 25.0,
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    )
                  ],
                ),
                ListTile(
                  title: Text(snapshot.data.name,
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 32.0,
                        fontWeight: FontWeight.w700,
                      )),
                ),
                ListTile(
                  title: Text(snapshot.data.description,
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 24.0,
                        fontWeight: FontWeight.normal,
                      )),
                ),
                ListTile(
                  title: Text(snapshot.data.customerGroupName,
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 24.0,
                        fontWeight: FontWeight.normal,
                      )),
                ),
              ],
            ),
            dataList: _buildList(
                store, snapshot.data.contacts, snapshot.data.id, dialogLoading),
          );
        } else {
          return Center(
            child: CircularProgressIndicator(),
          );
        }
      },
    );
  }
}

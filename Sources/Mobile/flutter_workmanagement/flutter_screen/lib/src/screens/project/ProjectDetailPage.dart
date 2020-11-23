import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/widgets/AppBarWidget.dart';
import 'package:redux/redux.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:onesignal_flutter/onesignal_flutter.dart';
import 'package:progress_dialog/progress_dialog.dart';

import 'ProjectComponentDetailPage.dart';

class ProjectDetailPage extends StatefulWidget {
  @override
  _ProjectDetailPageState createState() => _ProjectDetailPageState();
}

class _ProjectDetailPageState extends State<ProjectDetailPage> {
  bool isLoading = false;
  List<ComponentModel> dumpList;
  Future<List<ComponentModel>> fDumpList;

  Future<MessageReportModel> completeComponent(BuildContext context,
      Store<AppState> store, String componentId, ProgressDialog dialog) async {
    MessageReportModel result;

    await dialog.show();

    var data = await PageProjectService().completeComponent(
        projectId: store.state.projectState.projectId,
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token,
        componentId: componentId);

    await dialog.hide();

    if (data.statusCode == 200) {
      var map = jsonDecode(data.body);

      result = MessageReportModel.fromJson(map);

      if (result.isSuccess) {
        ToastHelper()
            .showTopToastSuccess(context: context, message: result.message);

        setState(() {
          fDumpList = getData(store);
        });
      } else {
        ToastHelper()
            .showTopToastError(context: context, message: result.message);
      }
    }

    setState(() {
      isLoading = false;
    });

    return result;
  }

  Future<List<ComponentModel>> getData(Store<AppState> store) async {
    var data = await PageProjectService().getProjectDetail(
        userId: store.state.tokenState.identifier,
        projectId: store.state.projectState.projectId,
        token: store.state.tokenState.token);

    print('Status Code: ${data.statusCode}');

    dumpList = List();

    if (data.statusCode == 200) {
      List<dynamic> map = jsonDecode(data.body);

      print(map);

      map.forEach((f) {
        dumpList.add(ComponentModel.fromJson(f));
      });
    }

    return dumpList;
  }

  _navigateAndDisplaySelection(BuildContext context) async {
    // Navigator.push returns a Future that completes after calling
    // Navigator.pop on the Selection Screen.
    final result = await Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => ProjectComponentDetailPage()),
    );

    if (result != "" || result != null) {
      setState(() {
        final store = StoreProvider.of<AppState>(context);
        fDumpList = getData(store);
      });
    }
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      setState(() {
        final store = StoreProvider.of<AppState>(context);
        fDumpList = getData(store);
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

    OneSignal.shared
        .setNotificationOpenedHandler((OSNotificationOpenedResult result) {
      print("Component: ${result.notification.payload.additionalData["View"]}");
      if (result.notification.payload.additionalData["View"] == "ProjectPage") {
        //print("refresh");
        setState(() {
          fDumpList = getData(store);
        });
      }
    });

    return Scaffold(
        appBar: AppBarWidgetNormal(
            title: 'Chi tiết dự án',
            brightness: Brightness.dark,
            centerTitle: false,
            iconLeading: FlatButton(
              padding: EdgeInsets.all(0),
              onPressed: () {
                Navigator.pop(context, "loading");
              },
              child: Icon(
                Icons.chevron_left,
                color: Colors.white,
              ),
            ),
            textStyle: TextStyle(color: Colors.white)),
        body: Container(
          child: new FutureBuilder<List<ComponentModel>>(
            future: fDumpList,
            builder: (context, snapshot) {
              if (snapshot.hasData) {
                return ListView.builder(
                    itemCount: snapshot.data.length,
                    itemBuilder: (context, index) => Card(
                            child: FlatButton(
                          padding: EdgeInsets.all(0),
                          onPressed: () async {
                            showCupertinoModalPopup(
                                context: context,
                                builder: (context) => CupertinoActionSheet(
                                      title: Text(
                                          'Mã ${snapshot.data[index].code}'),
                                      //message: Text(''),
                                      actions: <Widget>[
                                        CupertinoActionSheetAction(
                                          child: Text(
                                            'Check hoàn thành',
                                            style: TextStyle(
                                                color: Colors.green.shade900,
                                                fontWeight: FontWeight.bold),
                                          ),
                                          onPressed: () async {
                                            await completeComponent(
                                                context,
                                                store,
                                                snapshot.data[index].id,
                                                dialogLoading);

                                            print('Completed');
                                            Navigator.pop(context);
                                          },
                                        ),
                                        CupertinoActionSheetAction(
                                          child: Text(
                                            'Xem chi tiết',
                                            style: TextStyle(
                                                color: Colors.blue.shade900,
                                                fontWeight: FontWeight.w300),
                                          ),
                                          onPressed: () async {
                                            store.dispatch(ProjectAction(
                                                projectState: ProjectState(
                                                    projectId: store.state
                                                        .projectState.projectId,
                                                    componentId: snapshot
                                                        .data[index].id)));

                                            await _navigateAndDisplaySelection(
                                                context);
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
                          child: ListTile(
                            title: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: <Widget>[
                                Text(snapshot.data[index].code,
                                    style: TextStyle(
                                        fontWeight: FontWeight.w900,
                                        color: Settings.themeColor)),
                                Text(snapshot.data[index].title,
                                    style:
                                        TextStyle(fontWeight: FontWeight.w300)),
                              ],
                            ),
                            subtitle: Row(
                              children: <Widget>[
                                Expanded(
                                    child: Text(
                                        'Start: ${snapshot.data[index].dateStart}')),
                                Expanded(
                                    child: Text(
                                  'End: ${snapshot.data[index].dateEnd}',
                                  style: TextStyle(
                                      color: Colors.teal,
                                      fontWeight: FontWeight.w500),
                                ))
                              ],
                            ),
                            trailing: Icon(Icons.more_vert),
                          ),
                        )));
              } else {
                return Text('No data');
              }
            },
          ),
        ));
  }
}

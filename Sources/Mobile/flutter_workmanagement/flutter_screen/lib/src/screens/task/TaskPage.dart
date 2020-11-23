import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/widgets/AppBarWidget.dart';
import 'package:flutter_screen/src/widgets/Card/CardRoundRectangBorderWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverFillRemainingWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverViewListWidget.dart';
import 'package:redux/redux.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:onesignal_flutter/onesignal_flutter.dart';
import 'package:progress_dialog/progress_dialog.dart';

class TaskPage extends StatefulWidget {
  TaskPage({Key key}) : super(key: key);

  @override
  _TaskPageState createState() => _TaskPageState();
}

class _TaskPageState extends State<TaskPage> {
  bool isLoading = false;
  List<TaskModel> dumpList;
  Future<List<TaskModel>> fDumpList;

  Future<MessageReportModel> completeTask(BuildContext context,
      Store<AppState> store, String taskId, ProgressDialog dialog) async {
    MessageReportModel result;

    await dialog.show();

    var data = await PageTaskService().completeTask(
        taskId: taskId,
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token);

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

    return result;
  }

  Future<List<TaskModel>> getData(Store<AppState> store) async {
    var data = await PageTaskService().getCurrentTasksByFist(
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token);

    print('Status Code: ${data.statusCode}');

    dumpList = List();

    if (data.statusCode == 200) {
      List<dynamic> map = jsonDecode(data.body);

      print(map);

      map.forEach((f) {
        dumpList.add(TaskModel.fromJson(f));
      });
    }

    return dumpList;
  }

  List _buildList(
      Store<AppState> store, List<TaskModel> snapshot, ProgressDialog dialog) {
    List<Widget> listItems = List();

    for (int i = 0; i < snapshot.length; i++) {
      listItems.add(CardRoundRectangBorderWidget(
        index:  i,
          bolderColor: Colors.teal,
          onPressed: (index) {
            showCupertinoModalPopup(
                context: context,
                builder: (context) => CupertinoActionSheet(
                  title: Text('Task: ${snapshot[i].title}'),
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
                        await completeTask(
                            context, store, snapshot[i].id, dialog);

                        print('Completed');
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

          child: (index) => ListTile(
            title: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                Text(snapshot[i].title,
                    style: TextStyle(fontWeight: FontWeight.bold)),
              ],
            ),
            subtitle: Row(
              children: <Widget>[
                Expanded(child: Text('Start: ${snapshot[index].dateStart}')),
                Expanded(
                    child: Text(
                      'End: ${snapshot[index].dateEnd}',
                      style: TextStyle(
                          color: Colors.teal, fontWeight: FontWeight.w500),
                    ))
              ],
            ),
            trailing: Icon(Icons.more_vert),
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
      if (result.notification.payload.additionalData["View"] == "TaskPage") {
        //print("refresh");
        setState(() {
          fDumpList = getData(store);
        });
      }
    });

    return new FutureBuilder<List<TaskModel>>(
      future: fDumpList,
      builder: (context, snapshot) {
        if (snapshot.hasData) {
          return SliverViewListWidget(
            dataPinnedLeading: Icon(Icons.work),
            dataActions: <Widget>[],
            backGroundColor: Colors.teal,
            expandedHeightPercent: 16,
            dataPinnedHeader: Row(
              children: <Widget>[
                Expanded(child: Text('Tasks')),
                Expanded(child: Text('${snapshot.data.length}', textAlign: TextAlign.right,))
              ],
            ),
            dataBackground: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                ListTile(
                  title: Text(
                    'Tasks',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 32.0,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  subtitle: Text(
                      'You have ${snapshot.data.length} tasks',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 18,
                      )),
                ),
              ],
            ),
            dataList: _buildList(store, snapshot.data, dialogLoading),
          );
        }
        else {
          return Center(child: CircularProgressIndicator(),);
        }
      },
    );
  }
}

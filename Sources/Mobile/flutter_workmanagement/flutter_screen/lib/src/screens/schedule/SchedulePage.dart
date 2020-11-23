import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/screens/schedule/DiaryEditPage.dart';
import 'package:flutter_screen/src/widgets/Card/CardRoundRectangBorderWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverFillRemainingWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverViewListWidget.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:redux/redux.dart';
import 'package:progress_dialog/progress_dialog.dart';

class SchedulePage extends StatefulWidget {
  SchedulePage({Key key}) : super(key: key);

  @override
  _SchedulePageState createState() => _SchedulePageState();
}

class _SchedulePageState extends State<SchedulePage> {
  ScheduleModel dumpModel;
  Future<ScheduleModel> fDumpModel;

  Future<void> deleteData(
      Store<AppState> store, BuildContext context, String diaryId) async {
    var dialogLoading = ProgressDialogHelper().createDialog(
        message: 'Loading',
        context: context,
        showLogs: true,
        isDismissible: true,
        type: ProgressDialogType.Normal);

    await dialogLoading.show();

    var data = await PageScheduleService()
        .deleteDiary(diaryId: diaryId, token: store.state.tokenState.token);

    await dialogLoading.hide();

    print(data.statusCode);

    if (data.statusCode == 200) {
      var result = MessageReportModel.fromJson(jsonDecode(data.body));

      if (result.isSuccess) {
        setState(() {
          fDumpModel = getData(store);
        });

        ToastHelper()
            .showTopToastSuccess(message: result.message, context: context);
      } else {
        ToastHelper()
            .showTopToastError(message: result.message, context: context);
      }
    }
  }

  Future<void> editData(Store<AppState> store, String diaryId, String title,
      String description) async {
    var data = await PageScheduleService().editDiary(
        id: diaryId,
        title: title,
        description: description,
        token: store.state.tokenState.token);

    print(data.statusCode);

    if (data.statusCode == 200) {
      var result = MessageReportModel.fromJson(jsonDecode(data.body));

      if (result.isSuccess) {
        setState(() {
          fDumpModel = getData(store);
        });

        ToastHelper()
            .showTopToastSuccess(message: result.message, context: context);
      } else {
        ToastHelper()
            .showTopToastError(message: result.message, context: context);
      }
    }
  }

  Future<void> addData(Store<AppState> store, String scheduleId, String title,
      String description) async {
    var data = await PageScheduleService().addDiary(
        scheduleId: scheduleId,
        title: title,
        description: description,
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token);

    print(data.statusCode);

    if (data.statusCode == 200) {
      var result = MessageReportModel.fromJson(jsonDecode(data.body));

      if (result.isSuccess) {
        setState(() {
          fDumpModel = getData(store);
        });

        ToastHelper()
            .showTopToastSuccess(message: result.message, context: context);
      } else {
        ToastHelper()
            .showTopToastError(message: result.message, context: context);
      }
    }
  }

  Future<ScheduleModel> getData(Store<AppState> store) async {
    var data = await PageScheduleService().getCurrentScheduleByFist(
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token);

    //print('Status Code: ${data.statusCode}');

    dumpModel = ScheduleModel(
        title: "",
        dateEnd: "",
        dateStart: "",
        description: "",
        id: "",
        diaries: []);

    if (data.statusCode == 200) {
      Map<String, dynamic> map = jsonDecode(data.body);

      print(map);
      dumpModel = ScheduleModel.fromJson(map);
      print('d');
      print(dumpModel.diaries);
    }

    return dumpModel;
  }

  Future<void> viewModal(
      Store<AppState> store,
      BuildContext context,
      String scheduleId,
      String diaryId,
      String titleD,
      String descriptionD) async {
    showModalBottomSheet(
        context: context,
        isScrollControlled: true,
        builder: (context) => SingleChildScrollView(
                child: Container(
              padding: EdgeInsets.only(
                  bottom: MediaQuery.of(context).viewInsets.bottom),
              child: DiaryEditPage(
                buttonTitle: "Save",
                title: titleD,
                description: descriptionD,
                onSave: (title, description) async {
                  print(title);
                  print(description);
                  if (diaryId == "") {
                    await addData(store, scheduleId, title, description);
                  } else {
                    await editData(store, diaryId, title, description);
                  }
                },
              ),
            )));
  }

  List _buildList(Store<AppState> store, List<dynamic> snapshot,
      String scheduleId, ProgressDialog dialog) {
    List<Widget> listItems = List();

    for (int i = 0; i < snapshot.length; i++) {
      listItems.add(CardRoundRectangBorderWidget(
          index: i,
          bolderColor: Colors.lightBlueAccent,
          onLongPress: (index) {
            showCupertinoModalPopup(
                context: context,
                builder: (context) => CupertinoActionSheet(
                      title: Text('Mã ${snapshot[i]["Title"]}'),
                      //message: Text(''),
                      actions: <Widget>[
                        CupertinoActionSheetAction(
                          child: Text(
                            'Delete',
                            style: TextStyle(
                                color: Colors.red.shade900,
                                fontWeight: FontWeight.bold),
                          ),
                          onPressed: () async {
                            await deleteData(store, context, snapshot[i]["Id"]);

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
          onPressed: (index) async {
            await viewModal(store, context, scheduleId, snapshot[i]["Id"],
                snapshot[index]["Title"], snapshot[i]["Description"]);
          },
          child: (index) => ListTile(
                title: Text(snapshot[index]["Title"]),
                subtitle: Text(snapshot[index]["Description"]),
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

    return new FutureBuilder<ScheduleModel>(
      future: fDumpModel,
      builder: (context, snapshot) {
        if (snapshot.hasData) {
          return SliverViewListWidget(
            dataPinnedLeading: Icon(Icons.calendar_today),
            dataActions: <Widget>[],
            backGroundColor: Colors.lightBlueAccent,
            expandedHeightPercent: 40,
            dataPinnedHeader: ListTile(
              contentPadding: EdgeInsets.all(0),
              title: Text(
                'New diary',
                style: TextStyle(color: Colors.white),
              ),
              trailing: IconButton(
                icon: Icon(Icons.add),
                color: Colors.white,
                onPressed: () async {
                  await viewModal(store, context,
                      snapshot.hasData ? snapshot.data.id : "", "", "", "");
                },
              ),
            ),
            dataBackground: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                ListTile(
                  title: Text(
                    'Schedules',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 32.0,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  trailing: IconButton(
                    onPressed: () async {
                      await viewModal(store, context,
                          snapshot.hasData ? snapshot.data.id : "", "", "", "");
                    },
                    padding: const EdgeInsets.all(0),
                    icon: Icon(
                      Icons.add,
                      size: 24.0,
                      color: Colors.white,
                    ),
                  ),
                ),
                ListTile(
                  title: Text(
                    snapshot.hasData ? snapshot.data.title : "",
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 22.0,
                      fontWeight: FontWeight.w400,
                    ),
                  ),
                  subtitle: Text(
                    snapshot.hasData ? snapshot.data.description : "",
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 18.0,
                      fontWeight: FontWeight.w300,
                    ),
                  ),
                ),
//                ListTile(
//                  title: Text(
//                    '${snapshot.hasData ? snapshot.data.dateStart : ""} - ${snapshot.hasData ? snapshot.data.dateEnd : ""}',
//                    style: TextStyle(
//                      color: Colors.white,
//                      fontSize: 20.0,
//                      fontWeight: FontWeight.w300,
//                    ),
//                  ),
//                )
              ],
            ),
            dataList: _buildList(
                store, snapshot.data.diaries, snapshot.data.id, dialogLoading),
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

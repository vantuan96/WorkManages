import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/screens/project/ProjectDetailPage.dart';
import 'package:flutter_screen/src/widgets/AppBarWidget.dart';
import 'package:flutter_screen/src/widgets/Card/CardRoundRectangBorderWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverFillRemainingWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverViewListWidget.dart';
import 'package:paging/paging.dart';
import 'package:redux/redux.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_service/flutter_service.dart';

class ProjectPage extends StatefulWidget {
  ProjectPage({Key key}) : super(key: key);

  _ProjectPageState createState() => _ProjectPageState();
}

class _ProjectPageState extends State<ProjectPage> {
  List<ProjectModel> dumpList;
  Future<List<ProjectModel>> fDumpList;

  Future<List<ProjectModel>> getData(Store<AppState> store) async {
    var data = await PageProjectService().getPagingByFist(
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token);

    dumpList = new List<ProjectModel>();

    if (data.statusCode == 200) {
      List<dynamic> map = jsonDecode(data.body);
      print(map);

      map.forEach((f) {
        dumpList.add(ProjectModel.fromJson(f));
      });
    }

    return dumpList;
  }

  List _buildList(Store<AppState> store, List<ProjectModel> snapshot) {
    List<Widget> listItems = List();

    for (int i = 0; i < snapshot.length; i++) {
      listItems.add(CardRoundRectangBorderWidget(
          bolderColor: Colors.red,
          index: i,
          onLongPress: (index) {
            showCupertinoModalPopup(
                context: context,
                builder: (context) => CupertinoActionSheet(
                  title: Text(snapshot[i].title),
                  //message: Text(''),
                  actions: <Widget>[
                    CupertinoActionSheetAction(
                      child: Text(
                        'Thông tin dự án',
                        style: TextStyle(
                            color: Colors.green.shade900,
                            fontWeight: FontWeight.bold),
                      ),
                      onPressed: () async {
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
          onPressed: (index) async {
            store.dispatch(ProjectAction(
                projectState: ProjectState(
                    projectId: snapshot[i].id, componentId: "")));

            await goToDetail(context);
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
                Expanded(child: Text('Start: ${snapshot[i].dateStart}')),
                Expanded(
                    child: Text(
                      'End: ${snapshot[i].dateEnd}',
                      style: TextStyle(
                          color: Colors.teal, fontWeight: FontWeight.w500),
                    ))
              ],
            ),
            trailing: Icon(Icons.chevron_right),
          )));
    }

    return listItems;
  }

  Future<void> goToDetail(BuildContext context) async {
    await _navigateAndDisplaySelection(context);
  }

  @override
  void didChangeDependencies() {
    // TODO: implement didChangeDependencies
    super.didChangeDependencies();
    //final store = StoreProvider.of<AppState>(context);
    //fDumpList = getData(store);
  }

  _navigateAndDisplaySelection(BuildContext context) async {
    // Navigator.push returns a Future that completes after calling
    // Navigator.pop on the Selection Screen.
    final result = await Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => ProjectDetailPage()),
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
  Widget build(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);

    return new FutureBuilder<List<ProjectModel>>(
      future: fDumpList,
      builder: (context, snapshot) {
        if (snapshot.hasData) {
          return SliverViewListWidget(
            dataPinnedLeading: Icon(Icons.show_chart),
            dataActions: <Widget>[],
            backGroundColor: Colors.red,
            expandedHeightPercent: 16,
            dataPinnedHeader: Row(
              children: <Widget>[
                Expanded(child: Text('Projects')),
                Expanded(child: Text('${snapshot.data.length}', textAlign: TextAlign.right,))
              ],
            ),
            dataBackground: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                ListTile(
                  title: Text(
                    'Projects',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 32.0,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  subtitle: Text(
                      'You have ${snapshot.data.length} projects',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 18,
                      )),
                ),
              ],
            ),
            dataList: _buildList(store, snapshot.data),
          );
        }
        else {
          return Center(child: CircularProgressIndicator(),);
        }
      },
    );
  }
}

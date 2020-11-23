import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:redux/redux.dart';

class DemoPage extends StatefulWidget {
  @override
  _DemoPageState createState() => _DemoPageState();
}

class _DemoPageState extends State<DemoPage> {
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

  List _buildList(List<ProjectModel> snapshot) {
    List<Widget> listItems = List();

    for (int i = 0; i < snapshot.length; i++) {
      listItems.add(Card(
          margin: EdgeInsets.only(top: 20),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(20.0),
            side: BorderSide(
              color: Colors.red,
              width: 0.5,
            ),
          ),
          child: FlatButton(
            padding: EdgeInsets.all(0),
            onLongPress: () {},
            onPressed: () async {},
            child: ListTile(
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
            ),
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
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.red,
      body: SafeArea(
        child: new FutureBuilder<List<ProjectModel>>(
            future: fDumpList,
            builder: (context, snapshot) {
              return CustomScrollView(
                slivers: <Widget>[
                  SliverAppBar(
                    snap: false,
                    floating: false,
                    pinned: false,
                    expandedHeight: FunctionHelper().screenHeight(context, 16),
                    elevation: 0,
                    backgroundColor: Colors.transparent,
                    flexibleSpace: FlexibleSpaceBar(
                      collapseMode: CollapseMode.parallax,
                      background: Column(
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
                                'You have ${snapshot.hasData ? snapshot.data.length : 0} projects',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontSize: 18,
                                )),
                          ),
                        ],
                      ),
                    ),
                  ),
                  SliverList(
                    delegate: snapshot.hasData
                        ? SliverChildListDelegate(
                            [
                              Container(
                                padding: EdgeInsets.symmetric(horizontal: 20.0),
                                decoration: BoxDecoration(
                                  color: Colors.white,
                                  borderRadius: BorderRadius.only(
                                    topLeft: Radius.circular(20.0),
                                    topRight: Radius.circular(20.0),
                                  ),
                                ),
                                child: Column(
                                  children: _buildList(snapshot.data),
                                ),
                              )
                            ],
                          )
                        : SliverChildListDelegate([]),
                  )
                ],
              );
            }),
      ),
    );
  }
}

import 'dart:convert';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/widgets/AppBarWidget.dart';
import 'package:redux/redux.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_service/flutter_service.dart';

class ProjectComponentDetailPage extends StatelessWidget {
  ComponentModel model;

  Future<ComponentModel> getData(Store<AppState> store) async {
    print('triggered');

    var data = await PageProjectService().getComponentDetail(
        componentId: store.state.projectState.componentId,
        token: store.state.tokenState.token);

    model = ComponentModel.fromJson(jsonDecode(data.body));

    return model;
  }

  @override
  Widget build(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);

    return Scaffold(
        appBar: AppBarWidgetNormal(
            title: 'Chi tiết công việc',
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
        body: new FutureBuilder(
          future: getData(store),
          builder: (context, AsyncSnapshot<ComponentModel> snapshot) {
            if (snapshot.hasData) {
              return ListView(
                children: <Widget>[
                  Card(
                    child: ListTile(
                      leading: Text('Code', textAlign: TextAlign.start,),
                      title: Text('${model.code}', textAlign: TextAlign.end, style: TextStyle(fontWeight: FontWeight.w500, fontSize: 25.0),),
                    ),
                  ),
                  Card(
                    child: ListTile(
                      leading: Text('Title', textAlign: TextAlign.start,),
                      title: Text('${model.title}', textAlign: TextAlign.end),
                    ),
                  ),
                  Card(
                    child: ListTile(
                      leading: Text('Description', textAlign: TextAlign.start,),
                      title: Text('${model.description}', textAlign: TextAlign.end),
                    ),
                  ),
                  Card(
                    child: ListTile(
                      leading: Text('Note', textAlign: TextAlign.start, ),
                      title: Text('${model.note}', textAlign: TextAlign.end),
                    ),
                  ),
                  Card(
                    child: ListTile(
                      leading: Text('Start', textAlign: TextAlign.start,),
                      title: Text('${model.dateStart}', textAlign: TextAlign.end),
                    ),
                  ),
                  Card(
                    child: ListTile(
                      leading: Text('End', textAlign: TextAlign.start,),
                      title: Text('${model.dateEnd}', textAlign: TextAlign.end, style: TextStyle(fontWeight: FontWeight.w500, fontSize: 25.0)),
                    ),
                  ),
                  Card(
                    child: ListTile(
                      leading: Text('Created', textAlign: TextAlign.start,),
                      title: Text('${model.dateCreated}', textAlign: TextAlign.end,),
                    ),
                  )
                ],
              );
            } else {
              return Center(child: Text('No data'));
            }
          },
        ));
  }
}

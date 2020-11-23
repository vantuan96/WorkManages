import 'package:flutter/material.dart';
import 'package:flutter_pagewise/flutter_pagewise.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/widgets/AppBarWidget.dart';
import 'package:flutter_screen/src/widgets/Card/CardRoundRectangBorderWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverFillRemainingWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverViewPageWidget.dart';
import 'package:paging/paging.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_service/flutter_service.dart';
import 'dart:convert';

class NotificationPage extends StatefulWidget {
  NotificationPage({Key key}) : super(key: key);

  _NotificationPageState createState() => _NotificationPageState();
}

class _NotificationPageState extends State<NotificationPage> {
  List<NotificationModel> dumpList;
  Future<List<NotificationModel>> fDumpList;

  Future<List<NotificationModel>> pageData(
      int pageIndex, int pageSize, String token) async {
    pageIndex = pageIndex == 0 ? 1 : pageIndex;

    //print('currentListSize: $currentListSize');

    //var pageIndex = currentListSize == 0 ? 1 : (currentListSize / 10);
    //var number = pageIndex.round();

    var data = await NotificationService().getPagingByFist(
        pageIndex: pageIndex, pageSize: pageSize, token: token);
    //print('Status Code: ${data.statusCode}');

    List<NotificationModel> dumpList = List();

    if (data.statusCode == 200) {
      Map<String, dynamic> map = jsonDecode(data.body);
      print(map);

      var totalPage = map["TotalPage"] as int;
      print('totalPage: $totalPage');

      var totalItem = map["TotalIem"] as int;
      print('totalItem: $totalItem');

      map["Data"].forEach((f) {
        dumpList.add(NotificationModel.fromJson(f));
      });
    }

    return dumpList;
  }

  Widget itemBuilderRender(
      BuildContext context, NotificationModel entry, int index) {
    return CardRoundRectangBorderWidget(
        bolderColor: Colors.deepPurple,
        index: index,
        child: (index) => ListTile(
          title: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: <Widget>[
              Text(entry.title,
                  style: TextStyle(fontWeight: FontWeight.bold)),
              Text(entry.description,
                  style: TextStyle(fontWeight: FontWeight.normal))
            ],
          ),
          subtitle: Text(entry.dateCreated),
        ));
  }

  @override
  Widget build(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);

    return SliverViewPageWidget<NotificationModel>(
      backGroundColor: Colors.deepPurple,
      expandedHeightPercent: 16,
      dataPinnedHeader: Text('Notifications'),
      dataPinnedLeading: Icon(Icons.notifications),
      dataBackground: Column(
        children: <Widget>[
          ListTile(
            title: Text(
              'Notifications',
              style: TextStyle(
                color: Colors.white,
                fontSize: 32.0,
                fontWeight: FontWeight.w700,
              ),
            ),
          ),
        ],
      ),
      pageSize: 10,
      itemBuilder: (context, entry, index) =>
          itemBuilderRender(context, entry, index),
      pageFuture: (pageIndex, pageSize) =>
          pageData(pageIndex, pageSize, store.state.tokenState.token),
    );
  }
}

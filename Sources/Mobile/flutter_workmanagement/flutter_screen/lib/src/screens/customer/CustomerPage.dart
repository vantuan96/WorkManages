import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/widgets/Card/CardRoundRectangBorderWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SABTWidget.dart';
import 'package:flutter_screen/src/widgets/Sliver/SliverViewPageWidget.dart';
import 'package:flutter_service/flutter_service.dart';

import 'CustomerDetailPage.dart';

class CustomerPage extends StatefulWidget {
  @override
  _CustomerPageState createState() => _CustomerPageState();
}

class _CustomerPageState extends State<CustomerPage> {
  List<CustomerModel> dumpList;
  Future<List<CustomerModel>> fDumpList;

  Future<List<CustomerModel>> pageData(
      int pageIndex, int pageSize, String token) async {
    pageIndex = pageIndex == 0 ? 1 : pageIndex;

    //print('currentListSize: $currentListSize');

    //var pageIndex = currentListSize == 0 ? 1 : (currentListSize / 10);
    //var number = pageIndex.round();

    var data = await PageCustomerService().getPagingByFist(
        key: "", pageIndex: pageIndex, pageSize: pageSize, token: token);
    //print('Status Code: ${data.statusCode}');

    dumpList = List();

    if (data.statusCode == 200) {
      Map<String, dynamic> map = jsonDecode(data.body);
      print(map);

      var totalPage = map["TotalPage"] as int;
      print('totalPage: $totalPage');

      var totalItem = map["TotalIem"] as int;
      print('totalItem: $totalItem');

      map["Data"].forEach((f) {
        dumpList.add(CustomerModel.fromJson(f));
      });
    }

    return dumpList;
  }

  Widget itemBuilderRender(
      BuildContext context, CustomerModel entry, int index) {
    return CardRoundRectangBorderWidget(
        onPressed: (index) async {
          await Navigator.push(
            context,
            MaterialPageRoute(
                builder: (context) => CustomerDetailPage(
                  key: PageStorageKey('CustomerDetailPage'),
                  id: entry.id,
                )),
          );
        },
        bolderColor: Colors.indigoAccent,
        index: index,
        child: (index) => ListTile(
              title: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: <Widget>[
                  Text(entry.name,
                      style: TextStyle(fontWeight: FontWeight.bold)),
                  Text(entry.description,
                      style: TextStyle(fontWeight: FontWeight.normal))
                ],
              ),
              subtitle: Text(entry.customerGroupName),
            ));
  }

  @override
  Widget build(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);

    return SliverViewPageWidget<CustomerModel>(
      backGroundColor: Colors.indigoAccent,
      expandedHeightPercent: 16,
      dataPinnedHeader: Text("Contacts"),
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
      dataActions: <Widget>[
        SABTWidget(
          child: IconButton(
              icon: Icon(Icons.search),
              onPressed: () {
                showSearch(context: context, delegate: DataSearch(dumpList));
              }),
        )
      ],
      dataBackground: Row(
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
              "Contacts",
              style: TextStyle(
                color: Colors.white,
                fontSize: 25.0,
                fontWeight: FontWeight.w700,
              ),
            ),
          )
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

class DataSearch extends SearchDelegate<CustomerModel> {
  final List<CustomerModel> listWords;
  List<CustomerModel> dumpList;

  DataSearch(this.listWords);

  Future<List<CustomerModel>> pageData(
      String key, int pageIndex, int pageSize, String token) async {
    pageIndex = pageIndex == 0 ? 1 : pageIndex;

    //print('currentListSize: $currentListSize');

    //var pageIndex = currentListSize == 0 ? 1 : (currentListSize / 10);
    //var number = pageIndex.round();

    var data = await PageCustomerService().getPagingByFist(
        key: key, pageIndex: pageIndex, pageSize: pageSize, token: token);
    //print('Status Code: ${data.statusCode}');

    dumpList = List();

    if (data.statusCode == 200) {
      Map<String, dynamic> map = jsonDecode(data.body);
      print(map);

      var totalPage = map["TotalPage"] as int;
      print('totalPage: $totalPage');

      var totalItem = map["TotalIem"] as int;
      print('totalItem: $totalItem');

      map["Data"].forEach((f) {
        dumpList.add(CustomerModel.fromJson(f));
      });
    }

    return dumpList;
  }

  @override
  List<Widget> buildActions(BuildContext context) {
    //Actions for app bar
    return [
      IconButton(
          icon: Icon(Icons.clear),
          onPressed: () {
            query = '';
          })
    ];
  }

  @override
  Widget buildLeading(BuildContext context) {
    //leading icon on the left of the app bar
    return IconButton(
        icon: AnimatedIcon(
          icon: AnimatedIcons.menu_arrow,
          progress: transitionAnimation,
        ),
        onPressed: () {
          close(context, null);
        });
  }

  @override
  Widget buildResults(BuildContext context) {
    // show some result based on the selection
    return Center(
      child: Text(query),
    );
  }

  @override
  Widget buildSuggestions(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);

    return new FutureBuilder<List<CustomerModel>>(
        future: pageData(query, 1, 10, store.state.tokenState.token),
        builder: (context, snapshot) {
          if (snapshot.hasData) {
            return ListView.builder(
              itemBuilder: (context, index) => ListTile(
                onTap: () async {
                  await Navigator.push(
                    context,
                    MaterialPageRoute(
                        builder: (context) => CustomerDetailPage(
                              key: PageStorageKey('CustomerDetailPage'),
                              id: snapshot.data[index].id,
                            )),
                  );

                  //print(snapshot.data[index].id);
                },
                trailing: Icon(Icons.remove_red_eye),
                title: Text(snapshot.data[index].name),
              ),
              itemCount: snapshot.data.length,
            );
          } else {
            return ListView(
              children: <Widget>[
                ListTile(
                  trailing: Icon(Icons.remove_red_eye),
                  title: Text('No data'),
                )
              ],
            );
          }
        });
  }
}

import 'dart:convert';

import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/widgets/Card/CardRoundRectangBorderWidget.dart';
import 'package:flutter_service/flutter_service.dart';
import 'package:redux/redux.dart';

class PersonalReportPage extends StatefulWidget {
  @override
  _PersonalReportPageState createState() => _PersonalReportPageState();
}

class _PersonalReportPageState extends State<PersonalReportPage> {
  ReportPersonalModel dumpModel;
  Future<ReportPersonalModel> fDumpModel;

  ReportPersonalGrowModel dumpGModel;
  Future<ReportPersonalGrowModel> fDumpGModel;

  Future<ReportPersonalModel> getData(Store<AppState> store) async {
    var data = await PageReportService().getReportPersonalByFist(
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token);

    //print('Status Code: ${data.statusCode}');

    dumpModel = ReportPersonalModel();

    if (data.statusCode == 200) {
      Map<String, dynamic> map = jsonDecode(data.body);

      print(map);
      dumpModel = ReportPersonalModel.fromJson(map);
      print('d');
    }

    return dumpModel;
  }

  Future<ReportPersonalGrowModel> getDataGrow(Store<AppState> store) async {
    var data = await PageReportService().getReportPersonalGrowByFist(
        userId: store.state.tokenState.identifier,
        token: store.state.tokenState.token);

    //print('Status Code: ${data.statusCode}');

    dumpGModel = ReportPersonalGrowModel();

    if (data.statusCode == 200) {
      Map<String, dynamic> map = jsonDecode(data.body);

      print(map);
      dumpGModel = ReportPersonalGrowModel.fromJson(map);
      print('d');
    }

    return dumpGModel;
  }

  List<PieChartSectionData> dataPieProject(ReportPersonalModel model) {
    var k = List<PieChartSectionData>();

    k.add(PieChartSectionData(
        showTitle: true,
        color: Colors.red,
        radius: 140.0,
        title: '${model.projectStatus["Late"].toStringAsFixed(0)} %',
        value: model.projectStatus["Late"]));

    k.add(PieChartSectionData(
        showTitle: true,
        radius: 140.0,
        color: Colors.green,
        title: '${model.projectStatus["OnTime"].toStringAsFixed(0)} %',
        value: model.projectStatus["OnTime"]));

    k.add(PieChartSectionData(
        showTitle: true,
        radius: 140.0,
        color: Colors.blue,
        title: '${model.projectStatus["Doing"].toStringAsFixed(0)} %',
        value: model.projectStatus["Doing"]));

    return k;
  }

  List<PieChartSectionData> dataPieTask(ReportPersonalModel model) {
    var k = List<PieChartSectionData>();

    k.add(PieChartSectionData(
        showTitle: true,
        color: Colors.red,
        radius: 140.0,
        title: '${model.taskStatus["Late"].toStringAsFixed(0)} %',
        value: model.taskStatus["Late"]));

    k.add(PieChartSectionData(
        showTitle: true,
        radius: 140.0,
        color: Colors.green,
        title: '${model.taskStatus["OnTime"].toStringAsFixed(0)} %',
        value: model.taskStatus["OnTime"]));

    k.add(PieChartSectionData(
        showTitle: true,
        radius: 140.0,
        color: Colors.blue,
        title: '${model.taskStatus["Doing"].toStringAsFixed(0)} %',
        value: model.taskStatus["Doing"]));

    return k;
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      setState(() {
        final store = StoreProvider.of<AppState>(context);
        fDumpModel = getData(store);
        fDumpGModel = getDataGrow(store);
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
    return DefaultTabController(
      length: 3,
      child: Scaffold(
        appBar: AppBar(
          title: Text('Report'),
          bottom: TabBar(
            tabs: <Widget>[
              Tab(icon: Icon(Icons.info)),
              Tab(icon: Icon(Icons.pie_chart)),
              Tab(icon: Icon(Icons.info)),
            ],
          ),
        ),
        body: new FutureBuilder<ReportPersonalModel>(
            future: fDumpModel,
            builder: (context, snapshot) {
              if (snapshot.hasData) {
                return TabBarView(
                  children: <Widget>[
                    ListView(
                      padding: EdgeInsets.only(left: 8, right: 8),
                      children: <Widget>[
                        CardRoundRectangBorderWidget(
                          index: 1,
                          bolderColor: Colors.indigoAccent,
                          child: (index) => Column(
                            children: <Widget>[
                              ListTile(
                                title: Text(
                                  'Dự án',
                                  style: TextStyle(
                                      color: Settings.themeColor,
                                      fontWeight: FontWeight.w900),
                                ),
                              ),
                              Divider(
                                thickness: 1.0,
                              ),
                              ListTile(
                                title: Text('Tổng số: '),
                                trailing: Text(
                                  '${snapshot.data.projectTotal}',
                                  style: TextStyle(
                                      color: Colors.blue,
                                      fontWeight: FontWeight.w900),
                                ),
                              ),
                              ListTile(
                                title: Text('Đúng hạn: '),
                                trailing: Text(
                                  '${snapshot.data.projectCompleteOnTime}',
                                  style: TextStyle(
                                      color: Colors.green,
                                      fontWeight: FontWeight.w900),
                                ),
                              ),
                              ListTile(
                                title: Text('Chậm tiến độ: '),
                                trailing: Text(
                                  '${snapshot.data.projectCompleteLate}',
                                  style: TextStyle(
                                      color: Colors.red,
                                      fontWeight: FontWeight.w900),
                                ),
                              ),
                              ListTile(
                                title: Text('Đang làm: '),
                                trailing: Text('${snapshot.data.projectDoing}'),
                              )
                            ],
                          ),
                        ),
                        CardRoundRectangBorderWidget(
                          index: 1,
                          bolderColor: Colors.indigoAccent,
                          child: (index) => Column(
                            children: <Widget>[
                              ListTile(
                                title: Text(
                                  'Công việc',
                                  style: TextStyle(
                                      color: Settings.themeColor,
                                      fontWeight: FontWeight.w900),
                                ),
                              ),
                              Divider(
                                thickness: 1.0,
                              ),
                              ListTile(
                                title: Text('Tổng số: '),
                                trailing: Text(
                                  '${snapshot.data.taskTotal}',
                                  style: TextStyle(
                                      color: Colors.blue,
                                      fontWeight: FontWeight.w900),
                                ),
                              ),
                              ListTile(
                                title: Text('Đúng hạn: '),
                                trailing: Text(
                                  '${snapshot.data.taskCompleteOnTime}',
                                  style: TextStyle(
                                      color: Colors.green,
                                      fontWeight: FontWeight.w900),
                                ),
                              ),
                              ListTile(
                                title: Text('Chậm tiến độ: '),
                                trailing: Text(
                                  '${snapshot.data.taskCompleteLate}',
                                  style: TextStyle(
                                      color: Colors.red,
                                      fontWeight: FontWeight.w900),
                                ),
                              ),
                              ListTile(
                                title: Text('Đang làm: '),
                                trailing: Text('${snapshot.data.taskDoing}'),
                              )
                            ],
                          ),
                        )
                      ],
                    ),
                    ListView(
                      padding: EdgeInsets.only(left: 8, right: 8, top: 8),
                      children: <Widget>[
                        Text(
                          'Dự án',
                          style: TextStyle(
                              color: Settings.themeColor,
                              fontWeight: FontWeight.w900,
                              fontSize: 24.0),
                        ),
                        PieChart(
                          PieChartData(
                              sections: dataPieProject(snapshot.data),
                              borderData: FlBorderData(show: false),
                              pieTouchData: PieTouchData(enabled: true)),
                        ),
                        Row(
                          mainAxisSize: MainAxisSize.max,
                          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                          children: <Widget>[
                            Expanded(
                              child: ListTile(
                                leading: SizedBox(
                                  width: 15.0,
                                  height: 15.0,
                                  child: const DecoratedBox(
                                    decoration: const BoxDecoration(
                                      color: Colors.red,
                                    ),
                                  ),
                                ),
                                title: Center(child: Text('Chậm tiến độ')),
                              ),
                            ),
                            Expanded(
                              child: ListTile(
                                leading: SizedBox(
                                  width: 15.0,
                                  height: 15.0,
                                  child: const DecoratedBox(
                                    decoration: const BoxDecoration(
                                      color: Colors.green,
                                    ),
                                  ),
                                ),
                                title: Center(child: Text('Đúng hạn')),
                              ),
                            ),
                            Expanded(
                              child: ListTile(
                                leading: SizedBox(
                                  width: 15.0,
                                  height: 15.0,
                                  child: const DecoratedBox(
                                    decoration: const BoxDecoration(
                                      color: Colors.blue,
                                    ),
                                  ),
                                ),
                                title: Center(child: Text('Đang làm')),
                              ),
                            )
                          ],
                        ),
                        Divider(
                          thickness: 1.0,
                        ),
                        Text(
                          'Công việc',
                          style: TextStyle(
                              color: Settings.themeColor,
                              fontWeight: FontWeight.w900,
                              fontSize: 24.0),
                        ),
                        PieChart(
                          PieChartData(
                              sections: dataPieTask(snapshot.data),
                              borderData: FlBorderData(show: false),
                              pieTouchData: PieTouchData(enabled: true)),
                        ),
                        Row(
                          mainAxisSize: MainAxisSize.max,
                          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                          children: <Widget>[
                            Expanded(
                              child: ListTile(
                                leading: SizedBox(
                                  width: 15.0,
                                  height: 15.0,
                                  child: const DecoratedBox(
                                    decoration: const BoxDecoration(
                                      color: Colors.red,
                                    ),
                                  ),
                                ),
                                title: Center(child: Text('Chậm tiến độ')),
                              ),
                            ),
                            Expanded(
                              child: ListTile(
                                leading: SizedBox(
                                  width: 15.0,
                                  height: 15.0,
                                  child: const DecoratedBox(
                                    decoration: const BoxDecoration(
                                      color: Colors.green,
                                    ),
                                  ),
                                ),
                                title: Center(child: Text('Đúng hạn')),
                              ),
                            ),
                            Expanded(
                              child: ListTile(
                                leading: SizedBox(
                                  width: 15.0,
                                  height: 15.0,
                                  child: const DecoratedBox(
                                    decoration: const BoxDecoration(
                                      color: Colors.blue,
                                    ),
                                  ),
                                ),
                                title: Center(child: Text('Đang làm')),
                              ),
                            )
                          ],
                        ),
                      ],
                    ),
                    new FutureBuilder<ReportPersonalGrowModel>(
                        future: fDumpGModel,
                        builder: (context, snapshotInfo) {
                          if (snapshotInfo.hasData) {
                            return ListView(
                              padding: EdgeInsets.only(left: 8, right: 8),
                              children: <Widget>[
                                CardRoundRectangBorderWidget(
                                  child: (index) {
                                    return ListTile(
                                      title: Text('Dự án - Hiệu quả: '),
                                      trailing: Text('${snapshotInfo.data.projectCurrentPercent.toStringAsFixed(0)} %'),
                                      subtitle: Text('${ snapshotInfo.data.projectGrowPercent > 0 ? '+' : '-' } ${snapshotInfo.data.projectGrowPercent.toStringAsFixed(0)} %'),
                                    );
                                  },
                                  bolderColor: Colors.red,
                                  index: 1,
                                ),
                                CardRoundRectangBorderWidget(
                                  child: (index) {
                                    return ListTile(
                                      title: Text('Công việc - Hiệu quả: '),
                                      trailing: Text('${snapshotInfo.data.taskCurrentPercent.toStringAsFixed(0)} %'),
                                      subtitle: Text('${ snapshotInfo.data.taskGrowPercent > 0 ? '+' : '-' } ${snapshotInfo.data.taskGrowPercent.toStringAsFixed(0)} %'),
                                    );
                                  },
                                  bolderColor: Colors.teal,
                                  index: 1,
                                )
                              ],
                            );
                          } else {
                            return Center(
                              child: Text('No data'),
                            );
                          }
                        })
                  ],
                );
              } else {
                return Center(
                  child: CircularProgressIndicator(),
                );
              }
            }),
      ),
    );
  }
}

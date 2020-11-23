import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';

import 'SABTWidget.dart';
import 'SliverFillRemainingWidget.dart';

class SliverViewListWidget extends StatelessWidget {
  Color backGroundColor;
  Widget dataPinnedHeader;
  Widget dataPinnedLeading;
  Widget dataBackground;
  List<Widget> dataList;
  List<Widget> dataActions;
  double expandedHeightPercent;

  SliverViewListWidget({this.backGroundColor, this.expandedHeightPercent, this.dataPinnedHeader, this.dataPinnedLeading, this.dataBackground, this.dataList, this.dataActions});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: backGroundColor,
      body: SafeArea(
        child: CustomScrollView(
          slivers: <Widget>[
            SliverAppBar(
              title: SABTWidget(child: dataPinnedHeader),
              centerTitle: false,
              snap: false,
              floating: true,
              pinned: true,
              expandedHeight: FunctionHelper().screenHeight(context, expandedHeightPercent),
              elevation: 0,
              backgroundColor: backGroundColor,
              flexibleSpace: FlexibleSpaceBar(
                collapseMode: CollapseMode.pin,
                background: dataBackground,
              ),
              leading: SABTWidget(child: dataPinnedLeading),
              actions: dataActions,
            ),

            SliverList(
              delegate: new SliverChildListDelegate(
                [
                  Container(
                    padding: EdgeInsets.symmetric(horizontal: 20.0),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      border:
                      Border.all(width: 0, color: Colors.white),
                      borderRadius: BorderRadius.only(
                        topLeft: Radius.circular(20.0),
                        topRight: Radius.circular(20.0),
                      ),
                    ),
                    child: Column(
                      children: dataList,
                    ),
                  )
                ],
              )
            ),

            SilverFillRemainingWidget()
          ],
        ),
      ),
    );
  }
}


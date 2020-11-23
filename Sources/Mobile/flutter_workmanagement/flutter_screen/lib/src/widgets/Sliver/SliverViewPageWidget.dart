import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_pagewise/flutter_pagewise.dart';

import 'SABTWidget.dart';
import 'SliverFillRemainingWidget.dart';

typedef SliverViewPageWidget_itemBuilder<T> = Widget Function(BuildContext context, T model, int index);
typedef SliverViewPageWidget_pageFuture<T> = Future<List<T>> Function(int pageIndex, int pageSize);

class SliverViewPageWidget<T> extends StatelessWidget {
  Color backGroundColor;
  Widget dataPinnedHeader;
  Widget dataPinnedLeading;
  Widget dataBackground;
  List<Widget> dataActions;
  double expandedHeightPercent;
  int pageSize;

  SliverViewPageWidget_itemBuilder<T> itemBuilder;
  SliverViewPageWidget_pageFuture<T> pageFuture;

  SliverViewPageWidget({this.backGroundColor, this.expandedHeightPercent, this.dataPinnedHeader, this.dataPinnedLeading, this.dataBackground, this.pageSize, this.itemBuilder, this.pageFuture, this.dataActions});

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

            PagewiseSliverList<T>(
              pageSize: pageSize,
              itemBuilder: (context, entry, index) => Container(
                padding: EdgeInsets.symmetric(horizontal: 20.0),
                decoration: BoxDecoration(
                  color: Colors.white,
                  border:
                  Border.all(width: 0, color: Colors.white),
                  borderRadius: BorderRadius.only(
                    topLeft: Radius.circular(index == 0 ? 20.0 : 0.0),
                    topRight: Radius.circular(index == 0 ? 20.0 : 0.0),
                  ),
                ),
                child: itemBuilder(context, entry, index),
              ),
              pageFuture: (pageIndex) => pageFuture(pageIndex, pageSize),
            ),

            SilverFillRemainingWidget()
          ],
        ),
      ),
    );
  }
}

import 'package:bottom_navy_bar/bottom_navy_bar.dart';
import 'package:flutter/material.dart';

typedef BottomNavyBarWidget_onSelectedTab = void Function(int selectedIndex);

class BottomNavyBarWidget extends StatelessWidget {
  final BottomNavyBarWidget_onSelectedTab onSelectIndex;

  final List<BottomNavyBarItem> items;
  final int currentIndex;

  BottomNavyBarWidget({this.currentIndex, this.onSelectIndex, this.items});

  @override
  Widget build(BuildContext context) {
    return BottomNavyBar(
      selectedIndex: currentIndex,
      showElevation: true, // use this to remove appBar's elevation
      onItemSelected: (index) => onSelectIndex(index),
      items: items,
    );
  }
}

import 'package:flutter/material.dart';

typedef BottomNavigationBarWidget_onSelectedTab = void Function(int selectedIndex);

class BottomNavigationBarWidget extends StatelessWidget {
  final BottomNavigationBarWidget_onSelectedTab onSelectIndex;

  final List<BottomNavigationBarItem> items;
  final int currentIndex;

  BottomNavigationBarWidget({this.currentIndex, this.onSelectIndex, this.items});

  @override
  Widget build(BuildContext context) {
    return BottomNavigationBar(
      currentIndex: currentIndex,
      elevation: 0, // use this to remove appBar's elevation
      onTap: (index) => onSelectIndex(index),
      items: items,
    );
  }
}

import 'package:bubble_bottom_bar/bubble_bottom_bar.dart';
import 'package:flutter/material.dart';

typedef BubbleBottomBarWidget_onSelectedTab = void Function(int selectedIndex);

class BubbleBottomBarWidget extends StatelessWidget {
  final BubbleBottomBarWidget_onSelectedTab onSelectIndex;

  final List<BubbleBottomBarItem> items;
  final int currentIndex;

  BubbleBottomBarWidget({this.currentIndex, this.onSelectIndex, this.items});

  @override
  Widget build(BuildContext context) {
    return BubbleBottomBar(
      opacity: .2,
      currentIndex: currentIndex,
      onTap: (index) => onSelectIndex(index),
      borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      elevation: 8,
      //fabLocation: BubbleBottomBarFabLocation.end, //new
      hasNotch: false, //new
      hasInk: true, //new, gives a cute ink effect
      inkColor: Colors.black12,
      items: items,
    );
  }
}


import 'package:flutter/material.dart';
import 'package:flutter_speed_dial_material_design/flutter_speed_dial_material_design.dart';

typedef SpeedDialFloatingActionButtonWidget_onSelectedItem = void Function(int selectedIndex);

class SpeedDialFloatingActionButtonWidget extends StatelessWidget {
  final SpeedDialFloatingActionButtonWidget_onSelectedItem onSelectIndex;
  final SpeedDialController controller;
  final List<SpeedDialAction> actions;

  SpeedDialFloatingActionButtonWidget({this.onSelectIndex, this.controller, this.actions});

  @override
  Widget build(BuildContext context) {
    return SpeedDialFloatingActionButton(
      controller: controller,
      actions: actions,
      childOnFold: Icon(Icons.menu, key: UniqueKey()),
      childOnUnfold: Icon(Icons.add),
      useRotateAnimation: true,
      onAction: (index) => onSelectIndex(index),
    );
  }
}

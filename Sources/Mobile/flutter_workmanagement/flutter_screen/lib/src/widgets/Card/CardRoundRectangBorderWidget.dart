import 'package:flutter/material.dart';

typedef CardView1Widget_child = Widget Function(int index);
typedef CardView1Widget_onLongPress = void Function(int index);
typedef CardView1Widget_onPressed = void Function(int index);

class CardRoundRectangBorderWidget extends StatelessWidget {
  Color bolderColor;
  int index;
  CardView1Widget_onLongPress onLongPress;
  CardView1Widget_onPressed onPressed;
  CardView1Widget_child child;

  CardRoundRectangBorderWidget({this.bolderColor, this.index, this.onLongPress, this.onPressed, this.child});

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: EdgeInsets.only(top: index == 0 ? 20.0 : 10.0),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(20.0),
        side: BorderSide(
          color: bolderColor,
          width: 0.5,
        ),
      ),
      child: FlatButton(
        padding: EdgeInsets.all(0),
        onLongPress: () => onLongPress(index),
        onPressed: () => onPressed(index),
        child: child(index),
      ),
    );
  }
}

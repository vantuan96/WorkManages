import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';

Widget AppBarWidgetNoLeading({String title, Brightness brightness, bool centerTitle, TextStyle textStyle}) => AppBar (
  title: Text(title, style: textStyle ,),
  automaticallyImplyLeading: false,
  brightness: brightness,
  centerTitle: centerTitle,
  elevation: 0,
);

Widget AppBarWidgetTransparent({String title, Brightness brightness, bool centerTitle, Widget iconLeading}) => AppBar (
  title: Text(title, style: TextStyle(color: Settings.themeColor) ,),
  automaticallyImplyLeading: false,
  brightness: brightness,
  centerTitle: centerTitle,
  elevation: 0,
  backgroundColor: Colors.transparent,
  leading: iconLeading,
);

Widget AppBarWidgetNormal({String title, Brightness brightness, bool centerTitle, Widget iconLeading, TextStyle textStyle}) => AppBar (
  title: Text(title, style: textStyle,),
  brightness: brightness,
  centerTitle: centerTitle,
  elevation: 0,
  backgroundColor: Settings.themeColor,
  leading: iconLeading,
);
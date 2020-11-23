import 'package:flutter/material.dart';

class FunctionHelper {
  Size _screenSize(BuildContext context) {
    return MediaQuery.of(context).size;
  }

  /// Get percent base on screen height device
  double screenHeight(BuildContext context, double percent) {
    return (_screenSize(context).height * percent) / 100;
  }

  /// Get percent base on screen width device
  double screenWidth(BuildContext context, double percent) {
    return (_screenSize(context).width * percent) / 100;
  }
}
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:toast/toast.dart';

class ToastHelper {
  void showTopToastError({BuildContext context, String message}) {
    Toast.show(message, context,
        duration: Toast.LENGTH_LONG,
        gravity: Toast.TOP,
        backgroundColor: Colors.red);
  }

  void showTopToastSuccess({BuildContext context, String message}) {
    Toast.show(message, context,
        duration: Toast.LENGTH_SHORT,
        gravity: Toast.TOP,
        backgroundColor: Colors.green);
  }

  void showTopToastInfo({BuildContext context, String message}) {
    Toast.show(message, context,
        duration: Toast.LENGTH_SHORT,
        gravity: Toast.TOP,
        backgroundColor: Colors.blue);
  }

  void showTopToastWarning({BuildContext context, String message}) {
    Toast.show(message, context,
        duration: Toast.LENGTH_SHORT,
        gravity: Toast.TOP,
        backgroundColor: Colors.orange);
  }
}

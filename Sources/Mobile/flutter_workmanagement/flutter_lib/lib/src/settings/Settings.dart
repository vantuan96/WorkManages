import 'package:flutter/material.dart';

class Settings {
  static const MaterialAccentColor themeColor = Colors.indigoAccent;
  static const MaterialAccentColor iconColor = Colors.indigoAccent;
  static const String environmentApp = "RELEASE"; //RELEASE DEBUG
  static const String hostUrl = environmentApp == "DEBUG" ? "http://10.0.2.2:5002/" : "http://workmanagement.kztek.net/";
  static const bool isKeepCurrentUser = true;
  static const String welcomePageName = "Work management ";
  static const IconData appIcon = Icons.work;
  static const String kGoogleApiKey = "AIzaSyC0VujTDEvJUvrTudjsjfK7rXRboley4YE";
}

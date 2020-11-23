import 'package:flutter/material.dart';

Widget SilverFillRemainingWidget() => SliverFillRemaining(
  fillOverscroll: true,
  hasScrollBody: true,
  child: Container(
    decoration: BoxDecoration(
      color: Colors.white,
      border: Border.all(width: 0, color: Colors.white),
    ),
  ),
);
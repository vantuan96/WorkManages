import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';

class QRCodeResultPage extends StatefulWidget {
  @override
  _QRCodeResultPageState createState() => _QRCodeResultPageState();
}

class _QRCodeResultPageState extends State<QRCodeResultPage> {
  @override
  Widget build(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);
    var result = store.state.scanState.result != null
        ? store.state.scanState.result
        : "";

    return Scaffold(
      appBar: AppBar(title: Text('Scan result'), centerTitle: false),
      body: Center(
        child: Text(result),
      ),
      floatingActionButton: FloatingActionButton(
        child: const Icon(FontAwesomeIcons.clipboard),
        onPressed: () {
          Clipboard.setData(ClipboardData(text: store.state.scanState.result));
          ToastHelper().showTopToastInfo(context: context, message: "copied");
        },
      ),
    );
  }
}

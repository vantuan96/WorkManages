import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class MomoPage extends StatefulWidget {
  @override
  _MomoPageState createState() => _MomoPageState();
}

class _MomoPageState extends State<MomoPage> {
  static const platform =
      const MethodChannel('com.maximusnguyen.flutterWorkmanagement/momo');

  var model = {
    "merchantcode": "CGV01",
    "merchantname": "Công ty cổ phần đầu từ và phát triển Kztek",
    "merchantnamelabel" : "Service",
    "orderId": "1234567890",
    "orderLabel": "",
    "amount": 10000,
    "fee": 0,
    "description": "Phí dùng app",
    "extra": "",
    "username": ""
  };

  Future _loadSurveyMonkey() async {
    try {
      await platform.invokeMethod('payment', model).then((result) {
        print(result);
      });
    } on PlatformException catch (e) {
      print(e);
    }
  }

  @override
  Widget build(BuildContext context) {


    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(title: Text("Momo"),),
      body: Center(
        child: RaisedButton(
          child: Icon(Icons.attach_money),
          onPressed: () async {
            await _loadSurveyMonkey();
          },
        ),
      ),
    );
  }
}

import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_form_builder/flutter_form_builder.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:signalr_client/signalr_client.dart';
import 'package:logging/logging.dart';

class SignalRPage extends StatefulWidget {
  @override
  _SignalRPageState createState() => _SignalRPageState();
}

class _SignalRPageState extends State<SignalRPage> {
  String address = 'http://192.168.1.189:8080/workHub';
  HubConnection hubConnection;
  int state;
  bool forceStop;

  final GlobalKey<FormBuilderState> _fbKey = GlobalKey<FormBuilderState>();

  void updateAddress(String value) {
    print(value);

    setState(() {
      address = value;
    });
  }

  void connectSignalR() async {
    print('connecting');

    Logger.root.level = Level.ALL;
    Logger.root.onRecord.listen((LogRecord rec) {
      print('${rec.level.name}: ${rec.time}: ${rec.message}');
    });

    final hubLogger = Logger("SignalR - hub");
    final transportLogger = Logger("SignalR - transport");

    final httpOptions = new HttpConnectionOptions(logger: transportLogger);

    hubConnection = HubConnectionBuilder()
        .withUrl(address, options: httpOptions)
        .configureLogging(hubLogger)
        .build();

    await hubConnection.start();

    setState(() {
      state = hubConnection.state.index;
      forceStop = false;
    });

    print(hubConnection.state.index);

    hubConnection.on("ReceiveMessage", _handleAClientProvidedFunction);

    hubConnection.onclose((error) {
      print("Connection Closed");
      print(hubConnection.state.index);
      setState(() {
        state = hubConnection.state.index;
      });

      Timer.periodic(new Duration(seconds: 10), (timer) async {
        if (hubConnection.state.index == 0 && forceStop == false) {
          await hubConnection.start();

          setState(() {
            state = 1;
          });
        }
      });

    });

  }

  void callToServer() async {
    await hubConnection.invoke("SendMessage");
  }

  void _handleAClientProvidedFunction(List<Object> parameters) {
//    ToastHelper()
//        .showTopToastInfo(context: context, message: jsonEncode(parameters));
    print('Data: $parameters');
  }

  void stop() async {
    await hubConnection.stop();

    setState(() {
      forceStop = true;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('SignalR'),
        actions: <Widget>[
          IconButton(
            icon: Icon(
              state == 1 ? Icons.cloud : Icons.cloud_off,
              color: Colors.white,
            ),
          )
        ],
      ),
      body: ListView(
        padding: EdgeInsets.all(8),
        children: <Widget>[
          Card(
            child: FormBuilder(
                key: _fbKey,
                autovalidate: true,
                child: Column(
                  children: <Widget>[
                    FormBuilderTextField(
                      initialValue: address,
                      attribute: "address",
                      decoration: InputDecoration(
                          labelText: "Address", icon: Icon(Icons.my_location)),
                      onChanged: (value) => updateAddress(value),
                    ),
                  ],
                )),
          ),
          SizedBox(
            height: 25.0,
            width: 25.0,
          ),
          RaisedButton(
            onPressed: callToServer,
            child: Text('Hello to server'),
          ),
          RaisedButton(
            onPressed: connectSignalR,
            color: Colors.green,
            child: Text(
              'Connect to server',
              style: TextStyle(color: Colors.white),
            ),
          ),
          RaisedButton(
            onPressed: stop,
            color: Colors.red,
            child: Text(
              'Disconnect to server',
              style: TextStyle(color: Colors.white),
            ),
          )
        ],
      ),
    );
  }
}

import 'dart:ffi';

import 'package:barcode_scan/barcode_scan.dart';
import 'package:bubble_bottom_bar/bubble_bottom_bar.dart';
import 'package:camera/camera.dart';
import 'package:flutter/material.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:flutter_screen/src/screens/camera/CameraPage.dart';
import 'package:flutter_speed_dial/flutter_speed_dial.dart';
import 'package:flutter_speed_dial_material_design/flutter_speed_dial_material_design.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:onesignal_flutter/onesignal_flutter.dart';
import 'package:redux/redux.dart';

import '../flutter_screen.dart';
import 'screens/PaymentOnline/MomoPage.dart';
import 'screens/map/GoogleMapPage.dart';
import 'screens/qrcode/QRCodeResultPage.dart';
import 'screens/schedule/SchedulePage.dart';
import 'screens/signalr/SignalRPage.dart';
import 'screens/task/TaskPage.dart';
import 'widgets/BottomBar/BubbleBottomBarWidget.dart';
import 'widgets/SpeedDial/SpeedDialWidget.dart';

class Routes extends StatefulWidget {
  @override
  _RoutesState createState() => _RoutesState();
}

class _RoutesState extends State<Routes> {
  final List<Widget> pages = [
    HomePage(
      key: PageStorageKey('HomePage'),
    ),
    TaskPage(
      key: PageStorageKey('TaskPage'),
    ),
    SchedulePage(
      key: PageStorageKey('SchedulePage'),
    ),
    NotificationPage(
      key: PageStorageKey('NotificationPage'),
    ),
    AccountPage(
      key: PageStorageKey('AccountPage'),
    )
  ];

  final PageStorageBucket bucket = PageStorageBucket();

  Future scan(BuildContext context, Store<AppState> store) async {
    try {
      String qrResult = await BarcodeScanner.scan();

      print(qrResult);

      store.dispatch(ScanAction(scanState: ScanState(result: qrResult)));

      await Navigator.push(
        context,
        MaterialPageRoute(
            builder: (context) => QRCodeResultPage(), fullscreenDialog: true),
      );
    } catch (e) {
      print(e);
    }
  }

  Future goToCamera() async {
    WidgetsFlutterBinding.ensureInitialized();

    // Obtain a list of the available cameras on the device.
    final cameras = await availableCameras();

    // Get a specific camera from the list of available cameras.
    final firstCamera = cameras.first;

    await Navigator.push(
      context,
      MaterialPageRoute(
          builder: (context) => CameraPage(camera: firstCamera,), fullscreenDialog: true),
    );
  }

  Future goToMomo() async {

    await Navigator.push(
      context,
      MaterialPageRoute(
          builder: (context) => MomoPage(), fullscreenDialog: true),
    );
  }

  Future goToMap() async {

    await Navigator.push(
      context,
      MaterialPageRoute(
          builder: (context) => GoogleMapPage(), fullscreenDialog: true),
    );
  }

  Future goToSignalR() async {

    await Navigator.push(
      context,
      MaterialPageRoute(
          builder: (context) => SignalRPage(), fullscreenDialog: true),
    );
  }

  Future goToVideoCall(Store<AppState> store) async {


  }

  SpeedDialController _controller = SpeedDialController();

  @override
  Widget build(BuildContext context) {
    final store = StoreProvider.of<AppState>(context);

    OneSignal.shared
        .setNotificationOpenedHandler((OSNotificationOpenedResult result) {
      print(result.notification.payload.additionalData["View"]);


      if (result.notification.payload.additionalData["View"] ==
          "ProjectPage") {
        setState(() {
          store.dispatch(BottomBarAction(
              bottomBarState: BottomBarState(selectedIndex: 0)));
        });


      }

      if (result.notification.payload.additionalData["View"] ==
          "TaskPage") {
        setState(() {
          store.dispatch(BottomBarAction(
              bottomBarState: BottomBarState(selectedIndex: 1)));
        });


      }

      if (result.notification.payload.additionalData["View"] ==
          "NotificationPage") {
        setState(() {
          store.dispatch(BottomBarAction(
              bottomBarState: BottomBarState(selectedIndex: 3)));
        });
      }
    });

    return GestureDetector(
      onTap: () {
        // call this method here to hide soft keyboard
        FocusScope.of(context).requestFocus(new FocusNode());

        _controller.unfold();
        _controller.dispose();
      },
      child: Scaffold(
        //floatingActionButtonLocation: FloatingActionButtonLocation.endDocked,
//      floatingActionButton: FloatingActionButton(
//        child: const Icon(FontAwesomeIcons.qrcode),
//        onPressed: () => scan(context, store),
//      ),
        floatingActionButton: SpeedDialWidget(context, [
          SpeedDialChild(
              child: Icon(FontAwesomeIcons.qrcode),
              backgroundColor: Colors.red,
              label: 'Scan',
              labelStyle: TextStyle(fontSize: 18.0),
              onTap: () => scan(context, store)
          ),
          SpeedDialChild(
              child: Icon(FontAwesomeIcons.camera),
              backgroundColor: Colors.orange,
              label: 'Camera',
              labelStyle: TextStyle(fontSize: 18.0),
              onTap: () => goToCamera()
          ),
          SpeedDialChild(
              child: Icon(Icons.monetization_on),
              backgroundColor: Colors.purple,
              label: 'Momo',
              labelStyle: TextStyle(fontSize: 18.0),
              onTap: () => goToMomo()
          ),
          SpeedDialChild(
              child: Icon(Icons.map),
              backgroundColor: Colors.teal,
              label: 'Map',
              labelStyle: TextStyle(fontSize: 18.0),
              onTap: () => goToMap()
          ),
//          SpeedDialChild(
//              child: Icon(Icons.call),
//              backgroundColor: Colors.pinkAccent,
//              label: 'Video call',
//              labelStyle: TextStyle(fontSize: 18.0),
//              onTap: () => goToVideoCall(store)
//          )
          SpeedDialChild(
              child: Icon(Icons.settings_input_antenna),
              backgroundColor: Colors.teal,
              label: 'Signalr',
              labelStyle: TextStyle(fontSize: 18.0),
              onTap: () => goToSignalR()
          )
        ]),
        bottomNavigationBar: BubbleBottomBarWidget(
          currentIndex: store.state.bottomBarState.selectedIndex,
          onSelectIndex: (index) => setState(() {
            store.dispatch(BottomBarAction(
                bottomBarState: BottomBarState(selectedIndex: index)));
          }),
          items: [
            BubbleBottomBarItem(
                backgroundColor: Colors.red,
                icon: Icon(
                  Icons.timeline,
                  color: Colors.black,
                  size: 18,
                ),
                activeIcon: Icon(
                  Icons.timeline,
                  color: Colors.red,
                  size: 18,
                ),
                title: Text("Project", style: TextStyle(fontSize: 12),)),
            BubbleBottomBarItem(
                backgroundColor: Colors.teal,
                icon: Icon(
                  Icons.work,
                  color: Colors.black,
                  size: 18,
                ),
                activeIcon: Icon(
                  Icons.work,
                  color: Colors.teal,
                  size: 18,
                ),
                title: Text("Task", style: TextStyle(fontSize: 12),)),
            BubbleBottomBarItem(
                backgroundColor: Colors.lightBlueAccent,
                icon: Icon(
                  Icons.calendar_today,
                  color: Colors.black,
                  size: 18,
                ),
                activeIcon: Icon(
                  Icons.calendar_today,
                  color: Colors.lightBlueAccent,
                  size: 18,
                ),
                title: Text("Schedule", style: TextStyle(fontSize: 12),)),
            BubbleBottomBarItem(
                backgroundColor: Colors.deepPurple,
                icon: Icon(
                  Icons.notifications,
                  color: Colors.black,
                  size: 18,
                ),
                activeIcon: Icon(
                  Icons.notifications,
                  color: Colors.deepPurple,
                  size: 18,
                ),
                title: Text("Notifications", style: TextStyle(fontSize: 12),)),
            BubbleBottomBarItem(
                backgroundColor: Colors.indigo,
                icon: Icon(
                  Icons.account_circle,
                  color: Colors.black,
                  size: 18,
                ),
                activeIcon: Icon(
                  Icons.account_circle,
                  color: Colors.indigo,
                  size: 18,
                ),
                title: Text("Account", style: TextStyle(fontSize: 12),)),
          ],
        ),
        body: PageStorage(
          child: pages[store.state.bottomBarState.selectedIndex],
          bucket: bucket,
        ),
      ),
    );
  }
}

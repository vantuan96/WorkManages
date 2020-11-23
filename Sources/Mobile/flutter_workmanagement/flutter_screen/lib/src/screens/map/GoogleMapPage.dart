import 'dart:async';

import 'package:flutter/material.dart';
import 'package:geolocator/geolocator.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

class GoogleMapPage extends StatefulWidget {
  @override
  _GoogleMapPageState createState() => _GoogleMapPageState();
}

class _GoogleMapPageState extends State<GoogleMapPage> {
  Completer<GoogleMapController> _controller = Completer();
  Future<Position> dumpPosition;
  MapType _currentMapType = MapType.normal;
  final Set<Marker> _markers = {};

  Future<Position> currentPosition() async {
    var position = await Geolocator()
        .getCurrentPosition(desiredAccuracy: LocationAccuracy.high);

    return position;
  }

  void _onMapTypeButtonPressed() {
    setState(() {
      _currentMapType = _currentMapType == MapType.normal
          ? MapType.satellite
          : MapType.normal;
    });
  }

  void _onClearAllMarker() {
   setState(() {
     _markers.clear();
   });
  }

  void _onAddMarkerButtonPressed(LatLng target) {
    var marker = Marker(
      // This marker id can be anything that uniquely identifies each marker.
      markerId: MarkerId(target.toString()),
      position: target,
      icon: BitmapDescriptor.defaultMarker,
    );

    setState(() {
      _markers.add(marker);
    });

  }

  Future<void> _handlePressButton() async {
    try {
      final center = await currentPosition();


    } catch (e) {
      return;
    }
  }

  @override
  void initState() {
    // TODO: implement initState
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      setState(() {
        dumpPosition = currentPosition();
      });
    });
  }

  @override
  Widget build(BuildContext context) {
    return new FutureBuilder<Position>(
        future: dumpPosition,
        builder: (context, snapshot) {
          return Scaffold(
              appBar: AppBar(
                title: Text('Map'),
                centerTitle: false,
                actions: <Widget>[
                  IconButton(
                    icon: Icon(Icons.search),
                    onPressed: _handlePressButton,
                  ),
                  IconButton(
                    icon: Icon(_currentMapType == MapType.normal
                        ? Icons.satellite
                        : Icons.map),
                    onPressed: _onMapTypeButtonPressed,
                  ),
                  IconButton(
                    icon: Icon(Icons.clear_all),
                    onPressed: _onClearAllMarker,
                  )
                ],
              ),
              body: snapshot.hasData
                  ? GoogleMap(
                      myLocationEnabled: true,
                      myLocationButtonEnabled: true,
                      compassEnabled: true,
                      mapToolbarEnabled: true,
                      mapType: _currentMapType,
                      initialCameraPosition: CameraPosition(
                        target: LatLng(
                            snapshot.data.latitude, snapshot.data.longitude),
                        zoom: 14.4746,
                      ),
                      markers: _markers,
                      onMapCreated: (GoogleMapController controller) {
                        _controller.complete(controller);
                      },
                      onTap: (latLng) => _onAddMarkerButtonPressed(latLng),
                    )
                  : Center(
                      child: CircularProgressIndicator(),
                    ));
        });
  }
}

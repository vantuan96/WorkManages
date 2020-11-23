import 'package:flutter/foundation.dart';
import 'package:flutter_lib/flutter_lib.dart';
import 'package:flutter_lib/src/reduxs/models/ScanState.dart';

import 'BottomBarState.dart';
import 'ProjectState.dart';
import 'TokenState.dart';

@immutable
class AppState {
  final TokenState tokenState;
  final BottomBarState bottomBarState;
  final ProjectState projectState;
  final ScanState scanState;
  final AccountInfoState accountInfoState;

  AppState(
      {@required this.tokenState,
      @required this.bottomBarState,
      @required this.projectState,
      @required this.scanState,
      @required this.accountInfoState});

  factory AppState.initial() {
    return AppState(
        tokenState: TokenState.initial(),
        bottomBarState: BottomBarState.initial(),
        projectState: ProjectState.initial(),
        scanState: ScanState.initial(),
        accountInfoState: AccountInfoState.initial());
  }
}

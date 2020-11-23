
import 'package:flutter_lib/src/reduxs/reducers/AccountInfoReducer.dart';
import 'package:flutter_lib/src/reduxs/reducers/ScanReducer.dart';

import '../models/AppState.dart';
import 'BottomBarReducer.dart';
import 'ProjectReducer.dart';
import 'TokenReducer.dart';

AppState appReducer(AppState state, action) {
  return AppState(
    tokenState: tokenReducer(state.tokenState, action),
    bottomBarState: bottomBarReducer(state.bottomBarState, action),
    projectState:  projectReducer(state.projectState, action),
    scanState: scanReducer(state.scanState, action),
    accountInfoState: accountInfoReducer(state.accountInfoState, action)
  );
}
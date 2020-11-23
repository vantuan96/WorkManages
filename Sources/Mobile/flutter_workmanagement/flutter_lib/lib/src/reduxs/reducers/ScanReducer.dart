import 'package:flutter_lib/src/reduxs/actions/ScanAction.dart';
import 'package:flutter_lib/src/reduxs/models/ScanState.dart';
import 'package:redux/redux.dart';

final scanReducer = combineReducers<ScanState>([
  TypedReducer<ScanState, ScanAction>(_tokenStateUpdate)
]);

ScanState _tokenStateUpdate(ScanState state, ScanAction action) {
  return action.scanState;
}
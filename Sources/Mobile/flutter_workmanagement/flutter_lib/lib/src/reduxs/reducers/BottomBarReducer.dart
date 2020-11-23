import 'package:redux/redux.dart';

import '../actions/BottomBarAction.dart';
import '../models/BottomBarState.dart';


final bottomBarReducer = combineReducers<BottomBarState>([
  TypedReducer<BottomBarState, BottomBarAction>(_bottomBarStateUpdate)
]);

BottomBarState _bottomBarStateUpdate(BottomBarState state, BottomBarAction action) {
  print(action.bottomBarState.selectedIndex);
  return action.bottomBarState;
}
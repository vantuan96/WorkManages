import 'package:redux/redux.dart';

import '../actions/TokenAction.dart';
import '../models/TokenState.dart';


final tokenReducer = combineReducers<TokenState>([
  TypedReducer<TokenState, TokenAction>(_tokenStateUpdate)
]);

TokenState _tokenStateUpdate(TokenState state, TokenAction action) {
  return action.tokenState;
}
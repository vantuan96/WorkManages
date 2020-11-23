import 'package:flutter_lib/src/reduxs/actions/AccountInfoAction.dart';
import 'package:flutter_lib/src/reduxs/models/AccountInfoState.dart';
import 'package:redux/redux.dart';



final accountInfoReducer = combineReducers<AccountInfoState>([
  TypedReducer<AccountInfoState, AccountInfoAction>(_accountInfoStateUpdate)
]);

AccountInfoState _accountInfoStateUpdate(AccountInfoState state, AccountInfoAction action) {
  return action.accountInfoState;
}
import 'package:flutter_lib/src/reduxs/actions/ProjectAction.dart';
import 'package:flutter_lib/src/reduxs/models/ProjectState.dart';
import 'package:redux/redux.dart';



final projectReducer = combineReducers<ProjectState>([
  TypedReducer<ProjectState, ProjectAction>(_projectStateUpdate)
]);

ProjectState _projectStateUpdate(ProjectState state, ProjectAction action) {
  return action.projectState;
}
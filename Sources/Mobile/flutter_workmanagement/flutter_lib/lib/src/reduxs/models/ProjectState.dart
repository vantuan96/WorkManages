class ProjectState {
  String projectId;
  String componentId;
  ProjectState({this.projectId, this.componentId});

  factory ProjectState.initial() {
    return new ProjectState(projectId: "", componentId: "");
  }
}

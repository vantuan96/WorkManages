class ScanState {
  String result;

  ScanState({this.result});

  factory ScanState.initial() {
    return new ScanState(result: "");
  }
}
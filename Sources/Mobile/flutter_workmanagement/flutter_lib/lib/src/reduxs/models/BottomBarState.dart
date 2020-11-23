class BottomBarState {
  int selectedIndex;

  BottomBarState({this.selectedIndex});

  factory BottomBarState.initial(){
    return new BottomBarState(
        selectedIndex: 0
    );
  }

}
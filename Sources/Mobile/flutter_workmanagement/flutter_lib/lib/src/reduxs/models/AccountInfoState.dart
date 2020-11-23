class AccountInfoState {
  String avatar;
  String name;
  String phone;
  String email;
  bool isEnableFingerAuth;

  AccountInfoState(
      {this.avatar,
      this.name,
      this.phone,
      this.email,
      this.isEnableFingerAuth});

  factory AccountInfoState.initial() {
    return new AccountInfoState(
        email: "", avatar: "", isEnableFingerAuth: false, name: "", phone: "");
  }

  factory AccountInfoState.fromJson(Map<String, dynamic> json, bool isEnableFingerAuth) {
    return AccountInfoState(
      name: json["Name"],
      isEnableFingerAuth: isEnableFingerAuth,
      avatar: json["Avatar"],
      email: json.containsKey("Username") ? json["Username"] : "",
      phone: json.containsKey("Phone") ? json["Phone"] : "",
    );
  }
}

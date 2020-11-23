class TokenState {
  String identifier;
  int expiresIn;
  String token;

  TokenState({this.identifier, this.expiresIn, this.token});

  factory TokenState.initial(){
    return new TokenState(
      identifier: "",
      expiresIn: 0,
      token: ""
    );
  }

  factory TokenState.fromJson(Map<String, dynamic> json) {
    return TokenState(
      identifier: json['Identifier'] as String,
      expiresIn: json['Expires_In'] as int,
      token: json['Token'] as String,
    );
  }
}
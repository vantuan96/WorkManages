import 'package:onesignal_flutter/onesignal_flutter.dart';

class OneSignalHelper {
  Future<String> getUserId() async {
    var status = await OneSignal.shared.getPermissionSubscriptionState();

    var playerId = status.subscriptionStatus.userId;

    return playerId;
  }
}


import UIKit
import Flutter
import MomoiOSSwiftSdk
import GoogleMaps
import PushKit                     /* <------ add this line */
import CallKit

@UIApplicationMain
@objc class AppDelegate: FlutterAppDelegate {
    
    
    
  override func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {

    //Google map
    GMSServices.provideAPIKey("AIzaSyC0VujTDEvJUvrTudjsjfK7rXRboley4YE");
    
    //Momo
    let controller : FlutterViewController = window?.rootViewController as! FlutterViewController
    let momoChannel = FlutterMethodChannel.init(name: "com.maximusnguyen.flutterWorkmanagement/momo",
                                                binaryMessenger: controller.binaryMessenger)
    
    momoChannel.setMethodCallHandler { (call, result) in
        switch call.method {
        case "payment":
            
            let data = call.arguments as! NSMutableDictionary;
            
            //print(call.method);
            
            let paymentinfo = NSMutableDictionary()
            paymentinfo["merchantcode"] = data["merchantcode"]
            paymentinfo["merchantname"] = data["merchantname"]
            paymentinfo["merchantnamelabel"] = data["merchantnamelabel"]
            paymentinfo["orderId"] = data["orderId"]
            paymentinfo["orderLabel"] = data["orderLabel"]
            paymentinfo["amount"] = data["amount"]
            paymentinfo["fee"] = data["fee"]
            paymentinfo["description"] = data["description"]
            paymentinfo["extra"] = data["extra"]
            paymentinfo["username"] = data["username"] //user id/user identify/user email
            paymentinfo["appScheme"] = "momobet620190424"   //partnerSchemeId provided by MoMo , get from business.momo.vn
            MoMoPayment.createPaymentInformation(info: paymentinfo);
            
            MoMoPayment.requestToken();
            
            NotificationCenter.default.removeObserver(self, name: NSNotification.Name(rawValue: "NoficationCenterTokenReceived"), object: nil)
            
            NotificationCenter.default.addObserver(forName: NSNotification.Name(rawValue: "NoficationCenterTokenReceived"), object: nil, queue: nil) { (Notification) in
                let response:NSMutableDictionary = Notification.object! as! NSMutableDictionary
                
                let _statusStr = "\(response["status"] as! String)"
                //let _messageStr = "\(response["message"] as! String)"
                
                result(_statusStr);
            }
            
            break
            
        default:
            result(FlutterMethodNotImplemented);
        }
        
       
    }
    
    //voipRegistration();
    
    //ATCallManager.shared.configurePushKit();
    
    GeneratedPluginRegistrant.register(with: self)
    
    return super.application(application, didFinishLaunchingWithOptions: launchOptions)
  }
    
    override func application(_ application: UIApplication, open url: URL, sourceApplication: String?, annotation: Any) -> Bool {
        MoMoPayment.handleOpenUrl(url: url, sourceApp: sourceApplication!)
        return true
    }

    override func application(_ app: UIApplication, open url: URL, options: [UIApplication.OpenURLOptionsKey : Any]) -> Bool {
        MoMoPayment.handleOpenUrl(url: url, sourceApp: "")
        return true
    }
    
    // Register for VoIP notifications
//    func voipRegistration() {
//        let mainQueue = DispatchQueue.main
//        // Create a push registry object
//        let voipRegistry: PKPushRegistry = PKPushRegistry(queue: mainQueue)
//        // Set the registry's delegate to self
//        voipRegistry.delegate = self
//        // Set the push type to VoIP
//        voipRegistry.desiredPushTypes = [.voIP]
//    }
}

//extension AppDelegate: PKPushRegistryDelegate {
//
//    func pushRegistry(_ registry: PKPushRegistry, didUpdate pushCredentials: PKPushCredentials, for type: PKPushType) {
//        //let token = pushCredentials.token.map { String(format: "%02.2hhx", $0) }.joined()
//        FlutterVoipPushNotificationPlugin.didUpdate(pushCredentials, forType: type.rawValue);
//    }
//
//
//    func pushRegistry(_ registry: PKPushRegistry, didReceiveIncomingPushWith payload: PKPushPayload, for type: PKPushType, completion: @escaping () -> Void) {
//
//
//        print("DATA:")
//        let k = payload.dictionaryPayload["aps"] as? Dictionary<String, Any>;
//        let m = k?["alert"] as? Dictionary<String, Any>;
//        //print(m?["title"]);
//
//        //ATCallManager.shared.incommingCall(from: m?["title"] as! String)
//
////        FlutterCallKitPlugin.reportNewIncomingCall(UUID().uuidString, handle: (m?["title"] as! String), handleType: "email", hasVideo: true, localizedCallerName: m?["title"] as? String, fromPushKit: false);
//
//
//
//
//
//        FlutterVoipPushNotificationPlugin.didReceiveIncomingPush(with: payload, forType: type.rawValue);
//    }
//
//
//
//}





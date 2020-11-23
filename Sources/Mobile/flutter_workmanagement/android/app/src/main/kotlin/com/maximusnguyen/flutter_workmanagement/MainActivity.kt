package com.maximusnguyen.flutter_workmanagement

import androidx.annotation.NonNull;
import io.flutter.embedding.android.FlutterActivity
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugins.GeneratedPluginRegistrant
import io.flutter.plugin.common.MethodChannel
import android.icu.lang.UCharacter.GraphemeClusterBreak.T
import android.os.Bundle
import android.os.PersistableBundle
import android.icu.lang.UCharacter.GraphemeClusterBreak.T
import android.R.attr.description
import android.content.Intent
import java.util.HashMap
import android.icu.lang.UCharacter.GraphemeClusterBreak.T
import vn.momo.momo_partner.AppMoMoLib;
import vn.momo.momo_partner.MoMoParameterNameMap;

class MainActivity: FlutterActivity() {
    private val CHANNEL = "com.maximusnguyen.flutterWorkmanagement/momo"
    private val result: MethodChannel.Result? = null
    private val REQUESTCODE = 120

    override fun configureFlutterEngine(@NonNull flutterEngine: FlutterEngine) {
        GeneratedPluginRegistrant.registerWith(flutterEngine);

        MethodChannel(flutterEngine.dartExecutor.binaryMessenger, CHANNEL)
                .setMethodCallHandler { call, result ->
                        if (call.method == "payment") {
                            AppMoMoLib.getInstance().setAction(AppMoMoLib.ACTION.PAYMENT);
                            AppMoMoLib.getInstance().setActionType(AppMoMoLib.ACTION_TYPE.GET_TOKEN);

                            var model = call.arguments as HashMap<String, Any?>;

                            val eventValue = HashMap<String, Any?>()
                            //client Required
                            eventValue["merchantname"] = model["merchantName"] //Tên đối tác. được đăng ký tại https://business.momo.vn. VD: Google, Apple, Tiki , CGV Cinemas
                            eventValue["merchantcode"] = model["merchantCode"] //Mã đối tác, được cung cấp bởi MoMo tại https://business.momo.vn
                            eventValue["amount"] = model["total_amount"] //Kiểu integer
                            eventValue["orderId"] = model["orderId123456789"] //uniqueue id cho Bill order, giá trị duy nhất cho mỗi đơn hàng
                            eventValue["orderLabel"] = model["orderLabel"] //gán nhãn

                            //client Optional - bill info
                            eventValue["merchantnamelabel"] = model["merchantnamelabel"]//gán nhãn
                            eventValue["fee"] = model["fee"] //Kiểu integer
                            eventValue["description"] = model["description"] //mô tả đơn hàng - short description

                            //client extra data
                            eventValue["partnerCode"] = "momobet620190424"

                            AppMoMoLib.getInstance().requestMoMoCallBack(this, eventValue);


                        }

                }
    }


}

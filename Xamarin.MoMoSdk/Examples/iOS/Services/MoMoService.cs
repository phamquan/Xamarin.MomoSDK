﻿using System;
using MoMoExample.iOS.Services;
using MoMoExample.Services;
using Xamarin.Forms;
using MoMoSdkBindings.iOS;
using Foundation;
using UIKit;
using MoMoExample.Services.MoMo;

[assembly: Dependency(typeof(MoMoService))]
namespace MoMoExample.iOS.Services
{
    public class MoMoService : IMoMoService
    {

        public NSMutableDictionary ToNSMutableDictionary(PaymentInfo paymentInfo)
        {

            var result = NSMutableDictionary
                .FromObjectsAndKeys(
                    objects: new object[] {
                    paymentInfo.Amount, paymentInfo.Fee, paymentInfo.Description, paymentInfo.Extra, paymentInfo.Language, paymentInfo.Username
                },
                    keys: new object[] {
                    "amount", "fee", "description", "extra", "language", "username"
                });

            return result;
        }

        public void Init(string bundleid, string merchantId, string merchantname, string merchantNameTitle, string billTitle)
        {
            //MoMoPayment.ShareInstant.InitializingAppBundleId(bundleid, merchantId, merchantname, merchantNameTitle, billTitle);
            MoMoPayment.ShareInstant.InitializingAppBundleId("com.abcFoody.LuckyLuck", "CGV01", "CGV", merchantNameTitle, billTitle);
            MoMoPayment.ShareInstant.RequestToken();

            NSNotificationCenter
                .DefaultCenter
                .AddObserver(new NSString("NoficationCenterTokenReceived"),(notification) =>
            {
                Process(notification);
            });

            NSNotificationCenter
                .DefaultCenter
                .AddObserver(new NSString("NoficationCenterTokenStartRequest"), NoficationCenterTokenStartRequest);
        }

        void NoficationCenterTokenStartRequest(NSNotification notification)
        {
            if (notification.Object != null && notification.Object.ToString() == "MoMoWebDialogs")
            {
                var dialog = new MoMoDialogs();
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(dialog, true, null);
            }
        }

        void Process(NSNotification notification)
        {
            System.Diagnostics.Debug.WriteLine(notification.Object);
        }

        public void PayExample()
        {
            var paymentInfo = ToNSMutableDictionary(PaymentInfo.ExamplePayment());
            MoMoPayment.ShareInstant.InitPaymentInformation(info: paymentInfo, bundleId: "com.momo.appv2.ios",
                                                            type_environment: MomoEnvirontment.Development);

            MoMoPayment.ShareInstant.RequestToken();
        }
    }
}

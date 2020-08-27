using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ButtonCircle.FormsPlugin.iOS;
using Foundation;
using StayAtHome.iOS.Services;
using StayAtHome.Messages;
using UIKit;
using Xamarin.Forms;

namespace StayAtHome.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init(); // To use maps
            ButtonCircleRenderer.Init(); // for circle buttons to work

            //for local sqlite database
            string dbName = "address_db.sqlite";
            string folderPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
            string fullPath = Path.Combine(folderPath, dbName);

            LoadApplication(new App(fullPath));

            WireUpLongRunningTask(); //backgrounding the start and stop journey

            return base.FinishedLaunching(app, options);
        }
        iOSLongRunningTaskExample longRunningTaskExample;
        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", async message => {
                longRunningTaskExample = new iOSLongRunningTaskExample();
                await longRunningTaskExample.Start();
            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message => {
                longRunningTaskExample.Stop();
            });
        }
    }
}

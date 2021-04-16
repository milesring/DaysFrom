using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaysFrom.Droid
{
    [Application]
    public class MainApplication : Shiny.ShinyAndroidApplication<DaysFrom.DaysFromStartup>
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
            
        }
    }
}
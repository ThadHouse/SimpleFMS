using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Autofac;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Base;
using SimpleFMS.Networking.Client;
using SimpleFMS.Networking.Client.NetworkClients;
using static SimpleFMS.Android.AutoFacContainer;

namespace SimpleFMS.Android
{
    [Activity(Label = "SimpleFMS.Android")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }
    }
}


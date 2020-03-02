﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.Settings;
using SmartBoxCity.Activity.Box;
using SmartBoxCity.Activity.Driver;

namespace SmartBoxCity.Activity
{
    [Activity(Label = "SmartBoxCity")]
    public class MainActivity2: AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_driver);
            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation_driver);

            navigation.NavigationItemSelected += (sender, e) =>
            {
                FragmentTransaction transaction2 = this.FragmentManager.BeginTransaction();
                switch (e.Item.ItemId)
                {

                    case Resource.Id.tasks:
                        TaskActivity content2 = new TaskActivity();
                        transaction2.Replace(Resource.Id.frameDriverlayout, content2).AddToBackStack(null).Commit();
                        break;
                    case Resource.Id.boxes:
                        BoxListActivity content = new BoxListActivity();
                        transaction2.Replace(Resource.Id.frameDriverlayout, content).AddToBackStack(null).Commit();
                        break;
                    case Resource.Id.c_status:
                      
                        break;
                    case Resource.Id.alarms:
                        
                        break;

                    case Resource.Id.exit_driver:
                        string dir_path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        File.Delete(dir_path + "user_data.txt");
                        CrossSettings.Current.AddOrUpdateValue("isAuth", "false");
                        CrossSettings.Current.AddOrUpdateValue("role","");

                        Intent content1 = new Intent(this, typeof(MainActivity));
                        StartActivity(content1);
                        break;
                }
            };

            
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_driver_layout);
           
        }

        public interface IBackButtonListener
        {
            void OnBackPressed();
        }
        public override void OnBackPressed()
        {
            // Ignoring stuff about DrawerLayout, etc for demo purposes.
            var currentFragment = SupportFragmentManager.FindFragmentById(Resource.Id.framelayout);
            var listener = currentFragment as IBackButtonListener;
            if (listener != null)
            {
                listener.OnBackPressed();
                return;
            }
            base.OnBackPressed();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        

       
    }
}
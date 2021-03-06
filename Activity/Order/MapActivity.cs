﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cheesebaron.SlidingUpPanel;
using Entity.Model;
using Entity.Model.OrderResponse;
using Entity.Repository;
using WebService.Client;

namespace SmartBoxCity.Activity.Order
{
    public class MapActivity: Fragment, IOnMapReadyCallback
    {
        private const string SavedStateActionBarHidden = "saved_state_action_bar_hidden";
        private Task<Driver.TaskStatus> result;
        private TextView txtFrom;
        private TextView txtTo;
        private TextView Weight;
        private TextView LenhWidHeig;

        private string fromString, toString, weightString, lenhwidheigString;

        private GoogleMap GMap;
        MapView mMapView = null;
        private string error_message = "";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                this.GMap = googleMap;

                LatLng location = new LatLng(StaticOrder.way_points[0].lat, StaticOrder.way_points[0].lng);
                PolylineOptions rectOptions = new PolylineOptions()
                {

                };
                rectOptions.Geodesic(true);
                rectOptions.InvokeWidth(1);
                rectOptions.InvokeColor(Color.Blue);

                for (int i = 0; i < StaticOrder.way_points.Count; i++)
                {
                    var latitude = StaticOrder.way_points[i].lat;
                    var longitude = StaticOrder.way_points[i].lng;

                    LatLng new_location = new LatLng(
                       latitude,
                        longitude);

                    rectOptions.Add(new_location);

                    if (i == 0)
                    {
                        MarkerOptions markerOpt1 = new MarkerOptions();
                        //location = new LatLng(latitude, longitude);

                        markerOpt1.SetPosition(new LatLng(latitude, longitude));
                        markerOpt1.SetTitle("Пункт отправления\n" + StaticOrder.Inception_address);
                        //markerOpt1.SetSnippet(StaticOrder.Inception_address);

                        var bmDescriptor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue);
                        markerOpt1.InvokeIcon(bmDescriptor);

                        googleMap.AddMarker(markerOpt1);

                        continue;
                    }
                    else if(i == 1)
                    {
                        MarkerOptions markerOpt1 = new MarkerOptions();
                        //location = new LatLng(latitude, longitude);

                        markerOpt1.SetPosition(new LatLng(latitude, longitude));
                        markerOpt1.SetTitle("Пункт назначения\n" + StaticOrder.Destination_address);
                        //markerOpt1.SetSnippet(StaticOrder.Destination_address);

                        var bmDescriptor = BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed);
                        markerOpt1.InvokeIcon(bmDescriptor);

                        googleMap.AddMarker(markerOpt1);

                        continue;
                    }
                    MarkerOptions markerOptions = new MarkerOptions();

                    markerOptions.SetPosition(new_location);
                    markerOptions.SetTitle(i.ToString());
                    googleMap.AddMarker(markerOptions);

                }

                googleMap.AddPolyline(rectOptions);

                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(location);
                builder.Zoom(10);
                builder.Bearing(0);
                builder.Tilt(65);

                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                //googleMap.UiSettings.ZoomControlsEnabled = true;
                //googleMap.UiSettings.CompassEnabled = true;
                googleMap.MoveCamera(cameraUpdate);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, ex.Message, ToastLength.Long).Show();
            }            
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if (mMapView != null)
                mMapView.OnSaveInstanceState(outState);   
        }

        public override void OnResume()
        {
            if (mMapView != null)
                mMapView.OnResume();

            base.OnResume();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (mMapView != null)
                mMapView.OnDestroy();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            if (mMapView != null)
                mMapView.OnLowMemory();
            
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                Task<SmartBoxCity.Activity.Driver.TaskStatus> result = GetParameters();

                if (result.Result == Driver.TaskStatus.OK)
                {
                    var view = inflater.Inflate(Resource.Layout.activity_map, container, false);

                    txtFrom = view.FindViewById<TextView>(Resource.Id.MapTextFrom);
                    txtTo = view.FindViewById<TextView>(Resource.Id.MapTextTo);
                    Weight = view.FindViewById<TextView>(Resource.Id.MapTextWeight);
                    LenhWidHeig = view.FindViewById<TextView>(Resource.Id.MapTextLenhWidHeig);

                    txtFrom.Text = fromString;
                    txtTo.Text = toString;
                    Weight.Text = weightString;
                    LenhWidHeig.Text = lenhwidheigString;

                    var layout = view.FindViewById<SlidingUpPanelLayout>(Resource.Id.sliding_client_layout);
                    view.FindViewById<TextView>(Resource.Id.txt_info_order_new).MovementMethod = new LinkMovementMethod();



                    if (result.Result == SmartBoxCity.Activity.Driver.TaskStatus.OK)
                    {
                        layout.AnchorPoint = 0.3f;
                        layout.PanelExpanded += (s, e) => Log.Info(Tag, "PanelExpanded");
                        layout.PanelCollapsed += (s, e) => Log.Info(Tag, "PanelCollapsed");
                        layout.PanelAnchored += (s, e) => Log.Info(Tag, "PanelAnchored");
                        layout.PanelSlide += (s, e) =>
                        {
                            if (e.SlideOffset < 0.2)
                            {
                                //if (SupportActionBar.IsShowing)
                                //    SupportActionBar.Hide();
                            }
                            else
                            {
                                //if (!SupportActionBar.IsShowing)
                                //    SupportActionBar.Show();
                            }
                        };

                        var actionBarHidden = savedInstanceState != null &&
                                              savedInstanceState.GetBoolean(SavedStateActionBarHidden, false);
                        //if (actionBarHidden)
                        //    SupportActionBar.Hide();

                        MapsInitializer.Initialize(Activity);
                        mMapView = view.FindViewById<MapView>(Resource.Id.FragmentMapUser);

                        switch (GooglePlayServicesUtil.IsGooglePlayServicesAvailable(Activity))
                        {
                            case ConnectionResult.Success:
                                Toast.MakeText(Activity, "SUCCESS", ToastLength.Long).Show();
                                mMapView.OnCreate(savedInstanceState);
                                mMapView.GetMapAsync(this);
                                break;
                            case ConnectionResult.ServiceMissing:
                                Toast.MakeText(Activity, "ServiceMissing", ToastLength.Long).Show();
                                break;
                            case ConnectionResult.ServiceVersionUpdateRequired:
                                Toast.MakeText(Activity, "Update", ToastLength.Long).Show();
                                break;
                            default:
                                Toast.MakeText(Activity, GooglePlayServicesUtil.IsGooglePlayServicesAvailable(Activity), ToastLength.Long).Show();
                                break;
                        }
                    }

                    return view;
                }
                else
                {
                    var view = inflater.Inflate(Resource.Layout.activity_errors_handling, container, false);
                    var txt_error = view.FindViewById<TextView>(Resource.Id.TextOfError);
                    txt_error.Text = "Что-то пошло не так. Перезапустите страницу или обратитесь в центр поддержки!\nПричина: " + error_message;
                    return view;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, ex.Message, ToastLength.Long);
                var view = inflater.Inflate(Resource.Layout.activity_errors_handling, container, false);
                var txt_error = view.FindViewById<TextView>(Resource.Id.TextOfError);
                txt_error.Text = "Что-то пошло не так. Перезапустите страницу или обратитесь в центр поддержки!\nПричина: " + ex.Message;
                return view;
            } 
        }

        private async Task<SmartBoxCity.Activity.Driver.TaskStatus> GetParameters()
        {
            try
            {
                var o_data = new ServiceResponseObject<GeoResponseData>();
                o_data = await OrderService.GeoOrder(StaticOrder.Order_id);

                if (o_data.Status == System.Net.HttpStatusCode.OK)
                {

                    fromString = o_data.ResponseData.ORDER.inception_address;
                    toString = o_data.ResponseData.ORDER.destination_address;
                    weightString = o_data.ResponseData.ORDER.weight;
                    if (o_data.ResponseData.ORDER.length == null || o_data.ResponseData.ORDER.width == null || o_data.ResponseData.ORDER.height == null)
                    {
                        lenhwidheigString = "неизвестно";
                    }
                    else
                    {
                        var length = double.Parse(o_data.ResponseData.ORDER.length, CultureInfo.InvariantCulture);
                        var width = double.Parse(o_data.ResponseData.ORDER.width, CultureInfo.InvariantCulture);
                        var height = double.Parse(o_data.ResponseData.ORDER.height, CultureInfo.InvariantCulture);
                        var sum = length.ToString() + "X" + width.ToString() + "X" + height.ToString();
                        lenhwidheigString = sum;
                    }
                    var way_points = o_data.ResponseData.MAP_WAYPOINTS;
                    StaticOrder.AddWayPoints(way_points);
                    return SmartBoxCity.Activity.Driver.TaskStatus.OK;
                }
                error_message = o_data.Message;
                return SmartBoxCity.Activity.Driver.TaskStatus.ServerError;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, ex.Message, ToastLength.Long);
                return SmartBoxCity.Activity.Driver.TaskStatus.ServerError;
            }
        }
    }
}
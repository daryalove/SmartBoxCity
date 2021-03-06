﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.Text.Method;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Entity.Model.AccountViewModel.AuthViewModel;
using Entity.Repository;
using Newtonsoft.Json;
using Plugin.Settings;
using SmartBoxCity.Activity.Home;
using SmartBoxCity.Activity.Registration;
using SmartBoxCity.Service;
using WebService;
using WebService.Account;

namespace SmartBoxCity.Activity.Auth
{
    public class AuthActivity: Android.App.Fragment
    {
        #region
        /// <summary>
        /// Почта клиента
        /// </summary>
        private EditText s_login;

        /// <summary>
        /// Пароль клиента.
        /// </summary>
        private EditText s_pass;

        /// <summary>
        /// Пароль клиента.
        /// </summary>
        private CheckBox is_remember;

        /// <summary>
        /// Конпка регистрации.
        /// </summary>
        private Button btn_register;

        /// <summary>
        /// Конпка авторизации.
        /// </summary>
        private Button btn_auth;

        /// <summary>
        /// Кнопка прокрутки.
        /// </summary>
        private ProgressBar preloader;
        #endregion

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);           
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.activity_auth, container, false);

            btn_auth = view.FindViewById<Button>(Resource.Id.btn_auth);           
            s_login = view.FindViewById<EditText>(Resource.Id.s_login);
            s_pass = view.FindViewById<EditText>(Resource.Id.s_pass);
            is_remember = view.FindViewById<CheckBox>(Resource.Id.is_remember);
            preloader = view.FindViewById<ProgressBar>(Resource.Id.loader);

            //s_pass.InputType = Android.Text.InputTypes.TextVariationPassword |
            //              Android.Text.InputTypes.ClassText;
            s_pass.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, Resource.Drawable.HidePass, 0);
            s_pass.SetOnTouchListener(new OnDrawableTouchListener());
            is_remember.Checked = true;

            string file_data_remember = "";
            // Проверяю запомнил ли пользователя.
            string check = CrossSettings.Current.GetValueOrDefault("check", "");

            if (check == "1")
            {
                s_login.Text = CrossSettings.Current.GetValueOrDefault("login", "");
                s_pass.Text = CrossSettings.Current.GetValueOrDefault("password", "");
            }

            //ShowPasswrod.Click += delegate
            //{
            //    if (s_pass.InputType != Android.Text.InputTypes.TextVariationVisiblePassword)
            //        s_pass.InputType = Android.Text.InputTypes.TextVariationVisiblePassword;
            //    else
            //        s_pass.InputType = Android.Text.InputTypes.TextVariationPassword |
            //              Android.Text.InputTypes.ClassText;
            //};
            btn_auth.Click += async delegate
            {
                try
                {
                    preloader.Visibility = Android.Views.ViewStates.Visible;
                    AuthModel auth = new AuthModel
                    {
                        Login = s_login.Text,
                        Password = s_pass.Text
                    };

                    using (var client = ClientHelper.GetClient(auth.Login, auth.Password))
                    {
                        AuthService.InitializeClient(client);
                        var o_data = await AuthService.Login(auth);
                        
                        if (o_data.Status == HttpStatusCode.OK)
                        {
                            Toast.MakeText(Activity, o_data.Message, ToastLength.Long).Show();
                            AuthResponse o_user_data = new AuthResponse();
                             o_user_data = o_data.ResponseData;

                             if (is_remember.Checked == true)
                             {
                                 CrossSettings.Current.AddOrUpdateValue("check", "1");
                                 CrossSettings.Current.AddOrUpdateValue("login", s_login.Text);
                                 StaticUser.PresenceOnPage = true;
                                 //CrossSettings.Current.GetValueOrDefault("PresenceOnPage", "true");
                                 CrossSettings.Current.AddOrUpdateValue("password", s_pass.Text);
                             }
                             else
                             {
                                 CrossSettings.Current.AddOrUpdateValue("check", "0");
                             }

                            preloader.Visibility = Android.Views.ViewStates.Invisible;

                            StaticUser.PresenceOnPage = true;
                            //CrossSettings.Current.GetValueOrDefault("PresenceOnPage", "true");

                            CrossSettings.Current.AddOrUpdateValue("token", o_user_data.Token);
                            CrossSettings.Current.AddOrUpdateValue("role", o_user_data.Role);

                            if (StaticUser.OrderInStageOfBid == true)
                            {
                                StaticUser.NeedToCreateOrder = true;
                            }

                            Intent main = new Intent(Activity, typeof(MainActivity));
                            StartActivity(main);
                        }
                        else
                        {
                            preloader.Visibility = Android.Views.ViewStates.Invisible;
                            Toast.MakeText(Activity, o_data.Message, ToastLength.Long).Show();
                        }
                    }

                    using (var client = ClientHelper.GetClient(CrossSettings.Current.GetValueOrDefault("token","")))
                    {
                        AuthService.InitializeClient(client);
                        await AuthService.Login(CrossSettings.Current.GetValueOrDefault("token", ""));
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Activity, "" + ex.Message, ToastLength.Long).Show();
                }
            };

            return view;
        }
    }
    public class OnDrawableTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
    {
        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            try
            {
                if (v is EditText && e.Action == MotionEventActions.Up)
                {
                    EditText editText = (EditText)v;
                    if (e.RawX >= (editText.Right - editText.GetCompoundDrawables()[2].Bounds.Width()))
                    {
                        if (editText.TransformationMethod == null)
                        {
                            editText.TransformationMethod = PasswordTransformationMethod.Instance;
                            editText.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, Resource.Drawable.ShowPass, 0);
                        }
                        else
                        {
                            editText.TransformationMethod = null;
                            editText.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, Resource.Drawable.HidePass, 0);
                        }

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }

}
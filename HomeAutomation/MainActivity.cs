using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Views;
using Android.Views.InputMethods;
using Android.Telephony;
using Android.Speech;
using Android.Content;

namespace HomeAutomation
{
    [Activity(Label = "GSM Based Home Automation", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        const string on = " off";
        const string off = " on";
        LinearLayout Main, About, Details, Last;
        AlertDialog.Builder Exit;
        private int VOICE = 0;
        //string voice_input;
        public override void OnBackPressed()
        {
            if (About.Visibility == ViewStates.Visible)
            {
                Last.Visibility = ViewStates.Visible;
                About.Visibility = ViewStates.Gone;
            }
            else if (Details.Visibility == ViewStates.Visible)
            {
                Last.Visibility = ViewStates.Visible;
                Details.Visibility = ViewStates.Gone;
            }
            else
                Exit.Show();
        }

        //private string NumberToSub(int value)
        //{
        //    string s = "";
        //    do
        //        s = (char)(value % 10 + '\u2080') + s;
        //    while ((value = value / 10) != 0);
        //    return s;
        //}
        //private string D(double value)
        //{
        //    bool negtive = value < 0;
        //    if (negtive)
        //        value = -value;

        //    string stringNumber = value.ToString();//.Replace("-", "\u208A").Replace("+", ""); 
        //    int E = stringNumber.IndexOf("E");

        //    if (E != -1)
        //    {
        //        string Exponent = stringNumber.Substring(E + 2, stringNumber.Length - E - 2);
        //        if (stringNumber[E + 1] == '-')
        //            stringNumber = Double.Parse(stringNumber.Substring(0, E - 1)).ToString("0.####") + " \u00D7 10\u207B";
        //        else
        //            stringNumber = Double.Parse(stringNumber.Substring(0, E - 1)).ToString("0.####") + " \u00D7 10";
        //        for (int i = 0; i < Exponent.Length; i++)
        //            if (Exponent[i] == '1')
        //                stringNumber += '\u00B9';
        //            else if (Exponent[i] == '2')
        //                stringNumber += '\u00B2';
        //            else if (Exponent[i] == '3')
        //                stringNumber += '\u00B3';
        //            else
        //                stringNumber += (char)(Exponent[i] + '\u2070' - '0');
        //    }
        //    else
        //        stringNumber = Double.Parse(stringNumber).ToString("0.####");
        //    if (negtive)
        //        return '-' + stringNumber;
        //    return stringNumber;
        //}
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Layout.menu1, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //case Resource.Id.Formula:
                //    if (Main.Visibility == ViewStates.Visible)
                //        Last = Main;
                //    Last.Visibility = About.Visibility = ViewStates.Gone;
                //    Formula.Visibility = ViewStates.Visible;
                //    return true;
                case Resource.Id.About:
                    if (Main.Visibility == ViewStates.Visible)
                        Last = Main;
                    Last.Visibility = Details.Visibility = ViewStates.Gone;
                    About.Visibility = ViewStates.Visible;
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Main = (LinearLayout)FindViewById(Resource.Id.Main);
            About = (LinearLayout)FindViewById(Resource.Id.About);
            Details = (LinearLayout)FindViewById(Resource.Id.Formula);
            Details.Visibility = About.Visibility = ViewStates.Gone;
            Main.Visibility = ViewStates.Visible;
            Exit = new AlertDialog.Builder(this).SetMessage("Do you want to exit?").SetPositiveButton("Yes", (s, e) => Finish()).SetNegativeButton("Cancel", (s, e) => { });
            FindViewById<ToggleButton>(Resource.Id.Fan).Click += (s, e) => SendMessage((s as ToggleButton).Checked ? on : off, "Fan");
            FindViewById<ToggleButton>(Resource.Id.TV).Click += (s, e) => SendMessage((s as ToggleButton).Checked ? on : off, "TV");
            FindViewById<ToggleButton>(Resource.Id.Light).Click += (s, e) => SendMessage((s as ToggleButton).Checked ? on : off, "Light");
            FindViewById<ToggleButton>(Resource.Id.AC).Click += (s, e) => SendMessage((s as ToggleButton).Checked ? on : off, "AC");

            //string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            //if (rec != "android.hardware.microphone")
            //{
            //    var alert = new AlertDialog.Builder(recButton.Context);
            //    alert.SetTitle("You don't seem to have a microphone.");
            //    alert.SetPositiveButton("OK", (sender, e) =>
            //    {
            //        return;
            //    });
            //    alert.Show();
            //}
            FindViewById<ImageButton>(Resource.Id.VoiceButton).Click += (s, e) =>
            {
                var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, Application.Context.GetString(Resource.String.VoiceString));
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                StartActivityForResult(voiceIntent, VOICE);
            };

        }
        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            bool res = false;
            if (requestCode == VOICE && resultVal == Result.Ok)
            {
                var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                if (matches.Count != 0)
                {
                    //voice_input = matches[0];

                    if (matches[0].Contains("light"))
                    {
                        if (matches[0].Contains("on"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.Light).Checked = true;
                            SendMessage(on, "Light");
                            res = true;
                        }
                        else if (matches[0].Contains("off"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.Light).Checked = false;
                            SendMessage(off, "Light");
                            res = true;
                        }
                    }
                    if (matches[0].Contains("AC"))
                    {
                        if (matches[0].Contains("on"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.AC).Checked = true;
                            SendMessage(on, "AC");
                            res = true;
                        }
                        else if (matches[0].Contains("off"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.AC).Checked = false;
                            SendMessage(off, "AC");
                            res = true;
                        }
                    }
                    if (matches[0].Contains("fan"))
                    {
                        if (matches[0].Contains("on"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.Fan).Checked = true;
                            SendMessage(on, "Fan");
                            res = true;
                        }
                        else if (matches[0].Contains("off"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.Fan).Checked = false;
                            SendMessage(off, "Fan");
                            res = true;
                        }
                    }
                    if (matches[0].Contains("TV"))
                    {
                        if (matches[0].Contains("on"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.TV).Checked = true;
                            SendMessage(on, "TV");
                            res = true;
                        }
                        else if (matches[0].Contains("off"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.TV).Checked = false;
                            SendMessage(off, "TV");
                            res = true;
                        }
                    }
                    if (matches[0].Contains("everything"))
                    {
                        if (matches[0].Contains("on"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.Fan).Checked =
                               FindViewById<ToggleButton>(Resource.Id.TV).Checked =
                               FindViewById<ToggleButton>(Resource.Id.AC).Checked =
                               FindViewById<ToggleButton>(Resource.Id.Light).Checked = true;
                            SendMessage(on, "TV");
                            SendMessage(on, "AC");
                            SendMessage(on, "Fan");
                            SendMessage(on, "Light");
                            res = true;
                        }
                        else if (matches[0].Contains("off"))
                        {
                            FindViewById<ToggleButton>(Resource.Id.Fan).Checked =
                               FindViewById<ToggleButton>(Resource.Id.TV).Checked =
                               FindViewById<ToggleButton>(Resource.Id.AC).Checked =
                               FindViewById<ToggleButton>(Resource.Id.Light).Checked = false;
                            SendMessage(off, "TV");
                            SendMessage(off, "AC");
                            SendMessage(off, "Fan");
                            SendMessage(off, "Light");
                            res = true;
                        }
                    }
                }
                if (!res)
                    Toast.MakeText(ApplicationContext, "Unable to process the command.", ToastLength.Short).Show();
            }
            base.OnActivityResult(requestCode, resultVal, data);
        }

        private void SendMessage(string status, string control)
        {
            SmsManager.Default.SendTextMessage("01234567890", null, "#A." + control.ToLower() + status + '*', null, null);
            Toast.MakeText(ApplicationContext, "Signal to turn " + control + status + " sent", ToastLength.Short).Show();
        }
    }
}
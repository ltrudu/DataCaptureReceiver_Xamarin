using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Java.Lang;
using Java.Util;
using System.IO;
using Android.Text.Method;

namespace DataCaptureReceiver
{
    [Activity(Label = "DataCaptureReceiver", MainLauncher = true)]
    public class MainActivity : Activity
    {
        /**
         *  *************************************************************
         *  How to configure DataWedge to send intent to this application
         *  *************************************************************
         *
         * More info on DataWedge can be found here:
         *      http://techdocs.zebra.com/datawedge/5-0/guide/about/
         * More info on DataCapture Intents can be found here:
         *      http://techdocs.zebra.com/emdk-for-android/6-3/tutorial/tutdatacaptureintent/
         *
         * Setup1 (Automatic):
         * 0- Install the app on the device
         * 1- Copy the dwprofile_datacapture.db file that is provided in the asset folder in
         * device sdcard
         * 2- Open DataWedge
         * 3- Select Settings in the Menu
         * 4- Select Import Profile
         * 5- Select the previously imported file
         * 6- Quit DataWedge
         * 7- Run the application
         *
         * Setup2 (Manual):
         * 0- Install the app on the device
         * 1- Open DataWedge
         * 2- Menu -> New Profile
         * 3- Enter a name for the profile
         * 4- Select the newly created profile
         * 5- Select Applications -> Associated apps
         * 6- Menu -> New app/activity
         * 7- Select com.symbol.datacapturereceiver
         * 8- Select com.symbol.datacapturereceiver.MainActivity
         * 9- Go back
         * 10- Disable Keystroke output
         * 11- Enable Intent Output
         * 12- Select Intent Output -> Intent Action
         * 13- Enter (case sensitive): com.symbol.datacapturereceiver.RECVR
         * 14- Select Intent Output -> Intent Category
         * 15- Enter (case sensitive): android.intent.category.DEFAULT
         * 16- Select Intent Output -> Intent Delivery
         * 17- Select via StartActivity to handle the date inside the OnCreate Method and in onNewIntent
         *     If you select this option, don't forget to add com.android.launcher in the Associated apps if
         *     you want your app to be started from the launcher when a barcode is scanned, otherwise the scanner
         *     will only work when the datacapturereceiver app is running
         *     Configure android:launchMode="" in your AndroidManifest.xml application tag if you want
         *     specific behaviors.
         *     https://developer.android.com/guide/topics/manifest/activity-element.html
         *     http://androidsrc.net/android-activity-launch-mode-example/
         * 18- Select Broadcast Intent to handle the data inside a Broadcast Receiver
         * 19- Configure the Symbology : go to Barcode input -> Decoders
         * 20- Configure Aim (only the barcode in the center of the scanner target is read)
         *      Go to Barcode input -> Reader params -> Picklist -> Enabled
         */

        private static string mIntentAction = "com.symbol.datacapturereceiver.RECVR";
        private static string mIntentCategory = "android.intent.category.DEFAULT";
        private static string mProfileName = "DataCaptureReceiverXAM";
        private TextView tv_results;
        private string mResults = "";
        private ScrollView sv_results;

        private bool mContinuous = false;
        private Date mProfileProcessingStartDate = null;

        private string mSeparator = "-----------------------------------";

        /*
        Handler and runnable to scroll down textview
        */
        private Handler mScrollDownHandler = null;
        private Runnable mScrollDownRunnable = null;

        /**
        * Local Broadcast receiver
        */
        [BroadcastReceiver(Enabled = true, Exported = false)]
        private class MyBroadcastReceiver : BroadcastReceiver
        {
            public delegate void ReceiveDataDelegate(Intent intent);

            public event ReceiveDataDelegate Receive;

            public MyBroadcastReceiver()
            {
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (Receive != null)
                    Receive(intent);
            }
        }

        private MyBroadcastReceiver mMessageReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Create broadcast receiver instance
            mMessageReceiver = new MyBroadcastReceiver();
            mMessageReceiver.Receive += delegate (Intent intent) { HandleDecodeData(intent); };

            tv_results = FindViewById<TextView>(Resource.Id.tv_results);
            sv_results = FindViewById<ScrollView>(Resource.Id.sv_results);


            // Setup buttons behaviors
            Button btStart = FindViewById<Button>(Resource.Id.button_start);
            btStart.Click += delegate
            {
                SendDataWedgeIntentWithExtra(DataWedgeConstants.DWAPI_ACTION_SOFTSCANTRIGGER, DataWedgeConstants.EXTRA_PARAMETER, DataWedgeConstants.DWAPI_START_SCANNING);
            };

            Button btStop = FindViewById<Button>(Resource.Id.button_stop);
            btStop.Click += delegate
            {
                SendDataWedgeIntentWithExtra(DataWedgeConstants.DWAPI_ACTION_SOFTSCANTRIGGER, DataWedgeConstants.EXTRA_PARAMETER, DataWedgeConstants.DWAPI_STOP_SCANNING);
            };

            Button btToggle = FindViewById<Button>(Resource.Id.button_toggle);
            btToggle.Click += delegate
            {
                SendDataWedgeIntentWithExtra(DataWedgeConstants.DWAPI_ACTION_SOFTSCANTRIGGER, DataWedgeConstants.EXTRA_PARAMETER, DataWedgeConstants.DWAPI_TOGGLE_SCANNING);
            };

            Button btEnable = FindViewById<Button>(Resource.Id.button_enable);
            btEnable.Click += delegate
            {
                SendDataWedgeIntentWithExtra(DataWedgeConstants.DWAPI_ACTION_SCANNERINPUTPLUGIN, DataWedgeConstants.EXTRA_PARAMETER, DataWedgeConstants.DWAPI_PARAMETER_SCANNERINPUTPLUGIN_ENABLE);
            };

            Button btDisable = FindViewById<Button>(Resource.Id.button_disable);
            btDisable.Click += delegate
            {
                SendDataWedgeIntentWithExtra(DataWedgeConstants.DWAPI_ACTION_SCANNERINPUTPLUGIN, DataWedgeConstants.EXTRA_PARAMETER, DataWedgeConstants.DWAPI_PARAMETER_SCANNERINPUTPLUGIN_DISABLE);
            };

            Button btCreate = FindViewById<Button>(Resource.Id.button_create);
            btCreate.Click += delegate
            {
                CreateProfileAsync();
            };

            Button btImport = FindViewById<Button>(Resource.Id.button_import);
            btImport.Click += delegate
            {
                ImportProfile("dwprofile_datacapture");
            };

            Button btDelete = FindViewById<Button>(Resource.Id.button_delete);
            btDelete.Click += delegate
            {
                DeleteProfile();
            };

            Button btSwitch = FindViewById<Button>(Resource.Id.button_switch);
            btSwitch.Click += delegate
            {
                SwitchScannerParamsAsync();
            };

            Button btClear = FindViewById<Button>(Resource.Id.button_clear);
            btClear.Click += delegate
            {
                mResults = "";
                tv_results.Text = mResults;
            };


            if (this.Intent != null)
            {
                HandleDecodeData(this.Intent);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            IntentFilter myFilter = new IntentFilter();
            myFilter.AddAction(mIntentAction);
            myFilter.AddCategory(mIntentCategory);
            this.ApplicationContext.RegisterReceiver(mMessageReceiver, myFilter);
            mScrollDownHandler = new Handler(Looper.MainLooper);
        }

        protected override void OnPause()
        {
            // Unregister internal broadcast receiver when we are going in background
            this.ApplicationContext.UnregisterReceiver(mMessageReceiver);
            base.OnPause();
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (HandleDecodeData(intent))
                return;
            base.OnNewIntent(intent);
        }

        public bool HandleDecodeData(Intent i)
        {
            // check the intent action is for us
            if (i.Action.Equals(mIntentAction, System.StringComparison.InvariantCultureIgnoreCase))
            {
                // define a string that will hold our output
                string output = "";
                // get the source of the data
                string source = i.Extras.GetString(DataWedgeConstants.SOURCE_TAG);
                // save it to use later
                if (source == null) source = "scanner";
                // get the data from the intent
                string data = i.Extras.GetString(DataWedgeConstants.DATA_STRING_TAG);

                // let's define a variable for the data length
                int data_len = 0;
                // and set it to the length of the data
                if (data != null)
                    data_len = data.Length;

                string sLabelType = null;

                // check if the data has come from the barcode scanner
                if (source.Equals("scanner", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    // check if there is anything in the data
                    if (data != null && data.Length > 0)
                    {
                        // we have some data, so let's get it's symbology
                        sLabelType = i.Extras.GetString(DataWedgeConstants.LABEL_TYPE_TAG);
                        // check if the string is empty
                        if (sLabelType != null && sLabelType.Length > 0)
                        {
                            // format of the label type string is LABEL-TYPE-SYMBOLOGY
                            // so let's skip the LABEL-TYPE- portion to get just the symbology
                            sLabelType = sLabelType.Substring(11);
                        }
                        else
                        {
                            // the string was empty so let's set it to "Unknown"
                            sLabelType = "Unknown";
                        }
                        // let's construct the beginning of our output string
                        output = "Source: Scanner\n " + "Symbology: " + sLabelType + "\nLength: " + data_len.ToString();
                    }
                }

                // check if the data has come from the MSR
                if (source.Equals("msr", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    // construct the beginning of our output string
                    output = "Source: MSR, Length: " + data_len.ToString();
                }

                if (data != null)
                {

                    output = output + "\n" + ShowSpecialChars(data);
                }

                output += "\n" + mSeparator;

                AddLineToResults(output);

                return true;
            }
            return false;
        }

        private string ShowSpecialChars(string data)
        {
            string returnstring = "";
            char[] dataChar = data.ToCharArray();
            foreach (char acar in dataChar)
            {
                if (Character.IsLetterOrDigit(acar))
                {
                    returnstring += acar;
                }
                else
                {
                    returnstring += "[" + (int)acar + "]";
                }
            }

            return returnstring;
        }


        private void AddLineToResults(string lineToAdd)
        {
            mResults += lineToAdd + "\n";
            UpdateAndScrollDownTextView();
        }

        private void UpdateAndScrollDownTextView()
        {
            if (mScrollDownRunnable == null)
            {
                mScrollDownRunnable = new Runnable(() =>
                {            
                    RunOnUiThread(() =>
                    {

                        tv_results.Text = mResults;
                        sv_results.FullScroll(Android.Views.FocusSearchDirection.Down);
                    });
                });           
            }
            else
            {
                // A new line has been added while we were waiting to scroll down
                // reset handler to repost it....
                mScrollDownHandler.RemoveCallbacks(mScrollDownRunnable);
            }
            mScrollDownHandler.PostDelayed(mScrollDownRunnable, 200);
        }

        private void SendDataWedgeIntentWithExtra(string action, string extraKey, string extraValue)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extraValue);
            this.SendBroadcast(dwIntent);
        }

        private void SendDataWedgeIntentWithExtra(string action, string extraKey, Bundle extras, bool requestSendResult)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extras);
            if (requestSendResult)
                dwIntent.PutExtra(DataWedgeConstants.EXTRA_SEND_RESULT, "true");
            this.SendBroadcast(dwIntent);
        }

        private void ImportProfile(string progileFilenameWithoutDbExtension)
        {
            // Source : http://techdocs.zebra.com/datawedge/6-7/guide/settings/
            //Export your profile using
            //1. Open DataWedge
            //2. Open Hamburger Menu -> Settings (Paramètres)
            //3. Open "Export" list entry
            //4. Select profile to export
            //5. Retrieve exportes file in folder "\sdcard\Android\data\com.symbol.datawedge\files"

            string autoImportDir = "/enterprise/device/settings/datawedge/autoimport/";
            string temporaryFileName = progileFilenameWithoutDbExtension + ".tmp";
            string finalFileName = progileFilenameWithoutDbExtension + ".db";

            string tempFilePathToWrite = Path.Combine(autoImportDir, temporaryFileName);
            string finalFilePathToWrite = Path.Combine(autoImportDir, finalFileName);
            try
            {
                if (File.Exists(finalFilePathToWrite))
                {
                    AddLineToResults("Deleting old db file.");
                    File.Delete(finalFilePathToWrite);
                }

                int totalBytesWritten = 0;

                // Copy file from Assets with a temporary name
                using (var binreader = new BinaryReader(Application.Context.Assets.Open(finalFileName)))
                {
                    using (var binwriter = new BinaryWriter(new FileStream(tempFilePathToWrite, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int length = 0;
                        while ((length = binreader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            binwriter.Write(buffer, 0, length);
                            totalBytesWritten += length;
                        }

                        binwriter.Flush();
                        binwriter.Close();
                        binreader.Close();
                    }
                }

                AddLineToResults("Total byte copied: " + totalBytesWritten);

            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                //set permission to the file to read, write and exec.
                if (File.Exists(tempFilePathToWrite))
                {
                    File.SetAttributes(tempFilePathToWrite, FileAttributes.Normal);
                    // rename the file
                    File.Move(tempFilePathToWrite, finalFilePathToWrite);
                }

            }
        }

        private void DeleteProfile()
        {
            SendDataWedgeIntentWithExtra(DataWedgeConstants.ACTION_DATAWEDGE_FROM_6_2, DataWedgeConstants.EXTRA_DELETE_PROFILE, mProfileName);
        }

        private void SwitchScannerParamsAsync()
        {
            mContinuous = !mContinuous;
            // Store the current Date (to calculate elapsed time)
            mProfileProcessingStartDate = new Date();
            DWSwitchContinuousMode switchContinuous = new DWSwitchContinuousMode(this);
            DWSwitchContinuousMode.DWSwitchContinuousModeSettings settings = new DWSwitchContinuousMode.DWSwitchContinuousModeSettings
            {
                ProfileName = mProfileName,
                TimeOut = 30000,
                ContinuousMode = mContinuous
                
            };
            switchContinuous.Execute(settings, (switchContinuousResult) =>
            {
                if (string.IsNullOrEmpty(switchContinuousResult.Error))
                {
                    // Force "Not Continuous" mode succeeded.
                    AddLineToResults(mContinuous ? "Profile: " + switchContinuousResult.ProfileName + " switched to Continuous mode" : " switched to normal mode");
                    // Let's calculate the elapsed time since the begining of the process
                    Date current = new Date();
                    long timeDiff = current.Time - mProfileProcessingStartDate.Time;
                    AddLineToResults("Switch params took: " + timeDiff + " ms to process.");
                    AddLineToResults(mSeparator);
                }
                else
                {
                    AddLineToResults("Error switching params to " + (mContinuous ? "not " : "") + "continuous mode on profile: " + switchContinuousResult.ProfileName + "\n" + switchContinuousResult.Error);
                    AddLineToResults(mSeparator);
                }
            });
        }

        private void CreateProfileAsync()
        {
            // Store the current Date (to calculate elapsed time)
            mProfileProcessingStartDate = new Date();
            DWProfileChecker checker = new DWProfileChecker(this);
            DWProfileChecker.DWSettings checkerSettings = new DWProfileBase.DWSettings { ProfileName = mProfileName, TimeOut = 30000 };
            checker.Execute(checkerSettings, (profileCheckerResults) =>
            {
                if (string.IsNullOrEmpty(profileCheckerResults.Error))
                {
                    if (profileCheckerResults.Exists)
                    {
                        // Profile has been found, setting to not continuous mode
                        AddLineToResults("Profile " + profileCheckerResults.ProfileName + " found in DW profiles list.\n Forcing profile to not continuous mode.");
                        DWSwitchContinuousMode switchContinuous = new DWSwitchContinuousMode(this);
                        DWSwitchContinuousMode.DWSwitchContinuousModeSettings switchSettings = new DWSwitchContinuousMode.DWSwitchContinuousModeSettings
                        {
                            ProfileName = mProfileName,
                            TimeOut = 30000,
                            ContinuousMode = false
                        };
                        switchContinuous.Execute(switchSettings, (switchContinuousResult) =>
                        {
                            if (string.IsNullOrEmpty(switchContinuousResult.Error))
                            {
                                // Force "Not Continuous" mode succeeded.
                                AddLineToResults("Params switched to not continuous on profile: " + switchContinuousResult.ProfileName + " succeeded");
                                // Let's calculate the elapsed time since the begining of the process
                                Date current = new Date();
                                long timeDiff = current.Time - mProfileProcessingStartDate.Time;
                                AddLineToResults("Check+Switch took: " + timeDiff + " ms to process.");
                                AddLineToResults(mSeparator);
                            }
                            else
                            {
                                AddLineToResults("Error switching params to not continuous on profile: " + switchContinuousResult.ProfileName + "\n" + switchContinuousResult.Error);
                                AddLineToResults(mSeparator);
                            }
                        });
                    }
                    else
                    {
                        // Profile not found, let's create a new one
                        AddLineToResults("Profile " + profileCheckerResults.ProfileName + " not found in DW profiles list. Creating profile.");
                        DWProfileCreate profileCreator = new DWProfileCreate(this);
                        DWProfileCreate.DWSettings createSettings = new DWProfileCreate.DWSettings
                        {
                            ProfileName = mProfileName,
                            TimeOut = 30000
                        };
                        profileCreator.Execute(createSettings, (creationResult) =>
                        {
                            if (string.IsNullOrEmpty(creationResult.Error))
                            {
                                // Profile creation succeeded, let's set this profile initial parameters
                                AddLineToResults("Profile: " + creationResult.ProfileName + " created with success.\nSetting config now.");
                                DWProfileSetConfig profileSetConfig = new DWProfileSetConfig(this);
                                DWProfileSetConfig.DWProfileSetConfigSettings setConfigSettings = new DWProfileSetConfig.DWProfileSetConfigSettings
                                {
                                    ProfileName = mProfileName,
                                    TimeOut = 30000,
                                    IntentAction = mIntentAction,
                                    IntentCategory = mIntentCategory,
                                    AggressiveMode = false
                                };
                                profileSetConfig.Execute(setConfigSettings, (setConfigResult) =>
                                {
                                    if (string.IsNullOrEmpty(setConfigResult.Error))
                                    {
                                        // Initial parameters set successfully, let's force this profile to not continuous mode
                                        // It is not necessary since the default mode is "not continuous", but we like to ensure that
                                        // the config is exactly what we want...
                                        AddLineToResults("Set config on profile: " + setConfigResult.ProfileName + " succeeded.");
                                        Date current = new Date();
                                        long timeDiff = current.Time - mProfileProcessingStartDate.Time;
                                        AddLineToResults("Check+Create+Set took: " + timeDiff + " ms to process.");
                                        AddLineToResults(mSeparator);
                                    }
                                    else
                                    {
                                        AddLineToResults("Error setting params on profile: " + setConfigResult.ProfileName + "\n" + setConfigResult.Error);
                                        AddLineToResults(mSeparator);
                                    }
                                });
                            }
                            else
                            {
                                AddLineToResults("Error creating profile: " + creationResult.ProfileName + "\n" + creationResult.Error);
                                AddLineToResults(mSeparator);
                            }

                        });
                    }
                }
                else
                {
                    AddLineToResults("Error checking if profile " + profileCheckerResults.ProfileName + " exists: \n" + profileCheckerResults.Error);
                    AddLineToResults(mSeparator);
                }

            });
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace DataCaptureReceiver
{
    public class DWProfileChecker : DWProfileBase
    {
        public DWProfileChecker(Context aContext) : base(aContext)
        {
            mBroadcastReceiver = new CheckProfileReceiver(this);
        }

        /// <summary>
        /// Holder for the results of the action
        ///     string: ProfileName -> the processed profile name
        ///     bool: Exists -> true if the profile exists
        ///     string: Error -> not null and not empty if an error occurred
        /// </summary>
        public class ActionResult
        {
            public string ProfileName { get; set; }
            public bool Exists { get; set; }
            public string Error { get; set; }
        }

        /*
        A store to keep the callback to be fired when we will get the
        result of the intent
        <string profileName, bool exists, string error>
         */
        protected Action<ActionResult> mProfileExistsCallback = null;

        /*
        The receiver that we will register to retrieve DataWedge answer
         */
        private CheckProfileReceiver mBroadcastReceiver = null;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">
        ///     string: profileName -> the processed profile name
        ///     bool: exists -> true if the profile exists
        ///     string: error -> not null and not empty if an error occurred
        /// </param>
        public void Execute(DWSettings settings, Action<ActionResult> callback)
        {
            mProfileExistsCallback = callback;

            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction(DataWedgeConstants.ACTION_RESULT_DATAWEDGE_FROM_6_2);
            intentFilter.AddCategory(Intent.CategoryDefault);

            /*
            Register receiver for resutls
             */
            mContext.ApplicationContext.RegisterReceiver(mBroadcastReceiver, intentFilter);

            /*
            Ask for DataWedge profile list
             */
            SendDataWedgeIntentWithExtra(DataWedgeConstants.ACTION_DATAWEDGE_FROM_6_2, DataWedgeConstants.EXTRA_GET_PROFILES_LIST, DataWedgeConstants.EXTRA_EMPTY);

            /*
            Launch timeout mechanism
             */
            base.Execute(settings);
        }

        protected class CheckProfileReceiver : BroadcastReceiver
        {
            private DWProfileChecker mProfileChecker = null;

            public CheckProfileReceiver(DWProfileChecker dWProfileChecker)
            {
                mProfileChecker = dWProfileChecker;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.HasExtra(DataWedgeConstants.EXTRA_RESULT_GET_PROFILE_LIST))
                {
                    //  6.2 API to GetProfileList
                    string[] profilesList = intent.GetStringArrayExtra(DataWedgeConstants.EXTRA_RESULT_GET_PROFILE_LIST);
                    //  Profile list for 6.2 APIs
                    bool exists = profilesList.Contains(mProfileChecker.Settings.ProfileName);
                    if (mProfileChecker.mProfileExistsCallback != null)
                    {
                        mProfileChecker.mProfileExistsCallback(new ActionResult { ProfileName = mProfileChecker.Settings.ProfileName, Exists = exists, Error = null });
                        mProfileChecker.CleanAll();
                    }
                }
            }
        }

        /*
        This is what will happen if Datawedge does not answer before the timeout
         */
        protected override void OnError(string error)
        {
            if (mProfileExistsCallback != null)
            {
                mProfileExistsCallback(new ActionResult { ProfileName = Settings.ProfileName, Exists = false, Error = error });
                CleanAll();
            }
        }

        protected override void CleanAll()
        {
            Settings.ProfileName = "";
            mProfileExistsCallback = null;
            mContext.ApplicationContext.UnregisterReceiver(mBroadcastReceiver);
            base.CleanAll();
        }
    }
}
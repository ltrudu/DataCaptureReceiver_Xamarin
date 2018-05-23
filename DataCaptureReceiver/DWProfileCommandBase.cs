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
    public class DWProfileCommandBase : DWProfileBase
    {
        /*
        A command identifier used if we request a result from DataWedge
        */
        protected String mCommandIdentifier = "";

        public DWProfileCommandBase(Context aContext) : base(aContext)
        {
            mBroadcastReceiver = new DataWedgeActionResultReceiver(this);
        }

        public class CommandBaseResults
        {
            public String ProfileName { get; set; }
            public String Action { get; set; }
            public String Command { get; set; }
            public String Result { get; set; }
            public String ResultInfo { get; set; }
            public String CommandIdentifier { get; set; }
            public String Error { get; set; }
        }

        /*
        A store to keep the callback to be fired when we will get the
        result of the intent
        <String profileName, String action, String command, String result, String resultInfo, String commandidentifier, String error>
         */
        protected Action<CommandBaseResults> mCommandBaseCallback = null;

        /*
        The receiver that we will register to retrieve DataWedge answer
         */
        protected DataWedgeActionResultReceiver mBroadcastReceiver = null;

        protected virtual void Execute(DWSettings settings, Action<CommandBaseResults> callback)
        {
            mCommandBaseCallback = callback;

            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction(DataWedgeConstants.ACTION_RESULT_DATAWEDGE_FROM_6_2);
            intentFilter.AddCategory(Intent.CategoryDefault);

            /*
            Register receiver for resutls
             */
            mContext.ApplicationContext.RegisterReceiver(mBroadcastReceiver, intentFilter);

            /*
            Launch timeout mechanism
             */
            base.Execute(settings);
        }

        protected void SendDataWedgeIntentWithExtraRequestResult(String action, String extraKey, String extraValue)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extraValue);
            dwIntent.PutExtra("SEND_RESULT", "true");
            mCommandIdentifier = Settings.ProfileName + new Date().Time;
            dwIntent.PutExtra("COMMAND_IDENTIFIER", mCommandIdentifier);
            mContext.SendBroadcast(dwIntent);
        }

        protected void SendDataWedgeIntentWithExtraRequestResult(String action, String extraKey, Bundle extras)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extras);
            dwIntent.PutExtra("SEND_RESULT", "true");
            mCommandIdentifier = Settings.ProfileName + new Date().Time;
            dwIntent.PutExtra("COMMAND_IDENTIFIER", mCommandIdentifier);
            mContext.SendBroadcast(dwIntent);
        }

        protected class DataWedgeActionResultReceiver : BroadcastReceiver
        {
            private DWProfileCommandBase mCommandBase = null;

            public DataWedgeActionResultReceiver(DWProfileCommandBase dWProfileCommandBase)
            {
                mCommandBase = dWProfileCommandBase;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (String.Equals(intent.Action,DataWedgeConstants.ACTION_RESULT_DATAWEDGE_FROM_6_2,StringComparison.OrdinalIgnoreCase))
                {
                    // L'intent reçu est bien l'intent cible
                    // On trace le résultat
                    String action = intent.Action;
                    String command = intent.GetStringExtra("COMMAND");
                    String commandidentifier = intent.GetStringExtra("COMMAND_IDENTIFIER");
                    String result = intent.GetStringExtra("RESULT");

                    if (String.Equals(commandidentifier, mCommandBase.mCommandIdentifier, StringComparison.OrdinalIgnoreCase) == false)
                        return;

                    Bundle bundle = new Bundle();
                    String resultInfo = "";
                    if (intent.HasExtra("RESULT_INFO"))
                    {
                        bundle = intent.GetBundleExtra("RESULT_INFO");
                        ICollection<string> keys = bundle.KeySet();
                        foreach (String key in keys)
                        {
                            resultInfo += key + ": " + bundle.GetString(key) + "\n";
                        }
                    }

                    String text = "Action: " + action + "\n" +
                            "Command: " + command + "\n" +
                            "Result: " + result + "\n" +
                            "Result Info: " + resultInfo + "\n" +
                            "CID:" + commandidentifier;

                    if (mCommandBase.mCommandBaseCallback != null)
                    {
                        mCommandBase.mCommandBaseCallback(new CommandBaseResults() { ProfileName = mCommandBase.Settings.ProfileName, Action = action, Command = command, Result = result, ResultInfo = resultInfo, CommandIdentifier = commandidentifier, Error = null });
                        mCommandBase.CleanAll();
                    }
                }
            }
        }

        protected override void CleanAll()
        {
            Settings.ProfileName = "";
            mCommandBaseCallback = null;
            mContext.ApplicationContext.UnregisterReceiver(mBroadcastReceiver);
            base.CleanAll();
        }

        /*
        This is what will happen if Datawedge does not answer before the timeout
         */
        protected override void OnError(String error)
        {
            if (mCommandBaseCallback != null)
            {
                mCommandBaseCallback(new CommandBaseResults() { ProfileName = Settings.ProfileName, Action = null, Command = null, Result = null, ResultInfo = null, CommandIdentifier = null, Error = error });
                CleanAll();
            }
        }

    }
}
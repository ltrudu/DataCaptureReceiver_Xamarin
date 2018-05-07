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

namespace DataCaptureReceiver
{
    public abstract class DWProfileBase
    {
        /*
A context to work with intents
 */
        protected Context mContext = null;

        /*
        The profile we are working on
         */
        protected String mProfileName = "";

        /*
        A time out, in case we don't receive an answer
        from DataWedge
         */
        protected long mTimeoutMS = 5000;

        /*
        A handler that will be used by the derived
        class to prevent waiting to loong for DW in case
        of problem
         */
        protected Handler mTimeOutHandler = new Handler();

        /*
        What will be done at the end of the TimeOut
         */

        protected Action mTimeOutAction = null;


    public DWProfileBase(Context aContext, String aProfile, long aTimeOut)
    {
        mContext = aContext;
        mProfileName = aProfile;
        mTimeoutMS = aTimeOut;
    }

    protected void SendDataWedgeIntentWithExtra(String action, String extraKey, String extraValue)
    {
        Intent dwIntent = new Intent(action);
        dwIntent.PutExtra(extraKey, extraValue);
        mContext.SendBroadcast(dwIntent);
    }

    protected void SendDataWedgeIntentWithExtra(String action, String extraKey, Bundle extras)
    {
        Intent dwIntent = new Intent(action);
        dwIntent.PutExtra(extraKey, extras);
        mContext.SendBroadcast(dwIntent);
    }

    protected virtual void Execute()
    {
            if(mTimeOutAction == null)
            {
                mTimeOutAction = new Action(() =>
                {
                    OnError("TimeOut Reached");
                });
            }
            else
            {
                mTimeOutHandler.RemoveCallbacks(mTimeOutAction);
            }
        /*
        Start time out mechanism
         */
        mTimeOutHandler.PostDelayed(mTimeOutAction, mTimeoutMS);
    }

    protected abstract void OnError(String error);

    protected virtual void CleanAll()
    {
        if (mTimeOutHandler != null)
        {
            mTimeOutHandler.RemoveCallbacks(mTimeOutAction);
        }
    }
}
}
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
    public class DWProfileDelete : DWProfileCommandBase
    {
        public DWProfileDelete(Context aContext) : base(aContext)
        {
        }


        public new void Execute(DWSettings settings, Action<CommandBaseResults> callback)
        {
            /*
            Call base class Execute to register command result
            broadcast receiver and launch timeout mechanism
             */
            base.Execute(settings, callback);

            /*
            Delete the profile
             */
            DeleteProfile(settings);
        }

        private void DeleteProfile(DWSettings settings)
        {
            // Delete the profile using intents
            SendDataWedgeIntentWithExtraRequestResult(DataWedgeConstants.ACTION_DATAWEDGE_FROM_6_2,
                    DataWedgeConstants.EXTRA_DELETE_PROFILE,
                    settings.ProfileName);

        }
    }
}
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
    public class DWSwitchContinuousMode : DWProfileCommandBase
    {
        public class DWSwitchContinuousModeSettings : DWProfileBase.DWSettings
        {
            public bool ContinuousMode = false;
        }


        public DWSwitchContinuousMode(Context aContext) : base(aContext)
        {
        }

        public void Execute(DWSwitchContinuousModeSettings settings, Action<CommandBaseResults> callback)
        {
            /*
            Call base class Execute to register command result
            broadcast receiver and launch timeout mechanism
             */
            base.Execute(settings, callback);

            /*
            Create the profile
             */
            SwitchToContinuousMode(settings);
        }

        private void SwitchToContinuousMode(DWSwitchContinuousModeSettings settings)
        {
            Bundle barcodeProps = new Bundle();
            if (settings.ContinuousMode)
            {
                barcodeProps.PutString("aim_type", "5");
                barcodeProps.PutString("beam_timer", "0");
                barcodeProps.PutString("different_barcode_timeout", "0");
                barcodeProps.PutString("same_barcode_timeout", "0");
            }
            else
            {
                barcodeProps.PutString("aim_type", "0");
                barcodeProps.PutString("beam_timer", "5000");
                barcodeProps.PutString("different_barcode_timeout", "500");
                barcodeProps.PutString("same_barcode_timeout", "500");

            }

            SendDataWedgeIntentWithExtraRequestResult(DataWedgeConstants.ACTION_DATAWEDGE_FROM_6_2, DataWedgeConstants.EXTRA_SWITCH_SCANNER_PARAMS, barcodeProps);
        }

    }
}
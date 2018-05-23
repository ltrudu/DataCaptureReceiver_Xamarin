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
    public class DWProfileSetConfig : DWProfileCommandBase
    {
        public class DWProfileSetConfigSettings : DWProfileBase.DWSettings
        {
            public bool AggressiveMode = false;
            public String IntentAction = null;
            public String IntentCategory = null;
        }

        public DWProfileSetConfig(Context aContext) : base(aContext)
        {
        }

        public void Execute(DWProfileSetConfigSettings settings, Action<CommandBaseResults> callback)
        {
            /*
            Call base class Execute to register command result
            broadcast receiver and launch timeout mechanism
             */
            base.Execute(settings, callback);

            /*
            Create the profile
             */
            SetProfileConfig(settings);
        }

        private void SetProfileConfig(DWProfileSetConfigSettings settings)
        {
            // (Re)Configuration du profil via l'intent SET_PROFILE
            // NB : on peut envoyer cet intent sans soucis même si le profil est déjà configuré
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", settings.ProfileName);
            profileConfig.PutString("PROFILE_ENABLED", "true");
            profileConfig.PutString("CONFIG_MODE", "UPDATE");

            // Configuration des applications du profil
            Bundle appConfig = new Bundle();
            appConfig.PutString("PACKAGE_NAME", mContext.PackageName);
            appConfig.PutStringArray("ACTIVITY_LIST", new String[] { "*" });
            profileConfig.PutParcelableArray("APP_LIST", new Bundle[] { appConfig });

            // Configuration des différents plugins
            List<IParcelable> pluginConfigs = new List<IParcelable>();

            // Configuration du plugin BARCODE
            Bundle barcodePluginConfig = new Bundle();
            barcodePluginConfig.PutString("PLUGIN_NAME", "BARCODE");
            barcodePluginConfig.PutString("RESET_CONFIG", "true");

            Bundle barcodeProps = new Bundle();
            barcodeProps.PutString("aim_mode", "on");
            barcodeProps.PutString("lcd_mode", "3");

            // Use this for Datawedge < 6.7
            //barcodeProps.putString("scanner_selection", "auto");

            // Use this for Datawedge >= 6.7
            barcodeProps.PutString("scanner_selection_by_identifier", "INTERNAL_IMAGER");

            if (settings.AggressiveMode)
            {
                // Super aggressive continuous mode without beam timer and no timeouts
                barcodeProps.PutString("aim_type", "5");
                barcodeProps.PutInt("beam_timer", 0);
                barcodeProps.PutString("different_barcode_timeout", "0");
                barcodeProps.PutString("same_barcode_timeout", "0");

            }
            else
            {
                // Standard mode with beam timer and same/different timeout
                barcodeProps.PutString("aim_type", "3");
                barcodeProps.PutInt("beam_timer", 5000);
                barcodeProps.PutString("different_barcode_timeout", "500");
                barcodeProps.PutString("same_barcode_timeout", "500");
            }
            barcodePluginConfig.PutBundle("PARAM_LIST", barcodeProps);
            pluginConfigs.Add(barcodePluginConfig);


            // Configuration du plugin KEYSTROKE
            Bundle keystrokePluginConfig = new Bundle();
            keystrokePluginConfig.PutString("PLUGIN_NAME", "KEYSTROKE");
            keystrokePluginConfig.PutString("RESET_CONFIG", "true");
            Bundle keystrokeProps = new Bundle();
            keystrokeProps.PutString("keystroke_output_enabled", "false");
            keystrokePluginConfig.PutBundle("PARAM_LIST", keystrokeProps);
            pluginConfigs.Add(keystrokePluginConfig);

            // Configuration du plugin INTENT
            Bundle intentPluginConfig = new Bundle();
            intentPluginConfig.PutString("PLUGIN_NAME", "INTENT");
            intentPluginConfig.PutString("RESET_CONFIG", "true");
            Bundle intentProps = new Bundle();
            intentProps.PutString("intent_output_enabled", "true");
            intentProps.PutString("intent_action", settings.IntentAction);
            intentProps.PutString("intent_category", settings.IntentCategory);
            intentProps.PutString("intent_delivery", "2");
            intentPluginConfig.PutBundle("PARAM_LIST", intentProps);
            pluginConfigs.Add(intentPluginConfig);

            // Envoi d'intent de configuration du profil
            profileConfig.PutParcelableArrayList("PLUGIN_CONFIG", pluginConfigs);

            SendDataWedgeIntentWithExtraRequestResult(DataWedgeConstants.ACTION_DATAWEDGE_FROM_6_2,
                    DataWedgeConstants.EXTRA_SET_CONFIG,
                    profileConfig);
        }
    }
}
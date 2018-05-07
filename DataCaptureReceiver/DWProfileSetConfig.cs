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
        protected static String mIntentAction = null;
        protected static String mIntentCategory = null;

        public DWProfileSetConfig(string intentAction, string intentCategory, Context aContext, string aProfile, long aTimeOut) : base(aContext, aProfile, aTimeOut)
        {
            mIntentAction = intentAction;
            mIntentCategory = intentCategory;
        }

        public new void Execute(Action<CommandBaseResults> callback)
        {
            /*
            Call base class Execute to register command result
            broadcast receiver and launch timeout mechanism
             */
            base.Execute(callback);

            /*
            Create the profile
             */
            SetProfileConfig(mProfileName);
        }

        private void SetProfileConfig(String profileName)
        {
            // (Re)Configuration du profil via l'intent SET_PROFILE
            // NB : on peut envoyer cet intent sans soucis même si le profil est déjà configuré
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", profileName);
            profileConfig.PutString("PROFILE_ENABLED", "true");
            profileConfig.PutString("CONFIG_MODE", "UPDATE");

            // Configuration des applications du profil
            Bundle appConfig = new Bundle();
            appConfig.PutString("PACKAGE_NAME", mContext.PackageName);
            appConfig.PutStringArray("ACTIVITY_LIST", new String[] { "*" });
            profileConfig.PutParcelableArray("APP_LIST", new Bundle[] { appConfig });

            // Configuration des différents plugins
            List<IParcelable> pluginConfigs = new List<IParcelable>();

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
            intentProps.PutString("intent_action", mIntentAction);
            intentProps.PutString("intent_category", mIntentCategory);
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
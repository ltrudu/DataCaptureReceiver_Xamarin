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
    public class DWProfileCreate : DWProfileCommandBase
    {
        public DWProfileCreate(Context aContext) : base(aContext)
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
            Create the profile
             */
            CreateProfile(settings);
        }

        private void CreateProfile(DWSettings settings)
        {
            // On crée le profil via l'intent CREATE_PROFILE
            // NB : on peut envoyer cet intent sans soucis même si le profil existe déjà
            // On perdrait plus de temps si on recherchait si le profil existe avant de lancer la création
            SendDataWedgeIntentWithExtraRequestResult(DataWedgeConstants.ACTION_DATAWEDGE_FROM_6_2,
                    DataWedgeConstants.EXTRA_CREATE_PROFILE,
                    settings.ProfileName);

        }
    }
}
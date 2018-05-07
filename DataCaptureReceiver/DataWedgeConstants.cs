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
    public class DataWedgeConstants
    {
        // Let's define some intent strings
        // This intent string contains the source of the data as a string
        public static string SOURCE_TAG = "com.motorolasolutions.emdk.datawedge.source";
        // This intent string contains the barcode symbology as a string
        public static string LABEL_TYPE_TAG = "com.motorolasolutions.emdk.datawedge.label_type";
        // This intent string contains the barcode data as a byte array list
        public static string DECODE_DATA_TAG = "com.motorolasolutions.emdk.datawedge.decode_data";

        // This intent string contains the captured data as a string
        // (in the case of MSR this data string contains a concatenation of the track data)
        public static string DATA_STRING_TAG = "com.motorolasolutions.emdk.datawedge.data_string";

        // Let's define the API intent strings for the soft scan trigger
        public static string DWAPI_ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
        public static string EXTRA_PARAMETER = "com.symbol.datawedge.api.EXTRA_PARAMETER";
        public static string DWAPI_START_SCANNING = "START_SCANNING";
        public static string DWAPI_STOP_SCANNING = "STOP_SCANNING";
        public static string DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";

        public static string DWAPI_ACTION_SCANNERINPUTPLUGIN = "com.symbol.datawedge.api.ACTION_SCANNERINPUTPLUGIN";
        public static string DWAPI_PARAMETER_SCANNERINPUTPLUGIN_ENABLE = "ENABLE_PLUGIN";
        public static string DWAPI_PARAMETER_SCANNERINPUTPLUGIN_DISABLE = "DISABLE_PLUGIN";

        //  6.2 API and up Actions sent to DataWedge
        public static string ACTION_DATAWEDGE_FROM_6_2 = "com.symbol.datawedge.api.ACTION";
        //  6.2 API and up Extras sent to DataWedge
        public static string EXTRA_GET_ACTIVE_PROFILE = "com.symbol.datawedge.api.GET_ACTIVE_PROFILE";
        public static string EXTRA_GET_PROFILES_LIST = "com.symbol.datawedge.api.GET_PROFILES_LIST";
        public static string EXTRA_DELETE_PROFILE = "com.symbol.datawedge.api.DELETE_PROFILE";
        public static string EXTRA_CLONE_PROFILE = "com.symbol.datawedge.api.CLONE_PROFILE";
        public static string EXTRA_RENAME_PROFILE = "com.symbol.datawedge.api.RENAME_PROFILE";
        public static string EXTRA_ENABLE_DATAWEDGE = "com.symbol.datawedge.api.ENABLE_DATAWEDGE";
        public static string EXTRA_EMPTY = "";
        //  6.2 API and up Actions received from DataWedge
        public static string ACTION_RESULT_DATAWEDGE_FROM_6_2 = "com.symbol.datawedge.api.RESULT_ACTION";
        //  6.2 API and up Extras received from DataWedge
        public static string EXTRA_RESULT_GET_ACTIVE_PROFILE = "com.symbol.datawedge.api.RESULT_GET_ACTIVE_PROFILE";
        public static string EXTRA_RESULT_GET_PROFILE_LIST = "com.symbol.datawedge.api.RESULT_GET_PROFILES_LIST";

        //  6.3 API and up Extras sent to DataWedge
        public static string EXTRA_SOFTSCANTRIGGER_FROM_6_3 = "com.symbol.datawedge.api.SOFT_SCAN_TRIGGER";
        public static string EXTRA_SCANNERINPUTPLUGIN_FROM_6_3 = "com.symbol.datawedge.api.SCANNER_INPUT_PLUGIN";
        public static string EXTRA_ENUMERATESCANNERS_FROM_6_3 = "com.symbol.datawedge.api.ENUMERATE_SCANNERS";
        public static string EXTRA_SETDEFAULTPROFILE_FROM_6_3 = "com.symbol.datawedge.api.SET_DEFAULT_PROFILE";
        public static string EXTRA_RESETDEFAULTPROFILE_FROM_6_3 = "com.symbol.datawedge.api.RESET_DEFAULT_PROFILE";
        public static string EXTRA_SWITCHTOPROFILE_FROM_6_3 = "com.symbol.datawedge.api.SWITCH_TO_PROFILE";
        public static string EXTRA_GET_VERSION_INFO = "com.symbol.datawedge.api.GET_VERSION_INFO";
        public static string EXTRA_REGISTER_NOTIFICATION = "com.symbol.datawedge.api.REGISTER_FOR_NOTIFICATION";
        public static string EXTRA_UNREGISTER_NOTIFICATION = "com.symbol.datawedge.api.UNREGISTER_FOR_NOTIFICATION";
        public static string EXTRA_CREATE_PROFILE = "com.symbol.datawedge.api.CREATE_PROFILE";
        public static string EXTRA_SET_CONFIG = "com.symbol.datawedge.api.SET_CONFIG";
        public static string EXTRA_RESTORE_CONFIG = "com.symbol.datawedge.api.RESTORE_CONFIG";
        //  6.3 API and up Actions received from DataWedge
        public static string ACTION_RESULT_NOTIFICATION = "com.symbol.datawedge.api.NOTIFICATION_ACTION";
        //  6.3 API and up Extras received from DataWedge
        public static string EXTRA_RESULT_GET_VERSION_INFO = "com.symbol.datawedge.api.RESULT_GET_VERSION_INFO";
        public static string EXTRA_RESULT_NOTIFICATION = "com.symbol.datawedge.api.NOTIFICATION";
        public static string EXTRA_RESULT_ENUMERATE_SCANNERS = "com.symbol.datawedge.api.RESULT_ENUMERATE_SCANNERS";
        //  6.3 API and up Parameter keys and values associated with extras received from DataWedge
        public static string EXTRA_KEY_APPLICATION_NAME = "com.symbol.datawedge.api.APPLICATION_NAME";
        public static string EXTRA_KEY_NOTIFICATION_TYPE = "com.symbol.datawedge.api.NOTIFICATION_TYPE";
        public static string EXTRA_RESULT_NOTIFICATION_TYPE = "NOTIFICATION_TYPE";
        public static string EXTRA_KEY_VALUE_SCANNER_STATUS = "SCANNER_STATUS";
        public static string EXTRA_KEY_VALUE_PROFILE_SWITCH = "PROFILE_SWITCH";
        public static string EXTRA_KEY_VALUE_CONFIGURATION_UPDATE = "CONFIGURATION_UPDATE";
        public static string EXTRA_KEY_VALUE_NOTIFICATION_STATUS = "STATUS";
        public static string EXTRA_KEY_VALUE_NOTIFICATION_PROFILE_NAME = "PROFILE_NAME";

        //  6.4 API and up Extras sent to Datawedge
        public static string EXTRA_GET_DATAWEDGE_STATUS = "com.symbol.datawedge.api.GET_DATAWEDGE_STATUS";
        //  6.4 API and up Parameter keys and values associated with extras received from Datawedge
        public static string EXTRA_RESULT_GET_DATAWEDGE_STATUS = "com.symbol.datawedge.api.RESULT_GET_DATAWEDGE_STATUS";

        //  6.5 API and up Extras sent to Datawedge
        public static string EXTRA_SEND_RESULT = "SEND_RESULT";
        public static string EXTRA_COMMAND_IDENTIFIER = "COMMAND_IDENTIFIER";
        public static string EXTRA_GET_CONFIG = "com.symbol.datawedge.api.GET_CONFIG";
        public static string EXTRA_GET_DISABLED_APP_LIST = "com.symbol.datawedge.api.GET_DISABLED_APP_LIST";
        public static string EXTRA_SET_DISABLED_APP_LIST = "com.symbol.datawedge.api.SET_DISABLED_APP_LIST";
        public static string EXTRA_SWITCH_SCANNER = "com.symbol.datawedge.api.SWITCH_SCANNER";
        public static string EXTRA_SWITCH_SCANNER_PARAMS = "com.symbol.datawedge.api.SWITCH_SCANNER_PARAMS";
        //  6.5 API and up Parameter keys and values associated with extras received from Datawedge
        public static string EXTRA_RESULT = "RESULT";
        public static string EXTRA_RESULT_INFO = "RESULT_INFO";
        public static string EXTRA_COMMAND = "COMMAND";
        public static string EXTRA_RESULT_GET_CONFIG = "com.symbol.datawedge.api.RESULT_GET_CONFIG";
        public static string EXTRA_RESULT_GET_DISABLED_APP_LIST = "com.symbol.datawedge.api.RESULT_GET_DISABLED_APP_LIST";

    }
}
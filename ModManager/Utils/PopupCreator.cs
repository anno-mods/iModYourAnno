using Imya.GithubIntegration.Download;
using Imya.Models;
using Imya.UI.Popup;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Utils
{
    internal class PopupCreator
    {
        static TextManager TextManager = TextManager.Instance;
        public static GenericOkayPopup CreateSaveTweakPopup()
        {
            var dialog = new GenericOkayPopup()
            {
                MESSAGE = TextManager.GetText("TWEAK_UNSAVED_CHANGES"),
                OK_TEXT = TextManager.GetText("TWEAK_SAVE"),
                CANCEL_TEXT = TextManager.GetText("TWEAK_DISCARD"),
                OkayAction = async () => await TweakManager.Instance.SaveAsync(),
                CancelAction = () => TweakManager.Instance.Unload()
            };
            return dialog;
        }

        public static GenericOkayPopup CreateInvalidSetupPopup()
        {
            GenericOkayPopup popup = new GenericOkayPopup()
            {
                MESSAGE = TextManager.GetText("ATTRIBUTE_GAMESTART_WARNING"),
                CANCEL_TEXT = TextManager.GetText("DIALOG_CANCEL"),
                OK_TEXT = TextManager.GetText("DIALOG_OKAY"),
            };
            return popup;
        }

        public static GenericOkayPopup CreateInstallationAlreadyRunningPopup() => new() 
        {
            MESSAGE = TextManager.GetText("POPUP_INSTALLATION_ALREADY_RUNNING"),
            OK_TEXT = TextManager.GetText("DIALOG_OKAY"),
            HasCancelButton = false
        };

        public static GenericOkayPopup CreateExceptionPopup(Exception e) => new() 
        { 
            MESSAGE = new SimpleText(e.Message),
            OK_TEXT = TextManager.GetText("DIALOG_OKAY"),
            HasCancelButton = false
        };

        public static GenericOkayPopup CreateApiRateExceededPopup() => new()
        {
            MESSAGE = TextManager.GetText("API_RATELIMIT_REACHED"),
            OK_TEXT = TextManager.GetText("DIALOG_OKAY"),
            HasCancelButton = false
        };

        public static GenericOkayPopup CreateLogoutPopup() => new() 
        { 
            MESSAGE = TextManager.GetText("DASHBOARD_LOGOUTCONFIRMATION"),
            CANCEL_TEXT = TextManager.GetText("DIALOG_CANCEL"),
            OK_TEXT = TextManager.GetText("DIALOG_OKAY"),

        };

    }
}

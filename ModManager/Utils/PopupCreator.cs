using Imya.GithubIntegration.Download;
using Imya.Models;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.UI.Popup;
using System;

namespace Imya.UI.Utils
{
    public class PopupCreator
    {
        private readonly ITextManager _textManager;
        private readonly ITweakService _tweakService;

        public PopupCreator(
            ITextManager textManager,
            ITweakService tweakService) 
        { 
            _textManager = textManager;
            _tweakService = tweakService;
        }
        public GenericOkayPopup CreateSaveTweakPopup()
        {
            var dialog = new GenericOkayPopup()
            {
                MESSAGE = _textManager.GetText("TWEAK_UNSAVED_CHANGES"),
                OK_TEXT = _textManager.GetText("TWEAK_SAVE"),
                CANCEL_TEXT = _textManager.GetText("TWEAK_DISCARD"),
                OkayAction = async () => await _tweakService.SaveAsync(),
                CancelAction = () => _tweakService.Unload()
            };
            return dialog;
        }

        public GenericOkayPopup CreateInvalidSetupPopup()
        {
            GenericOkayPopup popup = new GenericOkayPopup()
            {
                MESSAGE = _textManager.GetText("ATTRIBUTE_GAMESTART_WARNING"),
                CANCEL_TEXT = _textManager.GetText("DIALOG_CANCEL"),
                OK_TEXT = _textManager.GetText("DIALOG_OKAY"),
            };
            return popup;
        }

        public GenericOkayPopup CreateInstallationAlreadyRunningPopup() => new() 
        {
            MESSAGE = _textManager.GetText("POPUP_INSTALLATION_ALREADY_RUNNING"),
            OK_TEXT = _textManager.GetText("DIALOG_OKAY"),
            HasCancelButton = false
        };

        public GenericOkayPopup CreateExceptionPopup(Exception e) => new() 
        { 
            MESSAGE = new SimpleText(e.Message),
            OK_TEXT = _textManager.GetText("DIALOG_OKAY"),
            HasCancelButton = false
        };

        public GenericOkayPopup CreateApiRateExceededPopup() => new()
        {
            MESSAGE = _textManager.GetText("API_RATELIMIT_REACHED"),
            OK_TEXT = _textManager.GetText("DIALOG_OKAY"),
            HasCancelButton = false
        };

        public GenericOkayPopup CreateLogoutPopup() => new() 
        { 
            MESSAGE = _textManager.GetText("DASHBOARD_LOGOUTCONFIRMATION"),
            CANCEL_TEXT = _textManager.GetText("DIALOG_CANCEL"),
            OK_TEXT = _textManager.GetText("DIALOG_OKAY"),

        };

        public GenericOkayPopup CreateModloaderPopup() => new()
        {
            MESSAGE = _textManager.GetText("STARTUP_REMOVECOMMUNITYMODLOADER"),
            CANCEL_TEXT = _textManager.GetText("STARTUP_REMOVECOMMUNITYMODLOADER_NO"),
            OK_TEXT = _textManager.GetText("STARTUP_REMOVECOMMUNITYMODLOADER_YES"),
        };

        public AuthCodePopup CreateAuthCodePopup(string AuthCode) => new AuthCodePopup(AuthCode);
    }
}

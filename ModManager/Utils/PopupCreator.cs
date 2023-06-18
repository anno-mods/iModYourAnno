using Imya.Enums;
using Imya.GithubIntegration.Download;
using Imya.Models;
using Imya.Services;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.UI.Popup;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Imya.UI.Utils
{
    public class PopupCreator
    {
        private readonly ITextManager _textManager;
        private readonly ITweakService _tweakService;
        private readonly IImyaSetupService _imyaSetupService;

        private readonly IServiceProvider _serviceProvider;

        public PopupCreator(
            ITextManager textManager,
            ITweakService tweakService,
            IImyaSetupService imyaSetupService,
            IServiceProvider serviceProvider) 
        { 
            _textManager = textManager;
            _tweakService = tweakService;
            _imyaSetupService = imyaSetupService;
            _serviceProvider = serviceProvider;
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

        public AuthCodePopup CreateAuthCodePopup(string AuthCode) => new AuthCodePopup(AuthCode)
        {
            MESSAGE = _textManager.GetText("USERCODE_POPUP_MESSAGE"),
            CANCEL_TEXT = _textManager.GetText("DIALOG_CANCEL")
        };

        public AddDlcPopup CreateAddDlcPopup(IEnumerable<DlcId> dlcIds) => new()
        {
            Title = _textManager.GetText("PROFILE_LOAD").Text,
            Dlcs = new ObservableCollection<DlcId>(dlcIds)
        };

        public ProfilesLoadPopup CreateProfilesLoadPopup() {
            var popup = new ProfilesLoadPopup(_serviceProvider.GetRequiredService<IProfilesService>())
            {
                OK_TEXT = _textManager.GetText("DIALOG_OKAY"),
                CANCEL_TEXT = _textManager.GetText("DIALOG_CANCEL"),
                Title = _textManager.GetText("PROFILE_LOAD").Text
            };
            popup.Load();
            return popup;
        }

        public ProfilesSavePopup CreateProfilesSavePopup() => new(_serviceProvider.GetRequiredService<IProfilesService>())
        {
            OK_TEXT = _textManager.GetText("DIALOG_OKAY"),
            CANCEL_TEXT = _textManager.GetText("DIALOG_CANCEL"),
            Title = _textManager.GetText("PROFILE_SAVE").Text
        };
    }
}

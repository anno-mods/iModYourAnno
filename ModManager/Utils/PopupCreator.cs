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
                MESSAGE = new SimpleText("You have unsaved changes. Save now?"),
                OK_TEXT = new SimpleText("Save Now"),
                CANCEL_TEXT = new SimpleText("Discard Changes")
            };
            return dialog;
        }

        public static GenericOkayPopup CreateInvalidSetupPopup()
        {
            GenericOkayPopup popup = new GenericOkayPopup()
            {
                MESSAGE = TextManager.GetText("ATTRIBUTE_GAMESTART_WARNING")
            };
            return popup;
        }
    }
}

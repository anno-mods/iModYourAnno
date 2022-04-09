using Imya.Enums;
using Imya.Models;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Imya.UI.ValueConverters
{
    /// <summary>
    /// A converter that maps DLC IDs to LocalizedTexts
    /// </summary>
    [ValueConversion(typeof(DlcId), typeof(LocalizedText))]
    internal class DlcTextConverter : IValueConverter
    {
        private TextManager TextManager = TextManager.Instance;

        private Dictionary<DlcId, String> DlcTextMapping = new Dictionary<DlcId, String>
        {
            { DlcId.SunkenTreasures, "DLC_SUNKENTREASURES"},
            { DlcId.Botanica, "DLC_BOTANICA" },
            { DlcId.ThePassage,"DLC_PASSAGE" },
            { DlcId.SeatOfPower, "DLC_SEATOFPOWER" },
            { DlcId.BrightHarvest, "DLC_BRIGHTHARVEST" },
            { DlcId.LandOfLions, "DLC_LANDOFLIONS" },
            { DlcId.Docklands, "DLC_DOCKLANDS" },
            { DlcId.Tourism, "DLC_TOURISTSEASON" },
            { DlcId.Highlife, "DLC_HIGHLIFE" },
            { DlcId.Christmas, "DLC_CHRISTMAS" },
            { DlcId.AmusementPark, "DLC_AMUSEMENTPARK" },
            { DlcId.CityLife, "DLC_CITYLIFE" },
            { DlcId.VehicleSkins, "DLC_VEHICLESKINS" },
            { DlcId.PedestrianZone, "DLC_PEDESTRIANZONE" },
            { DlcId.VibrantCity, "DLC_VIBRANTCITY" },
            { DlcId.Anarchist, "DLC_ANARCHIST" },
            { DlcId.SeedsOfChange, "DLC_SEEDSOFUSELESS" },
            { DlcId.EmpireOfTheSkies, "DLC_AIRSHIPS" },
            { DlcId.NewWorldRising, "DLC_NEWWORLDRISING" }
        };

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            var dlc = (DlcId)value;
            return TextManager[GetDlcText(dlc)];
        }

        private String GetDlcText(DlcId id)
        {
            if (DlcTextMapping.TryGetValue(id, out String? Filepath))
            {
                return Filepath;
            }
            else return "NO_TEXT";
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException();
        }
    }
}

using Imya.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(DlcId), typeof(String))]
    internal class DlcIconConverter : IValueConverter
    {
        private Dictionary<DlcId, String> DlcIconMapping = new Dictionary<DlcId, String>
        {
            { DlcId.SunkenTreasures, "dlc_cape"},
            { DlcId.Botanica, "dlc_botanica" },
            { DlcId.ThePassage,"dlc_passage" },
            { DlcId.SeatOfPower, "dlc_palace" },
            { DlcId.BrightHarvest, "dlc_brightharvest" },
            { DlcId.LandOfLions, "dlc_lol" },
            { DlcId.Docklands, "dlc_docklands" },
            { DlcId.Tourism, "dlc_tourists" },
            { DlcId.Highlife, "dlc_highlife" },
            { DlcId.Anarchist, "dlc_anarchist" },
            { DlcId.SeedsOfChange, "dlc_seedsofuseless"},
            { DlcId.EmpireOfTheSkies, "dlc_airships"},
            { DlcId.NewWorldRising, "dlc_newworldrising"},


            { DlcId.SeasonalDecorations, "icon_seasondecorations" },
            { DlcId.AmusementPark, "icon_amusement" },
            { DlcId.CityLife, "dlc_citylights" },
            { DlcId.Christmas, "icon_holidaypack" },
            { DlcId.PedestrianZone, "dlc_pedestrians" },
            { DlcId.VibrantCity, "dlc_residenceskins" },
            { DlcId.VehicleSkins, "dlc_vehicleskins" }
        };

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            var dlc = (DlcId)value;
            return "\\"+ Path.Combine("resources", "dlc_icons", GetDlcId(dlc) + ".png");
        }

        private String GetDlcId(DlcId id)
        {
            if (DlcIconMapping.TryGetValue(id, out String? Filepath))
            {
                return Filepath;
            }
            else return "dlc_anarchist";
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException(); 
        }
    }
}

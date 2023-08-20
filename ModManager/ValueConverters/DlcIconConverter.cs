using Anno.EasyMod.Metadata;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace Imya.UI.ValueConverters
{
    [ValueConversion(typeof(DlcId), typeof(Image))]
    internal class DlcIconConverter : IValueConverter
    {
        private FilepathToImageConverter _filepathToImageConverter;
        public DlcIconConverter(FilepathToImageConverter filepathToImageConverter) 
        {
            _filepathToImageConverter = filepathToImageConverter;
        }

        private Dictionary<DlcId, String> DlcIconMapping = new Dictionary<DlcId, String>
        {
            { DlcId.SunkenTreasures, "data/ui/2kimages/main/3dicons/icon_dlc_sunken_treasure.png"},
            { DlcId.Botanica, "data/ui/2kimages/main/3dicons/icon_dlc_botanica.png" },
            { DlcId.ThePassage,"data/ui/2kimages/main/3dicons/icon_dlc_passage_128.png" },
            { DlcId.SeatOfPower, "data/ui/2kimages/main/3dicons/icon_dlc_palace.png" },
            { DlcId.BrightHarvest, "data/ui/2kimages/main/3dicons/icon_dlc_bright_harvest_128.png" },
            { DlcId.LandOfLions, "data/ui/2kimages/main/3dicons/icon_dlc_land_of_lions_128.png" },
            { DlcId.Docklands, "data/ui/2kimages/main/3dicons/icon_dlc_docklands_128.png" },
            { DlcId.Tourism, "data/ui/2kimages/main/3dicons/icon_dlc_tourist_season_128.png" },
            { DlcId.Highlife, "data/ui/2kimages/main/3dicons/icon_dlc_high_life_128.png" },
            { DlcId.Anarchist, "data/ui/2kimages/main/3dicons/icon_anarchist.png" },
            { DlcId.SeedsOfChange, "data/ui/2kimages/main/3dicons/icon_dlc_seeds_of_change_128.png"},
            { DlcId.EmpireOfTheSkies, "data/ui/2kimages/main/3dicons/icon_dlc_empire_of_the_skies_128.png"},
            { DlcId.NewWorldRising, "data/ui/2kimages/main/3dicons/icon_dlc_new_world_rising_128.png"},
            { DlcId.SeasonalDecorations, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_07_seasonal_decoration.png" },
            { DlcId.AmusementPark, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_02_amusement_park.png" },
            { DlcId.CityLife, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_03_city_lights.png" },
            { DlcId.Christmas, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_01_holidays.png" },
            { DlcId.IndustryOrnaments, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_08_industrial_zone.png"},
            { DlcId.PedestrianZone, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_05_pedestrian_zone.png" },
            { DlcId.VibrantCity, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_06_vibrant_city.png" },
            { DlcId.VehicleSkins, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_04_vehicle_liveries.png" },
            { DlcId.DragonGarden, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_10_dragon_pack.png" },
            { DlcId.OldTown, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_09_old_town.png" },
            { DlcId.Fiesta, "data/ui/2kimages/main/3dicons/ornaments/cdlc_category/icon_cdlc_11_fiesta.png"}
        };

        public object Convert(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            if (value is not DlcId dlcId)
                return new System.Windows.Controls.Image();

            DlcIconMapping.TryGetValue(dlcId, out var path);

            return _filepathToImageConverter.Convert(path ?? "", typeof(System.Windows.Controls.Image), parameter, Culture);
        }

        public object ConvertBack(object value, Type TargetType, object parameter, CultureInfo Culture)
        {
            throw new NotImplementedException(); 
        }
    }
}

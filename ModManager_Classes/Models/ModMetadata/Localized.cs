using Imya.Utils;

namespace Imya.Models.ModMetadata
{
    public class Localized
    {
        public String? Chinese { get; set; }
        public String? English { get; set; }
        public String? French { get; set; }
        public String? German { get; set; }
        public String? Italian { get; set; }
        public String? Japanese { get; set; }
        public String? Korean { get; set; }
        public String? Polish { get; set; }
        public String? Russian { get; set; }
        public String? Spanish { get; set; }
        public String? Taiwanese { get; set; }

        public Localized() { }

        // keep most common languages on top
        public bool HasAny() =>
            English is not null || German is not null || 
            French is not null || Italian is not null || Polish is not null || Russian is not null || Spanish is not null || 
            Japanese is not null || Korean is not null || Taiwanese is not null;
    }

    public class FakeLocalized : Localized
    {
        public FakeLocalized(string text)
        {
            Chinese = text;
            English = text;
            French = text;
            German = text;
            Italian = text;
            Japanese = text;
            Korean = text;
            Polish = text;
            Russian = text;
            Spanish = text;
            Taiwanese = text;
        }
    }
}

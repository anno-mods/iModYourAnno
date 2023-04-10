using Imya.Utils;

namespace Imya.Models.ModMetadata.ModinfoModel
{
    public class Localized
    {
        public string? Chinese { get; set; }
        public string? English { get; set; }
        public string? French { get; set; }
        public string? German { get; set; }
        public string? Italian { get; set; }
        public string? Japanese { get; set; }
        public string? Korean { get; set; }
        public string? Polish { get; set; }
        public string? Russian { get; set; }
        public string? Spanish { get; set; }
        public string? Taiwanese { get; set; }

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

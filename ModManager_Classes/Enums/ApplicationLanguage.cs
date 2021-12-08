using System.Runtime.Serialization;

namespace Imya.Enums
{
    //when adding new entries here, make sure to also add the respective values in the LocalizedText.UpdateText() function.
    public enum ApplicationLanguage
    {
        [EnumMember] English,
        [EnumMember] German
    }
}

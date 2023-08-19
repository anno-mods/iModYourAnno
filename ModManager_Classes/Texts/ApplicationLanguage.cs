﻿using System.Runtime.Serialization;

namespace Imya.Models
{
    //when adding new entries here, make sure to also add the respective values in the LocalizedText.UpdateText() function.
    public enum ApplicationLanguage
    {
        [EnumMember] Chinese,
        [EnumMember] English,
        [EnumMember] German,
        [EnumMember] Russian,
        [EnumMember] Polish
    }
}

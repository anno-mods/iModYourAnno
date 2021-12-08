using System.Runtime.Serialization;

namespace Imya.Enums
{
    public enum DlcRequirement
    {
        [EnumMember] required,
        [EnumMember] partly,
        [EnumMember] atLeastOneRequired
    }
}

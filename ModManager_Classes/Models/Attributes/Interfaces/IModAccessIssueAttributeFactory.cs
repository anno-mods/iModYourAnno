using Anno.EasyMod.Attributes;

namespace Imya.Models.Attributes.Interfaces
{
    public interface IModAccessIssueAttributeFactory
    {
        IModAttribute Get();
        IModAttribute GetNoDelete();
    }
}
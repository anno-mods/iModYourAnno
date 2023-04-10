namespace Imya.Models.Attributes.Interfaces
{
    public interface IModAccessIssueAttributeFactory
    {
        IAttribute Get();
        IAttribute GetNoDelete();
    }
}
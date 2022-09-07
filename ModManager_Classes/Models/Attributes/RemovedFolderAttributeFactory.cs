using Imya.Utils;

namespace Imya.Models.Attributes
{
    internal class RemovedFolderAttributeFactory
    {
        static GenericAttribute TweakedAttribute =
            new GenericAttribute()
            {
                AttributeType = AttributeType.IssueModRemoved,
                //Description = TextManager.Instance.GetText("ATTRIBUTE_TWEAKED")
                Description = new SimpleText("This mod has been removed by another program")
            };

        public static IAttribute Get()
        {
            return TweakedAttribute;
        }
    }
}

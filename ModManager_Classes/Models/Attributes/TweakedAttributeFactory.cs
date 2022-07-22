using Imya.Utils;

namespace Imya.Models.Attributes
{
    public class TweakedAttributeFactory
    {
        static GenericAttribute TweakedAttribute =
            new GenericAttribute()
            {
                AttributeType = AttributeType.TweakedMod,
                Description = TextManager.Instance.GetText("ATTRIBUTE_TWEAKED")
            };

        public static IAttribute Get()
        {
            return TweakedAttribute;
        }
    }
}

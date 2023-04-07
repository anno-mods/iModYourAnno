using Imya.Models.Attributes;
using Xunit;

namespace Imya.UnitTests
{
    public class AttributeTests
    {
        [Fact]
        public void AllowMultiple()
        {
            AttributeCollection attributes = new();

            attributes.AddAttribute(new GenericModContextAttribute());
            attributes.AddAttribute(new GenericModContextAttribute());

            Assert.Equal(2, attributes.Count);
        }

        [Fact]
        public void DontAllowMultiple()
        {
            AttributeCollection attributes = new();

            attributes.AddAttribute(new GenericAttribute());
            attributes.AddAttribute(new GenericAttribute());

            Assert.Single(attributes);
        }
    }
}

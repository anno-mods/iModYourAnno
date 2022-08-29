using Imya.Models.Attributes;
using Imya.UnitTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Imya.UnitTests
{
    public class AttributeTests
    {
        [Fact]
        public void AllowMultiple()
        {
            TestAttributeCollection attributes = new();

            attributes.AddAttribute(new ModCompabilityIssueAttribute());
            attributes.AddAttribute(new ModCompabilityIssueAttribute());

            Assert.Equal(2, attributes.Count());
        }

        [Fact]
        public void DontAllowMultiple()
        {
            TestAttributeCollection attributes = new();

            attributes.AddAttribute(new GenericAttribute());
            attributes.AddAttribute(new GenericAttribute());

            Assert.Single(attributes);
        }
    }
}

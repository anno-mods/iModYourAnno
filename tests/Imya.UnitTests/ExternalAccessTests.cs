using Imya.Models;
using Imya.Models.Attributes;
using Imya.UnitTests.Models;
using Imya.Utils;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Imya.UnitTests
{
    public class ExternalAccessTests
    {
        public ExternalAccessTests()
        {
            // TODO going via side effects is not a good idea
            AttributeCollectionFactory.AttributeCollectionType = typeof(TestAttributeCollection);
        }

        [Fact]
        public async Task DeleteMod()
        {
            DirectoryEx.EnsureDeleted(@"tmp\ExternalAccessTests");

            // prepare a mod
            const string folder = @"tmp\ExternalAccessTests\mods\[a] mod1";
            Directory.CreateDirectory(folder);
            var mods = new ModCollection(@"tmp\ExternalAccessTests\mods");
            mods.LoadModsAsync().Wait();

            // check
            Assert.Single(mods.Mods);

            // delete mod
            DirectoryEx.EnsureDeleted(@"tmp\ExternalAccessTests\mods\[a] mod1");

            // deactivate deleted mod
            await mods.Mods[0].ChangeActivationAsync(false);

            // mod should be removed now
            Assert.Empty(mods.Mods);
        }
    }
}

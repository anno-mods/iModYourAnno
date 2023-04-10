using Imya.Models;
using Imya.Models.Mods;
using Imya.Utils;
using Imya.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Imya.UnitTests
{
    public class ExternalAccessTests
    {
        [Fact]
        public async Task ChangeActivationOnDeletedMod()
        {
            const string test = $@"temp\{nameof(ChangeActivationOnDeletedMod)}";
            DirectoryEx.EnsureDeleted(test);

            // prepare a mod
            const string folder = $@"{test}\mods\[a] mod1";
            Directory.CreateDirectory(folder);
            var mods = new ModCollection($@"{test}\mods");
            await mods.LoadModsAsync();

            // check
            Assert.Single(mods);

            // delete mod
            DirectoryEx.EnsureDeleted(folder);

            // deactivate deleted mod
            await mods.ChangeActivationAsync(mods.Mods[0], false);

            // mod should be still in the list, but marked as removed
            Assert.Single(mods);
            Assert.True(mods.Mods[0].IsRemoved);
            Assert.True(mods.Mods[0].Attributes.HasAttribute(Models.Attributes.AttributeType.IssueModRemoved));
            Assert.Single(mods.Mods[0].Attributes);

            // activation state should be unchanged
            Assert.True(mods.Mods[0].IsActive);
        }

        [Fact]
        public async Task ValidateOnDeletedMod()
        {
            const string test = $@"temp\{nameof(ValidateOnDeletedMod)}";
            DirectoryEx.EnsureDeleted(test);

            // prepare a mod
            const string folderA = $@"{test}\mods\[a] mod A";
            Directory.CreateDirectory(folderA);
            File.WriteAllText($@"{folderA}\modinfo.json", @"{""ModID"": ""modA""}");
            const string folderB = $@"{test}\mods\[a] mod B";
            Directory.CreateDirectory(folderB);
            File.WriteAllText($@"{folderB}\modinfo.json", @"{""ModID"": ""modB"", ""ModDependencies"": [ ""modA"" ]}");
            const string folderC = $@"{test}\mods\[a] mod C";
            Directory.CreateDirectory(folderC);
            var mods = new ModCollection($@"{test}\mods");
            await mods.LoadModsAsync();

            // check
            Assert.Equal(3, mods.Count);

            // delete mod
            DirectoryEx.EnsureDeleted(folderA);

            // deactivate deleted mod to trigger IsRemoved=true
            Mod modA = mods.Mods.Where(x => x.Name.Text == "mod A").First();
            modA.IsRemoved = true;

            // trigger validation by deleting mod3
            ModCollectionHooks hooks = new(mods);
            await mods.DeleteAsync(mods.Mods.Where(x => x.Name.Text == "mod C").ToArray());
            Assert.Equal(2, mods.Count);

            // mod B should have dependency issue
            Mod modB = mods.Mods.Where(x => x.Name.Text == "mod B").First();
            Assert.True(modB.Attributes.HasAttribute(Models.Attributes.AttributeType.UnresolvedDependencyIssue));
        }

        [Fact]
        public async Task InstallDeletedMod()
        {
            const string test = $@"temp\{nameof(InstallDeletedMod)}";
            DirectoryEx.EnsureDeleted(test);

            // prepare a mod
            const string folderA = $@"{test}\mods\[a] mod1";
            Directory.CreateDirectory(folderA);
            var mods = new ModCollection($@"{test}\mods");
            await mods.LoadModsAsync();

            // prepare reinstall mod
            const string folderB = $@"{test}\mods2\[a] mod1";
            Directory.CreateDirectory(folderB);
            var modsInstall = new ModCollection($@"{test}\mods2");
            await modsInstall.LoadModsAsync();

            // check
            Assert.Single(mods);
            DirectoryEx.EnsureDeleted(folderA);
            await mods.ChangeActivationAsync(mods.Mods[0], false);
            Assert.Single(mods);
            Assert.True(mods.Mods[0].IsRemoved);

            // reinstall
            await mods.MoveIntoAsync(modsInstall);

            // should be active mod now
            Assert.Single(mods);
            Assert.False(mods.Mods[0].IsRemoved);
            Assert.False(mods.Mods[0].Attributes.HasAttribute(Models.Attributes.AttributeType.IssueModRemoved));
            Assert.True(mods.Mods[0].IsActive);
            // check reevaluation of things like modinfo.json
            Assert.True(mods.Mods[0].Attributes.HasAttribute(Models.Attributes.AttributeType.MissingModinfo));
        }

        [Fact]
        public async Task DeleteDeletedMod()
        {
            const string test = $@"temp\{nameof(DeleteDeletedMod)}";
            DirectoryEx.EnsureDeleted(test);

            // prepare a mod
            const string folderA = $@"{test}\mods\[a] mod1";
            Directory.CreateDirectory(folderA);
            var mods = new ModCollection($@"{test}\mods");
            await mods.LoadModsAsync();

            // check
            Assert.Single(mods);
            DirectoryEx.EnsureDeleted(folderA);
            await mods.ChangeActivationAsync(mods.Mods[0], false);
            Assert.Single(mods);
            Assert.True(mods.Mods[0].IsRemoved);

            // delete
            await mods.DeleteAsync(mods.ToArray());

            // should be deleted now
            Assert.Empty(mods);
        }

        [Fact]
        public async Task DeleteDeletedModNoTrigger()
        {
            const string test = $@"{nameof(DeleteDeletedModNoTrigger)}";
            DirectoryEx.EnsureDeleted(test);

            // prepare a mod
            const string folderA = $@"{test}\mods\[a] mod1";
            Directory.CreateDirectory(folderA);
            var mods = new ModCollection($@"{test}\mods");
            await mods.LoadModsAsync();

            // check
            Assert.Single(mods);
            DirectoryEx.EnsureDeleted(folderA);
            Assert.Single(mods);
            Assert.False(mods.Mods[0].IsRemoved);

            // delete
            await mods.DeleteAsync(mods.ToArray());

            // should be deleted now
            Assert.Empty(mods);
        }
    }
}

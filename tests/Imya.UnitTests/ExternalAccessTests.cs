using Imya.Models;
using Imya.Models.Attributes.Factories;
using Imya.Models.Attributes.Interfaces;
using Imya.Models.ModMetadata;
using Imya.Models.Mods;
using Imya.Services.Interfaces;
using Imya.Texts;
using Imya.Utils;
using Imya.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Imya.UnitTests
{
    public class ExternalAccessTests
    {
        IModCollectionFactory _collectionFactory;

        IServiceProvider serviceProvider; 

        public ExternalAccessTests() 
        {
            var builder = Host.CreateDefaultBuilder();

            serviceProvider = builder.ConfigureServices(services =>
            {
                services.AddSingleton(Mock.Of<IGameSetupService>());
                services.AddSingleton(x => new ModCollectionHooks());
                services.AddSingleton(Mock.Of<ITextManager>());
                services.AddSingleton<LocalizedModinfoFactory>();
                services.AddSingleton<IModFactory, ModFactory>();
                services.AddSingleton<IModStatusAttributeFactory, ModStatusAttributeFactory>();
                services.AddSingleton<IModAccessIssueAttributeFactory, ModAccessIssueAttributeFactory>();
                services.AddSingleton<IMissingModinfoAttributeFactory, MissingModinfoAttributeFactory>();
                services.AddSingleton<IModDependencyIssueAttributeFactory, ModDependencyIssueAttributeFactory>();
                services.AddSingleton<IRemovedFolderAttributeFactory, RemovedFolderAttributeFactory>();
                services.AddSingleton<ModDependencyValidator>();
                services.AddSingleton<RemovedModValidator>();
                services.AddSingleton<ModCollectionFactory>();                
            }).Build()
            .Services;

            _collectionFactory = serviceProvider.GetRequiredService<ModCollectionFactory>();
        }

        [Fact]
        public async Task ChangeActivationOnDeletedMod()
        {
            const string test = $@"temp\{nameof(ChangeActivationOnDeletedMod)}";
            DirectoryEx.EnsureDeleted(test);

            // prepare a mod
            const string folder = $@"{test}\mods\[a] mod1";
            Directory.CreateDirectory(folder);
            var mods = _collectionFactory.Get($@"{test}\mods");
            await mods.LoadModsAsync();
            var hooks = new ModCollectionHooks();
            hooks.HookTo(mods);
            hooks.AddHook(serviceProvider.GetRequiredService<RemovedModValidator>());

            // check
            Assert.Single(mods);

            // delete mod
            DirectoryEx.EnsureDeleted(folder);

            // deactivate deleted mod
            await mods.ChangeActivationAsync(mods.Mods[0], false);

            // mod should be still in the list, but marked as removed
            Assert.Single(mods);
            Assert.True(mods.Mods[0].IsRemoved);
            //todo these should be seperate tests
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
            var mods = _collectionFactory.Get($@"{test}\mods");
            await mods.LoadModsAsync();

            // check
            Assert.Equal(3, mods.Count);

            // delete mod
            DirectoryEx.EnsureDeleted(folderA);

            // deactivate deleted mod to trigger IsRemoved=true
            Mod modA = mods.Mods.Where(x => x.ModID == "modA").First();
            modA.IsRemoved = true;

            // trigger validation by deleting mod3
            var hooks = new ModCollectionHooks();
            hooks.HookTo(mods);
            hooks.AddHook(serviceProvider.GetRequiredService<ModDependencyValidator>());
            await mods.DeleteAsync(mods.Mods.Where(x => x.ModID == "[a] mod C").ToArray());

            Assert.Equal(2, mods.Count);

            // mod B should have dependency issue
            Mod modB = mods.Mods.Where(x => x.ModID == "modB").First();
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
            var mods = _collectionFactory.Get($@"{test}\mods");
            await mods.LoadModsAsync();

            // prepare reinstall mod
            const string folderB = $@"{test}\mods2\[a] mod1";
            Directory.CreateDirectory(folderB);
            var modsInstall = _collectionFactory.Get($@"{test}\mods2");
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
            var mods = _collectionFactory.Get($@"{test}\mods");
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
            var mods = _collectionFactory.Get($@"{test}\mods");
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

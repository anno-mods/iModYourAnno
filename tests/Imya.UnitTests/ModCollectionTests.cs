using System.IO;
using System;
using System.Threading.Tasks;
using Xunit;
using Imya.Models;
using Imya.Utils;
using System.Linq;

namespace Imya.UnitTests
{
    // Note: don't use await, debug doesn't work well with it
    public class ModCollectionTests
    {
        [Fact]
        public void Single()
        {
            DirectoryEx.EnsureDeleted("tmp");

            const string folder = "tmp\\install1\\target\\[a] mod1";
            Directory.CreateDirectory(folder);

            var target = new ModCollection("tmp\\install1\\target");
            target.LoadModsAsync().Wait();

            Assert.Single(target.Mods);
            Assert.Equal(ModStatus.Default, target.Mods.First().Status);
        }

        /// <summary>
        /// Add new mods.
        /// </summary>
        [Fact]
        public void Add_NewMods()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            Directory.CreateDirectory("tmp\\target");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // verify target
            const string activeTargetMod = "tmp\\target\\[a] mod1";
            Assert.False(Directory.Exists(activeTargetMod));
            const string inactiveTargetMod = "tmp\\target\\-[a] mod2";
            Assert.False(Directory.Exists(inactiveTargetMod));

            // create source
            const string activeSourceMod = "tmp\\source\\[a] mod1";
            Directory.CreateDirectory(activeSourceMod);
            File.WriteAllText($"{activeSourceMod}\\add.txt", "");
            const string inactiveSourceMod = "tmp\\source\\-[a] mod2";
            Directory.CreateDirectory(inactiveSourceMod);
            File.WriteAllText($"{inactiveSourceMod}\\add.txt", "");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should be overwritten
            target.AddAsync(source).Wait();
            Assert.True(File.Exists($"{activeTargetMod}\\add.txt"));
            Assert.True(File.Exists($"{inactiveTargetMod}\\add.txt"));
            Assert.Equal(ModStatus.New, target.Mods[0].Status);
            Assert.Equal(ModStatus.New, target.Mods[1].Status);
        }

        /// <summary>
        /// Overwrite mod. Delete old files.
        /// </summary>
        [Fact]
        public void Add_OverwriteActiveMod()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\[a] mod1";
            Directory.CreateDirectory(targetMod);
            File.WriteAllText($"{targetMod}\\remove.txt", "");
            File.WriteAllText($"{targetMod}\\update.txt", "old text");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // verify target
            Assert.False(File.Exists($"{targetMod}\\add.txt"));
            Assert.True(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("old text", File.ReadAllText($"{targetMod}\\update.txt"));

            // create source
            const string sourceMod = "tmp\\source\\[a] mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\add.txt", "");
            File.WriteAllText($"{sourceMod}\\update.txt", "new text");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should be overwritten
            target.AddAsync(source).Wait();
            Assert.True(File.Exists($"{targetMod}\\add.txt"));
            Assert.False(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("new text", File.ReadAllText($"{targetMod}\\update.txt"));
            Assert.Equal(ModStatus.Updated, target.Mods.First().Status);
        }

        /// <summary>
        /// Overwrite mod but keep inactive state.
        /// </summary>
        [Fact]
        public void Add_OverwriteInactive()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\-[a] mod1";
            Directory.CreateDirectory(targetMod);
            File.WriteAllText($"{targetMod}\\remove.txt", "");
            File.WriteAllText($"{targetMod}\\update.txt", "old text");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // verify target
            Assert.False(File.Exists($"{targetMod}\\add.txt"));
            Assert.True(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("old text", File.ReadAllText($"{targetMod}\\update.txt"));

            // create source
            const string sourceMod = "tmp\\source\\[a] mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\add.txt", "");
            File.WriteAllText($"{sourceMod}\\update.txt", "new text");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should be overwritten
            target.AddAsync(source).Wait();
            Assert.True(File.Exists($"{targetMod}\\add.txt"));
            Assert.False(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("new text", File.ReadAllText($"{targetMod}\\update.txt"));
            Assert.Equal(ModStatus.Updated, target.Mods.First().Status);
        }

        /// <summary>
        /// Deactivate old folder name, place mod with new folder name.
        /// </summary>
        [Fact]
        public void Add_UpdateWithDifferentFolder()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create obsolete target
            const string obsoleteMod = "tmp\\target\\[a] mod1 oldname";
            Directory.CreateDirectory(obsoleteMod);
            File.WriteAllText($"{obsoleteMod}\\remove.txt", "");
            File.WriteAllText($"{obsoleteMod}\\update.txt", "old text");
            File.WriteAllText($"{obsoleteMod}\\modinfo.json", "{\"ModID\": \"mod1\"}");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // verify obsolete target
            const string targetMod = "tmp\\target\\[a] mod1";
            Assert.False(Directory.Exists(targetMod));

            // create source
            const string sourceMod = "tmp\\source\\[a] mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\add.txt", "");
            File.WriteAllText($"{sourceMod}\\update.txt", "new text");
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"ModID\": \"mod1\"}");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // verify results
            target.AddAsync(source).Wait();
            Assert.True(File.Exists($"{targetMod}\\add.txt"));
            Assert.False(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("new text", File.ReadAllText($"{targetMod}\\update.txt"));
            Assert.Equal(ModStatus.Updated, target.Mods.First(x => x.FolderName == "[a] mod1").Status);
            // obsolete mod is unmodified but disabled
            const string disabledMod = "tmp\\target\\-[a] mod1 oldname";
            Assert.False(File.Exists($"{disabledMod}\\add.txt"));
            Assert.True(File.Exists($"{disabledMod}\\remove.txt"));
            Assert.Equal("old text", File.ReadAllText($"{disabledMod}\\update.txt"));
            Assert.Equal(ModStatus.Obsolete, target.Mods.First(x => x.FolderName == "[a] mod1 oldname").Status);
        }

        /// <summary>
        /// Overwrite active mod from inactive source.
        /// </summary>
        [Fact]
        public void Add_OverwriteActiveModFromInactive()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\[a] mod1";
            Directory.CreateDirectory(targetMod);
            File.WriteAllText($"{targetMod}\\remove.txt", "");
            File.WriteAllText($"{targetMod}\\update.txt", "old text");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // verify target
            Assert.False(File.Exists($"{targetMod}\\add.txt"));
            Assert.True(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("old text", File.ReadAllText($"{targetMod}\\update.txt"));

            // create source
            const string sourceMod = "tmp\\source\\-[a] mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\add.txt", "");
            File.WriteAllText($"{sourceMod}\\update.txt", "new text");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should be overwritten
            target.AddAsync(source).Wait();
            Assert.True(File.Exists($"{targetMod}\\add.txt"));
            Assert.False(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("new text", File.ReadAllText($"{targetMod}\\update.txt"));
            Assert.Equal(ModStatus.Updated, target.Mods.First().Status);
        }

        /// <summary>
        /// Same folder, different ModID: rename folder
        /// </summary>
        [Fact]
        public void Add_SameFolderDifferentModID()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\[a] mod1";
            Directory.CreateDirectory(targetMod);
            File.WriteAllText($"{targetMod}\\modinfo.json", "{\"ModID\": \"mod1\"}");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // create source
            const string sourceMod = "tmp\\source\\[a] mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"ModID\": \"mod2\"}");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should be same, and new mod added with a different name
            target.AddAsync(source).Wait();
            Assert.Equal("{\"ModID\": \"mod1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));
            Assert.Equal(ModStatus.Default, target.Mods.First(x => x.Modinfo.ModID == "mod1").Status);
            Assert.Equal("{\"ModID\": \"mod2\"}", File.ReadAllText($"{targetMod}-1\\modinfo.json"));
            Assert.Equal(ModStatus.New, target.Mods.First(x => x.Modinfo.ModID == "mod2").Status);
        }
    }
}
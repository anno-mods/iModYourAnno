using System.IO;
using System;
using System.Threading.Tasks;
using Xunit;
using Imya.Models;
using Imya.Utils;
using System.Linq;
using Imya.Models.Attributes;

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
            Assert.Equal(ModStatus.Default, target.Mods.First().GetStatusAttribute()?.Status);
        }

        /// <summary>
        /// Loading of mods from a non-existant folder shall lead to empty collection.
        /// </summary>
        [Fact]
        public void LoadMods_InvalidPath()
        {
            DirectoryEx.EnsureDeleted("tmp");

            Assert.False(Directory.Exists("tmp\\asdf"));
            var col = new ModCollection("tmp\\asdf");
            col.LoadModsAsync().Wait();
            Assert.Empty(col.Mods);
        }

        /// <summary>
        /// Create mod folder if it doesn't exist yet.
        /// </summary>
        [Fact]
        public void MoveInto_CreateModFolder()
        {
            DirectoryEx.EnsureDeleted("tmp");

            Assert.False(Directory.Exists("tmp\\mods"));
            var col = new ModCollection("tmp\\mods");
            col.LoadModsAsync().Wait();
            Assert.Empty(col.Mods);

            Directory.CreateDirectory("tmp\\source");
            var empty = new ModCollection("tmp\\source");
            empty.LoadModsAsync().Wait();
            col.MoveIntoAsync(empty).Wait();

            // and another time to ensure double creation isn't a problem
            Directory.CreateDirectory("tmp\\source");
            empty = new ModCollection("tmp\\source");
            empty.LoadModsAsync().Wait();
            col.MoveIntoAsync(empty).Wait();
        }

        /// <summary>
        /// Add new mods.
        /// </summary>
        [Fact]
        public void MoveInto_NewMods()
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
            target.MoveIntoAsync(source).Wait();
            Assert.True(File.Exists($"{activeTargetMod}\\add.txt"));
            Assert.True(File.Exists($"{inactiveTargetMod}\\add.txt"));
            Assert.Equal(ModStatus.New, target.Mods[0].GetStatusAttribute()?.Status);
            Assert.Equal(ModStatus.New, target.Mods[1].GetStatusAttribute()?.Status);
        }

        /// <summary>
        /// Overwrite mod. Delete old files.
        /// </summary>
        [Fact]
        public void MoveInto_OverwriteActiveMod()
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
            target.MoveIntoAsync(source).Wait();
            Assert.True(File.Exists($"{targetMod}\\add.txt"));
            Assert.False(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("new text", File.ReadAllText($"{targetMod}\\update.txt"));
            Assert.Equal(ModStatus.Updated, target.Mods.First().GetStatusAttribute()?.Status);
        }

        /// <summary>
        /// Overwrite mod but keep inactive state.
        /// </summary>
        [Fact]
        public void MoveInto_OverwriteInactive()
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
            target.MoveIntoAsync(source).Wait();
            Assert.True(File.Exists($"{targetMod}\\add.txt"));
            Assert.False(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("new text", File.ReadAllText($"{targetMod}\\update.txt"));
            Assert.Equal(ModStatus.Updated, target.Mods.First().GetStatusAttribute()?.Status);
        }

        /// <summary>
        /// Deactivate old folder name, place mod with new folder name.
        /// </summary>
        [Fact]
        public void MoveInto_UpdateWithDifferentFolder()
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
            target.MoveIntoAsync(source).Wait();
            Assert.True(File.Exists($"{targetMod}\\add.txt"));
            Assert.False(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("new text", File.ReadAllText($"{targetMod}\\update.txt"));
            Assert.Equal(ModStatus.Updated, target.Mods.First(x => x.FolderName == "[a] mod1").GetStatusAttribute()?.Status);
            // obsolete mod is unmodified but disabled
            const string disabledMod = "tmp\\target\\-[a] mod1 oldname";
            Assert.False(File.Exists($"{disabledMod}\\add.txt"));
            Assert.True(File.Exists($"{disabledMod}\\remove.txt"));
            Assert.Equal("old text", File.ReadAllText($"{disabledMod}\\update.txt"));
            Assert.Equal(ModStatus.Obsolete, target.Mods.First(x => x.FolderName == "[a] mod1 oldname").GetStatusAttribute()?.Status);
        }

        /// <summary>
        /// Overwrite active mod from inactive source.
        /// </summary>
        [Fact]
        public void MoveInto_OverwriteActiveModFromInactive()
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
            target.MoveIntoAsync(source).Wait();
            Assert.True(File.Exists($"{targetMod}\\add.txt"));
            Assert.False(File.Exists($"{targetMod}\\remove.txt"));
            Assert.Equal("new text", File.ReadAllText($"{targetMod}\\update.txt"));
            Assert.Equal(ModStatus.Updated, target.Mods.First().GetStatusAttribute()?.Status);
        }

        /// <summary>
        /// Same folder, different ModID: rename folder
        /// </summary>
        [Fact]
        public void MoveInto_SameFolderDifferentModID()
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
            target.MoveIntoAsync(source).Wait();
            Assert.Equal("{\"ModID\": \"mod1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));
            Assert.Equal(ModStatus.Default, target.Mods.First(x => x.Modinfo.ModID == "mod1").GetStatusAttribute()?.Status);
            Assert.Equal("{\"ModID\": \"mod2\"}", File.ReadAllText($"{targetMod}-1\\modinfo.json"));
            Assert.Equal(ModStatus.New, target.Mods.First(x => x.Modinfo.ModID == "mod2").GetStatusAttribute()?.Status);
        }

        /// <summary>
        /// Don't update when source version is older or source is null and target not.
        /// </summary>
        [Fact]
        public void MoveInto_SkipVersionPatterns()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\mod1";
            Directory.CreateDirectory(targetMod);
            File.WriteAllText($"{targetMod}\\modinfo.json", "{\"Version\": \"1.0.1\"}");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // create source
            const string sourceMod = "tmp\\source\\mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"Version\": \"1\"}");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should not be overwritten
            target.MoveIntoAsync(source).Wait();
            Assert.Equal(ModStatus.Default, target.Mods.First().GetStatusAttribute()?.Status);
            Assert.Equal("{\"Version\": \"1.0.1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));

            // create source without version
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"Version\": null}");
            source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should not be overwritten
            target.MoveIntoAsync(source).Wait();
            Assert.Equal(ModStatus.Default, target.Mods.First().GetStatusAttribute()?.Status);
            Assert.Equal("{\"Version\": \"1.0.1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));
        }

        /// <summary>
        /// Update when source version is newer.
        /// </summary>
        [Fact]
        public void MoveInto_UpdateVersionPatterns()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\mod1";
            Directory.CreateDirectory(targetMod);
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // create source
            const string sourceMod = "tmp\\source\\mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"Version\": \"1\"}");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should be overwritten
            target.MoveIntoAsync(source).Wait();
            Assert.Equal(ModStatus.Updated, target.Mods.First().GetStatusAttribute()?.Status);
            Assert.Equal("{\"Version\": \"1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));

            // create source
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"Version\": \"1.0.1\"}");
            source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should be overwritten
            target.MoveIntoAsync(source).Wait();
            Assert.Equal(ModStatus.Updated, target.Mods.First().GetStatusAttribute()?.Status);
            Assert.Equal("{\"Version\": \"1.0.1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));
        }

        /// <summary>
        /// Force update when source version is older or source is null and target not.
        /// </summary>
        [Fact]
        public void MoveInto_ForceVersionPatterns()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\mod1";
            Directory.CreateDirectory(targetMod);
            File.WriteAllText($"{targetMod}\\modinfo.json", "{\"Version\": \"1.0.1\"}");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // create source
            const string sourceMod = "tmp\\source\\mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"Version\": \"1\"}");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should not be overwritten
            target.MoveIntoAsync(source, AllowOldToOverwrite: true).Wait();
            Assert.Equal(ModStatus.Updated, target.Mods.First().GetStatusAttribute()?.Status);
            Assert.Equal("{\"Version\": \"1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));

            // create source without version
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"Version\": null}");
            source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should not be overwritten
            target.MoveIntoAsync(source, AllowOldToOverwrite: true).Wait();
            Assert.Equal(ModStatus.Updated, target.Mods.First().GetStatusAttribute()?.Status);
            Assert.Equal("{\"Version\": null}", File.ReadAllText($"{targetMod}\\modinfo.json"));
        }

        /// <summary>
        /// Update when versions are same, but content changed.
        /// </summary>
        [Fact]
        public void MoveInto_SameVersionDifferentContent()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\mod1";
            Directory.CreateDirectory(targetMod);
            File.WriteAllText($"{targetMod}\\modinfo.json", "{\"Version\": \"1\"}");
            File.WriteAllText($"{targetMod}\\changed.txt", "old");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // create source
            const string sourceMod = "tmp\\source\\mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"Version\": \"1\"}");
            File.WriteAllText($"{sourceMod}\\changed.txt", "new");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should be overwritten
            target.MoveIntoAsync(source).Wait();
            Assert.Equal(ModStatus.Updated, target.Mods.First().GetStatusAttribute()?.Status);
            Assert.Equal("{\"Version\": \"1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));
            Assert.Equal("new", File.ReadAllText($"{targetMod}\\changed.txt"));
        }

        /// <summary>
        /// Don't update when versions and content are same.
        /// </summary>
        [Fact]
        public void MoveInto_SameVersionSameContent()
        {
            DirectoryEx.EnsureDeleted("tmp");

            // create target
            const string targetMod = "tmp\\target\\mod1";
            Directory.CreateDirectory(targetMod);
            File.WriteAllText($"{targetMod}\\modinfo.json", "{\"Version\": \"1\"}");
            File.WriteAllText($"{targetMod}\\unchanged.txt", "unchanged");
            var target = new ModCollection("tmp\\target");
            target.LoadModsAsync().Wait();

            // create source
            const string sourceMod = "tmp\\source\\mod1";
            Directory.CreateDirectory(sourceMod);
            File.WriteAllText($"{sourceMod}\\modinfo.json", "{\"Version\": \"1\"}");
            File.WriteAllText($"{sourceMod}\\unchanged.txt", "unchanged");
            var source = new ModCollection("tmp\\source");
            source.LoadModsAsync().Wait();

            // target should not be overwritten
            target.MoveIntoAsync(source).Wait();
            Assert.Equal(ModStatus.Default, target.Mods.First().GetStatusAttribute()?.Status);
            Assert.Equal("{\"Version\": \"1\"}", File.ReadAllText($"{targetMod}\\modinfo.json"));
            Assert.Equal("unchanged", File.ReadAllText($"{targetMod}\\unchanged.txt"));
        }
    }
}
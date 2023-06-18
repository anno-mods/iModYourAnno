using Imya.Models;
using Imya.Models.Attributes;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Imya.UnitTests
{
    public class SortOrderTests
    {
        /*
        [Fact]
        public void LoadOrder_NormalDependency()
        {
            DirectoryEx.EnsureDeleted("mods");

            //Mod1 
            const string mod1 = "mods\\mod1";
            Directory.CreateDirectory(mod1);
            File.WriteAllText($"{mod1}\\modinfo.json", "{\"ModID\": \"Mod1\", \"Version\": \"1\", \"LoadAfterIds\": [\"Mod2\"]}");

            //Mod2: Last in folder
            const string mod2 = "mods\\zmod2";
            Directory.CreateDirectory(mod2);
            File.WriteAllText($"{mod2}\\modinfo.json", "{\"ModID\": \"Mod2\", \"Version\": \"1\"}");

            var target = new ModCollection("mods");
            target.LoadModsAsync().Wait();

            var sorted = target.Mods.ToList().OrderBy(x => x, ComparebyLoadOrder.Default).ToArray();

            Assert.True(sorted[0].ModID == "Mod2");
            Assert.True(sorted[1].ModID == "Mod1");
        }

        [Fact]
        public void LoadOrder_WildcardDependency()
        {
            DirectoryEx.EnsureDeleted("mods");

            //Mod1 
            const string mod1 = "mods\\mod1";
            Directory.CreateDirectory(mod1);
            File.WriteAllText($"{mod1}\\modinfo.json", "{\"ModID\": \"Mod1\", \"Version\": \"1\", \"LoadAfterIds\": [\"*\"]}");

            //Mod2: Last in folder
            const string mod2 = "mods\\zmod2";
            Directory.CreateDirectory(mod2);
            File.WriteAllText($"{mod2}\\modinfo.json", "{\"ModID\": \"Mod2\", \"Version\": \"1\"}");

            var target = new ModCollection("mods");
            target.LoadModsAsync().Wait();

            var sorted = target.Mods.ToList().OrderBy(x => x, ComparebyLoadOrder.Default).ToArray();

            Assert.True(sorted[0].ModID == "Mod2");
            Assert.True(sorted[1].ModID == "Mod1");
        }

        [Fact]
        public void LoadOrder_ThreeMods()
        {
            DirectoryEx.EnsureDeleted("mods");

            //Mod1 
            const string mod1 = "mods\\mod1";
            Directory.CreateDirectory(mod1);
            File.WriteAllText($"{mod1}\\modinfo.json", "{\"ModID\": \"Mod1\", \"Version\": \"1\", \"LoadAfterIds\": [\"*\"]}");

            //Mod2: Last in folder
            const string mod2 = "mods\\zmod2";
            Directory.CreateDirectory(mod2);
            File.WriteAllText($"{mod2}\\modinfo.json", "{\"ModID\": \"Mod2\", \"Version\": \"1\"}");

            const string mod3 = "mods\\mod3";
            Directory.CreateDirectory(mod3);
            File.WriteAllText($"{mod3}\\modinfo.json", "{\"ModID\": \"Mod3\", \"Version\": \"1\", \"LoadAfterIds\": [\"Mod2\"]}");

            var target = new ModCollection("mods");
            target.LoadModsAsync().Wait();

            var sorted = target.Mods.ToList().OrderBy(x => x, ComparebyLoadOrder.Default).ToArray();

            Assert.True(sorted[0].ModID == "Mod2");
            Assert.True(sorted[1].ModID == "Mod3");
            Assert.True(sorted[2].ModID == "Mod1");
        }
        */
    }
}

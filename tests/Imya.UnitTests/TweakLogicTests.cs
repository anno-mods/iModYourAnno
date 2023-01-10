using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Imya.Models.ModTweaker;
using System.IO;
using Imya.Utils;
using System.Xml;

namespace Imya.UnitTests
{
    public class TweakLogicTests
    {
        public TweakLogicTests()
        {
            //we need to register our gamepath, else we get exceptions thrown all over the place.
            GameSetupManager gameSetupManager = GameSetupManager.Instance;
            gameSetupManager.SetGamePath("");
        }

        [Fact]
        public void CorrectlyEnsuresSkip()
        {
            const String AssetsXML =
                "<ModOps>" +
                    "<ModOp Path=\"/SomePath\" Type=\"add\" ModOpID=\"SomeID\">" +
                        "<Text>Some Text</Text>" +
                    "</ModOp>" +
                    "<ImyaExpose Path = \"self::Text\" ModOpID = \"SomeID\" ExposeID = \"Text\" />" +
                "</ModOps>";

            const String IncludeXML = "<ModOp Path=\"/SomePath\" Type=\"add\" ModOpID=\"SomeID\" Skip=\"1\">";

            InitWorkingDirectory();
            LoadAssets(AssetsXML);

            TweakerFile.TryInit("tweak_tmp", "assets.xml", TweakStorageShelf.Global.Get("TestStorage"), out var tweakerFile);
            //assets.xml only has one value
            tweakerFile.Save("tweak_tmp");

            XmlDocument AssetsWithExpectedSkip = new XmlDocument();
            AssetsWithExpectedSkip.Load("tweak_tmp/assets.xml");
            var node = AssetsWithExpectedSkip.SelectSingleNode("/ModOps/ModOp");

            Assert.Equal("ModOp", node?.Name);
            String? Path = null;
            String? Type = null;
            String? ModOpID = null;
            String? Skip = null;
            Assert.True(node?.TryGetAttribute("Path", out Path));
            Assert.Equal("/SomePath", Path);
            Assert.True(node?.TryGetAttribute("Type", out Type));
            Assert.Equal("add", Type);
            Assert.True(node?.TryGetAttribute("ModOpID", out ModOpID));
            Assert.Equal("SomeID", ModOpID);
            Assert.True(node?.TryGetAttribute("Skip", out Skip));
            Assert.Equal("1", Skip);
        }

        [Fact]
        public void CorrectlyEnsuresNoSkip()
        {
            const string assetsXML =
                "<ModOps>" +
                    "<ModOp Path=\"/SomePath\" Type=\"add\" ModOpID=\"SomeID\" Skip=\"1\">" +
                        "<Text>Some Text</Text>" +
                    "</ModOp>" +
                    "<ImyaExpose Path=\"self::Text\" ModOpID=\"SomeID\" ExposeID=\"Text\" Kind=\"SkipToggle\" />" +
                "</ModOps>";

            InitWorkingDirectory();
            LoadAssets(assetsXML);

            TweakerFile.TryInit("tweak_tmp", "assets.xml", TweakStorageShelf.Global.Get("TestStorage"), out var tweakerFile);
            var toggleVal = tweakerFile.Exposes.First() as ExposedToggleModValue;
            toggleVal!.IsTrue = true; // dont skip on true
            tweakerFile.Save("tweak_tmp");

            XmlDocument AssetsWithExpectedSkip = new();
            AssetsWithExpectedSkip.Load("tweak_tmp/assets.xml");
            var node = AssetsWithExpectedSkip.SelectSingleNode("/ModOps/ModOp");
            Assert.Equal("ModOp", node?.Name);

            string? path = null;
            Assert.True(node?.TryGetAttribute("Path", out path));
            Assert.Equal("/SomePath", path);

            Assert.False(node?.TryGetAttribute("Skip", out _));
        }

        [Fact]
        public void CorrectlyEnsuresSkipInverted()
        {
            const string assetsXML =
                "<ModOps>" +
                    "<ModOp Path=\"/SomePath\" Type=\"add\" ModOpID=\"SomeID\">" +
                        "<Text>Some Text</Text>" +
                    "</ModOp>" +
                    "<ImyaExpose Path=\"self::Text\" ModOpID=\"SomeID\" ExposeID=\"Text\" Kind=\"SkipToggle\" Invert=\"True\" />" +
                "</ModOps>";

            InitWorkingDirectory();
            LoadAssets(assetsXML);

            TweakerFile.TryInit("tweak_tmp", "assets.xml", TweakStorageShelf.Global.Get("TestStorage"), out var tweakerFile);
            var toggleVal = tweakerFile.Exposes.First() as ExposedToggleModValue;
            toggleVal!.IsTrue = true; // skip on true (inverted)
            tweakerFile.Save("tweak_tmp");

            XmlDocument AssetsWithExpectedSkip = new();
            AssetsWithExpectedSkip.Load("tweak_tmp/assets.xml");
            var node = AssetsWithExpectedSkip.SelectSingleNode("/ModOps/ModOp");
            Assert.Equal("ModOp", node?.Name);

            string? path = null;
            Assert.True(node?.TryGetAttribute("Path", out path));
            Assert.Equal("/SomePath", path);

            Assert.True(node?.TryGetAttribute("Skip", out _));
        }

        [Fact]
        public void CorrectlyEnsuresInclude()
        {
            const String AssetsXML =
                "<ModOps>" +
                    "<ModOp Path=\"/SomePath\" Type=\"add\" ModOpID=\"SomeID\">" +
                        "<Text>Some Text</Text>" +
                    "</ModOp>" +
                    "<ImyaExpose Path = \"self::Text\" ModOpID = \"SomeID\" ExposeID = \"Text\" />" +
                "</ModOps>";

            const String IncludeXML = "<Include File=\"./assets.imyatweak.include.xml\" />";

            InitWorkingDirectory();
            LoadAssets(AssetsXML);

            TweakerFile.TryInit("tweak_tmp", "assets.xml", TweakStorageShelf.Global.Get("TestStorage"), out var tweakerFile);
            //assets.xml only has one value
            tweakerFile.Save("tweak_tmp");

            XmlDocument AssetsWithExpectedInclude = new XmlDocument();
            AssetsWithExpectedInclude.Load("tweak_tmp/assets.xml");
            var node = AssetsWithExpectedInclude.SelectSingleNode("/ModOps/Include");

            Assert.Equal(IncludeXML, node?.OuterXml);
        }

        [Fact]
        public void InnerXmlValueChanges()
        {
            const String AssetsXML_XmlReplace =
            "<ModOps>" +
                "<ModOp Path=\"/SomePath\" Type=\"add\" ModOpID=\"SomeID\">" +
                    "<Text>" +
                        "<GUID>12345</GUID>" +
                        "<Text>Some Text</Text>" +
                    "</Text>" +
                "</ModOp>" +
                "<ImyaExpose Path = \"self::Text\" ModOpID = \"SomeID\" ExposeID = \"Text\" Kind=\"Toggle\">" +
                    "<AltValue>" +
                        "<Standard>" +
                            "<GUID>1337</GUID>" +
                        "</Standard>" +
                    "</AltValue>" +
                "</ImyaExpose>" +
            "</ModOps>";

            InitWorkingDirectory();

            LoadAssets(AssetsXML_XmlReplace);

            TweakerFile.TryInit("tweak_tmp", "assets.xml", TweakStorageShelf.Global.Get("TestStorage"), out var tweakerFile);
            //assets.xml only has one value
            var toggleVal = tweakerFile.Exposes.First() as ExposedToggleModValue;
            toggleVal!.IsTrue = false;
            tweakerFile.Save("tweak_tmp");

            XmlDocument ChangedAssets = new XmlDocument();
            ChangedAssets.Load("tweak_tmp/assets.imyatweak.include.xml");
            var node = ChangedAssets.SelectSingleNode("/ModOps/ModOp/Standard/GUID");

            Assert.Equal("1337", node?.InnerText);
        }

        [Fact]
        public void InnerTextValueChanges()
        {
            const String AssetsXML_TextReplace =
                "<ModOps>" +
                    "<ModOp Path=\"/SomePath\" Type=\"add\" ModOpID=\"SomeID\">" +
                        "<Text>Some Text</Text>" +
                    "</ModOp>" +
                    "<ImyaExpose Path = \"self::Text\" ModOpID = \"SomeID\" ExposeID = \"Text\" />" +
                "</ModOps>";

            InitWorkingDirectory(); 

            String NewValue = "Some other Text";
            LoadAssets(AssetsXML_TextReplace);

            TweakerFile.TryInit("tweak_tmp", "assets.xml", TweakStorageShelf.Global.Get("TestStorage"), out var tweakerFile);
            //assets.xml only has one value
            tweakerFile.Exposes.First().Value = NewValue;
            tweakerFile.Save("tweak_tmp");

            XmlDocument ChangedAssets = new XmlDocument();
            ChangedAssets.Load("tweak_tmp/assets.imyatweak.include.xml");
            var node = ChangedAssets.SelectSingleNode("/ModOps/ModOp/Text");

            Assert.Equal(NewValue, node?.InnerText);
        }

        [Fact]
        public void DoesNothingOnWrongPath()
        {
            const String AssetsXML_TextReplace =
                "<ModOps>" +
                    "<ModOp Path=\"/SomePath\" Type=\"add\" ModOpID=\"SomeID\">" +
                        "<Text>Some Text</Text>" +
                    "</ModOp>" +
                    "<ImyaExpose Path = \"self::NotThere\" ModOpID = \"SomeID\" ExposeID = \"Text\" />" +
                "</ModOps>";

            const String ImyaTweakXML =
                "<ModOps>" +
                    "<ModOp Path=\"/SomePath\" Type=\"add\">" +
                        "<Text>Some Text</Text>" +
                    "</ModOp>" +
                "</ModOps>";

            InitWorkingDirectory();

            LoadAssets(AssetsXML_TextReplace);

            TweakerFile.TryInit("tweak_tmp", "assets.xml", TweakStorageShelf.Global.Get("TestStorage"), out var tweakerFile);
            //assets.xml only has one value
            tweakerFile.Save("tweak_tmp");

            XmlDocument ChangedAssets = new XmlDocument();
            ChangedAssets.Load("tweak_tmp/assets.imyatweak.include.xml");
            var node = ChangedAssets.FirstChild;

            Assert.Equal(ImyaTweakXML, node?.OuterXml);
        }


        /// <summary>
        /// Creates a clean structure of assets.xml and assets.imyatweak.include.xml to work on.
        /// </summary>
        private void InitWorkingDirectory()
        {
            DirectoryEx.EnsureDeleted("tweak_tmp");
            Directory.CreateDirectory("tweak_tmp");
            File.Create("tweak_tmp/assets.xml").Close();
            File.Create("tweak_tmp/assets.imyatweak.include.xml").Close();
        }

        private void LoadAssets(String AssetsXML)
        {
            XmlDocument Assets = new XmlDocument();
            Assets.LoadXml(AssetsXML);
            Assets.Save("tweak_tmp/assets.xml");
        }
    }
}

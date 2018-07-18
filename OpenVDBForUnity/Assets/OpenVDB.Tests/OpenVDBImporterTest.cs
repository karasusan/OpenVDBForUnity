using NUnit.Framework;
using System.IO;
using UnityEditor;
using OpenVDB.PackageBuild;

namespace OpenVDB.Tests
{
    public class OpenVDBImporterTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LibraryDownloader.Run();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            LibraryDownloader.Clear();
        }

        [SetUp]
        public void SetUp()
        {
            OpenVDBAPI.oiInitialize();
        }

        [TearDown]
        public void TearDown()
        {
            OpenVDBAPI.oiUninitialize();
        }

        [Test]
        public void ImportVDBAssetPasses()
        {
            AssetDatabase.ImportAsset(Const.VDBSampleFilePath);
        }

        [Test, Ignore("This test cannot pass")]
        public void ImporterIsOpenVDBImporter()
        {
            // This code return null because Const.VDBSampleFilePath is not in Assets folder
            var importer = AssetImporter.GetAtPath(Const.VDBSampleFilePath);
            Assert.NotNull(importer);
            Assert.True(importer is OpenVDBAssetImporter);
        }

        [Test, Ignore("This test cannot pass")]
        public void OpenVDBImporterImportSettingsPasses()
        {
            var importer = AssetImporter.GetAtPath(Const.VDBSampleFilePath) as OpenVDBAssetImporter;
            Assert.NotNull(importer);
            Assert.NotNull(importer.streamSettings);
            Assert.NotZero(importer.streamSettings.scaleFactor);
        }
    }
}

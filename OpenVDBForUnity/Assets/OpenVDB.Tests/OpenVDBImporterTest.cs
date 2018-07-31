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

        // Unknown Error on Unity Cloud Build.
        //
        // Unhandled log message: '[Error] Platform Mac OS X 10.12.6 with device OpenGLCore is not supported, no rendering will occur'. Use UnityEngine.TestTools.LogAssert.Expect
        // UnityEditor.AssetDatabase:ImportAsset(String) OpenVDB.Tests.OpenVDBImporterTest:ImportVDBAssetPasses() (at Assets/OpenVDB.Tests/OpenVDBImporterTest.cs:37)
        //
        /*
        [Test, Ignore("This test cannot pass on Unity Cloud Build (Local Build is no problem)")]
        public void ImportVDBAssetPasses()
        {
            AssetDatabase.ImportAsset(Const.VDBSampleFilePath);
        }

        [Test, Ignore("This test cannot pass on UnityEditor 2018.2.0f2")]
        public void ImporterIsOpenVDBImporter()
        {
            // This code return null because Const.VDBSampleFilePath is not in Assets folder
            var importer = AssetImporter.GetAtPath(Const.VDBSampleFilePath);
            Assert.NotNull(importer);
            Assert.True(importer is OpenVDBAssetImporter);
        }

        [Test, Ignore("This test cannot pass on UnityEditor 2018.2.0f2")]
        public void OpenVDBImporterImportSettingsPasses()
        {
            var importer = AssetImporter.GetAtPath(Const.VDBSampleFilePath) as OpenVDBAssetImporter;
            Assert.NotNull(importer);
            Assert.NotNull(importer.streamSettings);
            Assert.NotZero(importer.streamSettings.scaleFactor);
        }
        */
    }
}

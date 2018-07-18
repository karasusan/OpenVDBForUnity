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

        static string CopyFileToTempFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(Const.AssetTempPath))
            {
                AssetDatabase.CreateFolder(Path.GetDirectoryName(Const.AssetTempPath), Path.GetFileName(Const.AssetTempPath));
            }
            var destPath = Path.Combine(Const.AssetTempPath, Path.GetFileName(path));
            var succeed = AssetDatabase.CopyAsset(path, destPath);
            if(succeed)
                return destPath;
            return null;
        }

        static void DeleteTempFolder()
        {
            AssetDatabase.DeleteAsset(Const.AssetTempPath);
        }
        [Test]
        public void ImporterIsOpenVDBImporter()
        {
            // This code return null because Const.VDBSampleFilePath is not in Assets folder
            // var importer = AssetImporter.GetAtPath(Const.VDBSampleFilePath);
            // 

            var path = CopyFileToTempFolder(Const.VDBSampleFilePath);

            var importer = AssetImporter.GetAtPath(path) as OpenVDBImporter;
            Assert.NotNull(importer);
            Assert.True(importer is OpenVDBImporter);

            DeleteTempFolder();
        }

        [Test]
        public void OpenVDBImporterImportSettingsPasses()
        {
            var path = CopyFileToTempFolder(Const.VDBSampleFilePath);

            var importer = AssetImporter.GetAtPath(path) as OpenVDBImporter;
            Assert.NotNull(importer);
            Assert.NotNull(importer.streamSettings);
            Assert.NotZero(importer.streamSettings.scaleFactor);

            DeleteTempFolder();
        }
    }
}

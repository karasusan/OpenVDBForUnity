using NUnit.Framework;
using UnityEditor;

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
    }
}

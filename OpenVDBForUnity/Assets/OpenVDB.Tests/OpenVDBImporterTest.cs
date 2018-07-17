using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

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

        [Test]
        public void ContextCreatePasses()
        {
            //var vdbFilePath = UnityEditor.PackageManager.Client.
            // UnityEditor.AssetDatabase.ImportAsset();
        }
    }
}

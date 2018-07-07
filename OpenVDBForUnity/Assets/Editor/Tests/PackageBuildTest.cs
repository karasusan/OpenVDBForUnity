using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor;
using System.IO;

namespace OpenVDB.Tests
{
    public class PackageBuildTest
    {
        [Test, Ignore("Fix library deployment")]
        public void LibraryDownloadPasses()
        {
            Assert.IsTrue(LibraryDownloader.Run());
        }

        [Test]
        public void PackageUploaderUploadPasses()
        {
            var tempPath = FileUtil.GetUniqueTempPathInProject();
            File.WriteAllText(tempPath, "");
            PackageUploader.Upload(tempPath);
        }

        [Test]
        public void PackageBuilderBuildPasses()
        {
            var tempPath = FileUtil.GetUniqueTempPathInProject();

            Directory.CreateDirectory(tempPath);
            var info = PackageBuilder.Build(tempPath);
            Assert.True(info.succeed);
        }
    }
}

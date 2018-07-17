using NUnit.Framework;
using UnityEditor;
using System.IO;

namespace OpenVDB.Tests
{
    public class PackageBuildTest
    {
        [Test]
        public void LibraryDownloadPasses()
        {
            Assert.IsTrue(LibraryDownloader.Run());
        }

        [Test]
        public void CopyPasses()
        {
            var tempPath = FileUtil.GetUniqueTempPathInProject();
            File.WriteAllText(tempPath, "");
            PackageBuilder.Copy(tempPath);
        }

        [Test]
        public void BuildPasses()
        {
            var tempPath = FileUtil.GetUniqueTempPathInProject();

            Directory.CreateDirectory(tempPath);
            var info = PackageBuilder.Build(tempPath);
            Assert.True(info.succeed);
        }
    }
}

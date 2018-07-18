using NUnit.Framework;
using UnityEditor;
using System.IO;
using OpenVDB.PackageBuild;

namespace OpenVDB.Tests
{
    public class PackageBuildTest
    {
        [Test]
        public void LibraryDownloaderRunPasses()
        {
            LibraryDownloader.Clear();
            Assert.IsTrue(LibraryDownloader.Run());
        }

        [Test]
        public void PackageBuilderCopyPasses()
        {
            var tempPath = FileUtil.GetUniqueTempPathInProject();
            File.WriteAllText(tempPath, "");
            PackageBuilder.Copy(tempPath);
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

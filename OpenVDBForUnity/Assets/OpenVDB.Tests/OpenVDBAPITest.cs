using NUnit.Framework;
using UnityEngine;
using OpenVDB.PackageBuild;

namespace OpenVDB.Tests
{
    public class OpenVDBAPITest
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
        public void ContextCreatePasses()
        {
            var obj = new GameObject();
            var ctx = oiContext.Create(obj.GetInstanceID());
            Assert.NotNull(ctx);
            Assert.AreNotEqual(System.IntPtr.Zero, ctx.self);
            Object.DestroyImmediate(obj);
        }

        [Test]
        public void ContextDestroyPasses()
        {
            var obj = new GameObject();
            var ctx = oiContext.Create(obj.GetInstanceID());
            ctx.Destroy();
            Assert.AreEqual(System.IntPtr.Zero, ctx.self);
            Object.DestroyImmediate(obj);
        }

        [Test]
        public void ContextLoadPasses()
        {
            var obj = new GameObject();
            var ctx = oiContext.Create(obj.GetInstanceID());
            Assert.True(ctx.Load(Const.VDBSampleFilePath));
            Assert.NotNull(ctx.volume);

            oiVolumeSummary summary = default(oiVolumeSummary);
            ctx.volume.GetSummary(ref summary);
            Assert.NotZero(summary.width);
            Assert.NotZero(summary.height);
            Assert.NotZero(summary.depth);
            Assert.NotZero(summary.format);
            Assert.NotZero(summary.voxelCount);
            Object.DestroyImmediate(obj);
        }

        [Test]
        public void ContextSetConfigPasses()
        {
            var obj = new GameObject();
            var ctx = oiContext.Create(obj.GetInstanceID());
            Assert.True(ctx.Load(Const.VDBSampleFilePath));

            var conf = new oiConfig();
            ctx.SetConfig(ref conf);
            Object.DestroyImmediate(obj);
        }

        [Test]
        public void VolumeFillDataPasses()
        {
            var obj = new GameObject();
            var ctx = oiContext.Create(obj.GetInstanceID());
            ctx.Load(Const.VDBSampleFilePath);

            oiVolumeSummary summary = default(oiVolumeSummary);
            ctx.volume.GetSummary(ref summary);

            var volumeData = default(oiVolumeData);
            var list = new PinnedList<Color>(new Color[summary.voxelCount]);

            volumeData.voxels = list;
            ctx.volume.FillData(ref volumeData);
        }

    }
}

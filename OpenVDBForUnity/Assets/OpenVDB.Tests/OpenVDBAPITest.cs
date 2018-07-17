using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

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
            var obj = new UnityEngine.GameObject();
            var ctx = oiContext.Create(obj.GetInstanceID());
            Assert.NotNull(ctx);
            UnityEngine.Object.DestroyImmediate(obj);
        }
    }
}

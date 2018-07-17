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

        [Test]
        public void InitializeSimplePasses()
        {
            OpenVDBAPI.oiInitialize();
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}

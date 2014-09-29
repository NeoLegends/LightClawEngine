using System;
using System.Linq;
using LightClaw.Engine.Core;
using LightClaw.Engine.Coroutines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Core
{
    [TestClass]
    public class GameObjectTest
    {
        [TestMethod]
        public void GameObjectComponentAttachment()
        {
            GameObject gameObject = new GameObject(new Transform());
            gameObject.Add(new CoroutineController());

            Assert.AreEqual(2, gameObject.Count, "The count was not two, so the item was not added properly.");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void GameObjectComponentAttachmentValidators()
        {
            GameObject gameObject = new GameObject(new Transform());
            gameObject.Add(new Transform());

            Assert.AreEqual(1, gameObject.Count, "Count was not one, as it should've been as adding another transform shouldn't be possible.");
        }

        [TestMethod]
        public void GameObjectComponentRemoval()
        {
            CoroutineController controller = new CoroutineController();
            GameObject gameObject = new GameObject(controller);

            Assert.AreEqual(gameObject.Count, 2, "Initial count was not two, as it should've been.");

            gameObject.Remove(controller);

            Assert.AreEqual(gameObject.Count, 1, "Count after removal wasn't one, as it should've been (as only the Transform should be left attached).");
        }

        [TestMethod]
        public void GameObjectComponentRemovalValidators()
        {
            Transform t = new Transform();
            GameObject gameObject = new GameObject(t);
            Assert.IsFalse(gameObject.Remove(t), "Component was successfully removed, this is not the expected behaviour.");
        }
    }
}

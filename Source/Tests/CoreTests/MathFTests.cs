using System;
using LightClaw.Engine.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreTests
{
    [TestClass]
    public class MathFTests
    {
        [TestMethod]
        public void TestGreatestCommonDivisor()
        {
            Assert.IsTrue(MathF.GreatestCommonDivisor(new[] { 6, 12, 24 }) == 6);

            Assert.IsFalse(MathF.GreatestCommonDivisor(new[] { 6, 18, 36 }) > 6);
            Assert.IsFalse(MathF.GreatestCommonDivisor(new[] { 6, 18, 36 }) < 6);
        }

        [TestMethod]
        public void TestIsAlmostZero()
        {
            Assert.IsTrue(MathF.IsAlmostZero(0.0));
            Assert.IsTrue(MathF.IsAlmostZero(0.00001, 2));

            Assert.IsFalse(MathF.IsAlmostZero(1));
        }

        [TestMethod]
        public void TestIsDivisorOf()
        {
            Assert.IsTrue(MathF.IsDivisorOf(10, 5));
            Assert.IsFalse(MathF.IsDivisorOf(10, 3));

            Assert.IsTrue(MathF.IsDivisorOf(10.0, 2.0));
            Assert.IsFalse(MathF.IsDivisorOf(10.0, 3.0));
        }

        [TestMethod]
        public void TestLeastCommonMultiple()
        {
            Assert.AreEqual(24, MathF.LeastCommonMultiple(new[] { 6, 12, 24 }));

            Assert.IsFalse(MathF.LeastCommonMultiple(new[] { 6, 18, 36 }) < 36);
            Assert.IsFalse(MathF.LeastCommonMultiple(new[] { 6, 18, 36 }) > 36);
        }

        [TestMethod]
        public void TestNextPowerOfTwo()
        {
            Assert.AreEqual(1024, (int)MathF.NextPowerOfTwo(513));

            Assert.AreNotEqual(1024, (int)MathF.NextPowerOfTwo(512));
        }
    }
}

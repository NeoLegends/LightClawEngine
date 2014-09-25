using System;
using LightClaw.Engine.Graphics;
using LightClaw.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Graphics
{
    [TestClass]
    public class ColorTest
    {
        [TestMethod]
        public void ColorSaveAndLoadHexString()
        {
            string colorAsHex = Color.Blue.HexCode;
            Color loadedFromHex = new Color(colorAsHex);

            Console.WriteLine("Values from hex - R: {0}, G: {1}, B: {2}, A: {3}".FormatWith(loadedFromHex.R, loadedFromHex.G, loadedFromHex.B, loadedFromHex.A));
            Assert.AreEqual(loadedFromHex.B, byte.MaxValue);
        }

        [TestMethod]
        public void ColorGrayScales()
        {
            Color red = Color.Red;

            Color grayScaleAverage = red.GrayScaleAverage;
            Color grayScaleLightness = red.GrayScaleLightness;
            Color grayScaleLuminosity = red.GrayScaleLuminosity;

            Console.WriteLine("Grayscale average: " + grayScaleAverage.ToString());
            Assert.AreEqual(grayScaleAverage.R, grayScaleAverage.G);
            Assert.AreEqual(grayScaleAverage.G, grayScaleAverage.B);

            Console.WriteLine("Grayscale lightness: " + grayScaleLightness.ToString());
            Assert.AreEqual(grayScaleLightness.R, grayScaleLightness.G);
            Assert.AreEqual(grayScaleLightness.G, grayScaleLightness.B);

            Console.WriteLine("Grayscale luminosity: " + grayScaleLuminosity.ToString());
            Assert.AreEqual(grayScaleLuminosity.R, grayScaleLuminosity.G);
            Assert.AreEqual(grayScaleLuminosity.G, grayScaleLuminosity.B);
        }
    }
}

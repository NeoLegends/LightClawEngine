using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Class that represents a color with RGBA data.
    /// </summary>
    [DataContract, ProtoContract]
    [StructureInformation(4, 1, false)]
    public partial struct Color : ICloneable,
#if SYSTEMDRAWING_INTEROP
                                  IEquatable<System.Drawing.Color>,
#endif
                                  IEquatable<Color>
    {
        /// <summary>
        /// Gets the size in bytes of the <see cref="Color"/>-struct.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Color));

        #region Predefined Colors

        /// <summary>
        /// Zero color.
        /// </summary>
        public static readonly Color Zero = Color.FromBgra(0x00000000);

        /// <summary>
        /// Transparent color.
        /// </summary>
        public static readonly Color Transparent = Color.FromBgra(0x00000000);

        /// <summary>
        /// AliceBlue color.
        /// </summary>
        public static readonly Color AliceBlue = Color.FromBgra(0xFFF0F8FF);

        /// <summary>
        /// AntiqueWhite color.
        /// </summary>
        public static readonly Color AntiqueWhite = Color.FromBgra(0xFFFAEBD7);

        /// <summary>
        /// Aqua color.
        /// </summary>
        public static readonly Color Aqua = Color.FromBgra(0xFF00FFFF);

        /// <summary>
        /// Aquamarine color.
        /// </summary>
        public static readonly Color Aquamarine = Color.FromBgra(0xFF7FFFD4);

        /// <summary>
        /// Azure color.
        /// </summary>
        public static readonly Color Azure = Color.FromBgra(0xFFF0FFFF);

        /// <summary>
        /// Beige color.
        /// </summary>
        public static readonly Color Beige = Color.FromBgra(0xFFF5F5DC);

        /// <summary>
        /// Bisque color.
        /// </summary>
        public static readonly Color Bisque = Color.FromBgra(0xFFFFE4C4);

        /// <summary>
        /// Black color.
        /// </summary>
        public static readonly Color Black = Color.FromBgra(0xFF000000);

        /// <summary>
        /// BlanchedAlmond color.
        /// </summary>
        public static readonly Color BlanchedAlmond = Color.FromBgra(0xFFFFEBCD);

        /// <summary>
        /// Blue color.
        /// </summary>
        public static readonly Color Blue = Color.FromBgra(0xFF0000FF);

        /// <summary>
        /// BlueViolet color.
        /// </summary>
        public static readonly Color BlueViolet = Color.FromBgra(0xFF8A2BE2);

        /// <summary>
        /// Brown color.
        /// </summary>
        public static readonly Color Brown = Color.FromBgra(0xFFA52A2A);

        /// <summary>
        /// BurlyWood color.
        /// </summary>
        public static readonly Color BurlyWood = Color.FromBgra(0xFFDEB887);

        /// <summary>
        /// CadetBlue color.
        /// </summary>
        public static readonly Color CadetBlue = Color.FromBgra(0xFF5F9EA0);

        /// <summary>
        /// Chartreuse color.
        /// </summary>
        public static readonly Color Chartreuse = Color.FromBgra(0xFF7FFF00);

        /// <summary>
        /// Chocolate color.
        /// </summary>
        public static readonly Color Chocolate = Color.FromBgra(0xFFD2691E);

        /// <summary>
        /// Coral color.
        /// </summary>
        public static readonly Color Coral = Color.FromBgra(0xFFFF7F50);

        /// <summary>
        /// CornflowerBlue color.
        /// </summary>
        public static readonly Color CornflowerBlue = Color.FromBgra(0xFF6495ED);

        /// <summary>
        /// Cornsilk color.
        /// </summary>
        public static readonly Color Cornsilk = Color.FromBgra(0xFFFFF8DC);

        /// <summary>
        /// Crimson color.
        /// </summary>
        public static readonly Color Crimson = Color.FromBgra(0xFFDC143C);

        /// <summary>
        /// Cyan color.
        /// </summary>
        public static readonly Color Cyan = Color.FromBgra(0xFF00FFFF);

        /// <summary>
        /// DarkBlue color.
        /// </summary>
        public static readonly Color DarkBlue = Color.FromBgra(0xFF00008B);

        /// <summary>
        /// DarkCyan color.
        /// </summary>
        public static readonly Color DarkCyan = Color.FromBgra(0xFF008B8B);

        /// <summary>
        /// DarkGoldenrod color.
        /// </summary>
        public static readonly Color DarkGoldenrod = Color.FromBgra(0xFFB8860B);

        /// <summary>
        /// DarkGray color.
        /// </summary>
        public static readonly Color DarkGray = Color.FromBgra(0xFFA9A9A9);

        /// <summary>
        /// DarkGreen color.
        /// </summary>
        public static readonly Color DarkGreen = Color.FromBgra(0xFF006400);

        /// <summary>
        /// DarkKhaki color.
        /// </summary>
        public static readonly Color DarkKhaki = Color.FromBgra(0xFFBDB76B);

        /// <summary>
        /// DarkMagenta color.
        /// </summary>
        public static readonly Color DarkMagenta = Color.FromBgra(0xFF8B008B);

        /// <summary>
        /// DarkOliveGreen color.
        /// </summary>
        public static readonly Color DarkOliveGreen = Color.FromBgra(0xFF556B2F);

        /// <summary>
        /// DarkOrange color.
        /// </summary>
        public static readonly Color DarkOrange = Color.FromBgra(0xFFFF8C00);

        /// <summary>
        /// DarkOrchid color.
        /// </summary>
        public static readonly Color DarkOrchid = Color.FromBgra(0xFF9932CC);

        /// <summary>
        /// DarkRed color.
        /// </summary>
        public static readonly Color DarkRed = Color.FromBgra(0xFF8B0000);

        /// <summary>
        /// DarkSalmon color.
        /// </summary>
        public static readonly Color DarkSalmon = Color.FromBgra(0xFFE9967A);

        /// <summary>
        /// DarkSeaGreen color.
        /// </summary>
        public static readonly Color DarkSeaGreen = Color.FromBgra(0xFF8FBC8B);

        /// <summary>
        /// DarkSlateBlue color.
        /// </summary>
        public static readonly Color DarkSlateBlue = Color.FromBgra(0xFF483D8B);

        /// <summary>
        /// DarkSlateGray color.
        /// </summary>
        public static readonly Color DarkSlateGray = Color.FromBgra(0xFF2F4F4F);

        /// <summary>
        /// DarkTurquoise color.
        /// </summary>
        public static readonly Color DarkTurquoise = Color.FromBgra(0xFF00CED1);

        /// <summary>
        /// DarkViolet color.
        /// </summary>
        public static readonly Color DarkViolet = Color.FromBgra(0xFF9400D3);

        /// <summary>
        /// DeepPink color.
        /// </summary>
        public static readonly Color DeepPink = Color.FromBgra(0xFFFF1493);

        /// <summary>
        /// DeepSkyBlue color.
        /// </summary>
        public static readonly Color DeepSkyBlue = Color.FromBgra(0xFF00BFFF);

        /// <summary>
        /// DimGray color.
        /// </summary>
        public static readonly Color DimGray = Color.FromBgra(0xFF696969);

        /// <summary>
        /// DodgerBlue color.
        /// </summary>
        public static readonly Color DodgerBlue = Color.FromBgra(0xFF1E90FF);

        /// <summary>
        /// Firebrick color.
        /// </summary>
        public static readonly Color Firebrick = Color.FromBgra(0xFFB22222);

        /// <summary>
        /// FloralWhite color.
        /// </summary>
        public static readonly Color FloralWhite = Color.FromBgra(0xFFFFFAF0);

        /// <summary>
        /// ForestGreen color.
        /// </summary>
        public static readonly Color ForestGreen = Color.FromBgra(0xFF228B22);

        /// <summary>
        /// Fuchsia color.
        /// </summary>
        public static readonly Color Fuchsia = Color.FromBgra(0xFFFF00FF);

        /// <summary>
        /// Gainsboro color.
        /// </summary>
        public static readonly Color Gainsboro = Color.FromBgra(0xFFDCDCDC);

        /// <summary>
        /// GhostWhite color.
        /// </summary>
        public static readonly Color GhostWhite = Color.FromBgra(0xFFF8F8FF);

        /// <summary>
        /// Gold color.
        /// </summary>
        public static readonly Color Gold = Color.FromBgra(0xFFFFD700);

        /// <summary>
        /// Goldenrod color.
        /// </summary>
        public static readonly Color Goldenrod = Color.FromBgra(0xFFDAA520);

        /// <summary>
        /// Gray color.
        /// </summary>
        public static readonly Color Gray = Color.FromBgra(0xFF808080);

        /// <summary>
        /// Green color.
        /// </summary>
        public static readonly Color Green = Color.FromBgra(0xFF008000);

        /// <summary>
        /// GreenYellow color.
        /// </summary>
        public static readonly Color GreenYellow = Color.FromBgra(0xFFADFF2F);

        /// <summary>
        /// Honeydew color.
        /// </summary>
        public static readonly Color Honeydew = Color.FromBgra(0xFFF0FFF0);

        /// <summary>
        /// HotPink color.
        /// </summary>
        public static readonly Color HotPink = Color.FromBgra(0xFFFF69B4);

        /// <summary>
        /// IndianRed color.
        /// </summary>
        public static readonly Color IndianRed = Color.FromBgra(0xFFCD5C5C);

        /// <summary>
        /// Indigo color.
        /// </summary>
        public static readonly Color Indigo = Color.FromBgra(0xFF4B0082);

        /// <summary>
        /// Ivory color.
        /// </summary>
        public static readonly Color Ivory = Color.FromBgra(0xFFFFFFF0);

        /// <summary>
        /// Khaki color.
        /// </summary>
        public static readonly Color Khaki = Color.FromBgra(0xFFF0E68C);

        /// <summary>
        /// Lavender color.
        /// </summary>
        public static readonly Color Lavender = Color.FromBgra(0xFFE6E6FA);

        /// <summary>
        /// LavenderBlush color.
        /// </summary>
        public static readonly Color LavenderBlush = Color.FromBgra(0xFFFFF0F5);

        /// <summary>
        /// LawnGreen color.
        /// </summary>
        public static readonly Color LawnGreen = Color.FromBgra(0xFF7CFC00);

        /// <summary>
        /// LemonChiffon color.
        /// </summary>
        public static readonly Color LemonChiffon = Color.FromBgra(0xFFFFFACD);

        /// <summary>
        /// LightBlue color.
        /// </summary>
        public static readonly Color LightBlue = Color.FromBgra(0xFFADD8E6);

        /// <summary>
        /// LightCoral color.
        /// </summary>
        public static readonly Color LightCoral = Color.FromBgra(0xFFF08080);

        /// <summary>
        /// LightCyan color.
        /// </summary>
        public static readonly Color LightCyan = Color.FromBgra(0xFFE0FFFF);

        /// <summary>
        /// LightGoldenrodYellow color.
        /// </summary>
        public static readonly Color LightGoldenrodYellow = Color.FromBgra(0xFFFAFAD2);

        /// <summary>
        /// LightGray color.
        /// </summary>
        public static readonly Color LightGray = Color.FromBgra(0xFFD3D3D3);

        /// <summary>
        /// LightGreen color.
        /// </summary>
        public static readonly Color LightGreen = Color.FromBgra(0xFF90EE90);

        /// <summary>
        /// LightPink color.
        /// </summary>
        public static readonly Color LightPink = Color.FromBgra(0xFFFFB6C1);

        /// <summary>
        /// LightSalmon color.
        /// </summary>
        public static readonly Color LightSalmon = Color.FromBgra(0xFFFFA07A);

        /// <summary>
        /// LightSeaGreen color.
        /// </summary>
        public static readonly Color LightSeaGreen = Color.FromBgra(0xFF20B2AA);

        /// <summary>
        /// LightSkyBlue color.
        /// </summary>
        public static readonly Color LightSkyBlue = Color.FromBgra(0xFF87CEFA);

        /// <summary>
        /// LightSlateGray color.
        /// </summary>
        public static readonly Color LightSlateGray = Color.FromBgra(0xFF778899);

        /// <summary>
        /// LightSteelBlue color.
        /// </summary>
        public static readonly Color LightSteelBlue = Color.FromBgra(0xFFB0C4DE);

        /// <summary>
        /// LightYellow color.
        /// </summary>
        public static readonly Color LightYellow = Color.FromBgra(0xFFFFFFE0);

        /// <summary>
        /// Lime color.
        /// </summary>
        public static readonly Color Lime = Color.FromBgra(0xFF00FF00);

        /// <summary>
        /// LimeGreen color.
        /// </summary>
        public static readonly Color LimeGreen = Color.FromBgra(0xFF32CD32);

        /// <summary>
        /// Linen color.
        /// </summary>
        public static readonly Color Linen = Color.FromBgra(0xFFFAF0E6);

        /// <summary>
        /// Magenta color.
        /// </summary>
        public static readonly Color Magenta = Color.FromBgra(0xFFFF00FF);

        /// <summary>
        /// Maroon color.
        /// </summary>
        public static readonly Color Maroon = Color.FromBgra(0xFF800000);

        /// <summary>
        /// MediumAquamarine color.
        /// </summary>
        public static readonly Color MediumAquamarine = Color.FromBgra(0xFF66CDAA);

        /// <summary>
        /// MediumBlue color.
        /// </summary>
        public static readonly Color MediumBlue = Color.FromBgra(0xFF0000CD);

        /// <summary>
        /// MediumOrchid color.
        /// </summary>
        public static readonly Color MediumOrchid = Color.FromBgra(0xFFBA55D3);

        /// <summary>
        /// MediumPurple color.
        /// </summary>
        public static readonly Color MediumPurple = Color.FromBgra(0xFF9370DB);

        /// <summary>
        /// MediumSeaGreen color.
        /// </summary>
        public static readonly Color MediumSeaGreen = Color.FromBgra(0xFF3CB371);

        /// <summary>
        /// MediumSlateBlue color.
        /// </summary>
        public static readonly Color MediumSlateBlue = Color.FromBgra(0xFF7B68EE);

        /// <summary>
        /// MediumSpringGreen color.
        /// </summary>
        public static readonly Color MediumSpringGreen = Color.FromBgra(0xFF00FA9A);

        /// <summary>
        /// MediumTurquoise color.
        /// </summary>
        public static readonly Color MediumTurquoise = Color.FromBgra(0xFF48D1CC);

        /// <summary>
        /// MediumVioletRed color.
        /// </summary>
        public static readonly Color MediumVioletRed = Color.FromBgra(0xFFC71585);

        /// <summary>
        /// MidnightBlue color.
        /// </summary>
        public static readonly Color MidnightBlue = Color.FromBgra(0xFF191970);

        /// <summary>
        /// MintCream color.
        /// </summary>
        public static readonly Color MintCream = Color.FromBgra(0xFFF5FFFA);

        /// <summary>
        /// MistyRose color.
        /// </summary>
        public static readonly Color MistyRose = Color.FromBgra(0xFFFFE4E1);

        /// <summary>
        /// Moccasin color.
        /// </summary>
        public static readonly Color Moccasin = Color.FromBgra(0xFFFFE4B5);

        /// <summary>
        /// NavajoWhite color.
        /// </summary>
        public static readonly Color NavajoWhite = Color.FromBgra(0xFFFFDEAD);

        /// <summary>
        /// Navy color.
        /// </summary>
        public static readonly Color Navy = Color.FromBgra(0xFF000080);

        /// <summary>
        /// OldLace color.
        /// </summary>
        public static readonly Color OldLace = Color.FromBgra(0xFFFDF5E6);

        /// <summary>
        /// Olive color.
        /// </summary>
        public static readonly Color Olive = Color.FromBgra(0xFF808000);

        /// <summary>
        /// OliveDrab color.
        /// </summary>
        public static readonly Color OliveDrab = Color.FromBgra(0xFF6B8E23);

        /// <summary>
        /// Orange color.
        /// </summary>
        public static readonly Color Orange = Color.FromBgra(0xFFFFA500);

        /// <summary>
        /// OrangeRed color.
        /// </summary>
        public static readonly Color OrangeRed = Color.FromBgra(0xFFFF4500);

        /// <summary>
        /// Orchid color.
        /// </summary>
        public static readonly Color Orchid = Color.FromBgra(0xFFDA70D6);

        /// <summary>
        /// PaleGoldenrod color.
        /// </summary>
        public static readonly Color PaleGoldenrod = Color.FromBgra(0xFFEEE8AA);

        /// <summary>
        /// PaleGreen color.
        /// </summary>
        public static readonly Color PaleGreen = Color.FromBgra(0xFF98FB98);

        /// <summary>
        /// PaleTurquoise color.
        /// </summary>
        public static readonly Color PaleTurquoise = Color.FromBgra(0xFFAFEEEE);

        /// <summary>
        /// PaleVioletRed color.
        /// </summary>
        public static readonly Color PaleVioletRed = Color.FromBgra(0xFFDB7093);

        /// <summary>
        /// PapayaWhip color.
        /// </summary>
        public static readonly Color PapayaWhip = Color.FromBgra(0xFFFFEFD5);

        /// <summary>
        /// PeachPuff color.
        /// </summary>
        public static readonly Color PeachPuff = Color.FromBgra(0xFFFFDAB9);

        /// <summary>
        /// Peru color.
        /// </summary>
        public static readonly Color Peru = Color.FromBgra(0xFFCD853F);

        /// <summary>
        /// Pink color.
        /// </summary>
        public static readonly Color Pink = Color.FromBgra(0xFFFFC0CB);

        /// <summary>
        /// Plum color.
        /// </summary>
        public static readonly Color Plum = Color.FromBgra(0xFFDDA0DD);

        /// <summary>
        /// PowderBlue color.
        /// </summary>
        public static readonly Color PowderBlue = Color.FromBgra(0xFFB0E0E6);

        /// <summary>
        /// Purple color.
        /// </summary>
        public static readonly Color Purple = Color.FromBgra(0xFF800080);

        /// <summary>
        /// Red color.
        /// </summary>
        public static readonly Color Red = Color.FromBgra(0xFFFF0000);

        /// <summary>
        /// RosyBrown color.
        /// </summary>
        public static readonly Color RosyBrown = Color.FromBgra(0xFFBC8F8F);

        /// <summary>
        /// RoyalBlue color.
        /// </summary>
        public static readonly Color RoyalBlue = Color.FromBgra(0xFF4169E1);

        /// <summary>
        /// SaddleBrown color.
        /// </summary>
        public static readonly Color SaddleBrown = Color.FromBgra(0xFF8B4513);

        /// <summary>
        /// Salmon color.
        /// </summary>
        public static readonly Color Salmon = Color.FromBgra(0xFFFA8072);

        /// <summary>
        /// SandyBrown color.
        /// </summary>
        public static readonly Color SandyBrown = Color.FromBgra(0xFFF4A460);

        /// <summary>
        /// SeaGreen color.
        /// </summary>
        public static readonly Color SeaGreen = Color.FromBgra(0xFF2E8B57);

        /// <summary>
        /// SeaShell color.
        /// </summary>
        public static readonly Color SeaShell = Color.FromBgra(0xFFFFF5EE);

        /// <summary>
        /// Sienna color.
        /// </summary>
        public static readonly Color Sienna = Color.FromBgra(0xFFA0522D);

        /// <summary>
        /// Silver color.
        /// </summary>
        public static readonly Color Silver = Color.FromBgra(0xFFC0C0C0);

        /// <summary>
        /// SkyBlue color.
        /// </summary>
        public static readonly Color SkyBlue = Color.FromBgra(0xFF87CEEB);

        /// <summary>
        /// SlateBlue color.
        /// </summary>
        public static readonly Color SlateBlue = Color.FromBgra(0xFF6A5ACD);

        /// <summary>
        /// SlateGray color.
        /// </summary>
        public static readonly Color SlateGray = Color.FromBgra(0xFF708090);

        /// <summary>
        /// Snow color.
        /// </summary>
        public static readonly Color Snow = Color.FromBgra(0xFFFFFAFA);

        /// <summary>
        /// SpringGreen color.
        /// </summary>
        public static readonly Color SpringGreen = Color.FromBgra(0xFF00FF7F);

        /// <summary>
        /// SteelBlue color.
        /// </summary>
        public static readonly Color SteelBlue = Color.FromBgra(0xFF4682B4);

        /// <summary>
        /// Tan color.
        /// </summary>
        public static readonly Color Tan = Color.FromBgra(0xFFD2B48C);

        /// <summary>
        /// Teal color.
        /// </summary>
        public static readonly Color Teal = Color.FromBgra(0xFF008080);

        /// <summary>
        /// Thistle color.
        /// </summary>
        public static readonly Color Thistle = Color.FromBgra(0xFFD8BFD8);

        /// <summary>
        /// Tomato color.
        /// </summary>
        public static readonly Color Tomato = Color.FromBgra(0xFFFF6347);

        /// <summary>
        /// Turquoise color.
        /// </summary>
        public static readonly Color Turquoise = Color.FromBgra(0xFF40E0D0);

        /// <summary>
        /// Violet color.
        /// </summary>
        public static readonly Color Violet = Color.FromBgra(0xFFEE82EE);

        /// <summary>
        /// Wheat color.
        /// </summary>
        public static readonly Color Wheat = Color.FromBgra(0xFFF5DEB3);

        /// <summary>
        /// White color.
        /// </summary>
        public static readonly Color White = Color.FromBgra(0xFFFFFFFF);

        /// <summary>
        /// WhiteSmoke color.
        /// </summary>
        public static readonly Color WhiteSmoke = Color.FromBgra(0xFFF5F5F5);

        /// <summary>
        /// Yellow color.
        /// </summary>
        public static readonly Color Yellow = Color.FromBgra(0xFFFFFF00);

        /// <summary>
        /// YellowGreen color.
        /// </summary>
        public static readonly Color YellowGreen = Color.FromBgra(0xFF9ACD32);

        /// <summary>
        /// Returns a random color.
        /// </summary>
        public static Color Random
        {
            get
            {
                return new Color(RandomF.GetByte(), RandomF.GetByte(), RandomF.GetByte(), RandomF.GetByte());
            }
        }

        #endregion

        /// <summary>
        /// The red <see cref="Color"/> value.
        /// </summary>
        [DataMember, ProtoMember(1)]
        public byte R { get; private set; }

        /// <summary>
        /// The green <see cref="Color"/> value.
        /// </summary>
        [DataMember, ProtoMember(2)]
        public byte G { get; private set; }

        /// <summary>
        /// The blue <see cref="Color"/> value.
        /// </summary>
        [DataMember, ProtoMember(3)]
        public byte B { get; private set; }

        /// <summary>
        /// Represents the alpha value of the color.
        /// </summary>
        [DataMember, ProtoMember(4)]
        public byte A { get; private set; }

        #region Calculated Properties

        /// <summary>
        /// This <see cref="Color"/> as gray scale using the luminosity method.
        /// </summary>
        /// <remarks>0.2125f * R + 0.7154f * G + 0.0721f * B.</remarks>
        public Color GrayScaleLuminosity
        {
            get
            {
                return new Color((byte)(0.2125f * this.R + 0.7154f * this.G + 0.0721f * this.B), this.A);
            }
        }

        /// <summary>
        /// This <see cref="Color"/> as gray scale using the average method.
        /// </summary>
        /// <remarks>(R + G + B) / 3.</remarks>
        public Color GrayScaleAverage
        {
            get
            {
                return new Color((byte)((this.R + this.G + this.B) / 3), this.A);
            }
        }

        /// <summary>
        /// This <see cref="Color"/> as gray scale using the lightness method.
        /// </summary>
        /// <remarks>Max(R, Max(G, B)) / 2.</remarks>
        public Color GrayScaleLightness
        {
            get
            {
                return new Color((byte)(Math.Max(this.R, Math.Max(this.G, this.B)) / 2), this.A);
            }
        }

        /// <summary>
        /// The negated color value.
        /// </summary>
        public Color Negated
        {
            get
            {
                return new Color
                {
                    R = (byte)(255 - this.R),
                    G = (byte)(255 - this.G),
                    B = (byte)(255 - this.B),
                    A = (byte)(255 - this.A)
                };
            }
        }

        /// <summary>
        /// The hexadecimal color code representing this <see cref="Color"/>.
        /// </summary>
        public string HexadecimalColorCode
        {
            get
            {
                return this.ToString();
            }
        }

        /// <summary>
        /// The Brightness of the color (HSB).
        /// </summary>
        public float Brightness
        {
            get
            {
                return GrayScaleLightness.R / 255f;
            }
        }

        /// <summary>
        /// Returns the color's hue (HSB).
        /// </summary>
        public float Hue
        {
            get
            {
                if (R == G && G == B)
                    return 0;

                float r = (float)R / 255.0f;
                float g = (float)G / 255.0f;
                float b = (float)B / 255.0f;

                float max = Math.Max(r, Math.Max(g, b));
                float min = Math.Min(r, Math.Min(g, b));
                float delta = max - min;
                float hue = 0.0f;

                if (r == max)
                    hue = (g - b) / delta;
                else if (g == max)
                    hue = 2 + (b - r) / delta;
                else if (b == max)
                    hue = 4 + (r - g) / delta;
                hue *= 60;

                if (hue < 0.0f)
                    hue += 360.0f;

                return hue;
            }
        }

        /// <summary>
        /// The color's saturation (HSB).
        /// </summary>
        public float Saturation
        {
            get
            {
                float r = (float)R / 255.0f;
                float g = (float)G / 255.0f;
                float b = (float)B / 255.0f;

                float max = Math.Max(r, Math.Max(g, b));
                float min = Math.Min(r, Math.Min(g, b));
                float l = 0.0f;
                float s = 0;

                // If max == min, then there is no color and the saturation is zero
                if (max != min)
                {
                    l = (max + min) / 2;
                    s = (l <= 0.5) ? (max - min) / (max + min) : (max - min) / (2 - max - min);
                }

                return s;
            }
        }

        /// <summary>
        /// Returns the color as packed RGBA-Integer.
        /// </summary>
        public int PackedRgba
        {
            get
            {
                return ((R << 24) | (G << 16) | (B << 8) | (A));
            }
        }

        #endregion

        /// <summary>
        /// All components contained in an array.
        /// </summary>
        public byte[] Array
        {
            get
            {
                Contract.Ensures(Contract.Result<byte[]>() != null);

                return new byte[] { this.R, this.G, this.B, this.A };
            }
        }

        /// <summary>
        /// Allows accessing the color values through an index.
        /// </summary>
        /// <param name="index">The component at the given index.</param>
        /// <returns>The desired color component.</returns>
        public byte this[int index]
        {
            get
            {
                Contract.Requires<IndexOutOfRangeException>(index >= 0 && index < 4);

                switch (index)
                {
                    case 0:
                        return this.R;
                    case 1:
                        return this.G;
                    case 2:
                        return this.B;
                    case 3:
                        return this.A;
                    default:
                        throw new IndexOutOfRangeException("Index must be greater than or equal to zero and smaller than four.");
                }
            }
        }

#if SYSTEMDRAWING_INTEROP

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to create the <see cref="Color"/> from</param>
        public Color(System.Drawing.Color color) : this(color.R, color.G, color.B, color.A) { }

#endif

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vec">The <see cref="Vector3"/> to create the <see cref="Color"/> from.</param>
        public Color(Vector3 vec) : this(ToByte(vec.X), ToByte(vec.Y), ToByte(vec.Z)) { }

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="Vector4"/>.
        /// </summary>
        /// <param name="vec">The <see cref="Vector4"/> to create the <see cref="Color"/> from.</param>
        public Color(Vector4 vec) : this(ToByte(vec.X), ToByte(vec.Y), ToByte(vec.Z), ToByte(vec.W)) { }

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given packed RGBA <see cref="Int32"/>.
        /// </summary>
        /// <param name="packedValue">The <see cref="System.Int32"/> containing the packed color values.</param>
        public Color(int packedValue)
            : this(
                (byte)((packedValue >> 24) & byte.MaxValue),
                (byte)((packedValue >> 16) & byte.MaxValue),
                (byte)((packedValue >> 8) & byte.MaxValue),
                (byte)(packedValue & byte.MaxValue)
            )
        { }

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="Single"/>.
        /// </summary>
        /// <param name="value">The <see cref="Single"/> to create the <see cref="Color"/> from.</param>
        public Color(float value) : this(ToByte(value)) { }

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="Byte"/>.
        /// </summary>
        /// <param name="value">The <see cref="Byte"/> to create the <see cref="Color"/> from.</param>
        public Color(byte value) : this(value, value, value) { }

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="Byte"/>.
        /// </summary>
        /// <param name="value">The <see cref="Byte"/> to create the <see cref="Color"/> from.</param>
        /// <param name="alpha">The <see cref="Color"/>'s alpha.</param>
        public Color(float value, float alpha) : this(ToByte(value), ToByte(value), ToByte(value), ToByte(alpha)) { }

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="Byte"/>.
        /// </summary>
        /// <param name="value">The <see cref="Byte"/> to create the <see cref="Color"/> from.</param>
        /// <param name="alpha">The <see cref="Color"/>'s alpha.</param>
        public Color(byte value, byte alpha) : this(value, value, value, alpha) { }

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="Single"/>s.
        /// </summary>
        /// <param name="r">Red color value.</param>
        /// <param name="g">Green color value.</param>
        /// <param name="b">Blue color Value.</param>
        public Color(float r, float g, float b) : this(ToByte(r), ToByte(g), ToByte(b)) { }

        /// <summary>
        /// Initializes a new instance of class <see cref="Color"/> and sets the alpha to 255 (opaque).
        /// </summary>
        /// <param name="r">Red color value.</param>
        /// <param name="g">Green color value.</param>
        /// <param name="b">Blue color Value.</param>
        public Color(byte r, byte g, byte b) : this(r, g, b, 255) { }

        /// <summary>
        /// Creates a new <see cref="Color"/> from the given <see cref="Single"/>s.
        /// </summary>
        /// <param name="r">Red color value.</param>
        /// <param name="g">Green color value.</param>
        /// <param name="b">Blue color Value.</param>
        /// <param name="a">Alpha value.</param>
        public Color(float r, float g, float b, float a) : this(ToByte(r), ToByte(g), ToByte(b), ToByte(a)) { }

        /// <summary>
        /// Initializes a new instance of class <see cref="Color"/>.
        /// </summary>
        /// <param name="r">Red color value.</param>
        /// <param name="g">Green color value.</param>
        /// <param name="b">Blue color Value.</param>
        /// <param name="a">Alpha value.</param>
        public Color(byte r, byte g, byte b, byte a)
            : this()
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        /// <summary>
        /// Returns a new deeply cloned instance.
        /// </summary>
        /// <returns>Returns a new color instance with the same color values.</returns>
        public object Clone()
        {
            return new Color(this.R, this.G, this.B, this.A);
        }

        /// <summary>
        /// Checks whether the current instance equals the given <see cref="Object"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            else if (ReferenceEquals(obj, this))
                return true;

#if SYSTEMDRAWING_INTEROP
            if (obj is System.Drawing.Color)
            {
                return this.Equals((System.Drawing.Color)obj);
            }
#endif
            return (obj is Color) ? this.Equals((Color)obj) : false;
        }

        /// <summary>
        /// Checks whether the current instance equals the specified <see cref="Color"/>.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare to.</param>
        /// <returns><see cref="M:System.IEqatable{Color}.CompareTo"/>.</returns>
        public bool Equals(Color other)
        {
            return ((this.R == other.R) && (this.G == other.G) && (this.B == other.B) && (this.A == other.A));
        }

#if SYSTEMDRAWING_INTEROP

        /// <summary>
        /// Checks whether the current instance equals the specified <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="other">The <see cref="System.Drawing.Color"/> to check.</param>
        /// <returns>A boolean indicating whether the two instances are equal.</returns>
        public bool Equals(System.Drawing.Color other)
        {
            return ((this.R == other.R) && (this.G == other.G) && (this.B == other.B) && (this.A == other.A));
        }

#endif

        /// <summary>
        /// Derived frrom <see cref="Object"/>.
        /// </summary>
        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.R, this.G, this.B, this.A);
        }

        /// <summary>
        /// Converts this <see cref="Color"/> into the representative hex <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> value representing the color values in this <see cref="Color"/>.</returns>
        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);

            return this.ToString(FormatOrder.Rgba);
        }

        /// <summary>
        /// Converts this <see cref="Color"/> into the representative hex <see cref="string"/>.
        /// </summary>
        /// <param name="order">The order in which to format the output.</param>
        /// <returns>A <see cref="string"/> value representing the color values in this <see cref="Color"/>.</returns>
        public string ToString(FormatOrder order)
        {
            Contract.Ensures(Contract.Result<string>() != null);

            switch (order)
            {
                case FormatOrder.Argb:
                    return MathF.HexTable[this.A] + MathF.HexTable[this.R] + MathF.HexTable[this.G] + MathF.HexTable[this.B];
                case FormatOrder.Bgra:
                    return MathF.HexTable[this.B] + MathF.HexTable[this.G] + MathF.HexTable[this.R] + MathF.HexTable[this.A];
                default:
                case FormatOrder.Rgba:
                    return MathF.HexTable[this.R] + MathF.HexTable[this.G] + MathF.HexTable[this.B] + MathF.HexTable[this.A];
            }
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in BGRA order.</param>
        /// <returns>A color.</returns>
        public static Color FromBgra(int color)
        {
            return new Color(
                (byte)((color >> 16) & byte.MaxValue),
                (byte)((color >> 8) & byte.MaxValue),
                (byte)(color & byte.MaxValue), 
                (byte)((color >> 24) & byte.MaxValue)
            );
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in BGRA order.</param>
        /// <returns>A color.</returns>
        public static Color FromBgra(uint color)
        {
            return FromBgra(unchecked((int)color));
        }

        /// <summary>
        /// Converts the color from a packed ABGR integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in ABGR order.</param>
        /// <returns>A color.</returns>
        public static Color FromAbgr(int color)
        {
            return new Color((byte)(color >> 24), (byte)(color >> 16), (byte)(color >> 8), (byte)color);
        }

        /// <summary>
        /// Converts the color from a packed ABGR integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in ABGR order.</param>
        /// <returns>A color.</returns>
        public static Color FromAbgr(uint color)
        {
            return FromAbgr(unchecked((int)color));
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in RGBA order.</param>
        /// <returns>A color.</returns>
        public static Color FromRgba(int color)
        {
            return new Color(color);
        }

        /// <summary>
        /// Converts the given float from range 0 to 1 to a byte.
        /// </summary>
        /// <param name="component">The float to convert.</param>
        /// <returns>The resulting byte value.</returns>
        public static byte ToByte(float component)
        {
            return (byte)(MathF.Clamp(component * 255.0f, 0.0f, 255.0f));
        }

        /// <summary>
        /// Converts the specified <see cref="Byte"/> from range 0-255 into a <see cref="Single"/> from 0-1.
        /// </summary>
        /// <param name="b">The <see cref="Byte"/> to convert.</param>
        /// <returns>The result.</returns>
        public static float ToFloat(byte b)
        {
            return (b / 255.0f);
        }

        /// <summary>
        /// Checks whether two colors are equal.
        /// </summary>
        public static bool operator ==(Color a, Color b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Checks whether two colors are unequal.
        /// </summary>
        public static bool operator !=(Color a, Color b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Adds two <see cref="Color"/>s.
        /// </summary>
        /// <param name="a">The first <see cref="Color"/>.</param>
        /// <param name="b">The second <see cref="Color"/>.</param>
        /// <returns>The added <see cref="Color"/>s.</returns>
        public static Color operator +(Color a, Color b)
        {
            return new Color
            {
                R = (byte)(a.R + b.R),
                G = (byte)(a.G + b.G),
                B = (byte)(a.B + b.B)
            };
        }

        /// <summary>
        /// Adds a <see cref="Single"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="a">The <see cref="Color"/> to add to.</param>
        /// <param name="b">The <see cref="System.Int32"/> that is being added to the <see cref="Color"/>.</param>
        /// <returns>The <see cref="Color"/> with new values.</returns>
        public static Color operator +(Color a, float b)
        {
            return new Color
            {
                R = (byte)(a.R + b),
                G = (byte)(a.G + b),
                B = (byte)(a.B + b)
            };
        }

        /// <summary>
        /// Substracts one <see cref="Color"/> by another <see cref="Color"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Color"/>.</param>
        /// <param name="b">The second <see cref="Color"/>.</param>
        /// <returns>The first <see cref="Color"/> substracted by the second <see cref="Color"/>.</returns>
        public static Color operator -(Color a, Color b)
        {
            return new Color
            {
                R = (byte)(a.R - b.R),
                G = (byte)(a.G - b.G),
                B = (byte)(a.B - b.B)
            };
        }

        /// <summary>
        /// Substracts one <see cref="Color"/> by an <see cref="Single"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Color"/>.</param>
        /// <param name="b">The <see cref="System.Int32"/> to substract by.</param>
        /// <returns>The first <see cref="Color"/> substracted by the second <see cref="Color"/>.</returns>
        public static Color operator -(Color a, float b)
        {
            return new Color
            {
                R = (byte)(a.R - b),
                G = (byte)(a.G - b),
                B = (byte)(a.B - b)
            };
        }

        /// <summary>
        /// Multiplies one <see cref="Color"/> with another <see cref="Color"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Color"/>.</param>
        /// <param name="b">The second <see cref="Color"/>.</param>
        /// <returns>The <see cref="Color"/> multiplied by the values of the second <see cref="Color"/>.</returns>
        public static Color operator *(Color a, Color b)
        {
            return new Color
            {
                R = (byte)(a.R * b.R),
                G = (byte)(a.G * b.G),
                B = (byte)(a.B * b.B)
            };
        }

        /// <summary>
        /// Multiplies RGB values with a given <see cref="Single"/>.
        /// </summary>
        /// <param name="a">The <see cref="Color"/>.</param>
        /// <param name="b">The <see cref="System.Int32"/> to multiply with.</param>
        /// <returns>The <see cref="Color"/> multiplied by the value of the integer.</returns>
        public static Color operator *(Color a, float b)
        {
            return new Color
            {
                R = (byte)(a.R * b),
                G = (byte)(a.G * b),
                B = (byte)(a.B * b)
            };
        }

        /// <summary>
        /// Explicitly converts the given <see cref="Vector4"/> into a <see cref="Color"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector4"/> to convert.</param>
        /// <returns>The converted <see cref="Color"/>.</returns>
        public static explicit operator Color(Vector4 vector)
        {
            return new Color(vector);
        }

#if SYSTEMDRAWING_INTEROP

        /// <summary>
        /// Implicity converts the given <see cref="Color"/> into a <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="color">The <see cref="Color"/><see cref="System.Drawing.Color"/> to convert</param>
        /// <returns>The converted <see cref="System.Drawing.Color"/></returns>
        public static implicit operator System.Drawing.Color(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Implicity converts the given <see cref="System.Drawing.Color"/> into a <see cref="Color"/>
        /// </summary>
        /// <param name="color">The <see cref="System.Drawing.Color"/> to convert</param>
        /// <returns>The converted <see cref="Color"/></returns>
        public static implicit operator Color(System.Drawing.Color color)
        {
            return new Color(color);
        }

#endif

        public enum FormatOrder : byte
        {
            Rgba,

            Argb,

            Bgra
        }
    }
}
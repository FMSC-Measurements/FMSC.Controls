using System;

using System.Drawing;

namespace FMSC.Controls
{
    public struct HLSColor 
    {
        private const int ShadowAdj = -333;
        private const int HilightAdj = 500;
        private const int WatermarkAdj = -50;
        private const int Range = 240;
        private const int HLSMax = 240;
        private const int RGBMax = 255;
        private const int Undefined = 160;
        private int hue;
        private int saturation;
        private int luminosity;
        private bool isSystemColors_Control;

        public int Hue
        {
            get
            {
                
                return this.hue;
            }
        }

        public int Luminosity
        {
            get
            {
                return this.luminosity;
            }
        }

        public int Saturation
        {
            get
            {
                return this.saturation;
            }
        }

        //http://en.wikipedia.org/wiki/HSL_and_HSV#Formal_derivation
        public HLSColor(Color color)
        {
            this.isSystemColors_Control = (color == SystemColors.Control);
            int R = color.R;
            int G = color.G;
            int B = color.B;

            int maxRGB = Math.Max(R, Math.Max(G, B));
            int minRGB = Math.Min(R, Math.Min(G, B));

            int sumMinMax = maxRGB + minRGB;
            this.luminosity = (sumMinMax * 240 + (int)byte.MaxValue) / 510;
            int chroma = maxRGB - minRGB;
            if (chroma == 0)
            {
                this.saturation = 0;
                this.hue = 160;
            }
            else
            {
                this.saturation = this.luminosity > 120 ? (chroma * 240 + (510 - sumMinMax) / 2) / (510 - sumMinMax) : (chroma * 240 + sumMinMax / 2) / sumMinMax;
                int mathStuff1 = ((maxRGB - R) * 40 + chroma / 2) / chroma;
                int mathStuff2 = ((maxRGB - G) * 40 + chroma / 2) / chroma;
                int mathStuff3 = ((maxRGB - B) * 40 + chroma / 2) / chroma;
                if (maxRGB == R)
                {
                    this.hue = mathStuff3 - mathStuff2;
                }
                else if (maxRGB == G)
                {
                    this.hue = 80 + mathStuff1 - mathStuff3;
                }
                else //maxRGB == B
                {
                    this.hue = 160 + mathStuff2 - mathStuff1;
                }

                if (this.hue < 0)
                    this.hue += 240;
                if (this.hue <= 240)
                    return;
                this.hue -= 240;
            }
        }

        public Color Darker(float percDarker)
        {
            if (this.isSystemColors_Control)
            {
                if ((double)percDarker == 0.0)
                    return SystemColors.ControlDark;
                if ((double)percDarker == 1.0)
                    return SystemColors.ControlDarkDark;
                Color controlDark = SystemColors.ControlDark;
                Color controlDarkDark = SystemColors.ControlDarkDark;
                int num1 = (int)controlDark.R - (int)controlDarkDark.R;
                int num2 = (int)controlDark.G - (int)controlDarkDark.G;
                int num3 = (int)controlDark.B - (int)controlDarkDark.B;
                return Color.FromArgb((int)(byte)((uint)controlDark.R - (uint)(byte)((double)num1 * (double)percDarker)), (int)(byte)((uint)controlDark.G - (uint)(byte)((double)num2 * (double)percDarker)), (int)(byte)((uint)controlDark.B - (uint)(byte)((double)num3 * (double)percDarker)));
            }
            else
            {
                int newL = this.NewLuma(-333, true);
                return this.ColorFromHLS(this.hue, newL - (int)((double)newL * (double)percDarker), this.saturation);
            }
        }

        public override bool Equals(object o)
        {
            if (!(o is HLSColor))
                return false;
            HLSColor hlsColor = (HLSColor)o;
            if (this.hue == hlsColor.hue && this.saturation == hlsColor.saturation && this.luminosity == hlsColor.luminosity)
                return this.isSystemColors_Control == hlsColor.isSystemColors_Control;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.hue << 6 | this.saturation << 2 | this.luminosity;
        }

        public Color Lighter(float percLighter)
        {
            if (this.isSystemColors_Control)
            {
                if ((double)percLighter == 0.0)
                    return SystemColors.ControlLight;
                if ((double)percLighter == 1.0)
                    return SystemColors.ControlLightLight;
                Color controlLight = SystemColors.ControlLight;
                Color controlLightLight = SystemColors.ControlLightLight;
                int num1 = (int)controlLight.R - (int)controlLightLight.R;
                int num2 = (int)controlLight.G - (int)controlLightLight.G;
                int num3 = (int)controlLight.B - (int)controlLightLight.B;
                return Color.FromArgb((int)(byte)((uint)controlLight.R - (uint)(byte)((double)num1 * (double)percLighter)), (int)(byte)((uint)controlLight.G - (uint)(byte)((double)num2 * (double)percLighter)), (int)(byte)((uint)controlLight.B - (uint)(byte)((double)num3 * (double)percLighter)));
            }
            else
            {
                int oldL = this.luminosity;
                int newL = this.NewLuma(500, true);
                return this.ColorFromHLS(this.hue, oldL + (int)((double)(newL - oldL) * (double)percLighter), this.saturation);
            }
        }

        private int NewLuma(int n, bool scale)
        {
            return this.NewLuma(this.luminosity, n, scale);
        }

        private int NewLuma(int luminosity, int n, bool scale)
        {
            if (n == 0)
                return luminosity;
            if (scale)
            {
                if (n > 0)
                    return (int)(((long)(luminosity * (1000 - n)) + 241L * (long)n) / 1000L);
                else
                    return luminosity * (n + 1000) / 1000;
            }
            else
            {
                int num = luminosity + (int)((long)n * 240L / 1000L);
                if (num < 0)
                    num = 0;
                if (num > 240)
                    num = 240;
                return num;
            }
        }

        private Color ColorFromHLS(int hue, int luminosity, int saturation)
        {
            byte num1;
            byte num2;
            byte num3;
            if (saturation == 0)
            {
                int num4;
                num1 = (byte)(num4 = (int)(byte)(luminosity * (int)byte.MaxValue / 240));
                num2 = (byte)num4;
                num3 = (byte)num4;
                if (hue == 160)
                {
                    ;//do nothing...
                }
            }
            else
            {
                int n2 = luminosity > 120 ? luminosity + saturation - (luminosity * saturation + 120) / 240 : (luminosity * (240 + saturation) + 120) / 240;
                int n1 = 2 * luminosity - n2;
                num3 = (byte)((this.HueToRGB(n1, n2, hue + 80) * (int)byte.MaxValue + 120) / 240);
                num2 = (byte)((this.HueToRGB(n1, n2, hue) * (int)byte.MaxValue + 120) / 240);
                num1 = (byte)((this.HueToRGB(n1, n2, hue - 80) * (int)byte.MaxValue + 120) / 240);
            }
            return Color.FromArgb((int)num3, (int)num2, (int)num1);
        }

        private int HueToRGB(int n1, int n2, int hue)
        {
            if (hue < 0)
                hue += 240;
            if (hue > 240)
                hue -= 240;
            if (hue < 40)
                return n1 + ((n2 - n1) * hue + 20) / 40;
            if (hue < 120)
                return n2;
            if (hue < 160)
                return n1 + ((n2 - n1) * (160 - hue) + 20) / 40;
            else
                return n1;
        }
    }
}

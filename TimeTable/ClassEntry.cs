using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TimeTable
{
    public class FirebaseResponse
    {
        public string Key { get; set; }
        public object Object { get; set; }
    }
    public class RootObject
    {
        public string Hash { get; set; }
        public List<ClassEntry> Classes { get; set; }
    }
    public class ClassEntry
    {
        public string Dan { get; set; }
        public string Datum { get; set; }
        public string Ura { get; set; }
        public string Prostor { get; set; }
        public string Opis { get; set; }
        public string Skupina { get; set; }
        public string Izvajalec { get; set; }
        public bool isFirst { get; set; }
        public bool hasOverlap { get; set; }

        public bool Vidno
        {
            get
            {
                return Preferences.Get(Opis + " " + Skupina, true);
            }
            set
            {
                Preferences.Set(Opis + " " + Skupina, value);
            }
        }

        public bool JePredavanje => Opis.Split(' ').Any(t => t.Equals("PR"));

        public string Predmet => String.Join(' ', Opis.Split(' ').Where(t => !(new[] { "SV", "RV", "PR" }).Contains(t)));

        private static double Lerp(double a, double b, double t) => a + (b - a) * t;

        // Compute contrast ratio between white text (#fff) and the provided background color
        // Contrast ratio = (L1 + 0.05) / (L2 + 0.05) where L1 is lighter luminance.
        private static double ContrastRatioWhite(Color bg)
        {
            // relative luminance of background (sRGB -> linear -> weighted sum)
            double lr = SrgbToLinear(bg.Red / 255.0);
            double lg = SrgbToLinear(bg.Green / 255.0);
            double lb = SrgbToLinear(bg.Blue / 255.0);
            double Lbg = 0.2126 * lr + 0.7152 * lg + 0.0722 * lb;

            double Lwhite = 1.0; // white luminance
                                 // L1 is lighter, so for white L1 = 1.0
            return (Lwhite + 0.05) / (Lbg + 0.05);
        }

        // sRGB component (0..1) -> linearized value
        private static double SrgbToLinear(double c)
        {
            if (c <= 0.04045) return c / 12.92;
            return Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        public Color Color
        {
            get
            {
                Color color;
                if (Preferences.ContainsKey("Color_" + Predmet))
                {
                    color = UserClassColor;
                }
                else
                {
                    color = DefaultColor;
                }

                float hue, sat, light;
                color.ToHsl(out hue, out sat, out light);

                // Non-lecture → lighter & less saturated
                if (!JePredavanje)
                {
                    color = Color.FromHsla(hue, Math.Max(0, sat - 0.1), Math.Max(0.2, light - 0.05));
                }
                return color;
            }
            set {
                UserClassColor = value;
            }
        }

        private Color UserClassColor
        {
            get
            {
                return Color.FromArgb(Preferences.Get("Color_" + Predmet, DefaultColor.ToHex()));
            }
            set
            {
                Preferences.Set("Color_" + Predmet, value.ToHex());
            }
        }

        public void ResetColor()
        {
            Preferences.Remove("Color_" + Predmet);
        }

        private Color DefaultColor
        {
            get
            {
                byte[] hash;
                using (var md = MD5.Create())
                {
                    hash = md.ComputeHash(Encoding.UTF8.GetBytes(Predmet));
                }

                // Properly derive hue from 4 bytes → [0, 360)
                uint hVal = BitConverter.ToUInt32(hash, 0);
                double hue = hVal / (double)uint.MaxValue;

                // Derive saturation and lightness from other bytes
                double satFraction = hash[5] / 255.0;
                double sat = Lerp(0.70, 1.00, satFraction);

                double lightFraction = hash[6] / 255.0;
                double light = Lerp(0.30, 0.50, lightFraction);

                // Create color from HSL
                Color color = Color.FromHsla(hue, sat, light);

                // Ensure readable contrast with white
                const double targetContrast = 4.5;
                int safety = 0;
                while (ContrastRatioWhite(color) < targetContrast && safety < 10)
                {
                    light = Math.Max(0.2, light - 0.045);
                    color = Color.FromHsla(hue, sat, light);
                    safety++;
                }

                return color;
            }

        }

    }
}

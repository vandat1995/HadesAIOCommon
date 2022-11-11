using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace HadesAIOCommon.Utils
{
    public class ColorUtils
    {
        public static Color FromMaterialArgb(int argb)
        {
            return Color.FromArgb((argb & 0xFF0000) >> 16, (argb & 0xFF00) >> 8, argb & 0xFF);
        }
    }
}

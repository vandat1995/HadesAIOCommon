using System;
using System.Collections.Generic;
using System.Text;

namespace HadesAIOCommon.WinAPI
{
    public class WindowStyle
    {
        public const int WM_SYSCOMMAND = 0x112;
        public const int SC_MAXIMIZE = 0xF030;

        public const int GWL_STYLE = -16;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_SIZEBOX = 0x00040000;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KMHelper
{
    public struct KeyMSG
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    public struct POINT
    {
        public int x;
        public int y;
    }

    public struct MouseHookStruct
    {
        public POINT pt;
        public int hWnd;
        public int wHitTestCode;
        public int dwExtraInfo;
    }
}

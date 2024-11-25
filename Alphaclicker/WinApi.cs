using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace AlphaClicker
{
    public class WinApi
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        private enum MOUSEEVENTF
        {
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MOVE = 0x0001,
            ABSOLUTE = 0x8000
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int character);

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        public static void DoClick(string button, bool useCustomCoords, int X, int Y)
        {
            if (useCustomCoords)
            {
                SetCursorPos(X, Y);
            }
            else
            {
                // Get current cursor position if not using custom coordinates
                POINT currentPos;
                GetCursorPos(out currentPos);
                X = currentPos.X;
                Y = currentPos.Y;
            }

            switch (button)
            {
                case "Left":
                    mouse_event((uint)MOUSEEVENTF.LEFTDOWN | (uint)MOUSEEVENTF.LEFTUP, (uint)X, (uint)Y, 0, 0);
                    break;
                case "Right":
                    mouse_event((uint)MOUSEEVENTF.RIGHTDOWN | (uint)MOUSEEVENTF.RIGHTUP, (uint)X, (uint)Y, 0, 0);
                    break;
                case "Middle":
                    mouse_event((uint)MOUSEEVENTF.MIDDLEDOWN | (uint)MOUSEEVENTF.MIDDLEUP, (uint)X, (uint)Y, 0, 0);
                    break;
            }
        }
    }
}

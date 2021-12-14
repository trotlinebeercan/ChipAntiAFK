using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ChipAntiAFK
{
    public class KeyCommand
    {
        public static void SendJump(Process process)
        {
            SendSyncKey(process.MainWindowHandle, Keys.Space, 50);
        }

        public static void OpenMainMenu(Process process)
        {
            SendSyncKey(process.MainWindowHandle, Keys.Escape, 250);
            SendSyncKey(process.MainWindowHandle, Keys.Escape, 250);
        }

        private static void SendSyncKey(IntPtr ffxivWindow, Keys key, int waitBetweenKeyUpInMs)
        {
            bool modifier = false;

            // This sleeps the thread intentionally. We are asking the game to take key inputs like a human, and want to insure everything is actually taken in.
            Keys key2 = (key & ~Keys.Control) & (key & ~Keys.Shift) & (key & ~Keys.Alt);

            if (key2 != key) modifier = true;

            if (modifier)
            {
                for (int i = 0; i < 5; i++)
                {
                    if ((key & Keys.Control) == Keys.Control) SendMessage(ffxivWindow, WM_KEYDOWN, ((IntPtr)Keys.ControlKey), ((IntPtr)0));
                    if ((key & Keys.Alt) == Keys.Alt) SendMessage(ffxivWindow, WM_SYSKEYDOWN, ((IntPtr)Keys.Menu), ((IntPtr)0));
                    if ((key & Keys.Shift) == Keys.Shift) SendMessage(ffxivWindow, WM_KEYDOWN, ((IntPtr)Keys.ShiftKey), ((IntPtr)0));

                    Thread.Sleep(5);
                }
            }

            SendMessage(ffxivWindow, WM_KEYDOWN, ((IntPtr)key2), ((IntPtr)0));
            Thread.Sleep(waitBetweenKeyUpInMs);
            SendMessage(ffxivWindow, WM_KEYUP, ((IntPtr)key2), ((IntPtr)0));

            if (modifier)
            {
                if ((key & Keys.Shift) == Keys.Shift)
                {
                    Thread.Sleep(5);
                    SendMessage(ffxivWindow, WM_KEYUP, ((IntPtr)Keys.ShiftKey), ((IntPtr)0));
                }
                if ((key & Keys.Alt) == Keys.Alt)
                {
                    Thread.Sleep(5);
                    SendMessage(ffxivWindow, WM_SYSKEYUP, ((IntPtr)Keys.Menu), ((IntPtr)0));
                }
                if ((key & Keys.Control) == Keys.Control)
                {
                    Thread.Sleep(5);
                    SendMessage(ffxivWindow, WM_KEYUP, ((IntPtr)Keys.ControlKey), ((IntPtr)0));
                }
            }
            Thread.Sleep(50);
        }

        #region Windows Keybind Imports

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        #endregion
    }
}

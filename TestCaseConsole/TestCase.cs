using MouseAndKeyboardSwitch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCaseConsole
{
    public class TestCase
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        private int numberOfWindows = 2;
        private int numberOfWindowsHorizontally = 2;
        private int numberOfWindowsVertically = 1;
        private string proccessName = "mspaint";
        private int displayWidth = 1920;
        private int displayHeight = 1080;
        private MKSwitch mkSwitch = new MKSwitch();

        public void ArrangeTestCaseEnviroment()
        {
            int sleepBetweenOpeningWindows = 500;
            int taskbarHeight = 40;

            var handles = new List<IntPtr>();
            int x = 0;
            int y = 0;

            int currentNumberOfWindows = 0;
            for (int i = 0; i < numberOfWindowsVertically; i++)
            {
                for (int j = 0; j < numberOfWindowsHorizontally; j++)
                {
                    if (currentNumberOfWindows == numberOfWindows)
                        break;

                    var process = Process.Start(proccessName);
                    IntPtr handle = process.MainWindowHandle;
                    Thread.Sleep(sleepBetweenOpeningWindows);
                    SetWindowPos(process.MainWindowHandle, new IntPtr((int)SpecialWindowHandles.HWND_TOP), x, y, displayWidth / numberOfWindowsHorizontally, (displayHeight - taskbarHeight) / numberOfWindowsVertically, SetWindowPosFlags.SWP_SHOWWINDOW);
                    handles.Add(process.MainWindowHandle);
                    mkSwitch.Users.Add(new MouseAndKeyboardSwitch.User { Id = currentNumberOfWindows + 1, ProccessName = proccessName, ProccessWindowHandle = process.MainWindowHandle, XOffset = x, YOffset = y });
                    x += displayWidth / numberOfWindowsHorizontally;

                    currentNumberOfWindows++;
                }

                y += (displayHeight - taskbarHeight) / numberOfWindowsVertically;
                x = 0;
            }
        }

        public void SimulateMouseKeyboardEvents()
        {
            int sleepBetweenEvents = 500;
            //mosue down point A
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(1, UserMouseButton.Left, UserMouseState.Down, 200, 200));
            Thread.Sleep(sleepBetweenEvents);
            //mosue down point B
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(2, UserMouseButton.Left, UserMouseState.Down, 200, 200));

            Thread.Sleep(sleepBetweenEvents);
            //mouse move point C
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(1, UserMouseButton.Left, UserMouseState.Down, 300, 400));
            Thread.Sleep(sleepBetweenEvents);
            //mouse move point D
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(2, UserMouseButton.Left, UserMouseState.Down, 300, 400));

            Thread.Sleep(sleepBetweenEvents);
            //mouse move point E
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(1, UserMouseButton.Left, UserMouseState.Down, 150, 350));
            Thread.Sleep(sleepBetweenEvents);
            //mouse move point F
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(2, UserMouseButton.Left, UserMouseState.Down, 150, 350));

            Thread.Sleep(sleepBetweenEvents);
            //mouse move point A
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(1, UserMouseButton.Left, UserMouseState.Down, 200, 200));
            Thread.Sleep(sleepBetweenEvents);
            //mouse move point B
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(2, UserMouseButton.Left, UserMouseState.Down, 200, 200));

            Thread.Sleep(sleepBetweenEvents);
            //mouse up point A
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(1, UserMouseButton.Left, UserMouseState.Up, 200, 200));
            Thread.Sleep(sleepBetweenEvents);
            //mouse move point B
            mkSwitch.MKSwitch_MouseInput(this, new UserMouseEventArgs(2, UserMouseButton.Left, UserMouseState.Up, 200, 200));
        }
    }
}

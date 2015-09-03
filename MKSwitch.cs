using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MouseAndKeyboardSwitch
{
    public class MKSwitch
    {
        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,
           [MarshalAs(UnmanagedType.LPArray), In] SendInputStructs.INPUT[] pInputs,
           int cbSize);

        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // For Windows Mobile, replace user32.dll with coredll.dll 
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        public delegate void UserMouseEventHandler(object sender, UserMouseEventArgs e);
        public event UserMouseEventHandler MouseInput;

        private List<User> users;
        public List<User> Users 
        {
            get
            {
                return this.users;
            }
            set
            {
                this.users = value;
            }
        }

        
        public MKSwitch()
        {
            this.users = new List<User>();
            this.MouseInput += MKSwitch_MouseInput;
        }

        public void MKSwitch_MouseInput(object sender, UserMouseEventArgs e)
        {
            var user = this.users.FirstOrDefault(u => u.Id == e.UserId);
            if(user != null)
            {
                int x = CalculateAbsoluteCoordinateX(e.X + user.XOffset);
                int y = CalculateAbsoluteCoordinateY(e.Y + user.YOffset);

                var hWindow = user.ProccessWindowHandle;
                SetForegroundWindow(hWindow);
                //SetActiveWindow(hWindow);
                //SetFocus(hWindow);
                //SetCapture(hWindow);
                //SetCursorPos(x, y);

                SendInputEnums.MOUSEEVENTF lastInputFlags = SendInputEnums.MOUSEEVENTF.MOVE | SendInputEnums.MOUSEEVENTF.ABSOLUTE;

                if(user.LastButton == e.Button && user.LastButtonState == UserMouseState.Down)
                {
                    //if last event for this user was with button state down, we need to repeat last send input
                    //Cursor must be brought back to last position (coordinates) and send mouse down input and then send current input
                    int lastX = CalculateAbsoluteCoordinateX(user.LastPositionX + user.XOffset);
                    int lastY = CalculateAbsoluteCoordinateY(user.LastPositionY + user.YOffset);

                    switch (user.LastButton)
                    {
                        case UserMouseButton.Left:
                            lastInputFlags = lastInputFlags | SendInputEnums.MOUSEEVENTF.LEFTDOWN;
                            break;
                        case UserMouseButton.Middle:
                            lastInputFlags = lastInputFlags | SendInputEnums.MOUSEEVENTF.MIDDLEDOWN;
                            break;
                        case UserMouseButton.Right:
                            lastInputFlags = lastInputFlags | SendInputEnums.MOUSEEVENTF.RIGHTDOWN;
                            break;
                        default:
                            break;
                    }

                    var pLastInputs = new[]
                    {
                        new SendInputStructs.INPUT()
                        {
                            type = (int)SendInputEnums.InputType.INPUT_MOUSE,
                            U = new SendInputStructs.InputUnion 
                            { 
                                mi = new SendInputStructs.MOUSEINPUT()
                                    {
                                        dx = lastX,
                                        dy = lastY,
                                        dwFlags = lastInputFlags
                                    }
                            }
                        }
                    };

                    SendInput((uint)pLastInputs.Length, pLastInputs, SendInputStructs.INPUT.Size);
                }

                //send current input
                SendInputEnums.MOUSEEVENTF flags = SendInputEnums.MOUSEEVENTF.MOVE | SendInputEnums.MOUSEEVENTF.ABSOLUTE;

                switch(e.State)
                {
                    case UserMouseState.Down:
                        switch(e.Button)
                        {
                            case UserMouseButton.Left:
                                flags = flags | SendInputEnums.MOUSEEVENTF.LEFTDOWN;
                                break;
                            case UserMouseButton.Middle:
                                flags = flags | SendInputEnums.MOUSEEVENTF.MIDDLEDOWN;
                                break;
                            case UserMouseButton.Right:
                                flags = flags | SendInputEnums.MOUSEEVENTF.RIGHTDOWN;
                                break;
                            default:
                                break;
                        }
                        break;
                    case UserMouseState.Up:
                        switch(e.Button)
                        {
                            case UserMouseButton.Left:
                                flags = flags | SendInputEnums.MOUSEEVENTF.LEFTUP;
                                break;
                            case UserMouseButton.Middle:
                                flags = flags | SendInputEnums.MOUSEEVENTF.MIDDLEUP;
                                break;
                            case UserMouseButton.Right:
                                flags = flags | SendInputEnums.MOUSEEVENTF.RIGHTUP;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                
                var pInputs = new[]
                {
                    new SendInputStructs.INPUT()
                    {
                        type = (int)SendInputEnums.InputType.INPUT_MOUSE,
                        U = new SendInputStructs.InputUnion 
                        { 
                            mi = new SendInputStructs.MOUSEINPUT()
                                {
                                    dx = x,
                                    dy = y,
                                    dwFlags = flags
                                }
                        }
                    }
                };

                SendInput((uint)pInputs.Length, pInputs, SendInputStructs.INPUT.Size);

                if(e.State == UserMouseState.Down)
                {
                    //if button state in send input is down, we need to send one more input with mouse up so we can release mouse down handling for other windows
                    //send input mouse button up so we can free mouse down handling for other windows

                    SendInputEnums.MOUSEEVENTF flagsUp = SendInputEnums.MOUSEEVENTF.MOVE | SendInputEnums.MOUSEEVENTF.ABSOLUTE;

                    switch (e.Button)
                    {
                        case UserMouseButton.Left:
                            flagsUp = flagsUp | SendInputEnums.MOUSEEVENTF.LEFTUP;
                            break;
                        case UserMouseButton.Middle:
                            flagsUp = flagsUp | SendInputEnums.MOUSEEVENTF.MIDDLEUP;
                            break;
                        case UserMouseButton.Right:
                            flagsUp = flagsUp | SendInputEnums.MOUSEEVENTF.RIGHTUP;
                            break;
                        default:
                            break;
                    }

                    pInputs = new[]
                    {
                        new SendInputStructs.INPUT()
                        {
                            type = (int)SendInputEnums.InputType.INPUT_MOUSE,
                            U = new SendInputStructs.InputUnion 
                            { 
                                mi = new SendInputStructs.MOUSEINPUT()
                                    {
                                        dx = x,
                                        dy = y,
                                        dwFlags = flagsUp
                                    }
                            }
                        }
                    };

                    SendInput((uint)pInputs.Length, pInputs, SendInputStructs.INPUT.Size);
                }

                user.LastButton = e.Button;
                user.LastButtonState = e.State;
                user.LastPositionX = e.X;
                user.LastPositionY = e.Y;
            }
        }

        int CalculateAbsoluteCoordinateX(int x)
        {
            return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        int CalculateAbsoluteCoordinateY(int y)
        {
            return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }
    }
}

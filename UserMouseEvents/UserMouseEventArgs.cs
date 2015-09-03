using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MouseAndKeyboardSwitch
{
    public class UserMouseEventArgs : EventArgs
    {
        private int userId = 0;
        private UserMouseButton button;
        private UserMouseState state;
        private int x = 0;
        private int y = 0;
        public UserMouseEventArgs(int userId, UserMouseButton button, UserMouseState state, int x, int y)
        {
            this.userId = userId;
            this.button = button;
            this.state = state;
            this.x = x;
            this.y = y;
        }

        public int UserId
        {
            get
            {
                return this.userId;
            }
        }

        public UserMouseButton Button
        {
            get
            {
                return this.button;
            }
        }
        public UserMouseState State
        { 
            get
            {
                return this.state;
            }
        }

        public int X 
        { 
            get
            {
                return this.x;
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
        }
    }
}

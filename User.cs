using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseAndKeyboardSwitch
{
    public class User
    {
        public int Id { get; set; }
        public IntPtr ProccessWindowHandle { get; set; }
        public string ProccessName { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public UserMouseState LastButtonState { get; set; }
        public UserMouseButton LastButton { get; set; }
        public int LastPositionX { get; set; }
        public int LastPositionY { get; set; }
    }
}

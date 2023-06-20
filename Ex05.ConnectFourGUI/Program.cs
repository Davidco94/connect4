using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex05.ConnectFourGUI
{
    public static class Program
    {
        public static void Main()
        {
            Application.EnableVisualStyles();
            new GameManager().RunGame();
        }
    }
}

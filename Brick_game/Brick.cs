using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace BrickGame;
public class Brick {
    //private int x;
    //private int y;
    private int pos;
    private Canvas can;
    private bool isOn;
    private bool check;
    //public int X { get => x; set => x = value; }
    //public int Y { get => y; set => y = value; }
    public int Pos { get => pos; set => pos = value; }
    public Canvas Can { get => can; set => can = value; }
    public bool IsOn {
        get => isOn;
        set {
            isOn = value;
            if (isOn) Can.Background = Brushes.Red;
        }
    }
    public bool Check { get => check; set => check = value; }

    public Brick(int pos, Canvas c) {
        IsOn = false;
        check = true;
        //this.x = x;
        //this.y = y;
        this.can = c;
        Pos = pos;
        //if (y == 0) { Pos = x; }
        //else { Pos = y*10 + x; }
        if (IsOn) c.Background = Brushes.Red;
    }
}



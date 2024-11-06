using System.Windows.Controls;

namespace BrickGame;
public class Brick {
    private int pos;
    private Canvas can;
    private bool check;
    public int Pos { get => pos; set => pos = value; }
    public Canvas Can { get => can; set => can = value; }
    public bool Check { get => check; set => check = value; }
    public Brick(int pos, Canvas c) {
        check = true;
        this.can = c;
        Pos = pos;
    }
}



using Brick_game;
using BrickGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Brick_game;
internal class Shapes {
    private int center;

    public int Columns { get; set; }
    public int Count { get; private set; }
    public int[] LShape { get; set; }
    public int[] JShape { get; set; }
    public int[] IShape { get; set; }
    public int[] OShape { get; set; }
    public int[] SShape { get; set; }
    public int[] TShape { get; set; }
    public int[] ZShape { get; set; }
    public int[][] ShapesArray { get; set; }

    /// <summary>
    /// Creates shapes for brick game
    /// </summary>
    /// <param name="columns">Width of gamefield</param>
    public Shapes(int columns) {
        this.Columns = columns;
        center = Columns / 2;

        int IS1 = center;
        int IS2 = center + Columns;
        int IS3 = center + Columns * 2;
        int IS4 = center + Columns * 3;
        IShape = new int[] { IS4, IS3, IS2, IS1 };


        int JS1 = center;
        int JS2 = center + Columns;
        int JS3 = center + Columns - 1;
        int JS4 = center + Columns - 2;
        JShape = new int[] { JS1, JS3, JS2, JS4 };

        int LS1 = center ;
        int LS2 = center + Columns ;
        int LS3 = center + Columns + 1;
        int LS4 = center + Columns + 2;
        LShape = new int[] { LS1, LS3, LS2, LS4 };

        int OS1 = center;
        int OS2 = center + 1;
        int OS3 = center + Columns;
        int OS4 = center + Columns + 1;
        OShape = new int[] { OS1, OS2, OS3, OS4 };

        int SS1 = center;
        int SS2 = center + 1;
        int SS3 = center + Columns;
        int SS4 = center + Columns - 1;
        SShape = new int[] { SS2, SS1, SS3, SS4 };

        int TS1 = center;
        int TS2 = center + Columns;
        int TS3 = center + Columns + 1;
        int TS4 = center + Columns - 1;
        TShape = new int[] { TS1, TS2, TS3, TS4 };

        int ZS1 = center - 1;
        int ZS2 = center ;
        int ZS3 = center + Columns ;
        int ZS4 = center + Columns + 1;
        ZShape = new int[] { ZS1, ZS2, ZS3, ZS4 };

        ShapesArray = new[] { IShape, JShape, LShape, OShape, SShape, TShape, ZShape };
        Count = ShapesArray.Length;
    }   
}

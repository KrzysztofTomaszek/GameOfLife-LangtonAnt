using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Ant
    {
        public string PreviousCellColor {get; private set;}
        public static string DeadColour {get; set;}
        public static string AliveColour {get; set;}
        readonly string traceColor;
        public int ColIndex { get; private set;}
        public int RowIndex {get; private set;}
        int facing;        
        public static Rectangle[,] Rects {get; set;}
        public static List<Ant> Ants {get; set;}

        public Ant(int row, int col, string previousCellColor, string _traceColor, int _facing)
        {
            PreviousCellColor = previousCellColor;
            traceColor = _traceColor;
            ColIndex = col;
            RowIndex = row;
            facing = _facing;
        }

        public void AntMove( int height, int width)
        {
            bool turnLeft = TurnLeft();
            FacingUptade(turnLeft);
            MakeTrace(turnLeft);
            AntStep(height, width);
        }

        bool TurnLeft()
        {
            bool turnLeft = false;
            if(PreviousCellColor == DeadColour) turnLeft = true;
            return turnLeft;
        }

        void FacingUptade(bool turnLeft)
        {
            if(turnLeft)
            {
                if(facing == 3) facing = 0;
                else facing += 1;
            }
            else
            {
                if(facing == 0) facing = 3;
                else facing -= 1;
            }
        }

        void MakeTrace(bool onDead)
        {
            if(onDead) Rects[RowIndex, ColIndex].Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(traceColor));
            else Rects[RowIndex, ColIndex].Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DeadColour));
        }

        void AntStep(int height, int width)
        {
            switch(facing)
            {
                case 0:
                    RowIndex -= 1;
                    if(RowIndex < 0) RowIndex = height - 1;
                    break;
                case 1:
                    ColIndex -= 1;
                    if(ColIndex < 0) ColIndex = width - 1;
                    break;
                case 2:
                    RowIndex += 1;
                    if(RowIndex >= height) RowIndex = 0;
                    break;
                case 3:
                    ColIndex += 1;
                    if(ColIndex >= width) ColIndex = 0;
                    break;
            }            
            PreviousCellColor = Rects[RowIndex, ColIndex].Fill.ToString();
            if(PreviousCellColor == AliveColour)
            {
                foreach(Ant ant in Ants)
                {
                    if(ant.ColIndex == ColIndex && ant.RowIndex == RowIndex)
                    {
                        PreviousCellColor = ant.PreviousCellColor;
                    }
                }
            }
            Rects[RowIndex, ColIndex].Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AliveColour));
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for GoLPlay.xaml
    /// </summary>
    public partial class GoLPlay : Window
    {
        readonly string aliveColour;
        readonly string deadColour;
        readonly int width;
        readonly int height;
        readonly int cellSize;
        readonly int oldRow;
        readonly int oldCol;
        readonly int invisibleSpace;
        int generation;
        float symSpeed;
        bool animationEnable;
        bool[,] lifes;
        Rectangle[,] rects;        
        DispatcherTimer timer;

        public GoLPlay( string _aliveColour, string _deadColour, int _width, int _height, int _cellSize)
        {            
            InitializeComponent();
            width = _width;
            height = _height;
            cellSize = _cellSize;
            aliveColour = _aliveColour;
            deadColour = _deadColour;
            animationEnable = false;
            generation = 0;
            invisibleSpace = 10;
            oldRow = height + (2*invisibleSpace);
            oldCol = width + (2 * invisibleSpace);
            rects = new Rectangle[height,width];
            lifes = new bool[oldRow, oldCol];            
            timer = new DispatcherTimer();
            this.Title = "Symulation Stopped";

            FirstInicializeLifesTable(lifes, oldRow, oldCol);
            symSpeed = 0.25f;
            timer.Interval = TimeSpan.FromSeconds(symSpeed);
            timer.Tick += Timer_Tick;            
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            this.Title ="Generation: "+(generation++).ToString();
            int row = height;
            int col = width;
            string[,] rectsOld = new string[oldRow, oldCol];
            Clone2DRectangleFillToStringTable(rects, rectsOld, oldCol, oldRow);
            GridUpdate(rectsOld,row, col);
        }

        void GridUpdate(string[,] rectsOld, int row, int col)
        {       
            for(int i = 0;i < oldRow;i++)
            {
                for(int j = 0;j < oldCol;j++)
                {
                    int countAlive = CountNeighbors(rectsOld,i, j);                

                    if(j < col + invisibleSpace && i < row + invisibleSpace && j > invisibleSpace - 1 && i > invisibleSpace - 1)
                    {
                        SetRectangleTab(countAlive, i - invisibleSpace, j - invisibleSpace);
                        
                    }
                    else
                    {
                        SetLifesTable(countAlive, i, j);                        
                    }
                }
            }
        }

        void SetRectangleTab(int countAlive,int row, int col)
        {
            switch(countAlive)
            {
                case 0:case 1:case 4:case 5:case 6:case 7:case 8:
                    rects[row, col].Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(deadColour));
                    break;
                case 3:
                    rects[row, col].Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(aliveColour));
                    break;
            }
        }

        void SetLifesTable(int countAlive,int row,int col)
        {
            switch(countAlive)
            {
                case 0:case 1:case 4:case 5:case 6:case 7:case 8:
                    lifes[row, col] = false;
                    break;
                case 3:
                    lifes[row, col] = true;
                    break;
            }
        }

        int CountNeighbors(string[,] rectsOld, int rowCell, int colCell)
        {
            int countAlive = 0;

            if(rowCell != 0) if(rectsOld[rowCell - 1, colCell] == aliveColour) countAlive++;
            if(rowCell != oldRow - 1) if(rectsOld[rowCell + 1, colCell] == aliveColour)countAlive++;
            if(colCell != 0) if(rectsOld[rowCell, colCell - 1] == aliveColour) countAlive++;
            if(colCell != oldCol - 1) if(rectsOld[rowCell, colCell + 1] == aliveColour) countAlive++;
            if(colCell != 0 && rowCell != 0) if(rectsOld[rowCell - 1, colCell - 1] == aliveColour) countAlive++;
            if(colCell != oldCol - 1 && rowCell != 0) if(rectsOld[rowCell - 1, colCell + 1] == aliveColour)countAlive++;
            if(colCell != 0 && rowCell != oldRow - 1) if(rectsOld[rowCell + 1, colCell - 1] == aliveColour) countAlive++;
            if(colCell != oldCol - 1 && rowCell != oldRow - 1) if(rectsOld[rowCell + 1, colCell + 1] == aliveColour) countAlive++;

            return countAlive;
        }

        public void Start()
        {
            Area.Height = height * cellSize;
            Area.Width = width * cellSize;
            CreateGrid(height,width, cellSize);
        }

        void CreateGrid(int _height,int _width, int _cellSize)
        {
            int row = _height;
            int col = _width;
            for(int i = 0; i < row; i++)
            {
                for(int j = 0;j < col;j++)
                {
                    Rectangle r = new Rectangle
                    {
                        Width = _cellSize,
                        Height = _cellSize,
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(deadColour))
                    };
                    Area.Children.Add(r);
                    Canvas.SetLeft(r, (j * r.Width) + 0.1f);
                    Canvas.SetTop(r, (i * r.Height) + 0.1f);
                    r.MouseLeftButtonUp += R_MouseLeftButtonUp;
                    rects[i,j] = r;   
                }
            }
        }

        void R_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle cell = (Rectangle)sender;
            if(!animationEnable)
            {
                if(cell.Fill.ToString() == new SolidColorBrush((Color)ColorConverter.ConvertFromString(deadColour)).ToString())
                {
                    cell.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(aliveColour));
                }
                else
                {
                    cell.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(deadColour));
                }
            }            
        }

        void Area_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            animationEnable = !animationEnable;
            if(animationEnable)
            {                
                generation = 0;
                timer.Start();
            }
            else
            {
                timer.Stop();
                this.Title = "Symulation Stopped";                
            }
                
        }

        void BorderExist(Rectangle[,] tabOld, int col, int row)
        {
            bool onBorderExist = false;
            for(int j = 0;j < col - (2*invisibleSpace);j++)onBorderExist = tabOld[0, j].Fill.ToString() == aliveColour ? true : false;
            for(int j = 0;j < col - (2 * invisibleSpace);j++)onBorderExist = tabOld[row - 21, j].Fill.ToString() == aliveColour ? true : false;
            for(int i = 1;i < row - ((2 * invisibleSpace)+1);i++)onBorderExist = tabOld[i, 0].Fill.ToString() == aliveColour ? true : false;
            for(int i = 1;i < row - ((2 * invisibleSpace)+1);i++)onBorderExist = tabOld[i, col - 21].Fill.ToString() == aliveColour ? true : false;
            if(onBorderExist)InicializeLifesTable(lifes, row, col);
        }


        void Clone2DRectangleFillToStringTable(Rectangle[,] tabOld, string[,] tabNew, int col, int row)
        {
            BorderExist(tabOld, col, row);

            for(int i = 0;i <= invisibleSpace;i++)for(int j = 0;j < col;j++)tabNew[i, j] = lifes[i, j] == true ? aliveColour : deadColour;
            for(int i = row-invisibleSpace;i < row;i++)for(int j = 0;j < col;j++)tabNew[i, j] = lifes[i, j] == true ? aliveColour : deadColour;
            for(int i = invisibleSpace;i < row - invisibleSpace;i++)for(int j = 0;j <= invisibleSpace;j++)tabNew[i, j] = lifes[i, j] == true ? aliveColour : deadColour;
            for(int i = invisibleSpace;i < row - invisibleSpace;i++)for(int j = col - invisibleSpace;j < col;j++)tabNew[i, j] = lifes[i, j] == true ? aliveColour : deadColour;
            
            for(int i = invisibleSpace;i < row- invisibleSpace;i++)
            {
                for(int j = invisibleSpace;j < col- invisibleSpace;j++)
                {
                    string tmp;
                    tmp = tabOld[i- invisibleSpace, j- invisibleSpace].Fill.ToString();
                    tabNew[i, j] = tmp;
                }
            }
        }

        void InicializeLifesTable(bool[,] tab,int row, int col)
        {
            for(int i = 0;i <= invisibleSpace;i++) for(int j = 0;j < col;j++) tab[i, j] = false;
            for(int i = row - invisibleSpace;i < row;i++) for(int j = 0;j < col;j++) tab[i, j] = false;
            for(int i = invisibleSpace;i < row - invisibleSpace;i++) for(int j = 0;j <= invisibleSpace;j++) tab[i, j] = false;
            for(int i = invisibleSpace;i < row - invisibleSpace;i++) for(int j = col - invisibleSpace;j < col;j++) tab[i, j] = false;
        }

        void FirstInicializeLifesTable(bool[,] tab, int row, int col)
        {
            for(int i = 0;i < row;i++)for(int j = 0;j < col;j++) tab[i, j] = false;
        }

        void GoLWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                symSpeed += 0.005f;
            }
            else
            {
                if(symSpeed - 0.005f > 0)symSpeed-= 0.005f;
            }
            timer.Interval = TimeSpan.FromSeconds(symSpeed);
        }
    }
}

using System;
using System.Collections.Generic;
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
    public partial class LAPlay: Window
    {
        static string aliveColour;
        static string deadColour;
        int width;
        int height;
        int cellSize;
        int oldRow;
        int oldCol;
        int generation;
        float symSpeed;
        bool animationEnable;
        static Rectangle[,] rects;
        List<Ant> ants;
        DispatcherTimer timer;

        public LAPlay(string _aliveColour, string _deadColour, int _width, int _height, int _cellSize)
        {
            InitializeComponent();
            width = _width;
            height = _height;
            cellSize = _cellSize;
            aliveColour = _aliveColour;
            deadColour = _deadColour;
            animationEnable = false;
            generation = 0;
            oldRow = height;
            oldCol = width;
            rects = new Rectangle[height, width];
            ants = new List<Ant>();
            timer = new DispatcherTimer();
            this.Title = "Symulation Stopped";

            Ant.AliveColour = aliveColour;
            Ant.DeadColour = deadColour;        
            
            symSpeed = 0.25f;
            timer.Interval = TimeSpan.FromSeconds(symSpeed);
            timer.Tick += Timer_Tick;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            this.Title = "Generation: " + (generation++).ToString();
            foreach(Ant ant in ants) ant.AntMove(height,width);
        }

        public void Start()
        {
            Area.Height = height * cellSize;
            Area.Width = width * cellSize;
            CreateGrid(height, width, cellSize);
        }

        void CreateGrid(int _height, int _width, int _cellSize)
        {
            int row = _height;
            int col = _width;
            for(int i = 0;i < row;i++)
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
                    rects[i, j] = r;
                }
            }
            Ant.Rects = rects;
        }

        void R_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle cell = (Rectangle)sender;
            if(!animationEnable)
            {
                if(!(cell.Fill.ToString() == new SolidColorBrush((Color)ColorConverter.ConvertFromString(aliveColour)).ToString()))
                {
                    CreateAnt(cell);
                }
                else if(cell.Fill.ToString() == new SolidColorBrush((Color)ColorConverter.ConvertFromString(aliveColour)).ToString())
                {
                    DestroyAnt(cell);
                }
            }
        }

        void CreateAnt(Rectangle cell)
        {
            bool CANbreak = false;
            for(int i = 0;i < height;i++)
            {
                for(int j = 0;j < width;j++)
                {
                    if(rects[i, j].Equals(cell))
                    {
                        Random random = new Random();
                        SolidColorBrush brush;
                        do
                        {
                            brush = new SolidColorBrush(
                                Color.FromRgb(
                                (byte)random.Next(255),
                                (byte)random.Next(255),
                                (byte)random.Next(255)
                                ));
                        } while(brush.ToString() == aliveColour || brush.ToString() == deadColour);
                        ants.Add(new Ant(i, j, cell.Fill.ToString(), brush.ToString(), 3));
                        Ant.Ants = ants;
                        CANbreak = true;
                        break;
                    }
                }
                if(CANbreak)
                    break;
            }
            cell.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(aliveColour));

        }

        void DestroyAnt(Rectangle cell)
        {
            foreach(Ant ant in ants)
            {
                bool CANbreak = false;
                for(int i = 0;i < height;i++)
                {
                    for(int j = 0;j < width;j++)
                    {

                        if(ant.RowIndex == i && ant.ColIndex == j)
                        {
                            ants.Remove(ant);
                            CANbreak = true;
                            break;
                        }
                    }
                    if(CANbreak)
                        break;
                }
                if(CANbreak)
                    break;
            }
            cell.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(deadColour));

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
                this.Title = "Symulation Stopped";
                timer.Stop();
            }
        }              

        void LAWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0) symSpeed += 0.005f;
            else if(symSpeed - 0.005f > 0) symSpeed -= 0.005f;
            timer.Interval = TimeSpan.FromSeconds(symSpeed);            
        }        
    }
}

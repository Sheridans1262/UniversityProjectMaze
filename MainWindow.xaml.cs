using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Maze.Algorithms;

namespace Maze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<List<Cell4Dir>> maze;

        private int XLength;
        private int YLength;

        // X & Y of Canvas painting area (in pixels)
        private int PaintingAreaX;
        private int PaintingAreaY;

        private double cellWallLength;
        private Point playerStart;
        private Point playerFinish;


        private Rectangle player;
        private Point playerCoordinatesInMaze;

        public MainWindow()
        {
            InitializeComponent();

            maze = new List<List<Cell4Dir>>();

            XLength = 0;
            YLength = 0;

            PaintingAreaX = 740;
            PaintingAreaY = 470;

            cellWallLength = 0;
            playerStart = new Point();
            playerFinish = new Point();

            player = new Rectangle();
            playerCoordinatesInMaze = new Point();
        }

        #region Event Handlers

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            // Set random values to X & Y in range (1, 20)
            Random random = new Random();

            XLengthBox.Text = Convert.ToString(random.Next(5, 20));
            XLengthBox.GotFocus -= TextBox_GotFocus;

            YLengthBox.Text = Convert.ToString(random.Next(5, 20));
            YLengthBox.GotFocus -= TextBox_GotFocus;
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            int XLength = 0, YLength = 0;

            // Check if X & Y are positive numbers
            if (!int.TryParse(XLengthBox.Text, out XLength) 
                || !int.TryParse(YLengthBox.Text, out YLength)
                || XLength <= 0 || YLength <= 0)
            {
                // If they're not, then Messagebox with warning
                MessageBox.Show("Invalid number!");

                XLengthBox.Text = string.Empty;
                YLengthBox.Text = string.Empty;

                return;
            }

            

            MazePaint(XLength, YLength);

            SetPlayer();

            XLengthBox.GotFocus += TextBox_GotFocus;
            YLengthBox.GotFocus += TextBox_GotFocus;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear TextBoxes when clicked (only once)
            TextBox textBox = (TextBox)sender;
            textBox.Text = string.Empty;
            textBox.GotFocus -= TextBox_GotFocus;
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            SolveAlorithm();
        }

        private void LayoutRoot_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    MoveLeft();
                    CheckWinConditions();
                    break;

                case Key.W:
                    MoveUp();
                    CheckWinConditions();
                    break;

                case Key.D:
                    MoveRight();
                    CheckWinConditions();
                    break;

                case Key.S:
                    MoveDown();
                    CheckWinConditions();
                    break;
            }
        }

        #endregion


        #region Maze Painting Functions

        private void MazePaint(int XLength, int YLength)
        {
            CanvasPaintingArea.Children.Clear();

            this.XLength = XLength;
            this.YLength = YLength;

            // Create Maze itself
            EllersAlgorithm2D mazeAlg = new EllersAlgorithm2D(XLength, YLength);
            maze = mazeAlg.CreateMaze();


            // Set starting point for painting:
            // cellWallLength - length of the wall of cell in maze - min(PaintingAreaX/X; PaintingAreaY/Y)
            cellWallLength = FindCellWallLength(XLength, YLength);

            double deltaX = (cellWallLength / 2) * (XLength - 1);
            double deltaY = (cellWallLength / 2) * (YLength - 1);

            // currentCenter - center of top-right cell in maze
            Point currentCenter = new Point(PaintingAreaX / 2 - deltaX, PaintingAreaY / 2 - deltaY);
            playerStart = new Point(currentCenter.X, currentCenter.Y + cellWallLength * (YLength - 1));
            playerFinish = new Point(currentCenter.X + cellWallLength * (XLength - 1), currentCenter.Y);


            SetStartingAndEndingPoints();


            for (int i = 0; i < maze.Count; i++)
            {
                MazeStringPainting(maze[i], currentCenter, cellWallLength);

                currentCenter.Y += cellWallLength;
            }

            playerCoordinatesInMaze = new Point(0, YLength - 1);
        }

        private void MazeStringPainting(List<Cell4Dir> mazeString, Point currentCenter, double cellWallLength)
        {
            for(int i = 0; i < mazeString.Count; i++)
            {
                if(mazeString[i].LeftWall)
                {
                    VerticalRectangle(currentCenter, cellWallLength, false);
                }

                if(mazeString[i].UpperWall)
                {
                    HorizontalRectangle(currentCenter, cellWallLength, false);
                }

                if(mazeString[i].RightWall)
                {
                    VerticalRectangle(currentCenter, cellWallLength, true);
                }

                if(mazeString[i].BottomWall)
                {
                    HorizontalRectangle(currentCenter, cellWallLength, true);
                }

                currentCenter.X += cellWallLength;
            }

        }

        private double FindCellWallLength(int xLength, int yLength)
        {
            if (PaintingAreaX / xLength > PaintingAreaY / yLength)
                return PaintingAreaY / yLength;

            return PaintingAreaX / xLength;
        }

        private void HorizontalRectangle(Point cellCenter, double cellWallLength, bool isUpperOrBottom)
        {
            Rectangle rectangle = new Rectangle()
            {
                Height = cellWallLength / 5,
                Width = cellWallLength * (6 / 5),
                Fill = Brushes.Green,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            CanvasPaintingArea.Children.Add(rectangle);

            switch (isUpperOrBottom)
            {
                case false:
                    Canvas.SetTop(rectangle, cellCenter.Y - cellWallLength / 2 - cellWallLength / 10);
                    break;
                case true:
                    Canvas.SetTop(rectangle, cellCenter.Y + cellWallLength / 2 - cellWallLength / 10);
                    break;
            }
            
            Canvas.SetLeft(rectangle, cellCenter.X - cellWallLength/2);
        }

        private void VerticalRectangle(Point cellCenter, double cellWallLength, bool isLeftOrRight)
        {
            Rectangle rectangle = new Rectangle()
            {
                Height = cellWallLength * (6 / 5),
                Width = cellWallLength / 5,
                Fill = Brushes.Green,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            CanvasPaintingArea.Children.Add(rectangle);

            switch (isLeftOrRight)
            {
                case false:
                    Canvas.SetLeft(rectangle, cellCenter.X - cellWallLength / 2 - cellWallLength / 10);
                    break;
                case true:
                    Canvas.SetLeft(rectangle, cellCenter.X + cellWallLength / 2 - cellWallLength / 10);
                    break;
            }

            Canvas.SetTop(rectangle, cellCenter.Y - cellWallLength / 2);
        }

        private void SetStartingAndEndingPoints()
        {
            Rectangle start = new Rectangle()
            {
                Height = cellWallLength,
                Width = cellWallLength,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetTop(start, playerStart.Y - cellWallLength / 2);
            Canvas.SetLeft(start, playerStart.X - cellWallLength / 2);

            CanvasPaintingArea.Children.Add(start);


            Rectangle finish = new Rectangle()
            {
                Height = cellWallLength,
                Width = cellWallLength,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetTop(finish, playerFinish.Y - cellWallLength / 2);
            Canvas.SetLeft(finish, playerFinish.X - cellWallLength / 2);

            CanvasPaintingArea.Children.Add(finish);
        }

        #endregion


        #region Maze Solving By Player Functions

        private void SetPlayer()
        {
            player.Height = cellWallLength / 3;
            player.Width = cellWallLength / 3;
            player.Fill = Brushes.GreenYellow;
            player.Stroke = Brushes.Black;
            player.StrokeThickness = 1;

            Canvas.SetTop(player, playerStart.Y - cellWallLength / 6);
            Canvas.SetLeft(player, playerStart.X - cellWallLength / 6);

            CanvasPaintingArea.Children.Add(player);
        }

        #endregion


        #region Maze Solving By Algorithm Functions

        private async void SolveAlorithm()
        {
            if (maze == null) return;

            SolvingAlgorithm2D solvingAlg = new SolvingAlgorithm2D(maze);

            byte facingDirection = ChooseDirection(solvingAlg, solvingAlg.ChooseDirectionFirstStep(
                                Convert.ToInt32(playerCoordinatesInMaze.Y),
                                Convert.ToInt32(playerCoordinatesInMaze.X)));

            do
            {
                facingDirection = ChooseDirection(solvingAlg, facingDirection);

                await Task.Delay(2000/(XLength*YLength));
            } while (!CheckWinConditions());
        }

        private byte ChooseDirection(SolvingAlgorithm2D solvingAlg, byte facingDirection)
        {
            switch (solvingAlg.ChooseDirection(
                                Convert.ToInt32(playerCoordinatesInMaze.Y),
                                Convert.ToInt32(playerCoordinatesInMaze.X),
                                facingDirection))
            {
                case 1:
                    MoveLeft();
                    return 1;

                case 2:
                    MoveUp();
                   return 2;

                case 3:
                    MoveRight();
                    return 3;  
            }

            MoveDown();
            return 4;
        }

        #endregion


        #region Move Functions

        private void MoveLeft()
        {
            if (!maze[Convert.ToInt32(playerCoordinatesInMaze.Y)][Convert.ToInt32(playerCoordinatesInMaze.X)].LeftWall)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - cellWallLength);

                playerCoordinatesInMaze.X--;
            }
        }

        private void MoveUp()
        {
            if (!maze[Convert.ToInt32(playerCoordinatesInMaze.Y)][Convert.ToInt32(playerCoordinatesInMaze.X)].UpperWall)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) - cellWallLength);

                playerCoordinatesInMaze.Y--;
            }
        }

        private void MoveRight()
        {
            if (!maze[Convert.ToInt32(playerCoordinatesInMaze.Y)][Convert.ToInt32(playerCoordinatesInMaze.X)].RightWall)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + cellWallLength);

                playerCoordinatesInMaze.X++;
            }
        }

        private void MoveDown()
        {
            if (!maze[Convert.ToInt32(playerCoordinatesInMaze.Y)][Convert.ToInt32(playerCoordinatesInMaze.X)].BottomWall)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + cellWallLength);

                playerCoordinatesInMaze.Y++;
            }
        }

        private bool CheckWinConditions()
        {
            if (playerCoordinatesInMaze.X == (XLength - 1) && playerCoordinatesInMaze.Y == 0)
            {
                MessageBox.Show("Congratulations! You've solved the maze.");
                return true;
            }
            return false;
        }

        #endregion
    }
}

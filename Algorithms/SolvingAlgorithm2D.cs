using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Algorithms
{
    class SolvingAlgorithm2D
    {
        public List<List<Cell4Dir>> Maze { get; set; }

        public SolvingAlgorithm2D(List<List<Cell4Dir>> Maze)
        {
            this.Maze = Maze;
        }

        public byte ChooseDirectionFirstStep(int Y, int X)
        {
            if (!Maze[Y][X].LeftWall)
            {
                return 1;
            }

            if (!Maze[Y][X].UpperWall)
            {
                return 2;
            }

            if (!Maze[Y][X].RightWall)
            {
                return 3;
            }

            return 4;
        }

        public byte ChooseDirection(int Y, int X, byte facingDirection)
        {
            switch (facingDirection)
            {
                case 1:
                    return FacingLeft(Y, X);

                case 2:
                    return FacingUp(Y, X);

                case 3:
                    return FacingRight(Y, X);
            }

            return FacingBottom(Y, X);
        }

        private byte FacingLeft(int Y, int X)
        {
            if (!Maze[Y][X].BottomWall)
            {
                return 4;
            }

            if (!Maze[Y][X].LeftWall)
            {
                return 1;
            }

            if (!Maze[Y][X].UpperWall)
            {
                return 2;
            }

            return 3;
        }

        private byte FacingUp(int Y, int X)
        {
            if (!Maze[Y][X].LeftWall)
            {
                return 1;
            }

            if (!Maze[Y][X].UpperWall)
            {
                return 2;
            }

            if (!Maze[Y][X].RightWall)
            {
                return 3;
            }

            return 4;
        }

        private byte FacingRight(int Y, int X)
        {
            if (!Maze[Y][X].UpperWall)
            {
                return 2;
            }

            if (!Maze[Y][X].RightWall)
            {
                return 3;
            }

            if (!Maze[Y][X].BottomWall)
            {
                return 4;
            }

            return 1;
        }

        private byte FacingBottom(int Y, int X)
        {
            if (!Maze[Y][X].RightWall)
            {
                return 3;
            }

            if (!Maze[Y][X].BottomWall)
            {
                return 4;
            }

            if (!Maze[Y][X].LeftWall)
            {
                return 1;
            }

            return 2;
        }
    }
}

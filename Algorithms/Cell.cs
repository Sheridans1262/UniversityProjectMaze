using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Maze.Algorithms
{
    class Cell4Dir
    {
        public bool LeftWall { get; set; }
        public bool UpperWall { get; set; }
        public bool RightWall { get; set; }
        public bool BottomWall { get; set; }

        public long SetIterator { get; set; }

        public byte WeWereHere { get; set; }


        public Cell4Dir()
        {
            LeftWall = false;
            UpperWall = false;
            RightWall = false;
            BottomWall = false;

            SetIterator = 0;

            WeWereHere = 0;
        }

        public Cell4Dir(Cell4Dir cell)
        {
            this.LeftWall = cell.LeftWall;
            this.UpperWall = cell.UpperWall;
            this.RightWall = cell.RightWall;
            this.BottomWall = cell.BottomWall;

            this.SetIterator = cell.SetIterator;

            this.WeWereHere = 0;
        }
    }
}

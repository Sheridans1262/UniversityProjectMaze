using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Algorithms
{
    class EllersAlgorithm2D
    {
        public int XLength { get; set; }

        public int YLength { get; set; }

        private long _setIterator;

        public List<List<Cell4Dir>> maze { get; set; }


        public EllersAlgorithm2D(int _XLength, int _YLength)
        {
            XLength = _XLength;
            YLength = _YLength;

            _setIterator = 0;

            maze = null;
        }


        public List<List<Cell4Dir>> CreateMaze()
        {
            maze = new List<List<Cell4Dir>>(YLength);


            List<Cell4Dir> mazeStringConstruction = FirstStringInit();
            List<Cell4Dir> tmp = new List<Cell4Dir>();

            Random random = new Random();

            // Constructing maze strings not including the last one

            for (int i = 0; i < YLength; i++)
            {
                mazeStringConstruction = IsCellBelongsToSet(mazeStringConstruction);

                RightWalls(mazeStringConstruction, random);
                BottomWalls(mazeStringConstruction, random);

                if (i < YLength - 1)
                {
                    maze.Add(new List<Cell4Dir>(XLength));
                    for (int j = 0; j < XLength; j++)
                    {
                        maze[i].Add(new Cell4Dir(mazeStringConstruction[j]));
                    }
                
                    NextStringPrepare(mazeStringConstruction);
                }
            }

            // Constructing the last string of the maze

            LastString(mazeStringConstruction);


            maze.Add(new List<Cell4Dir>(XLength));
            for (int j = 0; j < XLength; j++)
            {
                maze[YLength - 1].Add(new Cell4Dir(mazeStringConstruction[j]));
            }

            // Constructing walls by the perimether of the maze

            CreateMazeBorders(maze);

            return maze;
        }


        private List<Cell4Dir> FirstStringInit()
        {
            List<Cell4Dir> _mazeStringConstruction = new List<Cell4Dir>();

            for (int i = 0; i < XLength; i++)
            {
                _mazeStringConstruction.Add(new Cell4Dir());
            }

            return _mazeStringConstruction;
        }


        private List<Cell4Dir> IsCellBelongsToSet(List<Cell4Dir> _mazeStringConstruction)
        {
            for (int i = 0; i < XLength; i++)
            {
                if (_mazeStringConstruction[i].SetIterator == 0)
                {
                    _mazeStringConstruction[i].SetIterator = ++_setIterator;
                }
            }

            return _mazeStringConstruction;
        }


        private void RightWalls(List<Cell4Dir> _mazeStringConstruction, Random random)
        {
            for (int i = 0; i < XLength - 1; i++)
            {
                // Yes - build the right wall, no - merge current-right sets
                if (random.NextDouble() >= 0.5 ||
                    _mazeStringConstruction[i].SetIterator == _mazeStringConstruction[i + 1].SetIterator)
                {
                    _mazeStringConstruction[i].RightWall = true;
                    _mazeStringConstruction[i + 1].LeftWall = true;
                    continue;
                }

                MergeSets(_mazeStringConstruction,
                          _mazeStringConstruction[i].SetIterator,
                          _mazeStringConstruction[i + 1].SetIterator);
            }
        }


        private void BottomWalls(List<Cell4Dir> _mazeStringConstruction, Random random)
        {
            for (int i = 0; i < XLength; i++)
            {
                if (random.NextDouble() >= 0.5 &&
                    (from x in _mazeStringConstruction
                     where x.SetIterator == _mazeStringConstruction[i].SetIterator && x.BottomWall == false
                     select x).Count() > 1)
                {
                    _mazeStringConstruction[i].BottomWall = true;
                }
            }
        }


        private void NextStringPrepare(List<Cell4Dir> _mazeStringConstruction)
        {
            for (int i = 0; i < XLength; i++)
            {
                _mazeStringConstruction[i].RightWall = false;
                _mazeStringConstruction[i].LeftWall = false;

                if (_mazeStringConstruction[i].BottomWall == true)
                {
                    _mazeStringConstruction[i].SetIterator = 0;

                    _mazeStringConstruction[i].BottomWall = false;                   
                }
            }
        }


        private void LastString(List<Cell4Dir> _mazeStringConstruction)
        {
            for (int i = 0; i < XLength - 1; i++)
            {
                if(_mazeStringConstruction[i].SetIterator != _mazeStringConstruction[i + 1].SetIterator)
                { 
                    _mazeStringConstruction[i].RightWall = false;
                    _mazeStringConstruction[i + 1].LeftWall = false;
                    MergeSets(_mazeStringConstruction,
                              _mazeStringConstruction[i].SetIterator,
                              _mazeStringConstruction[i + 1].SetIterator);
                }
            }
        }


        private void CreateMazeBorders(List<List<Cell4Dir>> maze)
        {
            for (int i = 0; i < XLength; i++)
            {
                maze[0][i].UpperWall = true;
                maze[YLength - 1][i].BottomWall = true;
            }

            for (int i = 0; i < YLength; i++)
            {
                maze[i][0].LeftWall = true;
                maze[i][XLength - 1].RightWall = true;
            }

            for (int i = 1; i < YLength; i++)
            {
                for (int j = 0; j < XLength; j++)
                {
                    if (maze[i - 1][j].BottomWall)
                    {
                        maze[i][j].UpperWall = true;
                    }
                }
            }
        }


        private void MergeSets(List<Cell4Dir> _mazeStringConstruction, long setIterator1, long setIterator2)
        {
            for (int i = 0; i < XLength; i++)
            {
                if (_mazeStringConstruction[i].SetIterator == setIterator2)
                {
                    _mazeStringConstruction[i].SetIterator = setIterator1;
                }
            }
        }

    }
}

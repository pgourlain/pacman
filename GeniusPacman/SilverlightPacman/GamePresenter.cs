using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GeniusPacman.Core;
using GeniusPacman.Core.Sprites;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SilverlightPacman
{
    /// <summary>
    /// this class is the model of view for game 
    /// </summary>
    public class GamePresenter : BasePresenter
    {
        PacmanPresenter _pacman;
        Dictionary<Point, Geometry> _pillCoord = new Dictionary<Point, Geometry>();
        Labyrinth _Labyrinth;
        GhostPresenter[] _ghosts = new GhostPresenter[4];
        BonusPresenter _BonusPresenter;

        public GamePresenter()
        {
            
            _CurrentGame.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_CurrentGame_PropertyChanged);
            _CurrentGame.Timer = new WPFPacmanTimer();
            _pacman = new PacmanPresenter(CurrentGame);
            _BonusPresenter = new BonusPresenter(CurrentGame);
            for (int i = 0; i < 4; i++)
                _ghosts[i] = new GhostPresenter(CurrentGame, i);
        }

        void _CurrentGame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentLabyrinth")
            {
                if (_Labyrinth != null)
                    _Labyrinth.PillEated -= new PillEatedDelegate(_Labyrinth_PillEated);
                _Labyrinth = _CurrentGame.CurrentLabyrinth;
                if (_Labyrinth != null)
                    _Labyrinth.PillEated += new PillEatedDelegate(_Labyrinth_PillEated);
                ConstructWallAndPills(_CurrentGame.CurrentLabyrinth);
            }
            else if (e.PropertyName == "Status")
            {
                NotifyPropertyChanged("IsReady");
                NotifyPropertyChanged("IsGameOver");
                //NotifyPropertyChanged(e.PropertyName);
                //Debug.WriteLine("_CurrentGame_PropertyChanged : " + e.PropertyName);
            }
        }

        void _Labyrinth_PillEated(object sender, int x, int y)
        {
            Geometry pill;

            if (_pillCoord.TryGetValue(new Point(x, y), out pill))
            {
                this._Pills.Children.Remove(pill);
                NotifyPropertyChanged("Pills");
            }
        }

        private GeometryGroup _Wall = new GeometryGroup();
        /// <summary>
        /// wall geomety of current level
        /// </summary>
        public GeometryGroup Wall
        {
            get { return _Wall; }
            set
            {
                if (value != _Wall)
                {
                    _Wall = value;
                    NotifyPropertyChanged("Wall");
                }
            }
        }

        private GeometryGroup _Pills = new GeometryGroup();
        /// <summary>
        /// pills of current level
        /// </summary>
        public GeometryGroup Pills
        {
            get { return _Pills; }
            set
            {
                if (value != _Pills)
                {
                    _Pills = value;
                    NotifyPropertyChanged("Pills");
                }
            }
        }

        /// <summary>
        /// returns true if game is in ready mode
        /// </summary>
        public bool IsReady
        {
            get { return CurrentGame.Status == GameMode.READY; }
        }

        /// <summary>
        /// returns true if game is in game over mode
        /// </summary>
        public bool IsGameOver
        {
            get { return CurrentGame.Status == GameMode.GAME_OVER; }
        }


        private Game _CurrentGame = new Game();
        /// <summary>
        /// current game, it used for databinding
        /// </summary>
        public Game CurrentGame
        {
            get
            {
                return _CurrentGame;
            }
        }

        /// <summary>
        /// presenter of ghost 1
        /// </summary>
        public GhostPresenter Ghost1
        {
            get
            {
                return _ghosts[0];
            }
        }

        /// <summary>
        /// presenter of ghost 2
        /// </summary>
        public GhostPresenter Ghost2
        {
            get
            {
                return _ghosts[1];
            }
        }

        /// <summary>
        /// presenter of ghost 3
        /// </summary>
        public GhostPresenter Ghost3
        {
            get
            {
                return _ghosts[2];
            }
        }

        /// <summary>
        /// presenter of ghost 4
        /// </summary>
        public GhostPresenter Ghost4
        {
            get
            {
                return _ghosts[3];
            }
        }

        /// <summary>
        /// presenter for pacman
        /// </summary>
        public PacmanPresenter PacmanPresenter
        {
            get
            {
                return _pacman;
            }
        }

        /// <summary>
        /// presenter for bonus
        /// </summary>
        public BonusPresenter BonusPresenter
        {
            get
            {
                return _BonusPresenter;
            }
        }

        bool _isMuted = false;
        public bool IsMuted
        {
            get
            {
                return _isMuted;
            }
            set
            {
                if (_isMuted != value)
                {
                    _isMuted = value;
                    NotifyPropertyChanged("IsMuted");
                }
            }
        }

        #region private methods
        /// <summary>
        /// construct geometry of wall and pills for a specific level
        /// </summary>
        /// <param name="newValue"></param>
        private void ConstructWallAndPills(Labyrinth newValue)
        {
            this.Wall.Children.Clear();
            this.Pills.Children.Clear();
            _pillCoord.Clear();
            if (newValue == null)
                return;
            for (int i = 0; i < Labyrinth.WIDTH; i++)
            {
                for (int j = 0; j < Labyrinth.HEIGHT; j++)
                {
                    byte b = newValue[i, j];
                    if (b == 0)
                        continue;
                    int x, y;
                    newValue.GetScreenCoord(i, j, out x, out y);
                    switch (b)
                    {
                        case 1://pill
                            EllipseGeometry p = new EllipseGeometry();
                            p.Center = new Point(x + Constants.GRID_WIDTH_2, y + Constants.GRID_HEIGHT_2);
                            p.RadiusX = 2;
                            p.RadiusY = 2;
                            this.Pills.Children.Add(p);
                            _pillCoord.Add(new Point(i, j), p);
                            break;
                        case 2://super pill
                            RectangleGeometry superPill = new RectangleGeometry();
                            superPill.Rect = new Rect(x + Constants.GRID_WIDTH_4, y + Constants.GRID_WIDTH_4, 8, 8);
                            this.Pills.Children.Add(superPill);
                            _pillCoord.Add(new Point(i, j), superPill);
                            break;
                        case 3: //gate of ghost's house
                            break;
                        case 4://wall
                            Geometry current = GetGeometry(i, j, x, y, newValue);
                            //if (current == null)
                            //{
                            //    RectangleGeometry r = new RectangleGeometry();
                            //    //if (i == 0)
                            //    //    r.Rect = new Rect(x + Constants.GRID_WIDTH_2, y, Constants.GRID_WIDTH_2, Constants.GRID_HEIGHT);
                            //    //else
                            //    r.Rect = new Rect(x, y, Constants.GRID_WIDTH, Constants.GRID_HEIGHT);
                            //    current = r;
                            //}
                            if (current != null)
                                this.Wall.Children.Add(current);
                            break;
                    }
                }
            }

        }

        private Geometry GetGeometry(int i, int j, int xScreen, int yScreen, Labyrinth laby)
        {
            if (i == 0 && j == 0)
            {
                PathGeometry path = new PathGeometry();
                PathFigure pf = new PathFigure() { IsClosed = true, IsFilled = true };
                path.Figures.Add(pf);
                pf.StartPoint = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT);
                pf.Segments.Add(new ArcSegment()
                {
                    Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
                    Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT_2),
                    SweepDirection = SweepDirection.Clockwise
                });
                pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT) });
                return path;
            }
            else
            {
                RectangleGeometry r;
                PathGeometry corner;
                PathFigure pf;
                int OffsetX;
                int OffsetY;
                WallSide side = GetSide(i, j, laby);
                switch (side)
                {
                    case WallSide.InnerTopLeft :
                    case WallSide.TopLeft :
                        OffsetX = side == WallSide.InnerTopLeft ? Constants.GRID_WIDTH_2 : 0;
                        OffsetY = side == WallSide.InnerTopLeft ? Constants.GRID_HEIGHT_2 : 0;
                        corner = new PathGeometry();
                        pf = new PathFigure() { IsClosed = true, IsFilled = true };
                        corner.Figures.Add(pf);
                        pf.StartPoint = new Point(xScreen + OffsetX, yScreen + Constants.GRID_HEIGHT_2 + OffsetY);
                        pf.Segments.Add(new ArcSegment()
                        {
                            Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
                            Point = new Point(xScreen + Constants.GRID_WIDTH_2+OffsetX, yScreen + OffsetY),
                            SweepDirection = SweepDirection.Clockwise
                        });
                        if (side == WallSide.InnerTopLeft)
                        {
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT) });
                        }
                        else
                        {
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT_2) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT_2) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT) });
                        }
                        return corner;
                        break;
                    case WallSide.InnerTopRight:
                    case WallSide.TopRight :
                        OffsetX = side == WallSide.InnerTopRight ? -Constants.GRID_WIDTH_2 : 0;
                        OffsetY = side == WallSide.InnerTopRight ? Constants.GRID_HEIGHT_2 : 0;
                        corner = new PathGeometry();
                        pf = new PathFigure() { IsClosed = true, IsFilled = true };
                        corner.Figures.Add(pf);
                        pf.StartPoint = new Point(xScreen + Constants.GRID_WIDTH_2 + OffsetX, yScreen + OffsetY);
                        pf.Segments.Add(new ArcSegment()
                        {
                            Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
                            Point = new Point(xScreen + Constants.GRID_WIDTH + OffsetX, yScreen + Constants.GRID_HEIGHT_2 + OffsetY),
                            SweepDirection = SweepDirection.Clockwise
                        });
                        if (side == WallSide.InnerTopRight)
                        {
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT) });
                        }
                        else
                        {
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT_2) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT_2) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen) });
                        }
                        return corner;
                        break;
                    case WallSide.InnerBottomRight:
                    case WallSide.BottomRight:
                        OffsetX = side == WallSide.InnerBottomRight ? -Constants.GRID_WIDTH_2 : 0;
                        OffsetY = side == WallSide.InnerBottomRight ? -Constants.GRID_HEIGHT_2 : 0;
                        corner = new PathGeometry();
                        pf = new PathFigure() { IsClosed = true, IsFilled = true };
                        corner.Figures.Add(pf);
                        pf.StartPoint = new Point(xScreen + Constants.GRID_WIDTH + OffsetX, yScreen + Constants.GRID_HEIGHT_2 + OffsetY);
                        pf.Segments.Add(new ArcSegment()
                        {
                            Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
                            Point = new Point(xScreen + Constants.GRID_WIDTH_2 + OffsetX, yScreen + Constants.GRID_HEIGHT + OffsetY),
                            SweepDirection = SweepDirection.Clockwise
                        });
                        if (side == WallSide.InnerBottomRight)
                        {
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen) });
                        }
                        else
                        {
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen + Constants.GRID_HEIGHT_2) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT_2) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen) });
                        }
                        return corner;
                        break;
                    case WallSide.InnerBottomLeft :
                    case WallSide.BottomLeft:
                        OffsetX = side == WallSide.InnerBottomLeft ? Constants.GRID_WIDTH_2 : 0;
                        OffsetY = side == WallSide.InnerBottomLeft ? -Constants.GRID_HEIGHT_2 : 0;
                        corner = new PathGeometry();
                        pf = new PathFigure() { IsClosed = true, IsFilled = true };
                        corner.Figures.Add(pf);
                        pf.StartPoint = new Point(xScreen + Constants.GRID_WIDTH_2 + OffsetX, yScreen + Constants.GRID_HEIGHT + OffsetY);
                        pf.Segments.Add(new ArcSegment()
                        {
                            Size = new Size(Constants.GRID_WIDTH_2, Constants.GRID_WIDTH_2),
                            Point = new Point(xScreen + OffsetX, yScreen + Constants.GRID_HEIGHT_2 + OffsetY),
                            SweepDirection = SweepDirection.Clockwise
                        });
                        if (side == WallSide.InnerBottomLeft)
                        {
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen) });
                        }
                        else
                        {
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen, yScreen) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH_2, yScreen + Constants.GRID_HEIGHT_2) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT_2) });
                            pf.Segments.Add(new LineSegment() { Point = new Point(xScreen + Constants.GRID_WIDTH, yScreen + Constants.GRID_HEIGHT) });
                        }
                        return corner;
                        break;
                    case WallSide.Left:
                        r = new RectangleGeometry();
                        r.Rect = new Rect(xScreen, yScreen, Constants.GRID_WIDTH_2, Constants.GRID_HEIGHT);
                        return r;
                        break;
                    case WallSide.Right:
                        r = new RectangleGeometry();
                        r.Rect = new Rect(xScreen + Constants.GRID_WIDTH_2, yScreen, Constants.GRID_WIDTH_2, Constants.GRID_HEIGHT);
                        return r;
                    case WallSide.Top :
                        r = new RectangleGeometry();
                        r.Rect = new Rect(xScreen, yScreen, Constants.GRID_WIDTH, Constants.GRID_HEIGHT_2);
                        return r;
                    case WallSide.Bottom:
                        r = new RectangleGeometry();
                        r.Rect = new Rect(xScreen, yScreen + Constants.GRID_HEIGHT_2, Constants.GRID_WIDTH, Constants.GRID_HEIGHT_2);
                        return r;
                    default:
                        return null;
                }
            }
            return null;
        }

        enum WallSide {None, TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, InnerTopLeft, InnerTopRight, InnerBottomRight, InnerBottomLeft };


        private WallSide GetSide(int i, int j, Labyrinth laby)
        {
            //first column
            if (i == 0)
            {
                if (j == 0)
                    return WallSide.InnerTopLeft;
                if (j == Labyrinth.HEIGHT - 1)
                    return WallSide.InnerBottomLeft;
                if (laby[i + 1, j] == 4 && laby[i, j - 1] == 4 && laby[i + 1, j - 1] != 4)
                    return WallSide.InnerBottomLeft;
                if (laby[i + 1, j] == 4 && laby[i, j + 1] == 4 && laby[i + 1, j + 1] != 4)
                    return WallSide.InnerTopLeft;
                if (laby[i, j-1] == 4 && laby[i, j+1] == 4 && (laby[i+1, j] == 4 || laby[i+1, j] == 5 || laby[i+1, j] == 0))
                    return WallSide.None;
                return WallSide.Right;
            }
            //last column
            else if (i == Labyrinth.WIDTH - 1)
            {
                if (j == 0)
                    return WallSide.InnerTopRight;
                if (j == Labyrinth.HEIGHT - 1)
                    return WallSide.InnerBottomRight;
                if (laby[i - 1, j] == 4 && laby[i, j - 1] == 4 && laby[i - 1, j - 1] != 4)
                    return WallSide.InnerBottomRight;
                if (laby[i - 1, j] == 4 && laby[i, j + 1] == 4 && laby[i - 1, j + 1] != 4)
                    return WallSide.InnerTopRight;

                return WallSide.Left;
            }
            //first row
            else if (j == 0)
            {
                if (laby[i + 1, j + 1] != 4 && laby[i, j + 1] == 4)
                    return WallSide.InnerTopLeft;
                if (laby[i - 1, j + 1] != 4 && laby[i, j + 1] == 4)
                    return WallSide.InnerTopRight;
                if (laby[i - 1, j] == 4 && laby[i+1, j] == 4 && (laby[i, j+1] == 4 || laby[i, j+1] == 0))
                    return WallSide.None;

                return WallSide.Bottom;
            }
            //last row
            else if (j == Labyrinth.HEIGHT - 1)
            {
                if (laby[i - 1, j - 1] != 4 && laby[i, j - 1] == 4)
                    return WallSide.InnerBottomRight;
                if (laby[i + 1, j - 1] != 4 && laby[i, j - 1] == 4)
                    return WallSide.InnerBottomLeft;
                if (laby[i - 1, j] == 4 && laby[i + 1, j] == 4 && (laby[i, j - 1] == 4 || laby[i, j - 1] == 0))
                    return WallSide.None;
                return WallSide.Top;
            }
            else if (j > 0 && j < Labyrinth.HEIGHT - 1 && i > 0 && i < Labyrinth.WIDTH - 1)
            {
                //Corners
                if (laby[i - 1, j - 1] != 4 && laby[i - 1, j] != 4 && laby[i, j - 1] != 4)
                    return WallSide.TopLeft;
                if (laby[i + 1, j - 1] != 4 && laby[i + 1, j] != 4 && laby[i, j - 1] != 4)
                    return WallSide.TopRight;
                if (laby[i + 1, j + 1] != 4 && laby[i + 1, j] != 4 && laby[i, j + 1] != 4)
                    return WallSide.BottomRight;
                if (laby[i - 1, j + 1] != 4 && laby[i - 1, j] != 4 && laby[i, j + 1] != 4)
                    return WallSide.BottomLeft;
                //Top and Bottom
                if (laby[i, j - 1] != 4 && laby[i, j - 1] != 5)
                    return WallSide.Top;
                if (laby[i, j + 1] != 4)
                    return WallSide.Bottom;
                //Left and Right
                if (laby[i - 1, j] != 4 && laby[i-1, j] != 5)
                    return WallSide.Left;
                if (laby[i + 1, j] != 4)
                    return WallSide.Right;
                //inner corners
                if (laby[i - 1, j - 1] != 4)
                    return WallSide.InnerBottomRight;
                if (laby[i + 1, j + 1] != 4)
                    return WallSide.InnerTopLeft;

                if (laby[i - 1, j + 1] != 4)
                    return WallSide.InnerTopRight;

                if (laby[i + 1, j - 1] != 4)
                    return WallSide.InnerBottomLeft;

            }
            return WallSide.None;
        }

        #endregion
    }
}

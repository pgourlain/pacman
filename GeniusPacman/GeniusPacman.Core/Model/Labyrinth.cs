﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace GeniusPacman.Core
{
    public delegate void PillEatedDelegate(object sender, int x, int y);

    public class Labyrinth : Notifiable
    {
        public const int HEIGHT = 31;
        public const int WIDTH = 28;
        public const int HOUSE_X = 14;
        public const int HOUSE_Y = 14;
        private static Labyrinth instance = null;

        private byte[][] laby;
        // nombre de pastille
        private int _RemainingPills;
        // numéro du laby courant
        private int num;

        public event PillEatedDelegate PillEated;

        private Labyrinth(int num)
        {
            this.num = num;
            laby = new byte[HEIGHT][];

            RemainingPills = pastilles[num];
            for (int i = 0; i < HEIGHT; i++)
            {
                laby[i] = new byte[WIDTH];
                for (int j = 0; j < WIDTH; j++)
                {
                    laby[i][j] = labys[num][i][j];
                }
            }
        }

        public static Labyrinth getCurrent()
        {
            return instance;
        }

        public static void SetCurrent(int num)
        {
            instance = new Labyrinth(num);
        }

        public int RemainingPills
        {
            get
            {
                return _RemainingPills;
            }
            set
            {
                if (_RemainingPills != value)
                {
                    _RemainingPills = value;
                    DoPropertyChanged("RemainingPills");
                }
            }
        }

        public byte get(int x, int y)
        {
            return laby[y][x];
        }

        public byte this[int x, int y]
        {
            get
            {
                return laby[y][x];
            }
        }

        internal void Eat(int x, int y)
        {
            if (laby[y][x] == 1 || laby[y][x] == 2)
            {
                RemainingPills--;
                laby[y][x] = 0;
                if (PillEated != null)
                    PillEated(this, x, y);
            }
            else
            {
                Debug.WriteLine("WARNING : Can't eat pil at x=" + x + ", y=" + y);
            }

        }

        // teste si un sprite peut se déplacer dans une direction
        internal byte getWallAt(int fromX, int fromY, Direction direction)
        {
            byte wall;
            if (direction.isUp())
            {
                fromY--;
            }
            else if (direction.isRight())
            {
                fromX++;
            }
            else if (direction.isDown())
            {
                fromY++;
            }
            else if (direction.isLeft())
            {
                fromX--;
            }
            if (fromX < 0 || fromX >= WIDTH || fromY < 0 || fromY > HEIGHT)
            {
                wall = 0;
            }
            else
            {
                wall = get(fromX, fromY);
            }
            return wall;
        }

        public void GetScreenCoord(int xLaby, int yLaby, out int X, out int Y)
        {
            X = xLaby * Constants.GRID_WIDTH;
            Y = yLaby * Constants.GRID_HEIGHT;
        }

        public void GetScreenCoord(double xLaby, double yLaby, out double X, out double Y)
        {
            X = xLaby * Constants.GRID_WIDTH;
            Y = yLaby * Constants.GRID_HEIGHT;
        }
        #region static boards

        private static int[] pastilles = { 244, 260, 284, 242 };

        // 0 -> nothing
        // 1 -> pill
        // 2 -> super pill
        // 3 -> gate of ghost's house
        // 4 -> wall
        private static byte[][][] labys = 
		{
	// laby 0
		new byte[][] {
				new byte[]{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
			, 	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 2, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 2, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4}
			,	new byte[]{5, 5, 5, 5, 5, 4, 1, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 4, 1, 4, 5, 5, 5, 5, 5}
			,	new byte[]{5, 5, 5, 5, 5, 4, 1, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 1, 4, 5, 5, 5, 5, 5}
			,	new byte[]{5, 5, 5, 5, 5, 4, 1, 4, 4, 0, 4, 4, 4, 3, 3, 4, 4, 4, 0, 4, 4, 1, 4, 5, 5, 5, 5, 5}
			,	new byte[]{4, 4, 4, 4, 4, 4, 1, 4, 4, 0, 4, 4, 4, 3, 3, 4, 4, 4, 0, 4, 4, 1, 4, 4, 4, 4, 4, 4}
			,	new byte[]{0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 4, 4, 0, 0, 0, 0, 4, 4, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0}
			,	new byte[]{4, 4, 4, 4, 4, 4, 1, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 1, 4, 4, 4, 4, 4, 4}
			,	new byte[]{5, 5, 5, 5, 5, 4, 1, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 1, 4, 5, 5, 5, 5, 5}
			,	new byte[]{5, 5, 5, 5, 5, 4, 1, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 1, 4, 5, 5, 5, 5, 5}
			,	new byte[]{5, 5, 5, 5, 5, 4, 1, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 1, 4, 5, 5, 5, 5, 5}
			,	new byte[]{4, 4, 4, 4, 4, 4, 1, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 1, 4, 4, 4, 4, 4, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 2, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 2, 4}
			,	new byte[]{4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4}
			,	new byte[]{4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
		}
		,
	// laby 1
		new byte[][] {
				new byte[]{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 4, 1, 1, 1, 1, 1, 1, 4}
			, 	new byte[]{4, 2, 4, 4, 4, 4, 1, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 1, 4, 4, 4, 4, 2, 4}
			, 	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			, 	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{0, 0, 0, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 0, 0, 0}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 4}
			, 	new byte[]{4, 1, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 3, 3, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 1, 4}
			, 	new byte[]{4, 1, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 3, 3, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 1, 4}
			, 	new byte[]{4, 1, 1, 1, 4, 4, 1, 1, 0, 0, 4, 4, 0, 0, 0, 0, 4, 4, 0, 0, 1, 1, 4, 4, 1, 1, 1, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 1, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 1, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 1, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 1, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{0, 0, 0, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 1, 1, 1, 1, 0, 0, 0}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 4}
			, 	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			, 	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			, 	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			, 	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			, 	new byte[]{4, 2, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 2, 4}
			, 	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			, 	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			, 	new byte[]{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
		}
		,
		// laby 2
		new byte[][] {
				new byte[]{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
			, 	new byte[]{0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0}
			,	new byte[]{4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 2, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 2, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4}
			,	new byte[]{4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4}
			,	new byte[]{4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 0, 4, 4, 4, 3, 3, 4, 4, 4, 0, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 0, 4, 4, 4, 3, 3, 4, 4, 4, 0, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 4, 4, 0, 4, 4, 0, 0, 0, 0, 4, 4, 0, 4, 4, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 1, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 1, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4}
			,	new byte[]{4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4}
			,	new byte[]{0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0}
			,	new byte[]{4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 4}
			,	new byte[]{4, 2, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 2, 4}
			,	new byte[]{4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4}
			,	new byte[]{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
			}
		,
	// laby 3
		new byte[][] {
				new byte[]{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 2, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 2, 4}
			,	new byte[]{4, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 1, 1, 1, 4}
			,	new byte[]{4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4}
			,	new byte[]{4, 4, 4, 4, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 4, 4, 4, 4}
			,	new byte[]{0, 1, 1, 1, 1, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 1, 1, 1, 1, 0}
			,	new byte[]{4, 1, 4, 4, 0, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 0, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 3, 3, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 3, 3, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 0, 0, 0, 0, 0, 4, 4, 0, 4, 4, 0, 0, 0, 0, 4, 4, 0, 4, 4, 0, 0, 0, 0, 0, 1, 4}
			,	new byte[]{4, 1, 4, 4, 0, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 0, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 0, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 0, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4}
			,	new byte[]{4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 1, 4, 4, 4}
			,	new byte[]{4, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 2, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 1, 4, 4, 4, 4, 2, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 1, 4, 4, 1, 4, 4, 4, 4, 1, 4}
			,	new byte[]{4, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1, 1, 1, 1, 4}
			,	new byte[]{4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4}
			}
		};
        #endregion

    }

}

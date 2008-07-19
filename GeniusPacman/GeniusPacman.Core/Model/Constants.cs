using System;
using System.Collections.Generic;
using System.Text;

namespace GeniusPacman.Core
{
    public sealed class Constants
    {
        // grille
        public const int GRID_HEIGHT = 20;
        public const int GRID_WIDTH = 20;

        //distance pour la colision
        public const int COLISION_DIST = GRID_WIDTH + 2;

        public const int GRID_HEIGHT_2 = GRID_HEIGHT / 2;
        public const int GRID_WIDTH_2 = GRID_WIDTH / 2;
        public const int GRID_WIDTH_4 = GRID_WIDTH / 4;
        public const int GRID_WIDTH_X2 = GRID_WIDTH_2 * 2;
        // laby
        public const int DEST_LABY_Y = 3 * GRID_HEIGHT;
        // sprites
        public const int SPRITE_HEIGHT = 2 * GRID_HEIGHT;
        public const int SPRITE_WIDTH = 2 * GRID_WIDTH;
        public const int SRC_SPRITE_X = 0;
        public const int SRC_SPRITE_Y = GRID_HEIGHT;
        // bonus
        public const int BONUS_HEIGHT = 2 * GRID_HEIGHT;
        public const int BONUS_WIDTH = 2 * GRID_WIDTH;
        public const int SRC_BONUS_X = 0;
        public const int SRC_BONUS_Y = 4 * GRID_HEIGHT;
        // points
        public const int POINT_HEIGHT = GRID_HEIGHT;
        public const int POINT_WIDTH = 2 * GRID_WIDTH;
        public const int SRC_POINT_X = 0;
        public const int SRC_POINT_Y = 3 * GRID_HEIGHT;
        // digit
        public const int DIGIT_HEIGHT = GRID_HEIGHT;
        public const int DIGIT_WIDTH = GRID_WIDTH;
        public const int SRC_DIGIT_X = 22 * GRID_WIDTH;
        public const int SRC_DIGIT_Y = 0;
        // scores
        public const int DEST_SCORE_Y = GRID_HEIGHT;
        public const int DEST_SCORE_X = 13 * GRID_WIDTH;
        public const int DEST_BEST_SCORE_X = 27 * GRID_HEIGHT;
        public const int SCORE_HEIGHT = GRID_HEIGHT;
        // gameover
        public const int DEST_GAME_OVER_X = 9 * GRID_WIDTH;
        public const int DEST_GAME_OVER_Y = 20 * GRID_HEIGHT;
        public const int GAME_OVER_WIDTH = 10 * GRID_WIDTH;
        public const int GAME_OVER_HEIGHT = GRID_HEIGHT;
        // ready
        public const int DEST_READY_X = 11 * GRID_WIDTH;
        public const int DEST_READY_Y = 20 * GRID_HEIGHT;
        public const int READY_WIDTH = 6 * GRID_WIDTH;
        public const int READY_HEIGHT = GRID_HEIGHT;
        public const int SRC_READY_X = 0;
        public const int SRC_READY_Y = 0;
    }

    public enum PacmanKey {None, Space,  Pause, Left, Right, Up, Down, Escape, NextLevel};
}

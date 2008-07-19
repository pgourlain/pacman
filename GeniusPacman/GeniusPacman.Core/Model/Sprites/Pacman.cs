using System;
using System.Collections.Generic;
using System.Text;

namespace GeniusPacman.Core.Sprites
{
    public class Pacman : Sprite
    {
        private int past;

        public Pacman()
        {
            Init();
        }

        public void Init()
        {
            X = (13 * Constants.GRID_WIDTH) + Constants.GRID_WIDTH_2;
            Y = 23 * Constants.GRID_HEIGHT;
            setDirection(Direction.Right);
            past = 0;
        }

        private void setDirection(Direction direction)
        {
            this._DesiredDirection = direction;
            CurrentDirection = direction;
        }

        public override int animate()
        {
            int ret = 0;
            int car, nbBouge = 1;

            spriteCount = (spriteCount + 1) % 3;

            // Les changements de direction sont autorisé tout le temps
            if (_DesiredDirection.isOpposite(CurrentDirection))
            {
                CurrentDirection = _DesiredDirection;
            }
            if (X % Constants.GRID_WIDTH == 0 &&
                Y % Constants.GRID_HEIGHT == 0 &&
                X >= 0 &&
                X < Labyrinth.WIDTH * Constants.GRID_WIDTH)
            {
                // Le pacman est pile sur un block
                xLaby = X / Constants.GRID_WIDTH;
                yLaby = Y / Constants.GRID_HEIGHT;
                System.Diagnostics.Debug.WriteLine(string.Format("pos in laby({0},{1})", xLaby, yLaby));
                car = Labyrinth.getCurrent().get(xLaby, yLaby);
                if (car == 1 || car == 2)
                {
                    // le pacman est sur une pastille
                    ret = 1;
                    if (++past == 4)
                    {
                        past = 0;
                        nbBouge--;
                    }
                    // enlève la pastille dans les images écran et labyrinthe
                    Labyrinth.getCurrent().Eat(xLaby, yLaby);
                    if (car == 2)
                    {
                        ret++;
                    }
                }

                if (!_DesiredDirection.Equals(CurrentDirection) &&
                    (car = getWallAt(_DesiredDirection)) < 3)
                {
                    // Le pacman est autorisé a changer de direction
                    if (((CurrentDirection.isLeft() || CurrentDirection.isRight()) &&
                        (_DesiredDirection.isUp() || _DesiredDirection.isDown())) ||
                        ((CurrentDirection.isUp() || CurrentDirection.isDown()) &&
                        (_DesiredDirection.isLeft() || _DesiredDirection.isRight())))
                    {
                        nbBouge++;
                    }
                    CurrentDirection = _DesiredDirection;
                }
                if (getWallAt(CurrentDirection) > 2)
                {
                    nbBouge = 0;
                    // le pacman ne bouge pas, afficher toujours le même sprite
                    spriteCount = 1;
                }
            }
            // notify that pacman is about to move or moves again
            InMove = nbBouge > 0;
            System.Diagnostics.Debug.WriteLine("nbbouge : " + CurrentDirection.ToEnum().ToString());
            int currentX = X;
            int currentY = Y;
            while (--nbBouge >= 0)
            {
                switch (CurrentDirection.ToEnum())
                {
                    case DirectionEnum.Up:
                        currentY -= Constants.GRID_WIDTH_4;
                        break;
                    case DirectionEnum.Right:
                        currentX += Constants.GRID_WIDTH_4;
                        if (currentX >= (Labyrinth.WIDTH * Constants.GRID_WIDTH + Constants.GRID_WIDTH_X2))
                        {
                            currentX = -Constants.GRID_WIDTH_X2;
                        }
                        break;
                    case DirectionEnum.Down:
                        currentY += Constants.GRID_WIDTH_4;
                        break;
                    case DirectionEnum.Left:
                        currentX -= Constants.GRID_WIDTH_4;
                        if (currentX <= -Constants.GRID_WIDTH_X2)
                        {
                            currentX = Labyrinth.WIDTH * Constants.GRID_WIDTH + Constants.GRID_WIDTH_X2;
                        }
                        break;
                }
            }
            System.Diagnostics.Debug.WriteLine(string.Format("pacman (x,y) : ({0},{1})", currentX, currentY));
            X = currentX;
            Y = currentY;
            return ret;
        }

        public void Right()
        {
            _DesiredDirection = Direction.Right;
        }

        public void Up()
        {
            _DesiredDirection = Direction.Up;
        }

        public void Left()
        {
            _DesiredDirection = Direction.Left;
        }

        public void Down()
        {
            _DesiredDirection = Direction.Down;
        }
    }
}

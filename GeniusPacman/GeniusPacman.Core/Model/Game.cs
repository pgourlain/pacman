using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using GeniusPacman.Core.Sprites;
using GeniusPacman.Core.Interfaces;
using System.Collections.ObjectModel;

namespace GeniusPacman.Core
{
    public enum GameMode {GAME_OVER, READY, PLAY, DEATH};
    // Classe de gestion du jeu
    public class Game : Notifiable
    {
        public const bool DEBUG = false;
        private const int FANTOME_COUNT = DEBUG ? 1 : 4;
        private const int FANTOME_FIRST = DEBUG ? 3 : 0;
        private const int INITIAL_SLEEP_TIME = DEBUG ? 40 : 40;

        #region fields
        // le jeu est t-il en pause ?
        private bool _paused = false;

        // compteur de tour de jeu pour un état donné (game over, ready, ...)
        private int turn = 0;

        // état du jeu
        private GameMode _mode = GameMode.GAME_OVER;

        //TODO: use a level manager
        // index of labyrinth in current use
        private int numLaby = 0;

        // Vitesse du jeu
        private int _sleepTime = INITIAL_SLEEP_TIME;
        private int _level = 1;

        // nombre de vies
        private int _lives;

        // objet pacman et fantomes
        private Pacman _Pacman = null;
        private List<Ghost> _Ghosts;
        private Bonus _Bonus = null;

        // liste des points évoluant vers le haut
        private ObservableCollection<FlyingScore> _FlyingScores = null;

        /// <summary>
        /// current score
        /// </summary>
        private int _Score = 0;
        /// <summary>
        /// high socre
        /// </summary>
        private int _HighScore = 0;
        /// <summary>
        /// score to obtain an another life
        /// </summary>
        private int _LifeScore;
        /// <summary>
        /// points when you eat a ghost
        /// </summary>
        private int _GhostScore;
        /// <summary>
        /// timer to animate the game
        /// </summary>
        private IPacmanTimer _Timer;
        #endregion

        public Game()
        {
            _FlyingScores = new ObservableCollection<FlyingScore>();
            SetMode(GameMode.GAME_OVER);
        }

        #region properties

        /// <summary>
        /// You must provide a timer to animate the game
        /// </summary>
        public IPacmanTimer Timer
        {
            get
            {
                return _Timer;
            }
            set
            {
                if (_Timer != value)
                {
                    if (_Timer != null)
                    {
                        _Timer.Tick -= new EventHandler(_Timer_Tick);
                    }
                    _Timer = value;
                    if (_Timer != null)
                    {
                        _Timer.Tick += new EventHandler(_Timer_Tick);
                        _Timer.Elapsed = SleepTime;
                    }
                    DoPropertyChanged("Timer");
                }
            }
        }

        void _Timer_Tick(object sender, EventArgs e)
        {
            //TODO: synchronize with UI thread, user will provide a delegate
            Animate();
        }

        /// <summary>
        /// Your score
        /// </summary>
        public int Score
        {
            get
            {
                return _Score;
            }
            private set
            {
                if (_Score != value)
                {
                    _Score = value;
                    DoPropertyChanged("Score");
                }
            }
        }

        /// <summary>
        /// High score
        /// </summary>
        public int HighScore
        {
            get
            {
                return _HighScore;
            }
            private set
            {
                if (_HighScore != value)
                {
                    _HighScore = value;
                    DoPropertyChanged("HighScore");
                }                
            }
        }

        /// <summary>
        /// Score to obtain one more life
        /// </summary>
        public int LifeScore
        {
            get
            {
                return _LifeScore;
            }
            private set
            {
                if (value != _LifeScore)
                {
                    _LifeScore = value;
                    DoPropertyChanged("LifeScore");
                }
            }
        }

        /// <summary>
        /// how points you win, when you eat a ghost
        /// </summary>
        public int FantScore
        {
            get
            {
                return _GhostScore;
            }
            private set
            {
                if (value != _GhostScore)
                {
                    _GhostScore = value;
                    DoPropertyChanged("FantScore");
                }
            }
        }

        /// <summary>
        /// your lifes
        /// </summary>
        public int Lives
        {
            get
            {
                return _lives;
            }
            private set
            {
                if (value != _lives)
                {
                    _lives = value;
                    DoPropertyChanged("Lives");
                }
            }
        }

        /// <summary>
        /// true when game is paused
        /// </summary>
        public bool Paused
        {
            get
            {
                return _paused;
            }
            private set
            {
                if (value != _paused)
                {
                    _paused = value;
                    DoPropertyChanged("Paused");
                }
            }
        }

        /// <summary>
        /// level of current game
        /// </summary>
        public int Level
        {
            get
            {
                return _level;
            }
            private set
            {
                if (_level != value)
                {
                    _level = value;
                    DoPropertyChanged("Level");
                }
            }
        }

        /// <summary>
        /// the elapsed time used to animate the game
        /// </summary>
        public int SleepTime
        {
            get
            {
                return _sleepTime;
            }
            private set
            {
                if (value != _sleepTime)
                {
                    _sleepTime = value;
                    if (_Timer != null)
                        _Timer.Elapsed = value;
                    DoPropertyChanged("SleepTime");
                }
            }
        }

        /// <summary>
        /// status of the game (game over, ready, playing...)
        /// </summary>
        public GameMode Status
        {
            get
            {
                return _mode;
            }
            private set
            {
                if (_mode != value)
                {
                    _mode = value;
                    DoPropertyChanged("Status");
                }
            }
        }

        public Pacman PacMan
        {
            get
            {
                return _Pacman;
            }
            private set
            {
                if (value != _Pacman)
                {
                    _Pacman = value;
                    DoPropertyChanged("PacMan");
                }
            }
        }

        public List<Ghost> Ghosts
        {
            get
            {
                return _Ghosts;
            }
            private set
            {
                if (_Ghosts != value)
                {
                    _Ghosts = value;
                    DoPropertyChanged("Ghosts");
                }
            }
        }

        public Bonus CurrentBonus
        {
            get
            {
                return _Bonus;
            }
            private set
            {
                if (_Bonus != value)
                {
                    _Bonus = value;
                    DoPropertyChanged("CurrentBonus");
                }
            }
        }

        public int Width
        {
            get
            {
                return Constants.GRID_WIDTH * Labyrinth.WIDTH;
            }
        }

        public int Height
        {
            get
            {
                return Constants.GRID_HEIGHT * Labyrinth.HEIGHT;
            }
        }

        public Labyrinth CurrentLabyrinth
        {
            get
            {
                return Labyrinth.getCurrent();
            }
        }

        public ObservableCollection<FlyingScore> FlyingScores
        {
            get
            {
                return _FlyingScores;
            }
        }
        #endregion

        internal static int GetRandom(int max)
        {
            return (int)(new Random().NextDouble() * (double)max);
        }

        #region private methods
        private void DrawReady()
        {
            //draw.drawReady
        }

        // Le jeu est dans l'état GAMEOVER
        private void AnimateGameOver()
        {
            //draw.drawBack();
            //drawScore();
        }

        // Le jeu est dans l'état READY
        private void AnimateReady()
        {
            if (turn == 20)
            {
                // passe en mode de jeu
                //draw.clearReady();
                SetMode(GameMode.PLAY);
            }
            //draw.drawBack();
            DrawScore();
        }

        // Le jeu est dans l'état PLAY
        private void AnimatePlay()
        {
            if (Labyrinth.getCurrent().RemainingPills <= 0)
            {
                // toutes les pastilles ont été mangées, changement de niveau
                // accélère un peu le jeu
                Level++;
                SleepTime -= 1;
                numLaby = (numLaby + 1) % 4;
                SetLabyrinth(numLaby);
                SetMode(GameMode.READY);
            }
            else
            {
                int i;
                int xp, yp;
                GhostState state;

                EraseSprites();
                ErasePoints();

                // testes de collisions
                xp = _Pacman.X;
                yp = _Pacman.Y;
                // collisions avec les fantomes
                foreach (Ghost ghost in _Ghosts)
                {
                    state = ghost.State;
                    // teste si le Pacman est en collision avec le Fantome i
                    if (_Pacman.HasCollision(ghost))
                    {
                        if (state.isHunt() || state.isRandom())
                        {
                            // Perdu une vie
                            SetMode(GameMode.DEATH);
                            if (Audio != null)
                                Audio.PlayDeath();
                            //Audio.playDeath();
                            return;
                        }
                        else if (state.isFlee())
                        {
                            // le fantome est mangé
                            ghost.setState(GhostState.EYE);
                            AddScore(_GhostScore);
                            FantScore *= 2;
                            AddPoints(_GhostScore);
                            if (Audio != null)
                                Audio.PlayEatGhost();
                            //Audio.playEatGhost();
                        }
                    }
                }
                // collisions avec le bonus
                if (CurrentBonus != null)
                {
                    // le bonus existe
                    if (_Pacman.HasCollision(CurrentBonus))
                    {
                        int points = Math.Min(1600, Level * 100);
                        AddScore(points);
                        AddPoints(points);
                        CurrentBonus = null;
                        if (Audio != null)
                            Audio.PlayBonus();
                        //Audio.playBonus();
                    }
                    if (CurrentBonus != null && CurrentBonus.animate() <= 0)
                    {
                        // le bonus est en fin de vie
                        CurrentBonus = null;
                    }
                }
                else
                {
                    // pas de bonus
                    if (Game.GetRandom(1000) < 5)
                    {
                        CurrentBonus = new Bonus(Level - 1);
                    }
                }

                // animate pacman
                i = _Pacman.animate();
                if (i == 1)
                {
                    // une pastille est mangée
                    AddScore(10);
                    if (Audio != null)
                        Audio.PlayPill();
                    //Audio.playPil();
                }
                else if (i == 2)
                {
                    FantScore = 100;
                    // Les fantomes qui chasse deviennent fuyards
                    AddScore(50);
                    foreach(Ghost ghost in _Ghosts)
                    {
                        ghost.setState(GhostState.FLEE);
                    }
                }
                // animate fantomes
                //      bool siren = false;
                foreach (Ghost ghost in _Ghosts)
                {
                    ghost.setPacmanCoor(xp, yp);
                    ghost.animate();
                    //        if (!siren && ghosts[i].getState().isHunt()) {
                    //          siren = true;
                    //          Audio.playSiren();
                    //        }
                }
                // animate points
                foreach (FlyingScore pt in _FlyingScores)
                {
                    pt.animate();
                }

                DrawSprites();
                DrawPoints();
                DrawScore();
                DrawLives();
                //if (DEBUG)
                //{
                //    foreach(Ghost ghost in _Ghosts)
                //    {
                //        //draw.drawDebugString(4, 12 + i * 12, 12, ghosts[i].toString());
                //    }
                //}
            }
        }

        private void SetLabyrinth(int numLaby)
        {
            Labyrinth.SetCurrent(numLaby);
            DoPropertyChanged("CurrentLabyrinth");
        }

        // Le jeu est dans l'état DEATH
        private void AnimateDeath()
        {
            if (turn == 70)
            {
                if (Lives == 0)
                {
                    SetMode(GameMode.GAME_OVER);
                }
                else
                {
                    SetMode(GameMode.PLAY);
                }
            }
        }


        private void DrawSprites()
        {
            //if (_Bonus != null)
            //{
            //    _Bonus.draw();
            //}
            //// Affiche le Pacman
            //_Pacman.draw();
            //// Affiche les fantomes
            //foreach (Ghost ghost in _Ghosts)
            //{
            //    ghost.draw();
            //}
        }

        private void EraseSprites()
        {
            //if (_Bonus != null)
            //{
            //    _Bonus.restore();
            //}
            //_Pacman.restore();
            //foreach (Ghost ghost in _Ghosts)
            //{
            //    ghost.restore();
            //}
        }

        private void AddPoints(int points)
        {
            this._FlyingScores.Add(new FlyingScore(_Pacman.X, _Pacman.Y, (points / 100) - 1));
        }

        private void DrawScore()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
            }
            //draw.drawScore(score, meilleurScore);
        }

        private void DrawPoints()
        {

            // Parcours la liste des Points et affiche
            for (int i = 0; i < _FlyingScores.Count; i++)
            {
                FlyingScore point = _FlyingScores[i];
                if (point.count() > 0)
                {
                    //point.draw();
                }
                else
                {
                    _FlyingScores.RemoveAt(i);
                    i--;
                }
            }
        }

        private void ErasePoints()
        {
            //// Parcours la liste des Points et restaure le fond
            //foreach (FlyingScore pt in FlyingScores)
            //{
            //    pt.restore();
            //}
        }

        private void DrawLives()
        {
            //draw.drawLives(lives, numLaby);
        }

        private void AddScore(int num)
        {
            Score += num;
            if (Score > LifeScore)
            {
                Lives++;
                LifeScore += 2 * LifeScore;
            }
        }

        private void Animate()
        {
            // C'est le coeur du jeu
            if (!Paused)
            {
                turn++;
                switch (Status)
                {
                    case GameMode.GAME_OVER:
                        AnimateGameOver();
                        break;
                    case GameMode.READY:
                        AnimateReady();
                        break;
                    case GameMode.PLAY:
                        AnimatePlay();
                        break;
                    case GameMode.DEATH:
                        AnimateDeath();
                        break;
                }
            }
        }
        /// <summary>
        /// changes the game's state
        /// </summary>
        /// <param name="newMode"></param>
        private void SetMode(GameMode newMode)
        {
            turn = 0;
            Status = newMode;
            switch (Status)
            {
                case GameMode.GAME_OVER:
                    // copie le niveau dans le labyrinthe courant
                    numLaby = 0;
                    SetLabyrinth(numLaby);
                    SleepTime = INITIAL_SLEEP_TIME;
                    if (Timer != null && Timer.IsStarted)
                        Timer.Stop();
                    //draw.drawGameOver();
                    //drawScore();
                    break;
                case GameMode.READY:
                    // copie le niveau dans le labyrinthe courant
                    DrawReady();
                    // init du laby courant
                    SetLabyrinth(numLaby);
                    DrawLives();
                    if (Timer == null)
                        throw new Exception("You must provide a timer, fill 'Timer' property");
                    if (!Timer.IsStarted)
                        Timer.Start();
                    break;
                case GameMode.PLAY:
                    //draw.drawBack();
                    if (PacMan == null)
                        PacMan = new Pacman();
                    else
                        PacMan.Init();
                    List<Ghost> tmpList = new List<Ghost>();
                    CurrentBonus = null;
                    for (int i = 0; i < FANTOME_COUNT; i++)
                    {
                        tmpList.Add(new Ghost(FANTOME_FIRST + i));
                    }
                    Ghosts = tmpList;
                    break;
                case GameMode.DEATH:
                    Lives--;
                    _FlyingScores.Clear();
                    if (PacMan != null)
                        PacMan.Dead();
                    break;
            }
        }

        public IAudio Audio { get; set; }
        #endregion

        #region public methods

        /// <summary>
        /// press space to start or pause, Escape to switch in game over mode
        /// </summary>
        /// <param name="key"></param>
        public void KeyPressed(PacmanKey key)
        {
            // Gestion des flèches pour le déplacement du pacman

            if (key == PacmanKey.Pause)
            {
                // touche Pause ou P
                Paused = !Paused;
            }

            if (_Pacman != null)
            {
                switch (key)
                {
                    case PacmanKey.Left :
                        _Pacman.Left();
                        break;
                    case PacmanKey.Right:
                        _Pacman.Right();
                        break;
                    case PacmanKey.Down:
                        _Pacman.Down();
                        break;
                    case PacmanKey.Up:
                        _Pacman.Up();
                        break;
                }
            }

            if (key == PacmanKey.Space && GameMode.GAME_OVER == Status)
            {
                // passe en mode de jeu READY
                numLaby = 0;
                Level = 1;
                SetLabyrinth(numLaby);
                // initialisation du score, des vies, ...
                Lives = 3;
                LifeScore = 5000;
                Score = 0;
                SetMode(GameMode.READY);
                if (Audio != null)
                    Audio.PlayIntro();
            }
            else if (key == PacmanKey.Escape)
            {
                SetMode(GameMode.GAME_OVER);
            }
            else if (key == PacmanKey.NextLevel)
            {
                Labyrinth.getCurrent().RemainingPills = 0;
            }
        }
        #endregion

    }
}

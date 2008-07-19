using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GeniusPacman.Core;

namespace SilverlightPacman
{
    public partial class PacmanUC : UserControl, IAudio
    {
        private bool _animationInProgress;
        private bool _animforever;
        GamePresenter _gamepresenter;
        public PacmanUC()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PacmanUC_Loaded);
            this.KeyDown += new KeyEventHandler(PacmanUC_KeyDown);
        }

        void PacmanUC_Loaded(object sender, RoutedEventArgs e)
        {
            GamePresenter gp = ((GamePresenter)this.DataContext);
            gp.PacmanPresenter.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PacmanPresenter_PropertyChanged);
            ((GhostUCPresenter)this.ghost1.DataContext).GhostPresenter = gp.Ghost1;
            ((GhostUCPresenter)this.ghost2.DataContext).GhostPresenter = gp.Ghost2;
            ((GhostUCPresenter)this.ghost3.DataContext).GhostPresenter = gp.Ghost3;
            ((GhostUCPresenter)this.ghost4.DataContext).GhostPresenter = gp.Ghost4;
            gp.CurrentGame.Audio = this;
            _gamepresenter = gp;
            mediaelem.MediaOpened += new RoutedEventHandler(mediaelem_MediaOpened);
            mediaelem.MediaEnded += new RoutedEventHandler(mediaelem_MediaEnded);
            mediaelem.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(mediaelem_MediaFailed);
        }

        void mediaelem_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        void mediaelem_MediaOpened(object sender, RoutedEventArgs e)
        {
            endOK = false;
            mediaelem.Play();
        }

        bool endOK = true;
        void mediaelem_MediaEnded(object sender, RoutedEventArgs e)
        {
            endOK = true;
        }



        void PacmanPresenter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Orientation")
            {
                orientationPacman.Angle = ((PacmanPresenter)sender).Orientation;
            }
            else if (e.PropertyName == "InMove")
            {
                _animforever = CurrentGame.PacMan.InMove;
                if (!_animationInProgress && _animforever)
                {
                    System.Diagnostics.Debug.WriteLine("startAnimation");
                    _animationInProgress = true;
                    StartAnimation();
                }
            }
        }

        private void StartAnimation()
        {
            const int CSt_MS = 50;
            PointAnimationUsingKeyFrames ptAnim = new PointAnimationUsingKeyFrames();
            ptAnim.KeyFrames.Add(new DiscretePointKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(CSt_MS)), Value = new Point(14.7, 8.5) });
            ptAnim.KeyFrames.Add(new DiscretePointKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(CSt_MS)), Value = new Point(15, 9.99) });

            Timeline anim = ptAnim;//new PointAnimation();
            //anim.To = new Point(15, 9.99);
            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            sb.Children.Add(anim);
            Storyboard.SetTarget(anim, mouseTopPosition);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Point"));

            PointAnimationUsingKeyFrames ptAnim1 = new PointAnimationUsingKeyFrames();
            ptAnim1.KeyFrames.Add(new DiscretePointKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(CSt_MS)), Value = new Point(14.7, 11.5) });
            ptAnim1.KeyFrames.Add(new DiscretePointKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(CSt_MS)), Value = new Point(15, 10.01) });

            Timeline anim1 = ptAnim1;// new PointAnimation();
            //anim1.To = new Point(15, 10.01);
            sb.Children.Add(anim1);
            Storyboard.SetTarget(anim1, mouseBottomPosition);
            Storyboard.SetTargetProperty(anim1, new PropertyPath("Point"));

            //DoubleAnimation dblAnim = new DoubleAnimation();
            //sb.Children.Add(dblAnim);
            //dblAnim.To = 7;
            //Storyboard.SetTarget(dblAnim, this);
            //Storyboard.SetTargetProperty(dblAnim, new PropertyPath("PacmanWidth"));

            sb.AutoReverse = true;
            sb.Completed += delegate
            {
                System.Diagnostics.Debug.WriteLine("Completed");
                if (_animforever)
                {
                    System.Diagnostics.Debug.WriteLine("reBegin");
                    StartAnimation();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("_animationInProgress = false");
                    _animationInProgress = false;
                }
            };
            sb.Begin();
        }

        private Game CurrentGame
        {
            get
            {
                return ((GamePresenter)this.DataContext).CurrentGame;
            }
        }

        void PacmanUC_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    e.Handled = true;
                    CurrentGame.KeyPressed(PacmanKey.Space);
                    break;
                case Key.Up:
                    e.Handled = true;
                    CurrentGame.KeyPressed(PacmanKey.Up);
                    break;
                case Key.Left:
                    e.Handled = true;
                    CurrentGame.KeyPressed(PacmanKey.Left);
                    break;
                case Key.Right:
                    e.Handled = true;
                    CurrentGame.KeyPressed(PacmanKey.Right);
                    break;
                case Key.Down:
                    e.Handled = true;
                    CurrentGame.KeyPressed(PacmanKey.Down);
                    break;
                case Key.Escape:
                    e.Handled = true;
                    CurrentGame.KeyPressed(PacmanKey.Escape);
                    break;
                case Key.P :
                    e.Handled = true;
                    CurrentGame.KeyPressed(PacmanKey.Pause);
                    break;
                case Key.F :
                    e.Handled = true;
                    Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
                    break;
                case Key.S :
                    e.Handled = true;
                    _gamepresenter.IsMuted = !_gamepresenter.IsMuted;
                    break;
                case Key.N:
#if DEBUG
                    e.Handled = true;
                    CurrentGame.KeyPressed(PacmanKey.NextLevel);
#endif
                    break;
                default:
                    //base.OnPreviewKeyDown(e);
                    break;
            }
        }

        #region IAudio Members

        Uri _AudioUri;
        private Uri AudioUri
        {
            get
            {
                return _AudioUri;
            }
            set
            {
                if (endOK)
                {
                    if (value != _AudioUri)
                    {
                        _AudioUri = value;
                        mediaelem.Source = _AudioUri;
                    }
                    else
                    {
                        if (endOK)
                        {
                            endOK = false;
                            mediaelem.Position = TimeSpan.FromSeconds(0);
                            mediaelem.Play();
                        }
                    }
                }
            }
        }

        void IAudio.PlayPill()
        {
            if (!_gamepresenter.IsMuted)
            {
                AudioUri = new Uri("Music/pacman_chomp.mp3", UriKind.RelativeOrAbsolute);
            }
        }

        void IAudio.PlayBonus()
        {
            if (!_gamepresenter.IsMuted)
            {
                AudioUri = new Uri("Music/pacman_extrapac.mp3", UriKind.RelativeOrAbsolute);
            }
        }

        void IAudio.PlayDeath()
        {
            if (!_gamepresenter.IsMuted)
            {
                endOK = true;
                AudioUri = new Uri("Music/pacman_death.mp3", UriKind.RelativeOrAbsolute);
            }
        }

        void IAudio.PlayEatGhost()
        {
            if (!_gamepresenter.IsMuted)
            {
                AudioUri = new Uri("Music/pacman_eatghost.mp3", UriKind.RelativeOrAbsolute);
            }
        }

        void IAudio.PlayIntro()
        {
            if (!_gamepresenter.IsMuted)
            {
                AudioUri = new Uri("Music/pacman_beginning.mp3", UriKind.RelativeOrAbsolute);
            }
        }

        #endregion
    }
}

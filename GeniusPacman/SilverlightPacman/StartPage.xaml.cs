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
using System.Diagnostics;

namespace SilverlightPacman
{
    public partial class StartPage : UserControl
    {
        Storyboard _sb;
        public StartPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(StartPage_Loaded);
        }

        void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as GamePresenter).NotifyOn<GamePresenter>("IsGameOver", delegate(GamePresenter presenter)
            {
                if (presenter.IsGameOver)
                    _sb.Begin();
                else
                    _sb.Stop();
            });
            _sb = this.LayoutRoot.Resources["sb1"] as Storyboard;
            _sb.Completed += new EventHandler(_sb_Completed);
            _sb.Begin();
        }

        void _sb_Completed(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                Debug.WriteLine("ghost animation re-started");
                _sb.Begin();
            }
            else
            {
                Debug.WriteLine("ghost animation is stopped");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _sb.Stop();
        }
    }
}

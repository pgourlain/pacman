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
using SilverlightPacman;

namespace SilverlightPacmanApp
{
    public partial class Page : UserControl
    {
        GamePresenter Presenter = new GamePresenter();

        public Page()
        {
            this.DataContext = Presenter;
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(Page_KeyDown);
        }

        void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //pacmanUC.Foreground = null;
        }
    }
}

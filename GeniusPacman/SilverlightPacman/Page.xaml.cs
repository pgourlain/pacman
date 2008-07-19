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
using System.Diagnostics;

namespace SilverlightPacman
{
    public partial class Page : UserControl
    {
        GamePresenter Presenter = new GamePresenter();

        public Page()
        {
            this.DataContext = Presenter;
            Presenter.CurrentGame.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentGame_PropertyChanged);
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
            this.KeyDown += new KeyEventHandler(Page_KeyDown);
        }

        void Page_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.pacmanUC.Focus();
        }

        void CurrentGame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                Debug.WriteLine(Presenter.CurrentGame.Status);
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
                        
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void Border_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

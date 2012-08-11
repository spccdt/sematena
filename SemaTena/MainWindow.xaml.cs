using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sematena.AudioVideoLib;

namespace Sematena.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AvLib _avlib;

        public MainWindow()
        {
            InitializeComponent();

            _avlib = new AvLib();
            _avlib.Initialize();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _avlib.Close();
        }
    }
}

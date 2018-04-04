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

namespace Transmission.Client.View.Fields
{
    /// <summary>
    /// Interaction logic for SpeedView.xaml
    /// </summary>
    public partial class SpeedView : UserControl
    {
        public int Speed
        {
            get => (int)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(int), typeof(SpeedView), new PropertyMetadata(default(int)));

        public SpeedView()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}

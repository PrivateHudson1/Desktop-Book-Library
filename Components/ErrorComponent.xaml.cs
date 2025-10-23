using System;
using System.Collections.Generic;
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

namespace Library.Components
{
    /// <summary>
    /// Interaction logic for ErrorComponent.xaml
    /// </summary>
    public partial class ErrorComponent : UserControl
    {
        public ErrorComponent()
        {
            InitializeComponent();
        }

        public ErrorComponent(string errorMessage, int state) : this()
        {
         
        }

        public void errorMessage(string errorMessage, int state)
        {
            if (state == 1) //+
            {
                iconState.Icon = BootstrapIcons.Net.BootstrapIconGlyph.CheckLg;
                iconState.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF11E846"));
            }
            else if (state == 2) // -
            {
                iconState.Icon = BootstrapIcons.Net.BootstrapIconGlyph.XCircle;
                iconState.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE81111"));
            }

            errorTextBlock.Text = errorMessage;
        }
    }
}

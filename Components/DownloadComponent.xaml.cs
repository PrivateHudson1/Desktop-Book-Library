using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for DownloadComponent.xaml
    /// </summary>
    public partial class DownloadComponent : UserControl
    {
        public event EventHandler ConditionMet;
        public event EventHandler ConditionError;
        public DownloadComponent()
        {
            InitializeComponent();
        }

        private void backRedirect_Click(object sender, RoutedEventArgs e)
        {
            ConditionMet?.Invoke(this, EventArgs.Empty);
        }

        private void iosBut_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
                {
                button.IsEnabled = false;

                SaveFileFromMedia("sample.epub", "EPUB файл (*.epub)|*.epub", (Button)sender);

                button.IsEnabled = true;
                }
         


        }



        private void pdfBut_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
            {
                button.IsEnabled = false;
                SaveFileFromMedia("sample.pdf", "PDF файл (*.pdf)|*.pdf", (Button)sender);
                button.IsEnabled = true;
            }
          
        }

        private void txtBut_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
            {
                button.IsEnabled = false;
                SaveFileFromMedia("Fox.txt", "Текстовый файл (*.txt)|*.txt", (Button)sender);
                button.IsEnabled = true;
            }
          
        }

        private void fbBut_Click(object sender, RoutedEventArgs e)
        {
       if(sender is Button button)
            {
                button.IsEnabled = false;
                SaveFileFromMedia("sample.fb2", "FB2 файл (*.fb2)|*.fb2", (Button)sender);
                button.IsEnabled = true;
            }
           

        }

        private void SaveFileFromMedia(string fileNameInMedia, string filter, Button button)
        {

            string baseDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string mediaPath = System.IO.Path.Combine(baseDirectory, "media", fileNameInMedia);

            if (!File.Exists(mediaPath))
            {
                button.IsEnabled = false;   
                ConditionError?.Invoke(this, EventArgs.Empty);
                button.IsEnabled = true;
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Title = $"Сохранить {fileNameInMedia}",
                Filter = filter,
                FileName = fileNameInMedia,
                DefaultExt = System.IO.Path.GetExtension(fileNameInMedia)
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    File.Copy(mediaPath, saveDialog.FileName, overwrite: true);
                    ConditionMet?.Invoke(this, EventArgs.Empty);
                }
                catch (IOException ex)
                {
                    button.IsEnabled = false;
                    ConditionError?.Invoke(this, EventArgs.Empty);
                    button.IsEnabled = true;
                }
            }
        }
    }
}

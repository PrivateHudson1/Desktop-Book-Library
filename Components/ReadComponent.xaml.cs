using Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
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
    /// Interaction logic for ReadComponent.xaml
    /// </summary>
    public partial class ReadComponent : UserControl
    {
        private List<string> _pages; 
        private int _currentPage;
        private int countPage;
        private int thisPage = 1;
        private string url = "http://78.29.32.36:5119/";

        private UpdateObjectModel _objectUpdate;
        public ReadComponent()
        {
            InitializeComponent();
            _currentPage = 0;
          
          //  LoadBook(path);
        }

        public async void LoadBook(UpdateObjectModel objectUpdate)
        {
            _objectUpdate = objectUpdate;

            try
            {
                using (var client = new HttpClient())
                {
                    var fileBytes = await client.GetByteArrayAsync(url + "api/files/download/book/48");
                    
                    var allText = Encoding.GetEncoding("windows-1251").GetString(fileBytes);
                    int maxCharsPerPage = 10000;


                    _pages = SplitTextIntoPages(allText, maxCharsPerPage);

                    countPage = _pages.Count;

                    PageIndicator.Text = $"{thisPage}/{countPage}";

                    ShowPage(_currentPage);
                }


              
            }
            catch (Exception ex)
            {
                
            }
        }


        private List<string> SplitTextIntoPages(string text, int maxCharsPerPage)
        {
            var pages = new List<string>();
            for (int i = 0; i < text.Length; i += maxCharsPerPage)
            {
                int length = Math.Min(maxCharsPerPage, text.Length - i);
                pages.Add(text.Substring(i, length)); 
            }
            return pages;
        }

        private void ShowPage(int pageIndex)
        {
            if (_pages == null || pageIndex < 0 || pageIndex >= _pages.Count)
                return;

            TextBoxContent.Text = _pages[pageIndex];

            TextBoxContent.ScrollToVerticalOffset(0);
            //TextScrollViewer.ScrollToHome();
            //TextScrollViewer.ScrollToVerticalOffset(0);
            //TextScrollViewer.ScrollToTop();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (_pages != null && _currentPage < _pages.Count - 1)
            {
               
                thisPage++;
                _currentPage++;
                PageIndicator.Text = $"{thisPage}/{countPage}";
                ShowPage(_currentPage); 
            }
        }

       
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pages != null && _currentPage > 0)
            {
               
                thisPage--;
                _currentPage--;
                PageIndicator.Text = $"{thisPage}/{countPage}";
                ShowPage(_currentPage); 
            }
        }

        //private void OpenBookDialog_Click(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new Microsoft.Win32.OpenFileDialog
        //    {
        //        Filter = "Text Files (*.txt)|*.txt"
        //    };

        //    if (dialog.ShowDialog() == true)
        //    {
        //        LoadBook(dialog.FileName); 
        //        _currentPage = 0; 
        //    }
        //}


      
    }
}

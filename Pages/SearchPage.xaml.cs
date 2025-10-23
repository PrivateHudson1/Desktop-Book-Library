using System;
using System.IO;
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
using System.Windows.Threading;
using PDFiumSharp;
using Microsoft.Win32;
using System.Drawing;
using PDFiumSharp.Enums;
using System.Drawing.Imaging;
using PDFiumSharp.Types;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using PdfDocument = Syncfusion.Pdf.PdfDocument;

namespace Library.Pages
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {

        private List<string> _pages; // Список страниц
        private int _currentPage; // Текущая страница

        public SearchPage()
        {
            InitializeComponent();
            _currentPage = 0;
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media", "Fox.txt");
            LoadBook(path);
        }

        private void LoadBook(string filePath)
        {
            try
            {
                var allText = File.ReadAllText(filePath, Encoding.GetEncoding("windows-1251")); // Чтение всего текста из файла
                int maxCharsPerPage = 2000; // Количество символов на одной странице (можно настроить)

                // Разделяем текст на страницы
                _pages = SplitTextIntoPages(allText, maxCharsPerPage);

                // Отображаем первую страницу
                ShowPage(_currentPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}");
            }
        }

        // Разделение текста на страницы
        private List<string> SplitTextIntoPages(string text, int maxCharsPerPage)
        {
            var pages = new List<string>();
            for (int i = 0; i < text.Length; i += maxCharsPerPage)
            {
                int length = Math.Min(maxCharsPerPage, text.Length - i);
                pages.Add(text.Substring(i, length)); // Добавляем текст на текущей странице
            }
            return pages;
        }

        // Метод для отображения страницы
        private void ShowPage(int pageIndex)
        {
            if (_pages == null || pageIndex < 0 || pageIndex >= _pages.Count)
                return;

          //  TextBoxContent.Text = _pages[pageIndex]; // Отображаем содержимое текущей страницы
        }

        // Обработчик для кнопки "Next" (Следующая страница)
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pages != null && _currentPage < _pages.Count - 1)
            {
                _currentPage++;
                ShowPage(_currentPage); // Переходим к следующей странице
            }
        }

        // Обработчик для кнопки "Previous" (Предыдущая страница)
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pages != null && _currentPage > 0)
            {
                _currentPage--;
                ShowPage(_currentPage); // Переходим к предыдущей странице
            }
        }

        // Метод для открытия файла через диалоговое окно
        private void OpenBookDialog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                LoadBook(dialog.FileName); // Загружаем книгу из выбранного файла
                _currentPage = 0; // Начинаем с первой страницы
            }
        }

        // Для примера добавим кнопку для открытия файла
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var openButton = new Button
            {
                Content = "Open Book",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10)
            };
            openButton.Click += OpenBookDialog_Click;
            this.AddVisualChild(openButton);
            //this.AddChild(openButton); // Добавляем кнопку на окно
        }

    }
}

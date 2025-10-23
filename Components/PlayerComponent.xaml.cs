using Library.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Library.Components
{
    /// <summary>
    /// Interaction logic for PlayerComponent.xaml
    /// </summary>
    public partial class PlayerComponent : UserControl
    {
        private ObservableCollection<string> _audioFiles;
        private ICollectionView collectionView;
        string _userLogin;
        string url = "http://78.29.32.36:5119/";
        private double DownloadProgress { get; set; }


        private Users _currentUser;
        public ICommand LoadAudioCommand { get; }
        public ObservableCollection<Books> Books { get; set; }
        public PlayerComponent()
        {
            InitializeComponent();

           

            //var savedFiles = GetSavedAudioFiles();
            //_audioFiles = new ObservableCollection<string>(savedFiles);

            //SaveAudioFile("media/Air.mp3");
        }


        public async Task<byte[]> DownloadFileAsyncTest(string url, ProgressBar progressBar)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.ExpectContinue = false;


                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();


                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var canReportProgress = totalBytes != -1;

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var memoryStream = new MemoryStream())
                {
                    var totalRead = 0L;
                    var buffer = new byte[8192];
                    var isMoreToRead = true;

                    do
                    {
                        var read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await memoryStream.WriteAsync(buffer, 0, read);
                            totalRead += read;

                            if (canReportProgress)
                            {
                                progressBar.Dispatcher.Invoke(() =>
                                {
                                    progressBar.Value = (double)totalRead / totalBytes * 100;
                                    
                                });
                            }
                        }
                    } while (isMoreToRead);

                    return memoryStream.ToArray(); // Возвращаем содержимое файла как массив байт
                }
            }
        }


        public bool LoadUserAudio(string userLogin, UpdateObjectModel objectUpdate)
        {

            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                string audioDir = System.IO.Path.Combine(appDataPath, "Library", "users", userLogin, "audio", objectUpdate.fileName);
                if (Directory.Exists(audioDir))
                {

                }
                else
                {
                    Directory.CreateDirectory(audioDir);
                }

              



                string destPath = System.IO.Path.Combine(audioDir, objectUpdate.fileName + ".mp3");
                if (File.Exists(destPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
              

            }
            catch (Exception ex)
            {
               
                return false;
            }


        }


        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T frameworkElement && frameworkElement.Name == name)
                    return frameworkElement;

                var result = FindVisualChildByName<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }


        public async Task<byte[]> DownloadFileAsync(string url, ProgressBar progressBar)
        {
            using (var client = new HttpClient())
            {
                

                // Начинаем загрузку
                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new byte[0];
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return new byte[0];
                   
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }


                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var memoryStream = new MemoryStream())
                {
                    var totalRead = 0L;
                    var buffer = new byte[8192]; // 8KB буфер
                    var isMoreToRead = true;
                    var lastProgressUpdate = 0;

                    do
                    {
                        var read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await memoryStream.WriteAsync(buffer, 0, read);
                            totalRead += read;

                            // Обновляем прогресс-бар не чаще чем каждые 100ms
                            var currentProgress = (int)(totalRead / 1024); // Прогресс в KB
                            if (currentProgress > lastProgressUpdate || !isMoreToRead)
                            {
                                progressBar.Dispatcher.Invoke(() =>
                                {
                                    // Показываем прогресс в KB
                                    progressBar.Value = currentProgress;
                                    
                                    // Если нужно, можно добавить текст с информацией
                                    // progressBar.ToolTip = $"{currentProgress} KB загружено";
                                });
                                lastProgressUpdate = currentProgress;
                            }
                        }
                    } while (isMoreToRead);

                    progressBar.Visibility = Visibility.Hidden;
                    return memoryStream.ToArray();
                }
            }
        }


      

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public void SetData(IEnumerable<UpdateObjectModel> _objectUpdate, string userLogin, Users user)
        {
            collectionView = CollectionViewSource.GetDefaultView(_objectUpdate);
            _currentUser = user;
            _userLogin = userLogin;
            MainList.ItemsSource = collectionView;




            // Вызываем после установки ItemsSource
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var item in MainList.Items)
                {

                    var container = MainList.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    if (container != null)
                    {

                        //var contentPresenter = FindVisualChild<ContentPresenter>(container);
                        //contentPresenter?.ApplyTemplate();
                        //var border = contentPresenter?.ContentTemplate?.LoadContent() as Border;
                        //var button = border?.FindName("listenAudioBut") as Button;

                        var button = FindChild<Button>(container, "listenAudioBut");

                        //var contentPresenter = FindVisualChildInTemplate<ContentPresenter>(container, "listenAudioBut");
                        //var templateRoot = contentPresenter?.ContentTemplate?.LoadContent() as FrameworkElement;
                        //var button = templateRoot?.FindName("listenAudioBut") as Button;

                        if (button != null && item is UpdateObjectModel model)
                        {
                            if(LoadUserAudio(_userLogin, model))
                            {
                                button.IsEnabled = true;
                            }
                        
                           // button.IsEnabled = LoadUserAudio(_userLogin, model);
                        }
                    }
                }
            }), DispatcherPriority.Render);
        }

        // Поиск родителя указанного типа вверх по дереву
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        // Поиск дочернего элемента с определённым именем
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // Проверяем совпадение имени
                if (child is T typedChild && ((FrameworkElement)child).Name == childName)
                {
                    return typedChild;
                }

                // Рекурсивно ищем глубже
                var foundChild = FindChild<T>(child, childName);
                if (foundChild != null)
                    return foundChild;
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject parent, string childName = null)
    where T : DependencyObject
        {
            if (parent == null)
                return null;

            // Рекурсивный поиск по всему визуальному дереву
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // Проверяем соответствие типу и имени (если имя задано)
                if (child is T result &&
                    (childName == null || (child is FrameworkElement fe && fe.Name == childName)))
                {
                    return result;
                }

                // Рекурсивный поиск в дочерних элементах
                var foundChild = FindVisualChild<T>(child, childName);
                if (foundChild != null)
                    return foundChild;
            }

            return null;
        }

        public static T FindVisualChildInTemplate<T>(DependencyObject parent, string childName)
    where T : DependencyObject
        {
            if (parent == null) return null;

            // 1. Находим ContentPresenter (он содержит наш DataTemplate)
            var contentPresenter = FindVisualChild<ContentPresenter>(parent);
            if (contentPresenter == null) return null;

            // 2. Загружаем шаблон (если еще не загружен)
            if (VisualTreeHelper.GetChildrenCount(contentPresenter) == 0)
            {
                contentPresenter.ApplyTemplate();
            }

            // 3. Ищем элемент в визуальном дереве шаблона
            var templateRoot = VisualTreeHelper.GetChild(contentPresenter, 0) as FrameworkElement;
            return templateRoot?.FindName(childName) as T;
        }
        private async void downloadAudioBut_Click(object sender, RoutedEventArgs e)
        {

            if(_currentUser.isActive == 1)
            {
                if (sender is Button firstButton)
                {

                    var buttonStackPanel = FindParent<StackPanel>(firstButton);
                    var buttonGrid = FindParent<Grid>(buttonStackPanel);
                    var mainGrid = FindParent<Grid>(buttonGrid);


                    if (mainGrid != null)
                    {
                        var progressBar = FindChild<ProgressBar>(mainGrid, "progressDownload");

                        var data = firstButton.DataContext as UpdateObjectModel;

                        if (LoadUserAudio(_userLogin, data))
                        {
                            var mainWindow = Application.Current.Windows
                          .OfType<MainWindow>()
                          .FirstOrDefault();
                            mainWindow?.errorMessageLeft("Аудиокнига уже загружена", 1);


                            return;
                        }


                        if (progressBar != null)
                        {
                            progressBar.Visibility = Visibility.Visible;

                            byte[] audioFile = await DownloadFileAsync(url + $"api/files/download/audio/{data.fileName}", progressBar);


                            if (audioFile == null || audioFile.Length == 0)
                            {

                                var mainWindow = Application.Current.Windows
                           .OfType<MainWindow>()
                           .FirstOrDefault();
                                mainWindow?.errorMessage("Аудиокнига еще не была добавлена в базу", 2);
                                progressBar.Visibility = Visibility.Hidden;
                                return;
                            }
                            else
                            {
                                var secondButton = FindChild<Button>(mainGrid, "listenAudioBut");
                                if (secondButton != null)
                                {
                                    secondButton.IsEnabled = true;
                                }

                                SaveUserAudio(_userLogin, audioFile, data);
                                progressBar.Visibility = Visibility.Hidden;
                            }







                        }
                    }


                }
            }
            else if(_currentUser.isActive == 0)
            {
                var mainWindow = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();
                mainWindow?.errorMessage("У вас нет подписки", 2);
            }
            else
            {
                var mainWindow = Application.Current.Windows
               .OfType<MainWindow>()
               .FirstOrDefault();
                mainWindow?.errorMessage("Ошибка", 2);
            }

          
        }

      

        private void listenAudioBut_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            UpdateObjectModel selectedBook = (UpdateObjectModel)button.DataContext;

            var mainWindow = Application.Current.Windows
  .OfType<MainWindow>()
  .FirstOrDefault();
            mainWindow?.listenComponentFunc(selectedBook);

        }

        public void SaveUserAudio(string userLogin, byte[] audioFile, UpdateObjectModel objectUpdate)
        {


            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string audioDir = System.IO.Path.Combine(appDataPath, "Library", "users", userLogin, "audio", objectUpdate.fileName);

            if(Directory.Exists(audioDir))
            {

            
            }
            else
            {
                Directory.CreateDirectory(audioDir);

            }



            string destPath = System.IO.Path.Combine(audioDir, objectUpdate.fileName + ".mp3");

            if (File.Exists(destPath))
            {
                
                return;
             
            }
            File.WriteAllBytes(destPath, audioFile);



            return;

        }
    }






    public class ProgressToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress && parameter is FrameworkElement parent)
            {
                return parent.ActualWidth * (progress / 100);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

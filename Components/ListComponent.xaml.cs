using Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using System.Xml;

namespace Library.Components
{
    /// <summary>
    /// Логика взаимодействия для ListComponent.xaml
    /// </summary>
    public partial class ListComponent : UserControl
    {
        public TranslateTransform translateTransform = new TranslateTransform();
        public DoubleAnimation downloadComponentAnimation = new DoubleAnimation();

        public TranslateTransform errorMessageTransform = new TranslateTransform();
        public DoubleAnimation errorAnimation = new DoubleAnimation();

        private double _scrollOffset = 0; 
        private const double ItemWidth = 280; 
        private const int ScrollItemsCount = 3; 

        private double ScrollViewerWidth => 850;

        private StackPanel _stackPanel;

        private ICollectionView collectionView;
        private string _userLogin;
        private Users _currentUser;
        public UpdateObjectModel _objectUpdate;

        private List<UpdateObjectModel> existingCollection;

        

        public ListComponent()
        {
            InitializeComponent();
            errorMessageTransform.Y = 500; 
            errorComponent.RenderTransform = errorMessageTransform;

            
        }

        //private void AnimateSlide()
        //{
        //    if (_stackPanel == null)
        //        return;

        //    double toValue = -_currentIndex * _itemWidth;

        //    var transform = _stackPanel.RenderTransform as TranslateTransform;
        //    if (transform == null)
        //    {
        //        transform = new TranslateTransform();
        //        _stackPanel.RenderTransform = transform;
        //    }

        //    var animation = new DoubleAnimation(toValue, TimeSpan.FromMilliseconds(300))
        //    {
        //        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        //    };
        //    transform.BeginAnimation(TranslateTransform.XProperty, animation);
        //}

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T tChild)
                    return tChild;

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
        public void SetData( string userLogin, Users user)
        {
            _currentUser = user;
            _userLogin = userLogin;
            DownloadUserCollection();
        }

        private void rightScrollBut_Click(object sender, RoutedEventArgs e)
        {
            // Двигаем вправо: увеличиваем сдвиг
            double maxOffset = -((SliderItems.Items.Count * ItemWidth) - ScrollViewerWidth);

            _scrollOffset -= ItemWidth * ScrollItemsCount;

            if (_scrollOffset < maxOffset)
                _scrollOffset = maxOffset; // чтобы не прокрутить правее конца

            AnimateScroll(_scrollOffset);
        }

        private void leftScrollButt_Click(object sender, RoutedEventArgs e)
        {
            // Двигаем влево: уменьшаем сдвиг
            _scrollOffset += ItemWidth * ScrollItemsCount;

            if (_scrollOffset > 0)
                _scrollOffset = 0; // чтобы не прокрутить левее начала

            AnimateScroll(_scrollOffset);
        }

        private void AnimateScroll(double toOffset)
        {
            var animation = new DoubleAnimation
            {
                To = toOffset,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            ItemsTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

 
        public void DownloadUserCollection()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string userDir = System.IO.Path.Combine(appDataPath, "Library", "users", _userLogin);
                
                string filePath = System.IO.Path.Combine(userDir, "collection.json");

                if (File.Exists(filePath))
                {
                    string existingJson = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        try
                        {

             
                            existingCollection = JsonSerializer.Deserialize<List<UpdateObjectModel>>(existingJson) ?? new List<UpdateObjectModel>();


                            var updateObject = existingCollection.Select(book => new UpdateObjectModel
                            {
                               
                                fileName = book.fileName,
                                image = book.image,
                                Author = book.Author
                            });


                            collectionView = CollectionViewSource.GetDefaultView(updateObject);

                            SliderItems.ItemsSource = collectionView;
                        }
                        catch (JsonException)
                        {
                            existingCollection = new List<UpdateObjectModel>();
                        }
                    }
                    return;


                }
                

            }
            catch
            {

            }
        }

        private void readBut_Click(object sender, RoutedEventArgs e)
        {
            if(_currentUser.isActive == 1)
            {
                var button = sender as Button;
                var data = button.DataContext as UpdateObjectModel;

                var mainWindow = Application.Current.Windows
         .OfType<MainWindow>()
         .FirstOrDefault();
                mainWindow?.readComponentFunc(data);
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

        private void downloadBut_Click(object sender, RoutedEventArgs e)
        {
            if(_currentUser.isActive == 1)
            {
                downloadComponent.RenderTransform = translateTransform;
                downloadComponentAnimation.From = -1400;
                downloadComponentAnimation.To = 700;
                downloadComponentAnimation.Duration = TimeSpan.FromSeconds(1);
                downloadComponentAnimation.SpeedRatio = 2;

                translateTransform.BeginAnimation(TranslateTransform.YProperty, downloadComponentAnimation);
            }    
            else if (_currentUser.isActive == 0)
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

      
        private void downloadComponent_ConditionMet(object sender, EventArgs e)
        {
            downloadComponent.RenderTransform = translateTransform;
            downloadComponentAnimation.From = 700;
            downloadComponentAnimation.To = -1400;
            downloadComponentAnimation.Duration = TimeSpan.FromSeconds(1);
            downloadComponentAnimation.SpeedRatio = 2;

            translateTransform.BeginAnimation(TranslateTransform.YProperty, downloadComponentAnimation);
        }

        private async void errorMessage(string errorMessage)
        {
            
            errorComponent.Visibility = Visibility.Visible;
            errorComponent.errorTextBlock.Text = errorMessage;
            
            errorComponent.RenderTransform = errorMessageTransform;
            errorAnimation.From = 500;
            errorAnimation.To = 300;
            errorAnimation.Duration = TimeSpan.FromSeconds(2);
            errorAnimation.SpeedRatio = 2;
            errorAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut};

            errorMessageTransform.BeginAnimation(TranslateTransform.YProperty, errorAnimation);

            await Task.Delay(3000);

            errorComponent.RenderTransform = errorMessageTransform;
            errorAnimation.From = 300;
            errorAnimation.To = 500;
            errorAnimation.Duration = TimeSpan.FromSeconds(2);
            errorAnimation.SpeedRatio = 2;
            errorAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            errorMessageTransform.BeginAnimation(TranslateTransform.YProperty, errorAnimation);

            await Task.Delay(2000);
            errorComponent.Visibility = Visibility.Hidden;
        }

        private void downloadComponent_ConditionError(object sender, EventArgs e)
        {

            var mainWindow = Application.Current.Windows
                  .OfType<MainWindow>()
                  .FirstOrDefault();
            mainWindow?.errorMessage("Ошибка при скачивании", 2);
        }

        private void deleteFromCollection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string userDir = System.IO.Path.Combine(appDataPath, "Library", "users", _userLogin);

                string filePath = System.IO.Path.Combine(userDir, "collection.json");

                if (File.Exists(filePath)) 
                {
                    string existingJson = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(existingJson))
                    {
                        existingCollection = JsonSerializer.Deserialize<List<UpdateObjectModel>>(existingJson) ?? new List<UpdateObjectModel>();

                        var button = sender as Button;
                        if (button != null)
                        {
                            var data = button.DataContext as UpdateObjectModel;

                            if(data != null)
                            {
                                var itemToRemove = existingCollection
                                    .FirstOrDefault(x => x.fileName == data.fileName); 

                                if (itemToRemove != null)
                                {
                                    existingCollection.Remove(itemToRemove);
                                    string updatedJson = JsonSerializer.Serialize(existingCollection, new JsonSerializerOptions { WriteIndented = true });

                                    File.WriteAllText(filePath, updatedJson);
                                    DownloadUserCollection();
                                }
                               

                               

                              //  DownloadUserCollection();
                            }


                        }
                    }



                   
                }


              
            }

            catch
            {

            }
           
        }
    }
}

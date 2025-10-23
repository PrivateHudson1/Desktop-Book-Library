using Library.Models;
using LibraryApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    /// Логика взаимодействия для SearchComponent.xaml
    /// </summary>
    public partial class SearchComponent : UserControl
    {
        private string serverAddress = "http://78.29.32.36:5119/";
        private readonly HttpClient _httpClient = new HttpClient();
        public ObservableCollection<Books> Books { get; set; }
        private ICollectionView collectionView;
        List<Authors> listAuthors = new List<Authors>();

        public TranslateTransform errorMessageTransform = new TranslateTransform();
        public DoubleAnimation errorAnimation = new DoubleAnimation();


        public IEnumerable<UpdateObjectModel> _objectUpdate;

        private Users _user;

        public SearchComponent()
        {
            InitializeComponent();

            errorMessageTransform.Y = 500; // Начальное положение (вне экрана снизу)
            errorComponent.RenderTransform = errorMessageTransform;



        }

        public void GetUser(Users user)
        {
            _user = user;

             
        }


        private async void errorMessage(string errorMessage)
        {

            errorComponent.Visibility = Visibility.Visible;
            errorComponent.iconState.Icon = BootstrapIcons.Net.BootstrapIconGlyph.CheckLg;
            errorComponent.iconState.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF11E846"));
            errorComponent.errorTextBlock.Text = errorMessage;
            errorComponent.RenderTransform = errorMessageTransform;
            errorAnimation.From = 500;
            errorAnimation.To = 300;
            errorAnimation.Duration = TimeSpan.FromSeconds(2);
            errorAnimation.SpeedRatio = 2;
            errorAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

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



        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (MainList.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                foreach (var item in MainList.Items)
                {
                    var container = MainList.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    if (container != null)
                    {
                        var targetButton = FindNamedVisualChild<Button>(container, "addBookCollectionBut");

                        if (targetButton != null && item is UpdateObjectModel model)
                        {
                            if (model.isBackground)
                            {
                                // Меняем фон только у этой нужной кнопки
                                targetButton.Background = new SolidColorBrush(Colors.Gray);
                            }
                            else
                            {
                                targetButton.ClearValue(Button.BackgroundProperty);
                            }
                        }
                    }
                }
            }
        }


        public bool LoadUserCollection(string userLogin, UpdateObjectModel objectUpdate)
        {

            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string filePath = System.IO.Path.Combine(appDataPath, "Library", "users", userLogin, "collection.json");

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var objectEquals = JsonSerializer.Deserialize<List<UpdateObjectModel>>(json);

                    if (objectEquals.Any(i => i.id == objectUpdate.id))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
               


                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

         
        }


        public static T FindNamedVisualChild<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild && typedChild.Name == childName)
                    return typedChild;

                var childOfChild = FindNamedVisualChild<T>(child, childName);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }




        public IEnumerable<UpdateObjectModel> GetObject()
        {
            return _objectUpdate;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                HttpResponseMessage responseBook = await _httpClient.GetAsync(serverAddress + "api/info/listLibrary");
                HttpResponseMessage responseAuthor = await _httpClient.GetAsync(serverAddress + "api/info/listAuthors");
                responseBook.EnsureSuccessStatusCode();
                responseAuthor.EnsureSuccessStatusCode();

                string resultBook = await responseBook.Content.ReadAsStringAsync();
                string resultAuthors = await responseAuthor.Content.ReadAsStringAsync();

                List<Books> listBooks = JsonSerializer.Deserialize<List<Books>>(resultBook);
                listAuthors = JsonSerializer.Deserialize<List<Authors>>(resultAuthors);
                
                listBooks = listBooks
                    .GroupBy(book => book.fileName)
                    .Select(group => group.First())
                    .ToList();

                var updateObject = listBooks.Select(book => new UpdateObjectModel
                {
                    id = book.id,
                    genreId = book.genreId,
                    fileName = book.fileName,
                    image = book.image,
                    description = book.description,
                    countPages = book.countPages ?? 0,
                    rating = book.rating,
                    Author = listAuthors.FirstOrDefault(i => i.id == book.authorId)?.name ?? "Автор не указан",
                    isEnable = true,
                    isContent = "Добавить"
                });

                _objectUpdate = updateObject;


                collectionView = CollectionViewSource.GetDefaultView(updateObject);

                MainList.ItemsSource = collectionView;


            }
            catch (Exception ex)
            {
               
            }
        }


        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {


            string filterText = searchBox.Text.Trim();

            if (!string.IsNullOrEmpty(filterText))
            {
                collectionView.Filter = item =>
                {
                    if (item is UpdateObjectModel book)
                    {
                        return book.Author.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }


                    return false;
                };

                collectionView.Refresh();
                MainList.Visibility = Visibility.Visible;
            }
            else
            {
                collectionView.Filter = null; 
                collectionView.Refresh();
                MainList.Visibility = Visibility.Collapsed;
            }


        }


      

        private void clearBut_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Clear();
            collectionView.Filter = null;
        }

        private void infoBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = (Button)sender;

                UpdateObjectModel selectedBook = (UpdateObjectModel)button.DataContext;


                var selectedItem = button.DataContext as UpdateObjectModel;


                var mainWindow = Application.Current.Windows
    .OfType<MainWindow>()
    .FirstOrDefault();
                mainWindow?.infoBookComponentFunc( selectedItem, button);
               

            }

            catch(Exception ex)
            {
               
            }


        }

        private async void genreBut_Click(object sender, RoutedEventArgs e)
        {
            HttpResponseMessage responseGenre = await _httpClient.GetAsync(serverAddress + "api/info/listGenres");
            responseGenre.EnsureSuccessStatusCode();
            string resultGenres = await responseGenre.Content.ReadAsStringAsync();

            List<Genres> listGenres = JsonSerializer.Deserialize<List<Genres>>(resultGenres);

            if (sender is Button button)
            {
                int genreId = int.Parse(button.Tag.ToString());

                    collectionView.Filter = item =>
                    {
                        if (item is UpdateObjectModel book)
                        {
                            return book.genreId == genreId;
                        }
                        return false;
                    };
            };

            collectionView.Refresh();
            MainList.Visibility = Visibility.Visible;
        }

        private void addBookCollection_Click(object sender, RoutedEventArgs e)
        {
            if(_user.isActive == 1)
            {
                var button = sender as Button;
                if (button != null)
                {

                    var data = button.DataContext as UpdateObjectModel;


                    if (LoadUserCollection(_user.firstName, data))
                    {
                        var mainWindow = Application.Current.Windows
                           .OfType<MainWindow>()
                           .FirstOrDefault();
                        mainWindow?.errorMessage("Книга уже добавлена", 1);


                        return;
                    }


                    if (data != null)
                    {
                        SaveUserBook(_user.firstName, data, button);
                    }
                }

            }
            else if (_user.isActive == 0)
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

        public void SaveUserBook(string userLogin, UpdateObjectModel newBook, Button button)
        {

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userDir = System.IO.Path.Combine(appDataPath, "Library", "users", userLogin);
            Directory.CreateDirectory(userDir);

            string filePath = System.IO.Path.Combine(userDir, "collection.json");

            List<UpdateObjectModel> existingCollection = new List<UpdateObjectModel>();

            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    try
                    {
                        existingCollection = JsonSerializer.Deserialize<List<UpdateObjectModel>>(existingJson) ?? new List<UpdateObjectModel>();
                    }
                    catch (JsonException)
                    {
                        existingCollection = new List<UpdateObjectModel>();
                    }
                }
            }

       
            bool alreadyExists = existingCollection.Any(book =>
                book.fileName.Equals(newBook.fileName, StringComparison.OrdinalIgnoreCase));

            if (!alreadyExists)
            {
                existingCollection.Add(newBook);

                string updatedJson = JsonSerializer.Serialize(existingCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);

              
             
            }
            else
            {
                return;
            }
       
        }


    }
}

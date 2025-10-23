using BootstrapIcons.Wpf;
using Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library.Components
{
    /// <summary>
    /// Interaction logic for InfoBook.xaml
    /// </summary>
    public partial class InfoBook : UserControl
    {
        private UpdateObjectModel _objectUpdate;
        Button _button;
        private Users _currentUser;
        string _userLogin;
        public InfoBook()
        {
            InitializeComponent();
        }


       


        public void SetData(string userLogin, UpdateObjectModel objectUpdate, Button button, Users user)
        {

            _currentUser = user;
            nameBookText.Text = objectUpdate.fileName;
            aboutAuthorText.Text = objectUpdate.Author;
            aboutBook.Text = objectUpdate.description;
            additionalText.Text = $"Объем: {objectUpdate.countPages} страниц {objectUpdate.rating}";

            _objectUpdate = objectUpdate;
            _button = button;
            _userLogin = userLogin;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(objectUpdate.image))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }


            imageBook.Source = image;
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

        private void addedButInfo_Click(object sender, RoutedEventArgs e)
        {
            if(_currentUser.isActive == 1)
            {
                if (LoadUserCollection(_userLogin, _objectUpdate))
                {
                    addedButInfo.IsEnabled = false;
                    var mainWindow = Application.Current.Windows
                       .OfType<MainWindow>()
                       .FirstOrDefault();
                    mainWindow?.errorMessage("Книга уже добавлена", 1);
                    addedButInfo.IsEnabled = true;

                    return;
                }

                SaveUserBook(_userLogin, _objectUpdate, _button);
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

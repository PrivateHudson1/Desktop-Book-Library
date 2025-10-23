using Library.Models;
using Syncfusion.Compression.Zip;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library.Pages
{
    /// <summary>
    /// Interaction logic for MainProfileFrame.xaml
    /// </summary>
    public partial class MainProfileFrame : Page
    {
        private Users _user;
        public MainProfileFrame(Users user)
        {
            InitializeComponent();
            _user = user;
            nameUser.Text = "Имя: " + _user.firstName;
            emailUser.Text = "Email: " + _user.email;


        }

        //public void SaveUserCollection(string userLogin, List<UpdateObjectModel> collection)
        //{
        //    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        //    string userDir = System.IO.Path.Combine(appDataPath, "Library", "users", userLogin);
        //    Directory.CreateDirectory(userDir);

        //    string filePath = System.IO.Path.Combine(userDir, "collection.json");

        //    string json = JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = true });
        //    File.WriteAllText(filePath, json);
        //}

        //private void saveBut_Click(object sender, RoutedEventArgs e)
        //{

        //    List<UpdateObjectModel> listObjects = new List<UpdateObjectModel>()
        //    {
        //        new UpdateObjectModel { id = 1, fileName = "Солярис"},
        //        new UpdateObjectModel {id = 2, fileName = "Снафф"}
        //    };


        //    SaveUserCollection("hudson", listObjects);
        //}

        //public List<Books> LoadUserCollection(string userLogin)
        //{
        //    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        //    string filePath = System.IO.Path.Combine(appDataPath, "MyLibraryApp", "users", userLogin, "collection.json");

        //    if (File.Exists(filePath))
        //    {
        //        string json = File.ReadAllText(filePath);
        //        return JsonSerializer.Deserialize<List<Books>>(json);
        //    }

        //    return new List<Books>(); 
        //}




    }
}

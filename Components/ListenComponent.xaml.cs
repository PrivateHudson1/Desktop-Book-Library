using Library.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Library.Components
{
    /// <summary>
    /// Interaction logic for ListenComponent.xaml
    /// </summary>
    public partial class ListenComponent : UserControl
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer timer = new DispatcherTimer();
        private TimeSpan pausedPosition = TimeSpan.Zero;
        private bool isPlaying = false;
        private bool isDraggingSlider = false;

        private BitmapImage _cachedImage;

        private string _userLogin;
        private UpdateObjectModel _objectUpdate;
        public ListenComponent()
        {
           


            InitializeComponent();


          


        }


        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
           
        }

        public void SetData(UpdateObjectModel objectModel, string userLogin)
        {
            _objectUpdate = objectModel;
            _userLogin = userLogin;

            using (var stream = new MemoryStream(_objectUpdate.image))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; 
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                imageBook.Source = bitmap;
            }

            nameBook.Text = _objectUpdate.fileName;
            nameAuthor.Text = _objectUpdate.Author;



            var btn = StartPlayer as Button;
            btn.Tag = "PlayCircle";

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string audioDir = System.IO.Path.Combine(appDataPath, "Library", "users", _userLogin, "audio", _objectUpdate.fileName);

            string destPath = System.IO.Path.Combine(audioDir, _objectUpdate.fileName + ".mp3");

            if (File.Exists(destPath))
            {
                mediaPlayer.Open(new Uri(destPath));
            }
            else
            {
                var mainWindow = Application.Current.Windows
                       .OfType<MainWindow>()
                       .FirstOrDefault();
                mainWindow?.errorMessage("Ошибка при загрузке аудио", 2);
            }


            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();


            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {

                timerText.Text = $"{mediaPlayer.Position:mm\\:ss} / {mediaPlayer.NaturalDuration.TimeSpan:mm\\:ss}";
            }
        }


        private void ProgressSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
            isDraggingSlider = false;
        }

        private void ProgressSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDraggingSlider = true;
        }

        private void StartPlayer_Click(object sender, RoutedEventArgs e)
        {


            if(!isPlaying)
            {
               
                mediaPlayer.Play();
                isPlaying = true;
            }
            else
            {
                pausedPosition = mediaPlayer.Position;
                mediaPlayer.Pause();
                isPlaying = false;
            }

            
            var btn = sender as Button;
            if(btn.Tag == "PlayCircle")
            {
                btn.Tag = "PauseCircle";
            }
            else if (btn.Tag == "PauseCircle")
            {
                btn.Tag = "PlayCircle";
            }
            else
            {
                MessageBox.Show("Неправильный таг");
            }
         
           

        }

        private void partsList_Click(object sender, RoutedEventArgs e)
        {
            DropdownPopup.IsOpen = true;
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is string content)
            {
               
                var parts = content.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3)
                {
                    string firstText = $"{parts[0]} {parts[1]}"; 
                    string secondText = parts[2];             

         
                    partsText.Text = firstText + " из 14";
                    namePartsText.Text = secondText;
                }
            }

            DropdownPopup.IsOpen = false;
            
        }
    }
}

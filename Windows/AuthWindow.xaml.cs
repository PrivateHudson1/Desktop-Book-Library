using Library.Components;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Shapes;

namespace Library.Windows
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        HttpClient _httpClient = new HttpClient();
        public string serverAddress = "http://78.29.32.36:5119/";

        public TranslateTransform errorMessageTransform = new TranslateTransform();
        public DoubleAnimation errorAnimation = new DoubleAnimation();
        public AuthWindow()
        {
            InitializeComponent();
            BackgroundVideo.Play();
            errorMessageTransform.Y = 500; // Начальное положение (вне экрана снизу)
            errorComponent.RenderTransform = errorMessageTransform;
        }


        private async void errorMessage(string errorMessage)
        {
            
            errorComponent.Visibility = Visibility.Visible;
            errorComponent.errorTextBlock.Text = errorMessage;
            errorComponent.RenderTransform = errorMessageTransform;
            errorAnimation.From = 500;
            errorAnimation.To = 350;
            errorAnimation.Duration = TimeSpan.FromSeconds(2);
            errorAnimation.SpeedRatio = 2;
            errorAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            errorMessageTransform.BeginAnimation(TranslateTransform.YProperty, errorAnimation);

            await Task.Delay(3000);

            errorComponent.RenderTransform = errorMessageTransform;
            errorAnimation.From = 350;
            errorAnimation.To = 500;
            errorAnimation.Duration = TimeSpan.FromSeconds(2);
            errorAnimation.SpeedRatio = 2;
            errorAnimation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            errorMessageTransform.BeginAnimation(TranslateTransform.YProperty, errorAnimation);

            await Task.Delay(2000);
            errorComponent.Visibility = Visibility.Hidden;
        }


        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            BackgroundVideo.Position = TimeSpan.Zero;
            BackgroundVideo.Play();
        }
        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Анимация подсказки
            if (string.IsNullOrEmpty(InputBox.Text))
            {
                AnimatePlaceholder(fontSize: 14, yOffset: -17, foregroundColor: (Color)ColorConverter.ConvertFromString("#FFABADB3"));
            }

            // Анимация рамки Border (изменение цвета)
            var rainbowBrush = (LinearGradientBrush)InputBorder.FindName("RainbowBrush");
            rainbowBrush.GradientStops[0].Color = (Color)ColorConverter.ConvertFromString("#FFFB009E");
            rainbowBrush.GradientStops[1].Color = (Color)ColorConverter.ConvertFromString("#FF2E0BDE");
            rainbowBrush.GradientStops[2].Color = (Color)ColorConverter.ConvertFromString("#FFFB009E");

        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Возврат подсказки в исходное состояние
            if (string.IsNullOrEmpty(InputBox.Text))
            {
                AnimatePlaceholder(fontSize: 16, yOffset: 0, foregroundColor: (Color)ColorConverter.ConvertFromString("#FFABADB3"));
            }

            // Возврат цвета рамки к исходному
            var rainbowBrush = (LinearGradientBrush)InputBorder.FindName("RainbowBrush");
            rainbowBrush.GradientStops[0].Color = (Color)ColorConverter.ConvertFromString("#FFABADB3");
            rainbowBrush.GradientStops[1].Color = (Color)ColorConverter.ConvertFromString("#FFABADB3");
            rainbowBrush.GradientStops[2].Color = (Color)ColorConverter.ConvertFromString("#FFABADB3");
        }

        private void AnimatePlaceholder(double fontSize, double yOffset, Color foregroundColor)
        {
            // Анимация шрифта
            DoubleAnimation fontSizeAnim = new DoubleAnimation
            {
                To = fontSize,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Placeholder.BeginAnimation(TextBlock.FontSizeProperty, fontSizeAnim);

            // Анимация положения
            DoubleAnimation translateAnim = new DoubleAnimation
            {
                To = yOffset,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            PlaceholderTransform.BeginAnimation(TranslateTransform.YProperty, translateAnim);

            // Анимация цвета текста
            var textBrush = Placeholder.Foreground as SolidColorBrush;
            if (textBrush != null)
            {
                ColorAnimation colorAnim = new ColorAnimation
                {
                    To = foregroundColor,
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                textBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }
        }

        private async void authBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InputBox.Text == "" || InputBox2.Password == "")
                {
                    return;
                
                }

                HttpResponseMessage response = await _httpClient.GetAsync(serverAddress + "api/info/listUsers");
                response.EnsureSuccessStatusCode();
                string resultUser = await response.Content.ReadAsStringAsync();

                List<Users> usersList = JsonSerializer.Deserialize<List<Users>>(resultUser);

                IEnumerable<Users> objectUser = usersList.Select(user => new Users
                {
                    id = user.id,
                    firstName = user.firstName,
                    email = user.email,
                    password = user.password,
                    isActive = user.isActive,
                   
                });

                Users matchedUser = objectUser.FirstOrDefault(u =>
                u.email.Equals(InputBox.Text, StringComparison.OrdinalIgnoreCase) ||
                u.firstName.Equals(InputBox.Text, StringComparison.OrdinalIgnoreCase));
                if (matchedUser != null)
                {
                    // Проверим пароль
                    if (matchedUser.password == InputBox2.Password)
                    {
                       MainWindow mainWindow = new MainWindow(matchedUser);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        errorMessage("Неправильный логин или пароль");
                    }
                }
                else
                {
                    errorMessage("Неправильный логин или пароль");
                }
            }
            catch(Exception ex)
            {
                errorMessage("Ошибка ответа от сервера");
            }
           
        }




        private void InputBox_GotFocus2(object sender, RoutedEventArgs e)
        {
            // Анимация подсказки
            if (string.IsNullOrEmpty(InputBox2.Password))
            {
                AnimatePlaceholder2(fontSize: 14, yOffset: -17, foregroundColor: (Color)ColorConverter.ConvertFromString("#FFABADB3"));
            }

            // Анимация рамки Border (изменение цвета)
            var rainbowBrush2 = (LinearGradientBrush)InputBorder2.FindName("RainbowBrush2");
            rainbowBrush2.GradientStops[0].Color = (Color)ColorConverter.ConvertFromString("#FFFB009E");
            rainbowBrush2.GradientStops[1].Color = (Color)ColorConverter.ConvertFromString("#FF2E0BDE");
            rainbowBrush2.GradientStops[2].Color = (Color)ColorConverter.ConvertFromString("#FFFB009E");

        }


        private void InputBox_LostFocus2(object sender, RoutedEventArgs e)
        {
            // Возврат подсказки в исходное состояние
            if (string.IsNullOrEmpty(InputBox2.Password))
            {
                AnimatePlaceholder2(fontSize: 16, yOffset: 0, foregroundColor: (Color)ColorConverter.ConvertFromString("#FFABADB3"));
            }

            // Возврат цвета рамки к исходному
            var rainbowBrush2 = (LinearGradientBrush)InputBorder2.FindName("RainbowBrush2");
            rainbowBrush2.GradientStops[0].Color = (Color)ColorConverter.ConvertFromString("#FFABADB3");
            rainbowBrush2.GradientStops[1].Color = (Color)ColorConverter.ConvertFromString("#FFABADB3");
            rainbowBrush2.GradientStops[2].Color = (Color)ColorConverter.ConvertFromString("#FFABADB3");
        }

        private void AnimatePlaceholder2(double fontSize, double yOffset, Color foregroundColor)
        {
            // Анимация шрифта
            DoubleAnimation fontSizeAnim = new DoubleAnimation
            {
                To = fontSize,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            Placeholder2.BeginAnimation(TextBlock.FontSizeProperty, fontSizeAnim);

            // Анимация положения
            DoubleAnimation translateAnim = new DoubleAnimation
            {
                To = yOffset,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            PlaceholderTransform2.BeginAnimation(TranslateTransform.YProperty, translateAnim);

            // Анимация цвета текста
            var textBrush = Placeholder2.Foreground as SolidColorBrush;
            if (textBrush != null)
            {
                ColorAnimation colorAnim = new ColorAnimation
                {
                    To = foregroundColor,
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                textBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
            }
        }
    }
}

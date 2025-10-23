using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
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
using System.Security.Cryptography;
using System.Net.Http;
using Library.Models;

namespace Library.Pages
{
    /// <summary>
    /// Interaction logic for ChangePassProfileFrame.xaml
    /// </summary>
    public partial class ChangePassProfileFrame : Page
    {
        string urlServer = "http://78.29.32.36:5119/api/update/ChangePassword/";
        private Users _currentUser;
        public ChangePassProfileFrame(Users user)
        {
            _currentUser = user;
            InitializeComponent();
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

        private string GenerateComplexPassword(int length)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ" +
                                     "abcdefghijklmnopqrstuvwxyz" +
                                     "0123456789" +
                                     "!@#$%^&*?_-";

            StringBuilder password = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (password.Length < length)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    password.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }

            return password.ToString();
        }


        public async Task ChangeUserPasswordAsync(int userId, string newPassword)
        {
            var httpClient = new HttpClient();
            var url = urlServer + userId;

            var content = new StringContent($"\"{newPassword}\"", Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                passTitleText.Visibility = Visibility.Visible;
                newPassText.Visibility = Visibility.Visible;

                newPassText.Text = newPassword;

                var mainWindow = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();
                mainWindow?.errorMessage("Пароль успешно обновлен", 1);
            }
            else
            {
               
                var mainWindow = Application.Current.Windows
                 .OfType<MainWindow>()
                 .FirstOrDefault();
                mainWindow?.errorMessage("Ошибка обновления пароля", 2);
            }
        }

        private void SendEmail(string email)
        {
            try
            {
                string newPassword = GenerateComplexPassword(8);
                var fromAddress = new MailAddress("rasheb782@mail.ru", "Hudson");
                var toAddress = new MailAddress(email, "Recipient Name");
                const string subject = "Смена пароля";
                 string body = $"Вы получили это письмо, потому что поменяли свой пароль для домашней библиотеки. Ваш новый пароль - {newPassword}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.mail.ru",
                    Port = 587,
                    Credentials = new NetworkCredential("rasheb782@mail.ru", "3FMk69ZsczHt7sYPMyxD"),
                    EnableSsl = true
                };

                // Создаем сообщение
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };

                // Добавляем файл в качестве вложения
              

                // Отправляем письмо
                smtp.Send(message);

                ChangeUserPasswordAsync(_currentUser.id,newPassword);
            }
            catch (Exception ex)
            {
                var mainWindow = Application.Current.Windows
                 .OfType<MainWindow>()
                 .FirstOrDefault();
                mainWindow?.errorMessage("Укажите корректную почту", 2);
            }
        }

        private void changePass_Click(object sender, RoutedEventArgs e)
        {
            SendEmail(InputBox.Text);
        }
    }
}

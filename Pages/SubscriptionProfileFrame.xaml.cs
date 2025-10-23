using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library.Pages
{
    /// <summary>
    /// Interaction logic for SubscriptionProfileFrame.xaml
    /// </summary>
    public partial class SubscriptionProfileFrame : Page
    {
        private bool _isActive;
        public SubscriptionProfileFrame(bool isActive)
        {
            InitializeComponent();
            _isActive = isActive;

            if(_isActive)
            {
                statusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF31EC0B"));
                statusText.Text = "Активно";
            }
            else
            {
                statusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFB0707"));
                statusText.Text = "Неактивно";
            }
        }


        private async void CreatePayment()
        {
            string shopId = "1060200";
            string secretKey = "test_8hUC5HHCMD8_VQbFBN6YcH_Cw2fQ4txBQOnLEl-LFX0";

            string idempotenceKey = Guid.NewGuid().ToString();


            var httpClient = new HttpClient();
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{shopId}:{secretKey}"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var paymentRequest = new
            {
                amount = new
                {
                    value = "300.00", // сумма оплаты
                    currency = "RUB"
                },
                confirmation = new
                {
                    type = "redirect",
                    return_url = "https://example.com/thankyou" // куда вернется пользователь
                },
                capture = true,
                description = "Оплата подписки на месяц"
            };

            var json = JsonSerializer.Serialize(paymentRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{shopId}:{secretKey}")));

            httpClient.DefaultRequestHeaders.Add("Idempotence-Key", idempotenceKey);

            var response = await httpClient.PostAsync("https://api.yookassa.ru/v3/payments", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var doc = JsonDocument.Parse(responseContent);
                var url = doc.RootElement.GetProperty("confirmation").GetProperty("confirmation_url").GetString();

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show($"Ошибка создания платежа: {responseContent}");
            }
        }

        private void saveBut_Click(object sender, RoutedEventArgs e)
        {
            CreatePayment();
        }
    }
}

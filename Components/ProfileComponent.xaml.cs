using Library.Models;
using Library.Pages;
using Library.Windows;
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

namespace Library.Components
{
    /// <summary>
    /// Interaction logic for ProfileComponent.xaml
    /// </summary>
    public partial class ProfileComponent : UserControl
    {
        private Button currentlySelectedButton;
        private Users _user;
        public ProfileComponent()
        {
            InitializeComponent();

            //profileFrame.NavigationService.Navigate(new MainProfileFrame());
        }

        public void GetUser(Users user)
        {
            _user = user;

            userName.Text = user.firstName;
            profileFrame.NavigationService.Navigate(new MainProfileFrame(_user));
        }

     


        public void mainDisplay()
        {
            profileFrame.NavigationService.Navigate(new MainProfileFrame(_user));
        }

        public void subscriptionDisplay(bool isActive)
        {
            profileFrame.NavigationService.Navigate(new SubscriptionProfileFrame(isActive));
        }

        public void changePassDisplay()
        {
            profileFrame.NavigationService.Navigate(new ChangePassProfileFrame(_user));
        }
    

        public void paramDisplay()
        {

        }

        private void changeBackground_Click(object sender, RoutedEventArgs e)
        {

            foreach (var child in ButtonPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.Tag = null; 
                }
            }

             (sender as Button).Tag = "Active";
            bool isActiveBool = _user.isActive == 1 ? true : false;

            switch ((sender as Button).Content)
            {
                case "Подписка":
                    subscriptionDisplay(isActiveBool);
                    break;
                case "Главная":
                    mainDisplay();
                    break;
                case "Смена пароля":
                    changePassDisplay();
                    break;
                case "Настройки":
                    paramDisplay();
                    break;
            }
        }

        private void exitBut_Click(object sender, RoutedEventArgs e)
        {

            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            var window = Window.GetWindow(this);
            window.Close();
        }
    }
}

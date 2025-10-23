using Library.Models;
using Library.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public TranslateTransform[] transforms;
        public UIElement[] components;


        public TranslateTransform errorMessageTransform = new TranslateTransform();
        public DoubleAnimation errorAnimation = new DoubleAnimation();

        public TranslateTransform errorMessageTransformLeft = new TranslateTransform();
        public DoubleAnimation errorAnimationLeft = new DoubleAnimation();
        Users _matchedUser;

        public MainWindow(Users matchedUser)
        {

            InitializeComponent();
            errorMessageTransform.Y = 500; // Начальное положение (вне экрана снизу)
            errorComponent.RenderTransform = errorMessageTransform;

            BackgroundVideo.Play();
            components = new UIElement[] { listComponent, searchComponent, collageComponent, playerComponent, listenComponent, readComponent, profileComponent, infoBookComponent, errorComponent, errorComponentLeft };

            transforms = new TranslateTransform[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                transforms[i] = new TranslateTransform();
                components[i].RenderTransform = transforms[i];
            }

            //MainFrame.Navigate(new SearchPage());
            _matchedUser = matchedUser;
            AnimateComponent(transforms[0], 800);
            listComponent.SetData(_matchedUser.firstName, _matchedUser);
            profileComponent.GetUser(matchedUser);
            searchComponent.GetUser(matchedUser);

          
        }


        public async void errorMessage(string errorMessage, int state)
        {

            errorComponent.errorMessage(errorMessage, state);

            errorComponent.Visibility = Visibility.Visible;
         
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


        public async void errorMessageLeft(string errorMessage, int state)
        {

            errorComponentLeft.errorMessage(errorMessage, state);

            errorComponentLeft.Visibility = Visibility.Visible;

            errorComponentLeft.RenderTransform = errorMessageTransformLeft;
            errorAnimationLeft.From = 500;
            errorAnimationLeft.To = 300;
            errorAnimationLeft.Duration = TimeSpan.FromSeconds(2);
            errorAnimationLeft.SpeedRatio = 2;
            errorAnimationLeft.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            errorMessageTransformLeft.BeginAnimation(TranslateTransform.YProperty, errorAnimationLeft);

            await Task.Delay(3000);

            errorComponentLeft.RenderTransform = errorMessageTransformLeft;
            errorAnimationLeft.From = 300;
            errorAnimationLeft.To = 500;
            errorAnimationLeft.Duration = TimeSpan.FromSeconds(2);
            errorAnimationLeft.SpeedRatio = 2;
            errorAnimationLeft.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            errorMessageTransformLeft.BeginAnimation(TranslateTransform.YProperty, errorAnimationLeft);

            await Task.Delay(2000);
            errorComponentLeft.Visibility = Visibility.Hidden;
        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            BackgroundVideo.Position = TimeSpan.Zero;
            BackgroundVideo.Play();
        }



        private void AnimateComponent(TranslateTransform transform, int targetY)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = transform.Y,
                To = targetY,
                Duration = TimeSpan.FromSeconds(1),
               SpeedRatio = 2
            };
            transform.BeginAnimation(TranslateTransform.YProperty, animation);
        }


        private void listBooksBut_Click(object sender, RoutedEventArgs e)
        {

            int selectedIndex = int.Parse(((FrameworkElement)sender).Tag.ToString());

            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);
                  
                }
                else
                {

                    AnimateComponent(transforms[i], 800);
                 
                }

                listComponent.DownloadUserCollection();
            }
        }



        private void searchBut_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = int.Parse(((FrameworkElement)sender).Tag.ToString());

            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);
                   
                }
                else
                {

                    AnimateComponent(transforms[i], 800);
                   
                }
            }
        }

        private void collageBut_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = int.Parse(((FrameworkElement)sender).Tag.ToString());

            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);

                }
                else
                {

                    AnimateComponent(transforms[i], 800);

                }
            }
        }

        private void playerBut_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = int.Parse(((FrameworkElement)sender).Tag.ToString());

            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);

                }
                else
                {

                    AnimateComponent(transforms[i], 800);

                }
            }


            playerComponent.SetData(searchComponent.GetObject(), _matchedUser.firstName, _matchedUser);
        }

       


        public void readComponentFunc(UpdateObjectModel objectUpdate)
        {
            int selectedIndex = 5;

            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);

                }
                else
                {

                    AnimateComponent(transforms[i], 800);

                }

                readComponent.LoadBook(objectUpdate);
            }
        }

        public void listenComponentFunc(UpdateObjectModel objectUpdate)
        {
            int selectedIndex = 4;

            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);

                }
                else
                {

                    AnimateComponent(transforms[i], 800);

                }
            }

            listenComponent.SetData(objectUpdate, _matchedUser.firstName);
        }

        public void infoBookComponentFunc(UpdateObjectModel objectUpdate, Button _button)
        {
            int selectedIndex = 7;
            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);

                }
                else
                {

                    AnimateComponent(transforms[i], 800);

                }
            }


            infoBookComponent.SetData(_matchedUser.firstName, objectUpdate, _button, _matchedUser);

        }

        public void componentFunc()
        {
            int selectedIndex = 4;

            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);

                }
                else
                {

                    AnimateComponent(transforms[i], 800);

                }
            }
        }

        private void profileBut_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = int.Parse(((FrameworkElement)sender).Tag.ToString());

            for (int i = 0; i < components.Length; i++)
            {
                if (i != selectedIndex)
                {

                    AnimateComponent(transforms[i], -1600);

                }
                else
                {

                    AnimateComponent(transforms[i], 800);

                }
            }
        }
    }
}

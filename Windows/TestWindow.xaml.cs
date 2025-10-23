using Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Library.Windows
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private int _currentIndex = 0;
        private const int _visibleItemsCount = 3; // сколько элементов видно сразу
        private const double _itemWidth = 230;    // ширина одного item включая margin
        private StackPanel _stackPanel;
        public TestWindow()
        {
            InitializeComponent();
            

        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Заполнение данными

            var _items = new ObservableCollection<Books>()
            {
                new Books{fileName = "Снафф"},
                   new Books{fileName = "Солярис"},
                      new Books{fileName = "Кибернетика"},
                         new Books{fileName = "Чапаев и пустота"},
                            new Books{fileName = "Оно"},
                               new Books{fileName = "Иметь или быть"},
                                  new Books{fileName = "Собирайся"}
            };

            SliderItems.ItemsSource = _items;

            // Находим StackPanel в визуальном дереве
            _stackPanel = FindVisualChild<StackPanel>(SliderItems);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex + _visibleItemsCount < SliderItems.Items.Count)
            {
                _currentIndex++;
                AnimateSlide();
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                AnimateSlide();
            }
        }

        private void AnimateSlide()
        {
            if (_stackPanel == null)
                return;

            double toValue = -_currentIndex * _itemWidth;

            var transform = _stackPanel.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                _stackPanel.RenderTransform = transform;
            }

            var animation = new DoubleAnimation(toValue, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

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

    }


}


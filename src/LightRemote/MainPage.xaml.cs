using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LightRemote
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            progressRing.Visibility = Visibility.Visible;
            progressRing.IsActive = true;
            lightList.IsEnabled = false;
            refreshButton.IsEnabled = false;
            await RefreshList();
            progressRing.Visibility = Visibility.Collapsed;
            progressRing.IsActive = false;
            lightList.IsEnabled = true;
            refreshButton.IsEnabled = true;
        }

        private async void LightOnOffButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (FrameworkElement)sender;
            var mode = (LightMode)button.DataContext;
            var model = FindParentDataContext(button);
            var light = (Light)FindParentDataContext(model).DataContext;

            light.RequestedMode = mode.ID;
            await light.TurnOn();
        }

        private async Task RefreshList()
        {
            await LightManager.Instance.FindConnectedLights();
            lightList.ItemsSource = LightManager.Instance.Lights.Values.OrderBy(x => x.Location);
        }

        private FrameworkElement FindParentDataContext(DependencyObject element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            if (parent is FrameworkElement p && p.DataContext != null)
            {
                return p;
            }
            else if (parent != null)
            {
                return FindParentDataContext(parent);
            }
            else
            {
                return null;
            }
        }
    }
}

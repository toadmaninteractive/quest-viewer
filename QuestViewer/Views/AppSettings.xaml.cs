using System.Windows.Forms;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuestGraph.Core;
using System;

namespace QuestViewer
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class AppSettings : Window
    {
        public bool IsUpdateChannelHasChanged { get; private set; }

        public AppSettings()
        {
            InitializeComponent();
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            var config = Config.Instance;
            switch (config.UpdateChannel)
            {
                case ApplicationUpdateChannel.Stable:
                    ctrStableUpdateChannel.IsChecked = true;
                    break;
                case ApplicationUpdateChannel.Beta:
                    ctrBetaUpdateChannel.IsChecked = true;
                    break;
                case ApplicationUpdateChannel.Dev:
                    ctrDevUpdateChannel.IsChecked = true;
                    break;
                default:
                    throw ExceptionConstructor.UnexpectedEnumValue(config.UpdateChannel, Environment.NewLine);
            }
        }

        private void SaveHandler(object sender, RoutedEventArgs e)
        {
            var selectedAppUpdateChannel = ApplicationUpdateChannel.Stable;
            if (ctrBetaUpdateChannel.IsChecked ?? false)
                selectedAppUpdateChannel = ApplicationUpdateChannel.Beta;
            else
                if (ctrDevUpdateChannel.IsChecked ?? false)
                selectedAppUpdateChannel = ApplicationUpdateChannel.Dev;

            var config = Config.Instance;
            IsUpdateChannelHasChanged = config.UpdateChannel != selectedAppUpdateChannel;
                       
            config.UpdateChannel = selectedAppUpdateChannel;
            config.Save();
            DialogResult = true;
        }
    }
}

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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuestViewer
{
    /// <summary>
    /// Interaction logic for AboutDialogView.xaml
    /// </summary>
    public partial class AboutDialogView : Window
    {
        public string AppVersion { get; set; } 

        public AboutDialogView()
        {
            InitializeComponent();
            AppVersion = Utils.GetAppVersionRevosionFormat();
            DataContext = this;
        }
    }
}

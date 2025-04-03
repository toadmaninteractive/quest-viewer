using QuestGraph.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuestViewer
{
    /// <summary>
    /// Interaction logic for ValidationComboBox.xaml
    /// </summary>
    public partial class ValidationComboBox : UserControl
    {
        public event Action<object, EventArgs> DropDownOpened;

        public static DependencyProperty ItemsSourceValueProperty =
            DependencyProperty.Register("ItemsSourceValue", typeof(IEnumerable), typeof(ValidationComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public IEnumerable ItemsSourceValue
        {
            get { return (IEnumerable)GetValue(ItemsSourceValueProperty); }
            set { SetValue(ItemsSourceValueProperty, value); }
        }


        public static DependencyProperty TextValueProperty =
            DependencyProperty.Register("TextValue", typeof(string), typeof(ValidationComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string TextValue
        {
            get { return (string)GetValue(TextValueProperty); }
            set { SetValue(TextValueProperty, value); }
        }

        public static DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(ValidationErrorsModel), typeof(ValidationComboBox));

        public ValidationErrorsModel Errors
        {
            get { return (ValidationErrorsModel)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        public static DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(ValidationComboBox), new PropertyMetadata(TextWrapping.NoWrap));

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public static DependencyProperty HeightValueProperty =
            DependencyProperty.Register("HeightValue", typeof(double), typeof(ValidationComboBox), new PropertyMetadata(32D));

        public double HeightValue
        {
            get { return (double)GetValue(HeightValueProperty); }
            set { SetValue(HeightValueProperty, value); }
        }

        public static DependencyProperty HeightTotalValueProperty =
            DependencyProperty.Register("HeightTotalValue", typeof(double), typeof(ValidationComboBox), new PropertyMetadata(50D));

        public double HeightTotalValue
        {
            get { return (double)GetValue(HeightTotalValueProperty); }
            set { SetValue(HeightTotalValueProperty, value); }
        }

        public static DependencyProperty AcceptsReturnValueProperty =
            DependencyProperty.Register("AcceptsReturnValue", typeof(bool), typeof(ValidationComboBox), new PropertyMetadata(false));

        public bool AcceptsReturnValue
        {
            get { return (bool)GetValue(AcceptsReturnValueProperty); }
            set { SetValue(AcceptsReturnValueProperty, value); }
        }

        public string InputControlId { get; set; }

        private bool hasFocusBefore = false;
        private bool hasFocus = false;
        private ErrorForegroundConverter errorForegroundConverter = new ErrorForegroundConverter();

        public ValidationComboBox()
        {
            InitializeComponent();
            Loaded += LoadedHandler;
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            if (Errors != null)
                Errors.OnChanged += ErrorsChangedHandler;
        }

        private void ErrorsChangedHandler(ValidationErrorsModel.ErrorShowingMode mode)
        {
            bool showErrorTextCondition;
            if (!hasFocusBefore)
            {
                hasFocusBefore = true;
                switch (mode)
                {
                    case ValidationErrorsModel.ErrorShowingMode.ShowIfHasError:
                        showErrorTextCondition = Errors.ContainsKey(InputControlId);
                        break;
                    case ValidationErrorsModel.ErrorShowingMode.ShowIfHasErrorAndFocus:
                        showErrorTextCondition = Errors.ContainsKey(InputControlId) && hasFocus;
                        break;
                    default:
                        throw ExceptionConstructor.UnexpectedEnumValue(mode, Environment.StackTrace);
                }
            }
            else
                showErrorTextCondition = Errors.ContainsKey(InputControlId);


            if (showErrorTextCondition)
            {
                ErrorControl.Text = Errors[InputControlId].NotificationText;
                ErrorControl.Foreground = (Brush)errorForegroundConverter.Convert(Errors[InputControlId].NotificationType, null, null, CultureInfo.InvariantCulture);
            }
            else
            {
                ErrorControl.Text = string.Empty;
                ErrorControl.Foreground = Brushes.Black;
            }
        }

        private void GotFocusHandler(object sender, RoutedEventArgs e)
        {
            hasFocus = true;
        }

        private void LostFocusHandler(object sender, RoutedEventArgs e)
        {
            Errors?.LostFocusRaise(InputControlId);
        }

        private void DropDownOpenedHandler(object sender, EventArgs e)
        {
            DropDownOpened?.Invoke(sender, e);
        }
    }
}
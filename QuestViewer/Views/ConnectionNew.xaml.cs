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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuestViewer
{
    /// <summary>
    /// Interaction logic for ConnectionNew.xaml
    /// </summary>
    public partial class ConnectionNew : Window
    {
        private ValidationErrorsViewModel validationErrorsContext;

        public ConnectionNew()
        {
            InitializeComponent();
            DataContextChanged += DataContextChangedHandler;
        }

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (validationErrorsContext != null) 
                validationErrorsContext.Errors.OnChanged -= ErrorsChangedHandler;

            validationErrorsContext = DataContext as ValidationErrorsViewModel;
            validationErrorsContext.Errors.OnChanged += ErrorsChangedHandler;

            //Init validation
            var viewModel = DataContext as CdbConnectionNewViewModel;
            viewModel.FieldsValidation();
            validationErrorsContext.Errors["Password"] = new UserNotification(Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
            validationErrorsContext.Errors.Changed();
        }

        private void ErrorsChangedHandler(ValidationErrorsModel.ErrorShowingMode obj)
        {
            var viewModel = DataContext as CdbConnectionNewViewModel;
            var isEnable = validationErrorsContext.Errors.ErrorsCount == 0;
            if (isEnable != btnSave.IsEnabled)
                btnSave.IsEnabled = isEnable;
        }

        private void PreviewKeyUpHandler(object sender, KeyEventArgs e)
        {
            PasswordValidation();
        }

        private void SaveClickHandler(object sender, RoutedEventArgs e)
        {
            PasswordValidation();
            var viewModel = DataContext as CdbConnectionNewViewModel;
            viewModel.FieldsValidation();
            if (viewModel.Errors.ErrorsCount == 0)
                DialogResult = true;
        }

        private void PasswordValidation()
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                validationErrorsContext.Errors["Password"] = new UserNotification(Texts.View.ConnectionManager.FieldIsEmpty, UserNotificationType.Error);
                txtPasswordError.Text = validationErrorsContext.Errors["Password"].NotificationText;
            }
            else
            {
                validationErrorsContext.Errors.Remove("Password");
                txtPasswordError.Text = string.Empty;
            }
            validationErrorsContext.Errors.Changed();
        }

        private void UpdateDataBasesHandler(object sender, RoutedEventArgs e)
        {
            UpdateDataBases().Track();
        }

        private void DropDownOpenedHandler(object arg1, EventArgs arg2)
        {
            UpdateDataBases().Track();
        }

        private async Task UpdateDataBases()
        {
            var viewModel = DataContext as CdbConnectionNewViewModel;
            PasswordValidation();
            viewModel.ServerRequestFieldsValidation();
            if (validationErrorsContext.Errors.ContainsKey("Password") || validationErrorsContext.Errors.ContainsKey("Url") || validationErrorsContext.Errors.ContainsKey("Username"))
                return;

            var passwordBytes = SerializationUtils.ConvertPasswordToBytes(PasswordBox.SecurePassword);
            CdbServer cdbServer = new CdbServer(new Uri(viewModel.Url), viewModel.Username, passwordBytes);
            var response = await cdbServer.GetDatabaseNamesAsync();
            viewModel.ServerDataBases = new List<string>(response);
        }
    }
}

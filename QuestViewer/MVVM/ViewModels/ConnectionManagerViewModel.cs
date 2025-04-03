using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuestViewer
{
    public class ConnectionManagerViewModel : DialogResultViewModel
    {
        public ICommand AddConnectionCommand { get; set; }
        public ICommand RemoveConnectionCommand { get; set; }
        public ICommand ConnectCommand {  get; set; }

        public ObservableCollection<CdbConnection> Connections
        {
            get { return connections; }
            set { connections = value; }
        }

        public CdbConnection SelectedConnecton { get; set; }

        private CdbConnection activeConnection;
        private ObservableCollection<CdbConnection> connections;
        private readonly Window owner;

        public ConnectionManagerViewModel(Window owner, CdbConnection activeConnection)
        {
            AddConnectionCommand = new RelayCommand(() => AddConnectionHandler(out _));
            RemoveConnectionCommand = new RelayCommand(RemoveConnectionHandler, () => SelectedConnecton != null && !SelectedConnecton.AreEquals(activeConnection));
            ConnectCommand = new RelayCommand(ConnectHandler, () => SelectedConnecton != null);
            this.owner = owner;
        }

        private void ConnectHandler()
        {
            if (SelectedConnecton != null)
                DialogResult = true;
        }

        public void LoadConnectios()
        {
            var connectionSerializationObject = SerializationUtils.LoadXml<CdbConnectionSerialization>(FileAndFolderNames.ConnectionsXmlFilePath, (x) => x.Connections = new List<CdbConnection>());
            Connections = new ObservableCollection<CdbConnection>(connectionSerializationObject.Connections);
        }

        public bool AddConnectionHandler(out CdbConnection newDbConnection)
        {
            var connectionNewVM = new CdbConnectionNewViewModel(Connections.Select(x => x.Title).ToList());
            var connectionNew = new ConnectionNew();
            if (Connections.Any())
            {
                var lastConnection = Connections.Last();
                connectionNewVM.Url = lastConnection.Url;
                connectionNewVM.Username = lastConnection.Username;
                connectionNew.PasswordBox.Password = Encoding.Unicode.GetString(ProtectedData.Unprotect(lastConnection.Password, null, DataProtectionScope.CurrentUser));
            }            
           
            connectionNew.DataContext = connectionNewVM;
            connectionNew.Owner = owner;
            if (connectionNew.ShowDialog() ?? false)
            {
                var passwordBytes = SerializationUtils.ConvertPasswordToBytes(connectionNew.PasswordBox.SecurePassword);
                var connection = new CdbConnection(connectionNewVM.Title, connectionNewVM.Url, connectionNewVM.Username, passwordBytes, connectionNewVM.Database);
                Connections.Add(connection);

                var connectionSerializationObject = new CdbConnectionSerialization();
                connectionSerializationObject.Connections = Connections.ToList();
                SerializationUtils.Serialize<CdbConnectionSerialization>(FileAndFolderNames.ConnectionsXmlFilePath, connectionSerializationObject);

                newDbConnection = connection;
                return true;
            }
            newDbConnection = null;
            return false;
        }

        private void RemoveConnectionHandler()
        {
            if (SelectedConnecton != null && Connections != null)
            {
                var confirmationViewModel = new ConfirmationViewModel(Texts.View.ConnectionManager.RemoveConnectionConfirmation);
                var confirmationView = new ConfirmationView();
                confirmationView.DataContext = confirmationViewModel;

                if (confirmationView.ShowDialog() ?? false)
                {
                    Connections.Remove(SelectedConnecton);
                    SelectedConnecton = null;
                    var connectionSerializationObject = new CdbConnectionSerialization();
                    connectionSerializationObject.Connections = Connections.ToList();
                    SerializationUtils.Serialize<CdbConnectionSerialization>(FileAndFolderNames.ConnectionsXmlFilePath, connectionSerializationObject);
                }
            }
        }
    }
}
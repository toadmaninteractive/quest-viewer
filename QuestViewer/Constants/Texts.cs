using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public static class Texts
    {
        public static string Ok { get; set; } = "OK";
        public static string Save { get; set; } = "Save";
        public static string Cancel { get; set; } = "Cancel";
        public static string Apply { get; set; } = "Apply";
        public static string Remove { get; set; } = "Remove";
        public static string No { get; set; } = "No";
        public static string Anonymous { get; set; } = "anonymous";
        public static string Confirmation { get; set; } = "Confirmation";
        public static string OperationFailedCaption { get; set; } = "Operation failed";

        public static class View
        {
            public static class ConnectionManager
            {
                public static string Title { get; set; } = "Database Connections";
                public static string Password { get; set; } = "Password";
                public static string UserName { get; set;} = "Username";
                public static string DatabaseName { get; set;} = "Database";
                public static string ConnectionTitle { get; set;} = "Connection Title";
                public static string Url { get; set;} = "Database URL";

                public static string AddConnection { get; set; } = "New Connection";
                public static string Connect { get; set; } = "Connect";

                public static string CreateConnection { get; set; } = "Create New Connection";
                public static string RemoveConnection { get; set; } = "Remove Connection";

                public static string FieldIsEmpty { get; set; } = "Required field is not set";
                public static string TitleIsExist { get; set; } = "Database connection with the same title already exists";

                public static string RemoveConnectionConfirmation { get; set; } = "Are you sure you want to remove the connection?";
            }

            public static class QuestViewer
            {
                public static string Title { get; set; } = "Quest Viewer";

                public static string BlockGroupCreateButtonCaption { get; set; } = "Create Croup";
                public static string BlockGroupDeleteButtonCaption { get; set; } = "Delete group";


                public static string LocalSorage { get; set; } = "Local Storage";

                public static string IsNotQuestGraph { get; set; } = "Selected item is not Quest graph";


                public static string Original { get; set; } = "Original";
                public static string About { get; set; } = "About Quest Viewer";

                public static class MainMenu
                {
                    public static string Main { get; set; } = "Main";

                    public static string NewConnections { get; set; } = "New Connection...";
                    public static string ConnectionManager { get; set; } = "Manage Connections...";
                    public static string Connections { get; set; } = "Open Connection";
                    public static string CloseConnection { get; set; } = "Close Connection";

                    public static string LocalPresetsManage { get; set; } = "Manage Local Presets...";
                    public static string SaveLocalPreset { get; set; } = "Save as Local Preset...";
                    public static string Settings { get; set; } = "Settings";
                    public static string OpenLogsFolder { get; set; } = "Open Logs Folder";
                    public static string Exit { get; set; } = "Exit";
                    public static string Help { get; set; } = "Help";
                    public static string About { get; set; } = "About";
                }
            }
            
            public static class SaveStateDialog
            {
                public static string Title { get; set; } = "Save State";
                public static string StateItemName { get; set; } = "Preset name:";

                public static string NameIsEmpty { get; set; } = "Preset name is not set";
                public static string NameIsExist { get; set; } = "Preset with the same name already exists. Existing preset will be overwritten";
            }

            public static class LocalStateManagerDialog
            {
                public static string Title { get; set; } = "Local Presets";
                public static string Save { get; set; } = "Save";
                public static string AddPreset { get; set; } = "Add Local Preset";
                public static string Remove { get; set; } = "Remove";
                public static string RemoveConfirmation { get; set; } = "Are you sure you want to remove the Preset?";
            }

            public static class GraphStateLoader
            {
                public static string Title { get; set; } = "Graph State Loader";
                public static string StateItemName { get; set; } = "Graph State Name:";
            }

            public static class AppSettings
            {
                public static string Title { get; set; } = "Application Settings";
                public static string AppUpdateChannel { get; set; } = "Application Update Channel";
            }
        }

        public static class CodeErrors
        {
            public const string UnexpectedEnumValue = "Unexpected value {0} for {1} enum";
            public const string ChronosLog = "Chronos log report failed";
            public const string ChronosConnectionClosed = "Chronos connection closed with status {0} ({1})";
            public const string DataBaseIsNotFound = "Database not exists:\n{0}{1}";
        }

        public static class ApplycationApdater
        {
            /// <summary>
            /// Failed to check for an Application update
            /// </summary>
            public const string CheckApplicationUpdates = "Failed to check for application updates";
            /// <summary>
            /// The application cannot receive data from the update server at the specified address
            /// </summary>
            public const string ServerNotFound = "Update server not found";
            public static string UpdateIsAvailableNotification { get; set; } = "Application update available";
        }
    }
}
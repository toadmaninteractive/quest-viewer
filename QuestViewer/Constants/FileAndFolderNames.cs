using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestViewer
{
    public static class FileAndFolderNames
    {
        public const string AppDataFolderName = @"Toadman Interactive\QuestViewer";
        public const string UserLocalSavesName = "UserLocalSaves";
        public const string GraphCustomPresetsFileName = "CustomPresets";
        public const string GraphBlockGroupFileName = "BlockGroups";
        public const string ConfigFileName = "config.xml";
        public const string ConnectionsFileName = "Connections.xml";

        public static readonly string ConfigXmlFilePath;
        public static readonly string AppDataFolderPath;
        public static readonly string UserLocalSavesFolder;
        public static readonly string ConnectionsXmlFilePath;

        static FileAndFolderNames()
        {
            AppDataFolderPath =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    AppDataFolderName);

            ConfigXmlFilePath = Path.Combine(AppDataFolderPath, ConfigFileName);
            UserLocalSavesFolder = Path.Combine(AppDataFolderPath, UserLocalSavesName);
            ConnectionsXmlFilePath = Path.Combine(AppDataFolderPath, ConnectionsFileName);
        }

        public static void Init()
        {
            if (!Directory.Exists(AppDataFolderPath))
                Directory.CreateDirectory(AppDataFolderPath);

            if (!Directory.Exists(UserLocalSavesFolder))
                Directory.CreateDirectory(UserLocalSavesFolder);
        }
    }
}
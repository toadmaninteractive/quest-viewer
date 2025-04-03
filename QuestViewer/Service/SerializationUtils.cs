using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

namespace QuestViewer
{
    public class SerializationUtils
    {
        /// <param name="fileName">Full path to file</param>
        public static T LoadXml<T>(string xmlFilePath, Action<T> Initialization = null) where T : class
        {
            T result = null;

            if (File.Exists(xmlFilePath))
                using (FileStream fsSource = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read))
                    result = Deserialize<T>(fsSource);

            if (result == null)
            {
                result = (T)Activator.CreateInstance(typeof(T));
                Initialization?.Invoke(result);
                Serialize<T>(xmlFilePath, result);
            }

            return result;
        }

        public static T Deserialize<T>(Stream stream) where T : class
        {
            var writerSettings = new XmlReaderSettings();
            var serializer = new XmlSerializer(typeof(T));

            using (var reader = XmlReader.Create(stream, writerSettings))
                return (T)serializer.Deserialize(reader);
        }

        public static void Serialize<T>(string filePath, object serializeObject) where T : class
        {
            var writerSettings = new XmlWriterSettings();
            writerSettings.NewLineChars = "\r\n";
            writerSettings.Indent = true;
            writerSettings.IndentChars = "\t";
            writerSettings.Encoding = new UTF8Encoding(false);

            var serializer = new XmlSerializer(typeof(T));
            using (var writer = XmlWriter.Create(filePath, writerSettings))
                serializer.Serialize(writer, serializeObject);
        }


        public static byte[] ConvertPasswordToBytes(SecureString securePassword)
        {
            return ProtectedData.Protect(Encoding.Unicode.GetBytes(ConvertToUnsecureString(securePassword).Trim()), null, DataProtectionScope.CurrentUser);
        }

        private static string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null)
                return string.Empty;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
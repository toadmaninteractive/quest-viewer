using System;

namespace QuestViewer
{
    public class CdbConnection
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }

        public CdbConnection() { }

        public CdbConnection(string title, string uri, string userName, byte[] password, string database)
        {
            Title = title;
            Url = uri;
            Username = userName;
            Password = password;
            Database = database;
        }

        public bool AreEquals(CdbConnection other)
        {
            if (other == null)
                return false;

            return Title == other.Title &&
                Url == other.Url &&
                Database == other.Database &&
                Username == other.Username;
        }
    }
}
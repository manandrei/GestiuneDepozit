namespace GestiuneDepozit.Data.Models
{
    public class Parameters
    {
        public bool AcceptedEULA { get; set; }
        public bool FirstConfiguration { get; set; }
        public string DatabaseServerType { get; set; }
        public string Database { get; set; }
        public string ServerAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsTrustedConnection { get; set; }
    }
}

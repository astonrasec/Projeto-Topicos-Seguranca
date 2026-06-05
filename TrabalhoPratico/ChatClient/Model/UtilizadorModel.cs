namespace ChatClient
{

    public class UtilizadorModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public UtilizadorModel() { }

        public UtilizadorModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}

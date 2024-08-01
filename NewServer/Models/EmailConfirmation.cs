namespace NewServer.Models
{
    public class EmailConfirmation
    {
        public User? user { get; set; }
        public string? authenticationCode { get; set; }
        public bool insertUserToDatabase { get; set; }
    }
}

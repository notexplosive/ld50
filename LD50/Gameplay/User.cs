namespace LD50.Gameplay
{
    public class User
    {
        public User(string firstName, string lastName)
        {
            Username = $"@{firstName}_{lastName}";
        }

        public string Username { get; }
    }
}

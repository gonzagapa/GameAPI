namespace GameStore.Models {
    
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty; 

        public string HashPassword { get; set; } = string.Empty; 

        public string Role { get; set; } = string.Empty;
    }
}
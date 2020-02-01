namespace Common.Authenticates
{
    public class BaseUserInfo : IBaseUserInfo
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public UserStatus Status { get; set; }
    }
}
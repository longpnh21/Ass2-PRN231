using BusinessObject;

namespace EBookStoreWebAPI.Models
{
    public class AuthenticateResponse
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse()
        {

        }

        public AuthenticateResponse(User user, string token)
        {
            UserId = user.UserId;
            Role = user.Role;
            EmailAddress = user.EmailAddress;
            Token = token;
        }
    }
}

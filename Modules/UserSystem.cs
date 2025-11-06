using System.Collections.Generic;
using System.Linq;

namespace SARAI.Modules
{
    public enum UserRole { Admin, Cashier, Viewer }

    public sealed class UserSession
    {
        public static UserSession Current { get; } = new();
        public string Username { get; private set; } = "admin";
        public UserRole Role { get; private set; } = UserRole.Admin;
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Username);
        private UserSession() { }
        public void SignIn(string username, UserRole role) { Username = username; Role = role; }
        public void SignOut() { Username = ""; Role = UserRole.Viewer; }
        public bool CanExportReports => Role is UserRole.Admin or UserRole.Cashier;
    }

    public static class UserSystem
    {
        private static readonly List<(string u, string p, UserRole r)> _users = new()
        {
            ("admin","1234",UserRole.Admin),
            ("kasiyer","0000",UserRole.Cashier)
        };

        public static (bool ok, UserRole role) Login(string username, string password)
        {
            var hit = _users.FirstOrDefault(x => x.u == username && x.p == password);
            return hit.u == null ? (false, UserRole.Viewer) : (true, hit.r);
        }
    }
}

namespace YuDB.Security
{
    public interface ISecurityEngine
    {
        bool Authenticate(string username, string password);
        void ChangeUsername(string newUsername);
        void ChangePassword(string newPassword);
    }
}

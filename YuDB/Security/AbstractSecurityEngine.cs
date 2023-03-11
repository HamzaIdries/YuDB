namespace YuDB.Security
{
    /// <summary>
    /// Manages everything related to user authentication
    /// </summary>
    public abstract class AbstractSecurityEngine
    {
        /// <summary>
        /// Creates the user with the given credentials and stores them securely in a password file
        /// </summary>
        public abstract void CreateUser(Credentials credentials);

        /// <summary>
        /// Authenticates a user by validating the given credentials against the password file
        /// </summary>
        public abstract bool Authenticate(Credentials credentials);

        /// <summary>
        /// Checks whether there is a registered user or not by seeing whether there is a valid (non-modified)
        /// password file
        /// </summary>
        public abstract bool UserRegistered();
    }
}
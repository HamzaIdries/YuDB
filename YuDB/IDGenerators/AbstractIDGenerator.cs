namespace YuDB.IDGenerators
{
    /// <summary>
    /// Generates unique ids for each document
    /// </summary>
    public abstract class AbstractIDGenerator
    {
        /// <summary>
        /// Adds an id field with the name "$id" to the document
        /// </summary>
        public abstract string AddID(string document, string ID);

        /// <summary>
        /// Generates a unique id
        /// </summary>
        public abstract string GenerateID();
    }
}
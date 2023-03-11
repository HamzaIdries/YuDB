namespace YuDB.IDGenerators
{
    public abstract class AbstractIDGenerator
    {
        public abstract string AddID(string document, string ID);
        public abstract string GenerateID();
    }
}
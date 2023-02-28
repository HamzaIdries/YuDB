namespace YuDB.Constraints
{
    class StringTypeConstraint : PrimitiveTypeConstraint<string>
    {
        override public string ToString()
        {
            return "TypeConstraint: String";
        }
    }
}

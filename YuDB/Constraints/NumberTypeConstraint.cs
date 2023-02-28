namespace YuDB.Constraints
{
    class NumberTypeConstraint : PrimitiveTypeConstraint<decimal>
    {
        override public string ToString()
        {
            return "TypeConstraint: Number";
        }
    }
}

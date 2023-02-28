namespace YuDB.Constraints
{
    class BooleanTypeConstraint : PrimitiveTypeConstraint<bool>
    {
        public override string ToString()
        {
            return "TypeConstraint: Boolean";
        }
    }
}

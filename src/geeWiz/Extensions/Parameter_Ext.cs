namespace geeWiz.Extensions
{
    public static class Parameter_Ext
    {
        public static bool Ext_HasUnitType(this Parameter parameter)
        {
            if (parameter == null) { return false; }

            ForgeTypeId spec = parameter.GetUnitTypeId();

            return parameter.StorageType == StorageType.Double
                && spec is not null
                && spec.IsValidObject;
        }
    }
}

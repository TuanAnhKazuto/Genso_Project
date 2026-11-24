public static class ElementRelation
{
    public static ElementType GetGeneratedElement(ElementType element)
    {
        return element switch
        {
            ElementType.Metallic => ElementType.Aqueous,
            ElementType.Aqueous => ElementType.Botanical,
            ElementType.Botanical => ElementType.Thermal,
            ElementType.Thermal => ElementType.Telluric,
            ElementType.Telluric => ElementType.Metallic,
            _ => element
        };
    }

    public static ElementType GetOvercomeElement(ElementType element)
    {
        return element switch
        {
            ElementType.Metallic => ElementType.Botanical,
            ElementType.Botanical => ElementType.Telluric,
            ElementType.Telluric => ElementType.Aqueous,
            ElementType.Aqueous => ElementType.Thermal,
            _ => element
        };
    }

    public static bool Generates(ElementType source, ElementType target)
    {
        return GetGeneratedElement(source).Equals(target);
    }

    public static bool Overcomes(ElementType source, ElementType target)
    {
        return GetOvercomeElement(source).Equals(target);
    }
}
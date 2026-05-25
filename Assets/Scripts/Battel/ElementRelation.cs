public static class ElementRelation
{
    public static ElementType GetGeneratedElement(ElementType element)
    {
        return element switch
        {
            ElementType.Metal => ElementType.Water,
            ElementType.Water => ElementType.Wood,
            ElementType.Wood => ElementType.Fire,
            ElementType.Fire => ElementType.Earth,
            ElementType.Earth => ElementType.Metal,
            _ => element
        };
    }

    public static ElementType GetOvercomeElement(ElementType element)
    {
        return element switch
        {
            ElementType.Metal => ElementType.Wood,
            ElementType.Wood => ElementType.Earth,
            ElementType.Earth => ElementType.Water,
            ElementType.Water => ElementType.Fire,
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
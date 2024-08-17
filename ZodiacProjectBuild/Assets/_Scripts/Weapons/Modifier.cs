public abstract class Modifier { }

public abstract class Modifier<T> : Modifier
{
    public abstract T ModifyValue(T value);
}
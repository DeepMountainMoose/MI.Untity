namespace MI
{
    public interface IWrapper<out T>
    {
        T Value { get; }
    }
}

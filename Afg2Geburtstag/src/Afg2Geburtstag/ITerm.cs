namespace Afg2Geburtstag
{
    public interface ITerm
    {
        Rational Value { get; }
        string ToString();
        string ToLaTeX();
    }
}

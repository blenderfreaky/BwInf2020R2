namespace Afg2Geburtstag
{
    /// <summary>
    /// Represents an arbitrary mathematical teerm.
    /// </summary>
    public interface ITerm
    {
        /// <summary>
        /// The value of the term.
        /// </summary>
        Rational Value { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        string ToString();

        /// <summary>
        /// Returns a string that represents the current object as latex code.
        /// </summary>
        /// <returns>A string that represents the current object as latex code.</returns>
        string ToLaTeX();
    }
}
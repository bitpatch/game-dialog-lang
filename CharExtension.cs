namespace BitPatch.DialogLang
{
    /// <summary>
    /// Extension methods for character checks.
    /// </summary>
    internal static class CharExtension
    {
        /// <summary>
        /// Checks if the character is a newline character.
        /// </summary>
        public static bool IsNewLine(this char c)
        {
            return c is '\n' or '\r' or '\u2028' or '\u2029' or '\u0085';
        }

        /// <summary>
        /// Checks if the integer represents a newline character.
        /// </summary>
        public static bool IsNewLine(this int n)
        {
            return n is not -1 && ((char)n).IsNewLine();
        }

        /// <summary>
        /// Checks if the integer represents a whitespace character that is not a newline.
        /// </summary>
        public static bool IsWhiteSpace(this int n)
        {
            return n is not -1 && char.IsWhiteSpace((char)n) && !((char)n).IsNewLine();
        }

        /// <summary>
        /// Checks if the integer represents a valid identifier character.
        /// </summary>
        public static bool IsIdentifierChar(this int n)
        {
            if (n is -1)
            {
                return false;
            }

            var c = (char)n;
            return c is '_' || char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// Checks if the integer represents a digit character.
        /// </summary>
        public static bool IsDigit(this int n)
        {
            return n is not -1 && char.IsDigit((char)n);
        }

        /// <summary>
        /// Checks if the integer represents a valid character.
        /// </summary>
        public static bool IsChar(this int n)
        {
            return n is not -1;
        }
    }
}
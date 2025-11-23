using System;

namespace BitPatch.DialogLang
{
    /// <summary>
    /// Exception thrown when an error occurs during script execution.
    /// </summary>
    public class ScriptException : Exception
    {
        public int Line => Location.Line;
        public int Initial => Location.Initial;
        public int Final => Location.Final;
        
        /// <summary>
        /// Gets the source code input where the error occurred.
        /// </summary>
        public Source SourceCode => Location.Source;

        internal Location Location { get; }

        internal ScriptException(string message, Location location) : base(message)
        {
            Location = location;
        }
    }

    public class InvalidSyntaxException : ScriptException
    {
        internal InvalidSyntaxException(Location location)
            : base("Invalid syntax", location)
        {
        }

        internal InvalidSyntaxException(Source source, int line, int column)
            : this(new Location(source, line, column))
        {
        }

        internal InvalidSyntaxException(string message, Location location)
            : base("Invalid syntax: " + message, location)
        {
        }

        internal InvalidSyntaxException(string message, Source source, int line, int column)
            : this(message, new Location(source, line, column))
        {
        }
    }
}

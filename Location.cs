using System;

namespace BitPatch.DialogLang
{
    /// <summary>
    /// Represents a position range in the source code.
    /// </summary>
    internal readonly struct Location
    {
        /// <summary>
        /// The source code input.
        /// </summary>
        public Source Source { get; }

        /// <summary>
        /// Line number (1-based).
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Start column (1-based).
        /// </summary>
        public int Initial { get; }

        /// <summary>
        /// End column (1-based, exclusive).
        /// </summary>
        public int Final { get; }

        /// <summary>
        /// Length of the location range.
        /// </summary>
        public int Length => Final - Initial;

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> struct.
        /// </summary>
        public Location(Source source, int line, int position) : this(source, line, position, position)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> struct.
        /// </summary>
        public Location(Source source, int line, int initial, int final)
        {
            Source = source;
            Line = line;
            Initial = initial;
            Final = final;
        }

        /// <summary>
        /// Combines two locations into a single range spanning from the start of the first to the end of the second.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the sources or lines of the two locations differ.</exception>
        public static Location operator |(Location left, Location right)
        {
            if (left.Source != right.Source)
            {
                throw new ArgumentException($"Cannot combine locations from different sources. Left source: {left.Source}], Right source: {right.Source}");
            }

            if (left.Line != right.Line)
            {
                throw new ArgumentException($"Cannot combine locations from different lines. Left line: {left.Line}, Right line: {right.Line}");
            }

            return new Location(left.Source, left.Line, left.Initial, right.Final);
        }

        /// <summary>
        /// Creates a new location with the specified length starting from the same position as the current location.
        /// </summary>
        /// <param name="location">The base location.</param>
        /// <param name="length">The length of the new location range (must be positive).</param>
        /// <returns>A new location starting at <c>Initial</c> and ending at <c>Initial + length</c> on the same line and source.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is less than 1.</exception>
        public static Location operator |(Location location, int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive.");
            }

            return new Location(location.Source, location.Line, location.Initial, location.Initial + length);
        }

        /// <summary>
        /// Returns a string representation of the location.
        /// </summary>
        public override string ToString()
        {
            return Initial == Final
                ? $"Source {Source}, Line {Line}, Col {Initial}"
                : $"Source {Source}, Line {Line}, Col {Initial}-{Final}";
        }
    }
}

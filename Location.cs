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
        /// Initializes a new instance of the <see cref="Location"/> struct.
        /// </summary>
        public Location(Source source, int line, int position) : this(source, line, position, position + 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> struct.
        /// </summary>
        public Location(Source source, int line, int initial, int final)
        {
            if (final <= initial)
            {
                throw new ArgumentOutOfRangeException(nameof(final), "Final position must be greater than initial position.");
            }

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

            if (left.Initial > right.Final)
            {
                throw new ArgumentException($"Left location must start before right one. Left initial: {left.Initial}, Right final: {right.Final}");
            }

            return new Location(left.Source, left.Line, left.Initial, right.Final);
        }

        /// <summary>
        /// Extends the location to a new final position on the same line and source.
        /// </summary>
        /// <param name="location">The base location.</param>
        /// <param name="final">The final position of the new location range (must be greater than initial).</param>
        /// <returns>A new location starting at <c>Initial</c> and ending at <c>final</c> on the same line and source.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="final"/> is less than or equal to <c>Initial</c>.</exception>
        public static Location operator |(Location location, int final)
        {
            if (final < location.Final)
            {
                throw new ArgumentOutOfRangeException(nameof(final), "Final position cannot be less than current final position.");
            }

            return new Location(location.Source, location.Line, location.Initial, final);
        }

        /// <summary>
        /// Returns a string representation of the location.
        /// </summary>
        public override string ToString()
        {
            var source = Source.Type switch
            {
                SourceType.Inline => "",
                SourceType.File => $"{Source}, ",
                SourceType.None => "",
                _ => throw new NotSupportedException($"Unknown source type: {Source.Type}"),
            };

            return Initial == Final - 1
                ? source + $"Line {Line}, Col {Initial}"
                : source + $"Line {Line}, Col {Initial}-{Final - 1}";
        }
    }
}

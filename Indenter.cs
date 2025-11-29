using System.Collections.Generic;
using BitPatch.DialogLang.Diagnostic;

namespace BitPatch.DialogLang
{
    /// <summary>
    /// Handles indentation tokens based on the current indentation level.
    /// </summary>
    internal class Indenter
    {
        /// <summary>
        /// The underlying reader for source code.
        /// </summary>
        private readonly Reader _reader;

        /// <summary>
        /// The current state of the indenter.
        /// </summary>
        private IndenterState _state = IndenterState.Default;

        /// <summary>
        /// The stack of indentation levels.
        /// </summary>
        private readonly Stack<int> _levels = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Indenter"/> class.
        /// </summary>
        public Indenter(Reader reader)
        {
            _reader = reader;
            _levels.Push(0);
        }

        /// <summary>
        /// Reads the current indentation level and generates corresponding tokens.
        /// </summary>
        public void Read(Queue<Token> output)
        {
            Assert.IsTrue(_reader.IsAtLineStart(), $"Expected start of line, column {_reader.Column}.");

            var startLocation = _reader.GetLocation();
            var last = _levels.Peek();
            int current = _state is IndenterState.Locked ? _reader.ReadIndentLevel(last) : _reader.ReadIndentLevel();

            if (_state is IndenterState.Locked && current != last)
            {
                throw new SyntaxError("Inconsistent indentation", current > 0 ? startLocation | _reader : startLocation);
            }

            if (current > last)
            {
                _levels.Push(current);
                var indentLocation = new Location(_reader.Source, startLocation.Line, last + 1, current + 1);
                output.Enqueue(Token.Indent(indentLocation));

                if (_state is IndenterState.NeedToLock)
                {
                    _state = IndenterState.Locked;
                }

                return;
            }
            else if (_state is IndenterState.NeedToLock)
            {
                throw new SyntaxError("Expecting indentation", _reader.GetLocation());
            }

            var count = 0;

            while (current < last)
            {
                var final = _levels.Pop();
                last = _levels.TryPeek(out var level) ? level : 0;
                output.Enqueue(Token.Dedent(new Location(_reader.Source, startLocation.Line, last + 1, final + 1)));
                count++;
            }

            if (current != last)
            {
                throw new SyntaxError("Inconsistent indentation", last > 0 ? startLocation | last + 1 : startLocation);
            }
        }

        /// <summary>
        /// Locks the indenter to enforce consistent indentation.
        /// </summary>
        public void Lock()
        {
            Assert.IsTrue(_state is IndenterState.Default, $"Expected default state, current: {_state}.");
            _state = IndenterState.NeedToLock;
        }

        /// <summary>
        /// Unlocks the indenter to allow flexible indentation.
        /// </summary>
        public void Unlock()
        {
            Assert.IsTrue(_state is IndenterState.Locked or IndenterState.NeedToLock, $"Expected locking state, current: {_state}.");
            _state = IndenterState.Default;
        }

        /// <summary>
        /// Empties all indentation levels, generating dedent tokens as needed.
        /// </summary>
        public void Empty(Queue<Token> output)
        {
            while (_levels.Count > 1)
            {
                _levels.Pop();
                output.Enqueue(Token.Dedent(_reader.GetLocation()));
            }
        }
    }
}
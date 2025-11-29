using System;
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
            else if (_state is IndenterState.Locking)
            {
                _levels.Push(current);
                _state = IndenterState.Locked;

                return;
            }

            if (current > last)
            {
                _levels.Push(current);
                var indentLocation = new Location(_reader.Source, startLocation.Line, last + 1, current + 1);
                output.Enqueue(Token.Indent(indentLocation));

                return;
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
        /// Starts the locking process to enforce consistent indentation.
        /// </summary>
        public void StartLocking()
        {
            Assert.IsTrue(_state is IndenterState.Default, $"Expected default state, current: {_state}.");
            _state = IndenterState.Locking;
        }

        /// <summary>
        /// Unlocks the indenter to allow flexible indentation.
        /// </summary>
        public void Unlock()
        {
            switch (_state)
            {
                case IndenterState.Default:
                    throw new InvalidOperationException("Cannot unlock when not locked.");
                case IndenterState.Locking:        
                    break;
                case IndenterState.Locked:
                    _levels.Pop();
                    break;
                default:
                    throw new InvalidOperationException($"Unknown indenter state: {_state}.");
            }

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
using System;
using System.Collections.Generic;
using System.IO;

namespace DialogLang
{
    /// <summary>
    /// Executes the Abstract Syntax Tree.
    /// Supports multiple output values using the &lt;&lt; operator and input requests using the &gt;&gt; operator.
    /// Example:
    /// <code>
    /// var interpreter = new Interpreter(logger);
    /// foreach (var result in interpreter.Run("a = 10\n&lt;&lt; a\nb = 20\n&lt;&lt; b\n&lt;&lt; a + b"))
    /// {
    ///     Console.WriteLine(result); // Outputs: 10, then 20, then 30.
    /// }
    /// 
    /// // Example with typed input.
    /// foreach (var result in interpreter.Run("&gt;&gt; x as number\n&lt;&lt; x * 2"))
    /// {
    ///     if (result is RequestNumber numberInput)
    ///     {
    ///         numberInput.Set(21);
    ///     }
    ///     else if (result != null)
    ///     {
    ///         Console.WriteLine(result); // Outputs: 42
    ///     }
    /// }
    /// </code>
    /// </summary>
    public class Interpreter
    {
        private readonly Dictionary<string, object> _variables;
        private readonly ILogger _logger;
        private readonly AstVisitor _visitor;

        public Interpreter(ILogger logger)
        {
            _variables = new Dictionary<string, object>();
            _logger = logger;
            _visitor = new AstVisitor(_variables, _logger);
        }

        /// <summary>
        /// Executes the script from text.
        /// </summary>
        public IEnumerable<object?> Run(string scriptText)
        {
            ProgramNode program;
            try
            {
                var lexer = new Lexer(scriptText);
                var parser = new Parser(lexer);
                program = parser.Parse();
            }
            catch (InterpreterException ex)
            {
                _logger.LogError($"[DialogLang] {ex.Message}");
                yield break;
            }

            foreach (var value in ExecuteProgram(program))
            {
                yield return value;
            }
        }

        /// <summary>
        /// Executes the script from a TextReader (file or stream).
        /// </summary>
        public IEnumerable<object?> Run(TextReader reader)
        {
            ProgramNode program;
            try
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);
                program = parser.Parse();
            }
            catch (InterpreterException ex)
            {
                _logger.LogError($"[DialogLang] {ex.Message}");
                yield break;
            }

            foreach (var value in ExecuteProgram(program))
            {
                yield return value;
            }
        }

        /// <summary>
        /// Executes a program and yields output values.
        /// </summary>
        private IEnumerable<object?> ExecuteProgram(ProgramNode program)
        {
            foreach (var statement in program.Statements)
            {
                var result = _visitor.Visit(statement);
                
                // Check if this is an output statement.
                if (statement is OutputNode)
                {
                    yield return result;
                }
                // Check if this is an input statement.
                else if (statement is InputNode inputNode)
                {
                    Request inputRequest = inputNode.ExpectedType switch
                    {
                        InputType.Any => new RequestAny(inputNode.VariableName, value => _variables[inputNode.VariableName] = value),
                        InputType.Number => new RequestNumber(inputNode.VariableName, value => _variables[inputNode.VariableName] = value),
                        InputType.String => new RequestString(inputNode.VariableName, value => _variables[inputNode.VariableName] = value),
                        InputType.Bool => new RequestBool(inputNode.VariableName, value => _variables[inputNode.VariableName] = value),
                        _ => throw new InterpreterException($"Unknown input type: {inputNode.ExpectedType}", 0, 0)
                    };
                    yield return inputRequest;
                }
            }
        }
    }
}

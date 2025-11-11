using System;
using System.Collections.Generic;

namespace DialogLang
{
    /// <summary>
    /// Handles AST node visiting and evaluation for the interpreter.
    /// </summary>
    internal class AstVisitor
    {
        private readonly Dictionary<string, object> _variables;
        private readonly ILogger _logger;

        public AstVisitor(Dictionary<string, object> variables, ILogger logger)
        {
            _variables = variables;
            _logger = logger;
        }

        /// <summary>
        /// Visits an AST node and executes it.
        /// </summary>
        public object? Visit(AstNode node)
        {
            return node switch
            {
                NumberNode n => VisitNumber(n),
                StringNode s => VisitString(s),
                BooleanNode b => VisitBoolean(b),
                BinaryOpNode b => VisitBinaryOp(b),
                UnaryOpNode u => VisitUnaryOp(u),
                VariableNode v => VisitVariable(v),
                AssignNode a => VisitAssign(a),
                IfNode i => VisitIf(i),
                FunctionCallNode f => VisitFunctionCall(f),
                BlockNode bl => VisitBlock(bl),
                OutputNode r => VisitOutput(r),
                InputNode inp => VisitInput(inp),
                _ => throw new Exception($"Unknown node type: {node.GetType()}")
            };
        }

        /// <summary>
        /// Visits a number node.
        /// </summary>
        private object VisitNumber(NumberNode node)
        {
            return node.Value;
        }

        /// <summary>
        /// Visits a string node.
        /// </summary>
        private string VisitString(StringNode node)
        {
            return node.Value;
        }

        /// <summary>
        /// Visits a boolean node.
        /// </summary>
        private bool VisitBoolean(BooleanNode node)
        {
            return node.Value;
        }

        /// <summary>
        /// Visits a block node.
        /// </summary>
        private object? VisitBlock(BlockNode node)
        {
            object? result = null;
            foreach (var statement in node.Statements)
            {
                result = Visit(statement);
            }
            return result;
        }

        /// <summary>
        /// Visits a binary operation node.
        /// </summary>
        private object VisitBinaryOp(BinaryOpNode node)
        {
            object? left = Visit(node.Left);
            object? right = Visit(node.Right);

            if (left == null || right == null)
            {
                throw new Exception("Binary operation operands cannot be null");
            }

            return node.Operator switch
            {
                TokenType.Plus => ArithmeticOperations.Add(left, right),
                TokenType.Minus => ArithmeticOperations.Subtract(left, right),
                TokenType.Multiply => ArithmeticOperations.Multiply(left, right),
                TokenType.Divide => ArithmeticOperations.Divide(left, right),
                TokenType.Equal => ArithmeticOperations.AreEqual(left, right),
                TokenType.NotEqual => !ArithmeticOperations.AreEqual(left, right),
                TokenType.Greater => ArithmeticOperations.IsGreater(left, right),
                TokenType.GreaterOrEqual => ArithmeticOperations.IsGreaterOrEqual(left, right),
                TokenType.Less => ArithmeticOperations.IsLess(left, right),
                TokenType.LessOrEqual => ArithmeticOperations.IsLessOrEqual(left, right),
                TokenType.And => LogicalOperations.And(left, right),
                TokenType.Or => LogicalOperations.Or(left, right),
                _ => throw new Exception($"Unknown operator: {node.Operator}")
            };
        }

        /// <summary>
        /// Visits a unary operation node.
        /// </summary>
        private object VisitUnaryOp(UnaryOpNode node)
        {
            object? operand = Visit(node.Operand);

            if (operand == null)
            {
                throw new Exception("Unary operation operand cannot be null");
            }

            return node.Operator switch
            {
                TokenType.Not => LogicalOperations.Not(operand),
                _ => throw new Exception($"Unknown unary operator: {node.Operator}")
            };
        }

        /// <summary>
        /// Visits a variable node.
        /// </summary>
        private object VisitVariable(VariableNode node)
        {
            if (_variables.TryGetValue(node.Name, out object value))
            {
                return value;
            }
            throw new Exception($"Undefined variable: {node.Name}");
        }

        /// <summary>
        /// Visits an assignment node.
        /// </summary>
        private object VisitAssign(AssignNode node)
        {
            object? value = Visit(node.Value);
            
            if (value == null)
            {
                throw new Exception("Cannot assign null value");
            }
            
            _variables[node.VariableName] = value;
            return value;
        }

        /// <summary>
        /// Visits an if statement node.
        /// </summary>
        private object? VisitIf(IfNode node)
        {
            object? condition = Visit(node.Condition);
            
            bool isTrue = condition is bool b ? b : Convert.ToBoolean(condition);

            if (isTrue)
            {
                return Visit(node.ThenBody);
            }
            else if (node.ElseBody != null)
            {
                return Visit(node.ElseBody);
            }

            return null;
        }

        /// <summary>
        /// Visits a function call node.
        /// </summary>
        private object? VisitFunctionCall(FunctionCallNode node)
        {
            if (node.FunctionName == "Log")
            {
                if (node.Arguments.Count > 0)
                {
                    object? arg = Visit(node.Arguments[0]);
                    _logger.LogInfo(FormatValue(arg));
                }
                return null;
            }

            throw new Exception($"Unknown function: {node.FunctionName}");
        }

        /// <summary>
        /// Visits an output statement node.
        /// </summary>
        private object? VisitOutput(OutputNode node)
        {
            return Visit(node.Expression);
        }

        /// <summary>
        /// Visits an input statement node.
        /// </summary>
        private object? VisitInput(InputNode node)
        {
            // Input handling is done in the Interpreter.
            // This method is called during traversal but doesn't need to do anything.
            return null;
        }

        /// <summary>
        /// Formats a value for display.
        /// </summary>
        private string FormatValue(object? value)
        {
            return value switch
            {
                null => "null",
                string s => s,
                double d => d.ToString("0.##"),
                float f => f.ToString("0.##"),
                int i => i.ToString(),
                bool b => b ? "true" : "false",
                _ => value.ToString() ?? "null"
            };
        }
    }
}

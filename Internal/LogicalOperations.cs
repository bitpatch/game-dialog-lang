using System;

namespace DialogLang
{
    /// <summary>
    /// Handles logical operations for the interpreter.
    /// </summary>
    internal static class LogicalOperations
    {
        /// <summary>
        /// Performs logical AND operation.
        /// </summary>
        public static bool And(object left, object right)
        {
            bool leftBool = left is bool lb ? lb : Convert.ToBoolean(left);
            bool rightBool = right is bool rb ? rb : Convert.ToBoolean(right);
            return leftBool && rightBool;
        }

        /// <summary>
        /// Performs logical OR operation.
        /// </summary>
        public static bool Or(object left, object right)
        {
            bool leftBool = left is bool lb ? lb : Convert.ToBoolean(left);
            bool rightBool = right is bool rb ? rb : Convert.ToBoolean(right);
            return leftBool || rightBool;
        }

        /// <summary>
        /// Performs logical NOT operation.
        /// </summary>
        public static bool Not(object operand)
        {
            bool operandBool = operand is bool b ? b : Convert.ToBoolean(operand);
            return !operandBool;
        }
    }
}

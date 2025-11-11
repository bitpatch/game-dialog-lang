using System;

namespace DialogLang
{
    /// <summary>
    /// Handles arithmetic and comparison operations for the interpreter.
    /// </summary>
    internal static class ArithmeticOperations
    {
        /// <summary>
        /// Performs addition operation.
        /// Supports int + int = int, but promotes to float if either operand is float.
        /// Also supports string concatenation.
        /// </summary>
        public static object Add(object left, object right)
        {
            if (left is string || right is string)
                return left.ToString() + right.ToString();
            
            if (left is int li && right is int ri)
                return li + ri;
            
            if (left is float || right is float)
                return Convert.ToSingle(left) + Convert.ToSingle(right);
            
            throw new Exception($"Cannot add {left} and {right}");
        }

        /// <summary>
        /// Performs subtraction operation.
        /// Supports int - int = int, but promotes to float if either operand is float.
        /// </summary>
        public static object Subtract(object left, object right)
        {
            if (left is int li && right is int ri)
                return li - ri;
            
            if (left is float || right is float)
                return Convert.ToSingle(left) - Convert.ToSingle(right);
            
            throw new Exception($"Cannot subtract {right} from {left}");
        }

        /// <summary>
        /// Performs multiplication operation.
        /// Supports int * int = int, but promotes to float if either operand is float.
        /// </summary>
        public static object Multiply(object left, object right)
        {
            if (left is int li && right is int ri)
                return li * ri;
            
            if (left is float || right is float)
                return Convert.ToSingle(left) * Convert.ToSingle(right);
            
            throw new Exception($"Cannot multiply {left} and {right}");
        }

        /// <summary>
        /// Performs division operation.
        /// Always returns float to support proper division (e.g., 3 / 2 = 1.5).
        /// </summary>
        public static object Divide(object left, object right)
        {
            // Always convert to float for division to get accurate results
            float leftValue = Convert.ToSingle(left);
            float rightValue = Convert.ToSingle(right);
            
            if (Math.Abs(rightValue) < float.Epsilon)
            {
                throw new DivideByZeroException("Division by zero");
            }
            
            return leftValue / rightValue;
        }

        /// <summary>
        /// Checks if two values are equal.
        /// </summary>
        public static bool AreEqual(object left, object right)
        {
            if (left is int li && right is int ri)
                return li == ri;
            
            if (left is float || right is float)
            {
                float leftFloat = Convert.ToSingle(left);
                float rightFloat = Convert.ToSingle(right);
                return Math.Abs(leftFloat - rightFloat) < 0.0001f;
            }
            
            return left.Equals(right);
        }

        /// <summary>
        /// Checks if left is greater than right.
        /// </summary>
        public static bool IsGreater(object left, object right)
        {
            if (left is int li && right is int ri)
                return li > ri;
            
            if (left is float || right is float)
                return Convert.ToSingle(left) > Convert.ToSingle(right);
            
            throw new Exception($"Cannot compare {left} and {right}");
        }

        /// <summary>
        /// Checks if left is greater than or equal to right.
        /// </summary>
        public static bool IsGreaterOrEqual(object left, object right)
        {
            return IsGreater(left, right) || AreEqual(left, right);
        }

        /// <summary>
        /// Checks if left is less than right.
        /// </summary>
        public static bool IsLess(object left, object right)
        {
            return !IsGreaterOrEqual(left, right);
        }

        /// <summary>
        /// Checks if left is less than or equal to right.
        /// </summary>
        public static bool IsLessOrEqual(object left, object right)
        {
            return !IsGreater(left, right);
        }
    }
}

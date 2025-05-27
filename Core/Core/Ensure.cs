using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NoeticTools.Git2SemVer.Core
{
    [DebuggerStepThrough]
    public sealed class Ensure
    {
        public static void ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentException("String cannot be empty", argumentName);
            }
        }

        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace OmlUtilities.Core
{
    class AssemblyUtilityException : Exception
    {
        public AssemblyUtilityException(string message) : base(message)
        {
            // Use default constructor
        }

        public AssemblyUtilityException(string message, Exception innerException) : base(message, innerException)
        {
            // Use default constructor
        }
    }
}

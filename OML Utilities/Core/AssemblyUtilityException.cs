using System;
using System.Runtime.Serialization;

namespace OmlUtilities.Core
{
    class AssemblyUtilityException : Exception
    {
        /// <summary>
        /// Exception related to the AssemblyUtility class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public AssemblyUtilityException(string message) : base(message)
        {
            // Use default constructor
        }

        /// <summary>
        /// Exception related to the AssemblyUtility class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Optional inner exception.</param>
        public AssemblyUtilityException(string message, Exception innerException) : base(message, innerException)
        {
            // Use default constructor
        }
    }
}

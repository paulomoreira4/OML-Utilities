using System.Xml.Linq;

namespace OmlUtilities.Core
{
    partial class Oml
    {
        public class OmlFragmentWriter
        {
            /// <summary>
            /// Assembly instance.
            /// </summary>
            protected object _instance;
            
            /// <summary>
            /// Writes XML to the fragment.
            /// </summary>
            /// <param name="fragment">XML to be written to the fragment.</param>
            public void Write(XElement fragment)
            {
                AssemblyUtility.ExecuteInstanceMethod<object>(_instance, "Replace", new object[] { fragment });
            }

            /// <summary>
            /// Closes the writer.
            /// </summary>
            public void Close()
            {
                AssemblyUtility.ExecuteInstanceMethod<object>(_instance, "Close");
            }

            /// <summary>
            /// Creates an fragment XML reader instance.
            /// </summary>
            /// <param name="oml">OML instance which owns the fragment.</param>
            /// <param name="fragmentName">Name of the fragment to be written.</param>
            public OmlFragmentWriter(Oml oml, string fragmentName)
            {
                _instance = AssemblyUtility.ExecuteInstanceMethod<object>(oml._instance, "GetFragmentXmlWriter", new object[] { fragmentName });
            }
        }
    }
}

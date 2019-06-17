using System.Xml.Linq;

namespace OmlUtilities.Core
{
    partial class Oml
    {
        public class OmlFragmentWriter
        {
            protected object _instance;

            public void Write(XElement fragment)
            {
                AssemblyUtility.ExecuteInstanceMethod<object>(_instance, "Replace", new object[] { fragment });
            }

            public void Close()
            {
                AssemblyUtility.ExecuteInstanceMethod<object>(_instance, "Close");
            }

            public OmlFragmentWriter(Oml oml, string fragmentName)
            {
                _instance = AssemblyUtility.ExecuteInstanceMethod<object>(oml._instance, "GetFragmentXmlWriter", new object[] { fragmentName });
            }
        }
    }
}

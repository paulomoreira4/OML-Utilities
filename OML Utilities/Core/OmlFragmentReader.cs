using System.Xml.Linq;

namespace OmlUtilities.Core
{
    partial class Oml
    {
        public class OmlFragmentReader
        {
            protected object _instance;

            public XElement GetXElement()
            {
                return AssemblyUtility.ExecuteInstanceMethod<XElement>(_instance, "ToXElement");
            }

            public void Close()
            {
                AssemblyUtility.ExecuteInstanceMethod<object>(_instance, "Close");
            }

            public OmlFragmentReader(Oml oml, string fragmentName)
            {
                _instance = AssemblyUtility.ExecuteInstanceMethod<object>(oml._instance, "GetFragmentXmlReader", new object[] { fragmentName });
            }
        }
    }
}

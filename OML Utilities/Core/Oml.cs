using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OmlUtilities.Core
{
    public partial class Oml
    {
        protected object _instance;

        public PlatformVersion PlatformVersion { get; }
        public OmlHeader Header { get; }

        public List<string> GetFragmentNames()
        {
            return AssemblyUtility.ExecuteInstanceMethod<IEnumerable<string>>(_instance, "DumpFragmentsNames").ToList();
        }

        public XElement GetFragment(string fragmentName)
        {
            OmlFragmentReader reader = new OmlFragmentReader(this, fragmentName);
            XElement element = reader.GetXElement();
            reader.Close();
            return element;
        }

        public void SetFragment(string fragmentName, XElement fragment)
        {
            OmlFragmentWriter writer = new OmlFragmentWriter(this, fragmentName);
            writer.Write(fragment);
            writer.Close();
        }

        public XDocument GetXML()
        {
            XElement root = new XElement("OML");
            
            foreach(string fragmentName in GetFragmentNames())
            {
                XElement fragment = GetFragment(fragmentName);
                fragment.SetAttributeValue("FragmentName", fragmentName);
                root.Add(fragment);
            }

            return new XDocument(root);
        }

        public void Save(Stream outputStream)
        {
            AssemblyUtility.ExecuteInstanceMethod<object>(_instance, "WriteTo", new object[] { outputStream });
        }

        public Oml(Stream omlStream)
        {
            if (AssemblyUtility.PlatformVersion == null)
            {
                throw new Exception("Platform version must be defined before loading the OML content.");
            }

            PlatformVersion = AssemblyUtility.PlatformVersion;
            Type assemblyType = AssemblyUtility.GetAssemblyType("OutSystems.Common", "OutSystems.Oml.Oml");

            try
            {
                _instance = Activator.CreateInstance(assemblyType, new object[] { omlStream, false, null, null });
                Header = new OmlHeader(this);
            }
            catch(Exception e)
            {
                if (e.GetBaseException() is AssemblyUtilityException)
                {
                    throw e.GetBaseException();
                }
                else if (e.InnerException != null && e.InnerException.GetType().Name.Equals("UnsupportedNewerVersion"))
                {
                    throw e.InnerException;
                }
                else
                {
                    throw new Exception("Unable to load OML. Make sure the given OML content is valid.", e);
                }
            }

            // Set OML version to the current version
            Header.Version = PlatformVersion.Version;
        }
    }
}

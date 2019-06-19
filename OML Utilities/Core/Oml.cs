using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OmlUtilities.Core
{
    public partial class Oml
    {
        /// <summary>
        /// Assembly instance.
        /// </summary>
        protected object _instance;

        /// <summary>
        /// Platform version used for loading the OML contents.
        /// </summary>
        public PlatformVersion PlatformVersion { get; }

        /// <summary>
        /// OML header class instance.
        /// </summary>
        public OmlHeader Header { get; }

        /// <summary>
        /// Returns a list of available fragment names.
        /// </summary>
        /// <returns>List of availabel fragment names.</returns>
        public List<string> GetFragmentNames()
        {
            return AssemblyUtility.ExecuteInstanceMethod<IEnumerable<string>>(_instance, "DumpFragmentsNames").ToList();
        }

        /// <summary>
        /// Returns the XML contents of a given fragment.
        /// </summary>
        /// <param name="fragmentName">Name of the desired fragment.</param>
        /// <returns>XML contents of the fragment.</returns>
        public XElement GetFragmentXml(string fragmentName)
        {
            OmlFragmentReader reader = new OmlFragmentReader(this, fragmentName);
            XElement element = reader.GetXElement();
            reader.Close();
            return element;
        }

        /// <summary>
        /// Sets the XML contents of a fragment.
        /// </summary>
        /// <param name="fragmentName">Name of the fragment to be set.</param>
        /// <param name="fragmentXml">New XML contents of the fragment.</param>
        public void SetFragmentXml(string fragmentName, XElement fragmentXml)
        {
            OmlFragmentWriter writer = new OmlFragmentWriter(this, fragmentName);
            writer.Write(fragmentXml);
            writer.Close();
        }

        /// <summary>
        /// Returns a single XML document containing all fragments.
        /// </summary>
        /// <returns>Full XML document.</returns>
        public XDocument GetXml()
        {
            XElement root = new XElement("OML");
            
            foreach(string fragmentName in GetFragmentNames())
            {
                XElement fragment = GetFragmentXml(fragmentName);
                fragment.SetAttributeValue("FragmentName", fragmentName);
                root.Add(fragment);
            }

            return new XDocument(root);
        }

        /// <summary>
        /// Exports the OML contents to an OML stream.
        /// </summary>
        /// <param name="outputStream">Destination stream to write the OML file contents to.</param>
        public void Save(Stream outputStream)
        {
            AssemblyUtility.ExecuteInstanceMethod<object>(_instance, "WriteTo", new object[] { outputStream });
        }

        /// <summary>
        /// Represents an OML file with manipulable OML headers and XML fragments.
        /// </summary>
        /// <param name="omlStream">Input stream containing the OML file contents.</param>
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

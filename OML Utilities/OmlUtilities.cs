using CommandDotNet.Attributes;
using OmlUtilities.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using static OmlUtilities.Core.Oml;
using static OmlUtilities.Core.Oml.OmlHeader;

namespace OmlUtilities
{
    public class OmlUtilities
    {
        protected Stream _GetStream(string path, bool isInput)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("The " + (isInput ? "input" : "output") + " argument is mandatory.");
            }

            if (path.StartsWith("pipe:", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!Console.IsInputRedirected)
                {
                    throw new Exception("Unable to pipe the " + (isInput ? "input" : "output") + " argument because the console input is not redirected.");
                }

                string pipeIndex = path.Substring(5);

                if (string.IsNullOrEmpty(pipeIndex))
                {
                    pipeIndex = isInput ? "0" : "1";
                }

                switch(pipeIndex)
                {
                    case "0": return Console.OpenStandardInput();
                    case "1": return Console.OpenStandardOutput();
                    case "2": return Console.OpenStandardError();
                    default: throw new Exception("Unknown pipe index " + pipeIndex + " used in the " + (isInput ? "input" : "output") + " argument.");
                }
            }
            else
            {
                if (isInput)
                {
                    return File.OpenRead(path);
                }
                else
                {
                    return File.OpenWrite(path);
                }
            }
        }

        protected Oml _GetOmlInstance(string input, string version)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new Exception("The input argument is mandatory.");
            }
            if (string.IsNullOrEmpty(version))
            {
                throw new Exception("The platform version argument is mandatory.");
            }

            if (version.Equals("OL", StringComparison.InvariantCultureIgnoreCase))
            {
                AssemblyUtility.PlatformVersion = PlatformVersion.LatestSupportedVersion;
            }
            else
            {
                PlatformVersion platformVersion = PlatformVersion.Versions.FirstOrDefault(p => p.Label.Equals(version, StringComparison.InvariantCultureIgnoreCase));
                AssemblyUtility.PlatformVersion = platformVersion ?? throw new Exception("Platform version \"" + version + "\" not recognized. Please run ShowPlatformVersions in order to list supported versions.");
            }

            return new Oml(_GetStream(input, true));
        }

        [ApplicationMetadata(Description = "Displays a list of compatible platform versions.",
        ExtendedHelpText = "Displays a list of Service Studio versions that this utility is compatible with for loading and saving OML files.")]
        public void ShowPlatformVersions(
            [Option(Description = "Whether only the latest compatible version should be shown.",
            LongName = "latest",
            ShortName = "l")]
            bool onlyLatest = false,
            [Option(Description = "Whether to show the full formatted version (e.g. '9.1.603.0' instead of 'O9.1').",
            LongName = "fullversion",
            ShortName = "v")]
            bool showFullVersion = false)
        {
            if (onlyLatest)
            {
                Console.WriteLine(showFullVersion ? PlatformVersion.LatestSupportedVersion.Version.ToString() : PlatformVersion.LatestSupportedVersion.ToString());
            }
            else
            {
                foreach (PlatformVersion version in PlatformVersion.Versions)
                {
                    Console.WriteLine(showFullVersion ? version.Version.ToString() : version.ToString());
                }
            }
        }

        [ApplicationMetadata(Description = "Prints header values.",
        ExtendedHelpText = "Displays a list containing key:value pairs of headers of the given OML file. These header values can be changed through the Manipulate command.")]
        public void ShowHeaders(
            [Argument(Description = "Path to the OML file to be loaded. It is possible to read from stdin instead of a file by using UNIX pipe access syntax (e.g.: 'pipe:').")]
            string input,
            [Argument(Description = "Target platform version to use for loading the OML file.")]
            string version,
            [Argument(Description = "If set, returns only the value of the specified header.")]
            string headerName = null)
        {
            Oml oml = _GetOmlInstance(input, version);
            bool found = false;

            foreach (PropertyInfo property in typeof(OmlHeader).GetProperties())
            {
                OmlHeaderAttribute attribute = (OmlHeaderAttribute)Attribute.GetCustomAttribute(property, typeof(OmlHeaderAttribute));

                if (attribute == null || !string.IsNullOrEmpty(headerName) && !headerName.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(headerName))
                {
                    Console.WriteLine(property.GetValue(oml.Header, null));
                }
                else
                {
                    Console.WriteLine("{0}:{1}", property.Name, property.GetValue(oml.Header, null));
                }

                found = true;
            }

            if (!string.IsNullOrEmpty(headerName) && !found)
            {
                throw new Exception("Header name \"" + headerName + "\" was not found.");
            }
        }

        [ApplicationMetadata(Description = "Prints fragment data.",
        ExtendedHelpText = "Lists fragment names or prints its XML content if the fragment name is specified.")]
        public void ShowFragments(
            [Argument(Description = "Path to the OML file to be loaded. It is possible to read from stdin instead of a file by using UNIX pipe access syntax (e.g.: 'pipe:').")]
            string input,
            [Argument(Description = "Target platform version to use for loading the OML file.")]
            string version,
            [Argument(Description = "If set, prints the XML content of the desired fragment.")]
            string fragmentName = null)
        {
            Oml oml = _GetOmlInstance(input, version);

            if (string.IsNullOrEmpty(fragmentName))
            {
                foreach(string innerFragmentName in oml.GetFragmentNames())
                {
                    Console.WriteLine(innerFragmentName);
                }
            }
            else
            {
                XElement fragment = oml.GetFragmentXml(fragmentName);

                if (fragment == null)
                {
                    throw new Exception("Unable to get XML content of fragment \"" + fragmentName + "\".");
                }

                Console.WriteLine(fragment.ToString(SaveOptions.DisableFormatting));
            }
        }

        [ApplicationMetadata(Description = "Manipulates an OML file.",
        ExtendedHelpText = "Opens an OML file in order to manipulate and save it.")]
        public void Manipulate(
            [Argument(Description = "Path to the OML file to be loaded. It is possible to read from stdin instead of a file by using UNIX pipe access syntax (e.g.: 'pipe:').")]
            string input,
            [Argument(Description = "Destination path to save the manipulated OML file. It is possible to send the data stream to stdout instead by using UNIX pipe access syntax (e.g.: 'pipe:').")]
            string output,
            [Argument(Description = "Target platform version to use for loading the OML file.")]
            string version,
            [Option(Description = "Destination file format. Possible formats are 'oml' and 'xml'. If not set, will be guessed according to the output file extension.",
            LongName = "format",
            ShortName = "f")]
            string format = null,
            [Option(Description = "Sets a header value. Name and value must be separated by colon (':').",
            LongName = "header",
            ShortName = "H")]
            List<string> headers = null,
            [Option(Description = "Sets the content of a fragment. Name and value must be separated by colon (':').",
            LongName = "fragment",
            ShortName = "F")]
            List<string> fragments = null)
        {
            Oml oml = _GetOmlInstance(input, version);

            // Set headers
            if (headers != null)
            {
                foreach (string headerLine in headers)
                {
                    int colonIndex = headerLine.IndexOf(':');

                    if (colonIndex == -1)
                    {
                        throw new Exception("Unable to parse header value \"" + headerLine + "\". Name and value must be separated by colon (':').");
                    }

                    string headerName = headerLine.Substring(0, colonIndex);

                    if (string.IsNullOrEmpty(headerName))
                    {
                        throw new Exception("The header name in the header parameter is mandatory.");
                    }

                    bool found = false;

                    foreach (PropertyInfo property in typeof(OmlHeader).GetProperties())
                    {
                        OmlHeaderAttribute attribute = (OmlHeaderAttribute)Attribute.GetCustomAttribute(property, typeof(OmlHeaderAttribute));

                        if (attribute == null || !headerName.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        if (attribute.IsReadOnly)
                        {
                            throw new Exception("Cannot change header \"" + property.Name + "\" because it is read-only.");
                        }

                        string headerValue = headerLine.Substring(colonIndex + 1);
                        property.SetValue(oml.Header, headerValue);
                        found = true;
                    }

                    if (!found)
                    {
                        throw new Exception("Header name \"" + headerName + "\" was not found.");
                    }
                }
            }

            // Set fragments
            if (fragments != null)
            {
                foreach (string fragmentLine in fragments)
                {
                    int colonIndex = fragmentLine.IndexOf(':');

                    if (colonIndex == -1)
                    {
                        throw new Exception("Unable to parse fragment value \"" + fragmentLine + "\". Name and value must be separated by colon (':').");
                    }

                    string fragmentName = fragmentLine.Substring(0, colonIndex);

                    if (string.IsNullOrEmpty(fragmentName))
                    {
                        throw new Exception("The fragment name in the fragment parameter is mandatory.");
                    }

                    XElement fragment = XElement.Parse(fragmentLine.Substring(colonIndex + 1));
                    oml.SetFragmentXml(fragmentName, fragment);
                }
            }

            // Save manipulated OML
            Stream outputStream = _GetStream(output, false);
            if (format != null && format.Equals("xml", StringComparison.InvariantCultureIgnoreCase) || format == null && output.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
            {
                StreamWriter sw = new StreamWriter(outputStream);
                sw.Write(oml.GetXml().ToString(SaveOptions.DisableFormatting)); // Export XML
                sw.Flush();
                sw.Close();
            }
            else
            {
                oml.Save(outputStream); // Export OML
            }
            outputStream.Close();
        }
    }
}

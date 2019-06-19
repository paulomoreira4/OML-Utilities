using System;

namespace OmlUtilities.Core
{
    public sealed class PlatformVersion
    {
        /// <summary>
        /// List of available platform versions.
        /// </summary>
        public static readonly PlatformVersion[] Versions = new PlatformVersion[]
        {
            new PlatformVersion("9.1.603.0", "O9.1"),
            new PlatformVersion("10.0.303.0", "O10"),
            new PlatformVersion("11.0.520.0", "O11")
        };

        /// <summary>
        /// Platform version constant for OutSystems 9.1.
        /// </summary>
        public static readonly PlatformVersion O_9_1 = Versions[0];

        /// <summary>
        /// Platform version constant for OutSystems 10.
        /// </summary>
        public static readonly PlatformVersion O_10_0 = Versions[1];

        /// <summary>
        /// Platform version constant for OutSystems 11.
        /// </summary>
        public static readonly PlatformVersion O_11_0 = Versions[2];

        /// <summary>
        /// Version number.
        /// </summary>
        public readonly Version Version;

        /// <summary>
        /// Label format of the platform version.
        /// </summary>
        public readonly string Label;

        /// <summary>
        /// Platform version supported by OML Utilities.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="label"></param>
        private PlatformVersion(string version, string label)
        {
            Version = Version.Parse(version);
            Label = label;
        }

        /// <summary>
        /// Returns the platform version in label format.
        /// </summary>
        /// <returns>Label of the version.</returns>
        public override string ToString()
        {
            return Label;
        }
        
        /// <summary>
        /// The latest platform version that OML Utilities supports.
        /// </summary>
        public static PlatformVersion LatestSupportedVersion
        {
            get
            {
                return Versions[Versions.Length - 1];
            }
        }
    }
}

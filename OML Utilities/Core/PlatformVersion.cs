using System;

namespace OmlUtilities.Core
{
    public sealed class PlatformVersion
    {
        public static readonly PlatformVersion[] Versions = new PlatformVersion[]
        {
            new PlatformVersion("9.1.603.0", "O9.1"),
            new PlatformVersion("10.0.303.0", "O10"),
            new PlatformVersion("11.0.520.0", "O11")
        };

        public static readonly PlatformVersion O_9_1 = Versions[0];
        public static readonly PlatformVersion O_10_0 = Versions[1];
        public static readonly PlatformVersion O_11_0 = Versions[2];

        public readonly Version Version;
        public readonly string Label;

        private PlatformVersion(string version, string label)
        {
            Version = Version.Parse(version);
            Label = label;
        }

        public override string ToString()
        {
            return Label;
        }
        
        public static PlatformVersion LatestSupportedVersion
        {
            get
            {
                return Versions[Versions.Length - 1];
            }
        }
    }
}

using System;

namespace OmlUtilities.Core
{
    partial class Oml
    {
        public class OmlHeader
        {
            protected object _instance;

            public OmlHeader(Oml oml)
            {
                _instance = AssemblyUtility.GetInstanceField<object>(oml._instance, "Header");
            }

            [OmlHeader]
            public string ActivationCode
            {
                get
                {
                    return AssemblyUtility.GetInstanceField<string>(_instance, "ActivationCode");
                }
                set
                {
                    AssemblyUtility.SetInstanceField(_instance, "ActivationCode", value);
                }
            }

            [OmlHeader]
            public string Name
            {
                get
                {
                    return AssemblyUtility.GetInstanceField<string>(_instance, "Name");
                }
                set
                {
                    AssemblyUtility.SetInstanceField(_instance, "Name", value);
                }
            }

            [OmlHeader]
            public string Description
            {
                get
                {
                    return AssemblyUtility.GetInstanceField<string>(_instance, "Description");
                }
                set
                {
                    AssemblyUtility.SetInstanceField(_instance, "Description", value);
                }
            }

            [OmlHeader]
            public DateTime LastModifiedUTC
            {
                get
                {
                    return AssemblyUtility.GetInstanceField<DateTime>(_instance, "LastModifiedUTC");
                }
                set
                {
                    AssemblyUtility.SetInstanceField(_instance, "LastModifiedUTC", value);
                }
            }

            [OmlHeader]
            public bool NeedsRecover
            {
                get
                {
                    return AssemblyUtility.GetInstanceField<bool>(_instance, "NeedsRecover");
                }
                set
                {
                    AssemblyUtility.SetInstanceField(_instance, "NeedsRecover", value);
                }
            }

            [OmlHeader(IsReadOnly = true)]
            public string Signature
            {
                get
                {
                    return AssemblyUtility.GetInstanceField<string>(_instance, "Signature");
                }
            }

            [OmlHeader]
            public Version Version
            {
                get
                {
                    return AssemblyUtility.GetInstanceField<Version>(_instance, "Version");
                }
                set
                {
                    AssemblyUtility.SetInstanceField(_instance, "Version", value);
                }
            }

            [OmlHeader]
            public Version LastUpgradeVersion
            {
                get
                {
                    return AssemblyUtility.GetInstanceField<Version>(_instance, "LastUpgradeVersion");
                }
                set
                {
                    AssemblyUtility.SetInstanceField(_instance, "LastUpgradeVersion", value);
                }
            }

            public sealed class OmlHeaderAttribute : Attribute
            {
                public bool IsReadOnly;
            }
        }
    }
}

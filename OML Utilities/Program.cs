using OmlUtilities.Core;

namespace OmlUtilities
{
    class Program
    {
        static int Main(string[] args)
        {
            // This is done so the static class initializes before loading
            // CommandDotNet and setting assembly loading mechanism
            AssemblyUtility.PlatformVersion = null;
            return ProgramWorker.Run(args);
        }
    }
}

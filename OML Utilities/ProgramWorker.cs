using CommandDotNet;

namespace OmlUtilities
{
    public static class ProgramWorker
    {
        public static int Run(string[] args)
        {
            AppRunner<OmlUtilities> appRunner = new AppRunner<OmlUtilities>();
            return appRunner.Run(args);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace ILLinkBenchmarks
{
    static class Utilities
    {
        public static readonly string CurrentRID = RidString(
            GetCurrentOSPlatform(),
            RuntimeInformation.ProcessArchitecture,
            RuntimeInformation.RuntimeIdentifier.Contains("musl") ? "musl" : "default");

        private static string RidString(OSPlatform OS, Architecture Arch, string Libc)
        {
            string os =
                OS == OSPlatform.Windows ? "win" :
                OS == OSPlatform.Linux ? "linux" :
                OS == OSPlatform.OSX ? "osx" :
                throw new NotSupportedException("Unsupported OS: " + OS);

            string arch = Arch switch
            {
                Architecture.X64 => "x64",
                _ => throw new NotSupportedException("Unsupported architecture")
            };
            return Libc switch
            {
                "default" => string.Join("-", os, arch),
                "musl" => string.Join('-', os, arch, "musl"),
                _ => throw new NotImplementedException()
            };
        }

        private static OSPlatform GetCurrentOSPlatform()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OSPlatform.OSX :
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? OSPlatform.Windows :
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OSPlatform.Linux :
                throw new NotSupportedException("Current OS is not supported: " + RuntimeInformation.OSDescription);
        }

        /// <summary>
        /// Shells out to run `dotnet publish --self-contained` on the project file passed in, and outputs to a random temp folder. Returns path to the output folder.
        /// </summary>
        public static string PublishSampleProject(string projectFilePath, params string[] extraArgs)
        {
            string outputDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(outputDirectory);
            ProcessStartInfo processStartInfo = new ProcessStartInfo("dotnet", $"publish {projectFilePath} -r {CurrentRID} --self-contained -o {outputDirectory} {extraArgs.Aggregate("", (agg, val) => agg + " " + val)}");
            processStartInfo.RedirectStandardError = false;
            processStartInfo.RedirectStandardOutput = true;
            var p = Process.Start(processStartInfo);
            p.WaitForExit();
            if (p.ExitCode != 0)
                throw new ApplicationException($"Failed to publish application: \n\"{p.StandardOutput.ReadToEnd()}\n\"");
            return outputDirectory;
        }
    }
}

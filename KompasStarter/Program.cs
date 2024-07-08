using System.Diagnostics;
using System.Web;

namespace KompasStarter {

    public class Program {

        static string exePath = @"D:\Games\ASCON\KOMPAS-3D v22 Study\Bin\kStudy.exe";

        public static void Main(string[] args) {
            Uri uri;
            if (args.Length == 0 || (uri = new Uri(args[0])).Scheme != "kgrpc") {
                return;
            }
            var parameters = HttpUtility.ParseQueryString(uri.Query);
            switch (uri.Host) {
                case "start":
                    if (parameters.Count == 0) {
                        break;
                    }
                    Environment.SetEnvironmentVariable(
                        "KOMPAS_WebServer", parameters["server"], EnvironmentVariableTarget.Machine
                    );
                    Process.Start(new ProcessStartInfo {
                        FileName = exePath,
                        UseShellExecute = true,
                        CreateNoWindow = true,
                    });
                    break;
                case "stop":
                    foreach (var process in Process.GetProcessesByName("kStudy")) {
                        process.Kill();
                        process.WaitForExit();
                        process.Dispose();
                    }
                    foreach (var process in Process.GetProcessesByName("WebClient")) {
                        process.Kill();
                        process.WaitForExit();
                        process.Dispose();
                    }
                    break;
            }
        }
    }
}

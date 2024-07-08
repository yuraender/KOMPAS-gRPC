using Grpc.Core;
using gRPCLibrary.Services;
using KOMPAS;
using Kompas6API5;
using KompasAPI7;
using KompasLibrary;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gRPCLibrary {

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Program : IKompasLibrary {

        public static KompasObject KompasAPI {
            get; set;
        }
        public static IApplication Application {
            get; set;
        }
        public static Server GrpcServer {
            get; set;
        }

        private void GrpcRunner() {
            const string host = "127.0.0.1";
            const int port = 5491;
            GrpcServer = new Server {
                Services = { Kompas.BindService(new KompasService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            GrpcServer.Start();
        }

        #region KOMPAS library

        public int Version => 1;

        public string LibraryName => "Служба gRPC";

        public string DisplayLibraryName => LibraryName;

        public string LibraryHelpFile => "";

        public int ProtectNumber => 0;

        public bool IsOnApplication7 => false;

        public int RunLibraryCommand(int Command, int DemoMode) {
            if (KompasAPI == null || Application == null) {
                return 0;
            }
            return 1;
        }

        public bool InitLibrary(object ApplicationInterface) {
            try {
                KompasAPI = (KompasObject) ApplicationInterface;
                Application = (IApplication) KompasAPI.ksGetApplication7();
            } catch (Exception ex) {
                MessageBox.Show(string.Format("При запуске произошла ошибка: {0}", ex));
                return false;
            }
            Task.Run(() => GrpcRunner());
            Process.Start(new ProcessStartInfo {
                FileName = @"C:\Users\user\VisualStudioProjects\KOMPAS-gRPC\WebClient\bin\Release\net8.0\WebClient.exe",
                UseShellExecute = true,
                CreateNoWindow = true,
            });
            return true;
        }

        public bool BeginUnloadLibrary() {
            if (GrpcServer != null) {
                GrpcServer.ShutdownTask.Wait();
                GrpcServer = null;
            }
            if (KompasAPI != null) {
                Marshal.ReleaseComObject(KompasAPI);
                GC.SuppressFinalize(KompasAPI);
                KompasAPI = null;
            }
            if (Application != null) {
                Marshal.ReleaseComObject(Application);
                GC.SuppressFinalize(Application);
                Application = null;
            }
            return true;
        }

        public bool FillLibraryMenu(IKompasLibraryMenu Menu) {
            throw new NotImplementedException();
        }

        public bool GetLibraryCommandState(int Command, out bool Enable, out int Checked) {
            throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.BStr)]
        public string GetDisableReason(int Command) {
            throw new NotImplementedException();
        }

        public bool FillContextPanel(object ContextPanel) {
            throw new NotImplementedException();
        }

        public bool ContextPanelStyleComboChanged(string StyleComboID, int styleType, int newValue) {
            throw new NotImplementedException();
        }

        public dynamic GetKompasConverter() {
            throw new NotImplementedException();
        }

        public bool CreateMacroFromSample(int MacroReference) {
            throw new NotImplementedException();
        }

        public bool get_IsFunctionEnable(ksKompasLibraryFunctionEnum FunctionID) {
            return FunctionID == ksKompasLibraryFunctionEnum.ksLFLibraryName
                || FunctionID == ksKompasLibraryFunctionEnum.ksLFDisplayLibraryName
                || FunctionID == ksKompasLibraryFunctionEnum.ksLFRunLibraryCommand
                || FunctionID == ksKompasLibraryFunctionEnum.ksLFIsOnApplication7
                || FunctionID == ksKompasLibraryFunctionEnum.ksLFInitLibrary
                || FunctionID == ksKompasLibraryFunctionEnum.ksLFBeginUnloadLibrary;
        }

        #endregion

        #region COM Registration

        [ComRegisterFunction]
        public static void Register(Type type) {
            var hkcr = Registry.ClassesRoot;
            try {
                var clsidkey = hkcr.OpenSubKey(@"CLSID\{" + type.GUID.ToString() + "}", true);
                if (clsidkey == null) {
                    return;
                }
                clsidkey.CreateSubKey("Kompas_Library");
                var inprocServer32 = clsidkey.OpenSubKey("InprocServer32", true);
                if (inprocServer32 != null) {
                    inprocServer32.SetValue(null, Environment
                        .GetFolderPath(Environment.SpecialFolder.System)
                        + @"\mscoree.dll");
                    inprocServer32.Close();
                }
                var protocolKey = hkcr.CreateSubKey("kgrpc", true);
                if (protocolKey != null) {
                    protocolKey.SetValue("URL Protocol", "");
                    var commandKey = protocolKey.CreateSubKey(@"Shell\open\command", true);
                    if (commandKey != null) {
                        commandKey.SetValue(null, "\"C:\\Users\\user\\VisualStudioProjects\\KOMPAS-gRPC\\KompasStarter\\bin\\Release\\net8.0\\KompasStarter.exe\" \"%1\"");
                        commandKey.Close();
                    }
                    protocolKey.Close();
                }
                clsidkey.Close();
            } catch (Exception ex) {
                MessageBox.Show(string.Format("При регистрации произошла ошибка: {0}", ex));
            }
        }

        [ComUnregisterFunction]
        public static void Unregister(Type type) {
            var hkcr = Registry.ClassesRoot;
            try {
                var clsidkey = hkcr.OpenSubKey(@"CLSID\{" + type.GUID.ToString() + "}", true);
                if (clsidkey == null) {
                    return;
                }
                clsidkey.DeleteSubKey("Kompas_Library");
                clsidkey.Close();
                hkcr.DeleteSubKeyTree("kgrpc");
            } catch (Exception ex) {
                MessageBox.Show(string.Format("При удалении произошла ошибка: {0}", ex));
            }
        }

        #endregion
    }
}

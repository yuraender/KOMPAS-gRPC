using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using KOMPAS;
using Kompas6API5;
using KompasAPI7;
using KompasLibrary;
using Microsoft.Win32;
using System;
using System.Net.Http;
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

        private async Task GrpcRunner() {
            try {
                using (var channel = GrpcChannel.ForAddress(
                    "http://" + Environment.GetEnvironmentVariable("KOMPAS_gRPC"),
                    new GrpcChannelOptions() {
                        HttpHandler = new GrpcWebHandler(new HttpClientHandler())
                    }
                )) {
                    var client = new Kompas.KompasClient(channel);
                    using (var call = client.SendRequest(new Google.Protobuf.WellKnownTypes.Empty())) {
                        var responseStream = call.ResponseStream;
                        await responseStream.MoveNext();
                        while (await responseStream.MoveNext()) {
                            RunAction(client, responseStream.Current);
                        }
                    }
                }
            } catch (Exception ex) {
                KompasAPI.ksMessage("Ошибка: " + ex.Message);
            }
        }

        private void RunAction(Kompas.KompasClient client, Request request) {
            if (request.Action == null || request.Data == null) {
                return;
            }
            switch (request.Action) {
                case "SayHello":
                    client.SayHello(new Hello { Message = "Привет, " + request.Data });
                    break;
            }
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
            Task.Run(async () => await GrpcRunner());
            return true;
        }

        public bool BeginUnloadLibrary() {
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

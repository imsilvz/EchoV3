using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace EchoV3.Services
{
    public class InjectionService
    {
        [Flags]
        public enum AllocationType : uint
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [Flags]
        public enum MemoryProtection : uint
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        private int _activeProcessId;
        private string _injectionDllPath;
        private Thread? _pollingThread;

        // events
        public event EventHandler<int>? ProcessReady;
        public InjectionService()
        {
            _activeProcessId = -1;
            _injectionDllPath = "Resources/deucalion.dll";
        }

        public void StartScanner()
        {
            _pollingThread = new Thread(PollProcesses);
            _pollingThread.IsBackground = true;
            _pollingThread.Start();
        }

        private void PollProcesses()
        {
            while (true) 
            {
                // check if process is active
                if (_activeProcessId > 0)
                {
                    try
                    {
                        Process.GetProcessById((int)_activeProcessId);
                    }
                    catch (ArgumentException)
                    {
                        _activeProcessId = -1;
                        Debug.WriteLine("[!] Handle to active process lost!");
                    }
                    Thread.Sleep(1000);
                    continue;
                }
                // attempt to find process
                Process[] processes = Process.GetProcessesByName("ffxiv_dx11");
                foreach (Process process in processes)
                {
                    _activeProcessId = process.Id;
                    // check for active pipe!
                    if (DeucalionService.PipeExists(_activeProcessId))
                    {
                        ProcessReady?.Invoke(this, _activeProcessId);
                    }
                    else
                    {
                        if (Inject(_activeProcessId))
                        {
                            ProcessReady?.Invoke(this, _activeProcessId);
                        }
                        else
                        {
                            _activeProcessId = -1;
                        }
                    }
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        private bool Inject(int processId)
        {
            // Make sure file exists
            if (!File.Exists(_injectionDllPath))
            {
                Debug.WriteLine($"[!] File {_injectionDllPath} does not exist. Please check file path.");
                return false;
            }
            var filePath = Path.GetFullPath(_injectionDllPath);

            // Get handle to Kernel32.dll and get address for LoadLibraryA
            IntPtr Kernel32Handle = GetModuleHandle("Kernel32.dll");
            IntPtr LoadLibraryAAddress = GetProcAddress(Kernel32Handle, "LoadLibraryA");
            if (LoadLibraryAAddress == IntPtr.Zero)
            {
                Debug.WriteLine("[!] Obtaining an addess to LoadLibraryA function has failed.");
                return false;
            }
            Debug.WriteLine("[+] LoadLibraryA function address (0x" + LoadLibraryAAddress + ") has been obtained.");

            // Open handle to the target process
            Debug.WriteLine(processId);
            IntPtr ProcHandle = OpenProcess(
                ProcessAccessFlags.CreateThread | 
                ProcessAccessFlags.QueryInformation | 
                ProcessAccessFlags.VirtualMemoryOperation | 
                ProcessAccessFlags.VirtualMemoryRead | 
                ProcessAccessFlags.VirtualMemoryWrite,
                false,
                processId);
            if (ProcHandle == IntPtr.Zero)
            {
                Debug.WriteLine(Marshal.GetLastPInvokeError());
                Debug.WriteLine("[!] Handle to target process could not be obtained!");
                return false;
            }
            Debug.WriteLine("[+] Handle (0x" + ProcHandle + ") to target process has been be obtained.");

            // Allocate DLL space
            IntPtr DllSpace = VirtualAllocEx(
                ProcHandle,
                IntPtr.Zero,
                (uint)((filePath.Length + 1) * Marshal.SizeOf(typeof(char))),
                AllocationType.Reserve | AllocationType.Commit,
                MemoryProtection.ExecuteReadWrite);
            if (DllSpace == IntPtr.Zero)
            {
                Debug.WriteLine("[!] DLL space allocation failed.");
                return false;
            }
            Debug.WriteLine("[+] DLL space (0x" + DllSpace + ") allocation is successful.");

            // Write DLL content to VAS of target process
            byte[] bytes = Encoding.Default.GetBytes(filePath);
            bool DllWrite = WriteProcessMemory(
                ProcHandle,
                DllSpace,
                bytes,
                (uint)((filePath.Length + 1) * Marshal.SizeOf(typeof(char))),
                out var bytesWritten
                );
            if (!DllWrite)
            {
                Debug.WriteLine("[!] Writing DLL content to target process failed.");
                return false;
            }
            Debug.WriteLine("[+] Writing DLL content to target process is successful.");

            // Create remote thread in the target process
            IntPtr RemoteThreadHandle = CreateRemoteThread(
                ProcHandle,
                IntPtr.Zero,
                0,
                LoadLibraryAAddress,
                DllSpace,
                0,
                IntPtr.Zero
                );
            if (RemoteThreadHandle == IntPtr.Zero)
            {
                MessageBox.Show("Obtaining a handle to remote thread in target process failed.", "An error occurred while starting Echo", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
            Debug.WriteLine("[+] Obtaining a handle to remote thread (0x" + RemoteThreadHandle + ") in target process is successful.");
            return true;
        }
    }
}

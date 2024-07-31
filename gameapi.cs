using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shell;
using game;

namespace game
{
    class gameapi
    {
        // Imports
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int flAllocationType, int flProtect);

        [DllImport("kernel32.dll")]
        static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint Allicationprotect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [Flags]
        public enum MemoryProtection
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

        // Process permission variables
        const int PROCESS_ALL = 0x001F0FF;
        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_WM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        // Allocation Types
        const int MEM_COMMIT = 0x00001000;
        const int MEM_RESERVE = 0x00002000;



        // Handle & Process
        private Process[] procs;
        private string processName;
        private IntPtr handle = IntPtr.Zero;
        private IntPtr base_address = IntPtr.Zero;
        private int error_iterator = 0;
        public Process proc;

        public gameapi(string procName)
        {
            try
            {
                processName = procName;
                procs = Process.GetProcessesByName(processName);
                if (procs.Length == 0)
                {
                    throw new IndexOutOfRangeException();
                }
                proc = procs[0];
                handle = OpenProcess(PROCESS_ALL, false, proc.Id);
                base_address = proc.Modules[0].BaseAddress;
            }
            catch (IndexOutOfRangeException)
            {
                error_iterator++;
            }
        }

        public bool injectorActive()
        {
            Process[] new_procs = Process.GetProcessesByName(processName);
            if (new_procs.Length == 0 || procs.Length == 0) { return false; }
            if (new_procs[0].Id != procs[0].Id) { return false; }
            return true;
        }

        public bool reInject()
        {
            try
            {
                procs = Process.GetProcessesByName(processName);
                if (procs.Length == 0)
                {
                    throw new IndexOutOfRangeException();
                }
                proc = procs[0];
                handle = OpenProcess(PROCESS_ALL, false, proc.Id);
                base_address = proc.Modules[0].BaseAddress;
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                error_iterator++;
                return false;
            }
        }

        // Read/Write
        public byte[] ReadBytes(int address, int byteCount)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[byteCount];

            ReadProcessMemory(handle, base_address + address, buffer, buffer.Length, ref bytesRead);
            return buffer;
        }

        public byte[] ReadBytesCodeCave(int address, int byteCount)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[byteCount];

            ReadProcessMemory(proc.Handle, address, buffer, buffer.Length, ref bytesRead);
            return buffer;
        }

        public void AddToByte(int address, int value)
        {
            int intToWrite = Convert.ToInt16(ReadByte(address)) + value;
            WriteByte(address, (byte)intToWrite);


        }

        public void WriteBytes(int address, byte[] data)
        {
            int bytesWrite = 0;
            WriteProcessMemory(handle, base_address + address, data, data.Length, ref bytesWrite);
        }

        public int WriteBytesCodeCave(nint address, byte[] data)
        {
            int bytesWrite = 0;
            WriteProcessMemory(proc.Handle, address, data, data.Length, ref bytesWrite);
            return bytesWrite;
        }

        public void nullBytes(int address, int i)
        {
            byte[] data = new byte[i];
            for (int x = 0; x < i; x++)
            {
                data[x] = 0x90;
            }
            int bytesWrite = 0;
            WriteProcessMemory(handle, base_address + address, data, data.Length, ref bytesWrite);
        }

        public byte[] nullByteArray(int i)
        {
            byte[] data = new byte[i];
            for (int x = 0; x < i; x++)
            {
                data[x] = 0x90;
            }
            return data;
        }

        public byte ReadByte(int address)
        {
            return ReadBytes(address, 1)[0];
        }

        public void WriteByte(int address, byte data)
        {
            WriteBytes(address, new byte[] { data });
        }

        public bool ReadBool(int address)
        {
            return BitConverter.ToBoolean(ReadBytes(address, 1), 0);
        }

        public void WriteBool(int address, bool data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        public ushort ReadUInt16(int address)
        {
            return BitConverter.ToUInt16(ReadBytes(address, 2), 0);
        }

        public void WriteUInt16(int address, ushort data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        public short ReadInt16(int address)
        {
            return BitConverter.ToInt16(ReadBytes(address, 2), 0);
        }

        public void WriteInt16(int address, short data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        public uint ReadUInt32(int address)
        {
            return BitConverter.ToUInt32(ReadBytes(address, 4), 0);
        }

        public void WriteUInt32(int address, uint data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        public int ReadInt32(int address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4), 0);
        }

        public void WriteInt32(int address, int data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        public ulong ReadUInt64(int address)
        {
            return BitConverter.ToUInt64(ReadBytes(address, 8), 0);
        }

        public void WriteUInt64(int address, ulong data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        public long ReadInt64(int address)
        {
            return BitConverter.ToInt64(ReadBytes(address, 8), 0);
        }

        public void WriteInt64(int address, long data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        public float ReadSingle(int address)
        {
            return BitConverter.ToSingle(ReadBytes(address, 4), 0);
        }

        public void WriteSingle(int address, float data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        public double ReadDouble(int address)
        {
            return BitConverter.ToDouble(ReadBytes(address, 8), 0);
        }

        public void WriteDouble(int address, double data)
        {
            WriteBytes(address, BitConverter.GetBytes(data));
        }

        // Byte Comparison
        public bool CompareBytes(byte[] firstArray, byte[] secondArray)
        {
            return firstArray.SequenceEqual(secondArray);
        }

        // Convert String to Bytes
        public byte[] ConvertStringToBytes(string byteString)
        {
            string[] el = byteString.Split(' ');
            byte[] convBytes = new byte[el.Length];
            
            for(int i = 0; i < el.Length; i++) 
            {
                if (el[i].Contains("?"))
                {
                    convBytes[i] = 0x0;
                } else
                {
                    convBytes[i] = Convert.ToByte(el[i], 16);
                }
            }
            return convBytes;
        }

        public byte[] ConvertIntPtrToArray(IntPtr input)
        {
            return BitConverter.GetBytes((int)input);
        }

        // AOB
        public List<IntPtr> ScanMemory(string byteString)
        {
            List<IntPtr> results = new List<IntPtr>();
            IntPtr currentAddr = IntPtr.Zero;
            byte[] bytes = ConvertStringToBytes(byteString);
            int bytesRead = 0;

            while (VirtualQueryEx(proc.Handle, currentAddr, out MEMORY_BASIC_INFORMATION mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))))
            {
                if (mbi.State == MEM_COMMIT && (mbi.Protect == (uint)MemoryProtection.ReadWrite || mbi.Protect == (uint)MemoryProtection.ReadOnly))
                {
                    byte[] buffer = new byte[(int)mbi.RegionSize];
                    if (ReadProcessMemory(proc.Handle, mbi.BaseAddress, buffer, buffer.Length, ref bytesRead))
                    {
                        for (int i = 0; i < bytesRead - bytes.Length; i++)
                        {
                            bool match = true;
                            for(int j = 0; j < bytes.Length; j++) {
                                if (bytes[j] != 0 && buffer[i + j] != bytes[j])
                                {
                                    match = false;
                                    break;
                                }
                            }
                            if (match)
                            {
                                results.Add(mbi.BaseAddress + i);
                            }
                        }
                    }
                }
                currentAddr = new nint(currentAddr.ToInt64() + mbi.RegionSize.ToInt64());
            }
            return results;
        }

        // Code Cave
        public byte[] CalculateJump(long jmpAddr, long targetAddr, int nopLength, bool jmpBack)
        {
            // JMP Byte
            byte[] jmpBytes = new byte[] { 0xE9 };

            long jmpOp;
            if(jmpBack)
            {
                jmpOp = jmpAddr + nopLength - 5 - targetAddr + base_address;
            } else
            {
                jmpOp = jmpAddr - 5 - targetAddr - base_address;
            }
            
            byte[] byteAssign = BitConverter.GetBytes(jmpOp).ToArray();
            byte[] bytes = jmpBytes.Concat(new byte[] { byteAssign[0], byteAssign[1], byteAssign[2], byteAssign[3] }).ToArray();
            
            while(bytes.Length < nopLength && !jmpBack)
            {
                bytes = bytes.Concat(new byte[] { 0x90 }).ToArray();
            }

            if(bytes.Length < nopLength && !jmpBack)
            {
                throw new Exception("Nop length is too short");
            }

            return bytes;
        }

        public void CreateJump(IntPtr jmpAddr, IntPtr targetAddr, int nopLength, bool jmpBack)
        {
            var bytes =  CalculateJump(jmpAddr.ToInt64(), targetAddr.ToInt64(), nopLength, jmpBack);
            if(jmpBack)
            {
                int bytesWritten = 0;
                WriteProcessMemory(proc.Handle, targetAddr.ToInt32(), bytes, bytes.Length, ref bytesWritten);
            } else
            {
                WriteBytes(targetAddr.ToInt32(), bytes);
            }
        }

        public codecave createCodeCave(IntPtr targetAddr, int nopLength, codecave codeCave = null, byte[] patchSeed = null)
        {
            if(proc != null && proc.Handle != IntPtr.Zero && codeCave != null)
            {
                // Create CodeCave
                IntPtr codeCaveBase = VirtualAllocEx(proc.Handle, IntPtr.Zero, 0x1000, MEM_COMMIT, (int)MemoryProtection.ExecuteReadWrite);

                // Inject Patch
                if(patchSeed != null)
                {
                    int bytesWrite = WriteBytesCodeCave(codeCaveBase, patchSeed);

                    // Create JMP back
                    CreateJump(targetAddr, codeCaveBase + bytesWrite, nopLength, true);

                    // Store info in object for return
                    codeCave.Success = bytesWrite > 0 ? true : false;
                } else
                {
                    codeCave.Success = true;
                }


                // Read Original Code
                byte[] originalCode = ReadBytes(targetAddr.ToInt32(), nopLength);

                // Create JMP to CodeCave
                CreateJump(codeCaveBase, targetAddr, nopLength, false);

                // Store info in object for return
                codeCave.TargetAddr = targetAddr;
                codeCave.AllocBaseAddr = codeCaveBase;
                codeCave.OriginalCode = originalCode;
                codeCave.InjectedCode = patchSeed;
                return codeCave;

            } else if (proc != null && proc.Handle != IntPtr.Zero && codeCave != null && codeCave.doesExist() == true)
            {
                // Reinject Patch
                int bytesWritten = 0;
                WriteProcessMemory(proc.Handle, codeCave.AllocBaseAddr, patchSeed, patchSeed.Length, ref bytesWritten);
                // Recreate Jump Back incase bytesize has changed
                CreateJump(targetAddr, codeCave.AllocBaseAddr + bytesWritten, nopLength, true);
                return codeCave;
            } else
            {
                reInject();
                return (codeCave != null) ? codeCave : new codecave();
            }
        }
    }
}
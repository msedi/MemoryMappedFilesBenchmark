using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp17
{
    class Program
    {
        static int N = 10;
        static long B = 100;
        static long S = 4000 * 256;
        static long T = B * S * 4;

        static void Main(string[] args)
        {
            {
                Stopwatch w = Stopwatch.StartNew();
                TestMemoryMappedFile();
                w.Stop();
                Console.WriteLine($"MemoryMappedFile: {w.ElapsedMilliseconds}ms");
            }

            {
                Stopwatch w = Stopwatch.StartNew();
                TestStream();
                w.Stop();
                Console.WriteLine($"FileStream: {w.ElapsedMilliseconds}ms");
            }


            Console.WriteLine("Ready");
            Console.ReadKey();
        }

        static void TestMemoryMappedFile()
        {
            int[] Data = new int[S];
            for (int i = 0; i < Data.Length; i++)
                Data[i] = i;

            for (int i = 0; i < N; i++)
            {
                using (var aFileStream = new FileStream($"D:\\file{i}.map", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (MemoryMappedFile aFile = MemoryMappedFile.CreateFromFile(aFileStream, i.ToString(), 0, MemoryMappedFileAccess.ReadWrite, HandleInheritability.Inheritable, false))
                    {
                        using (var aAccessor = aFile.CreateViewAccessor())
                        {
                            for (long j = 0; j < T; j += S * 4)
                            {
                                int[] ReadData = new int[S * 4];
                                aAccessor.ReadArray<int>(j, ReadData, 0, ReadData.Length);
                            }
                        }
                    } }
            }


        }

        static unsafe void TestStream()
        {
            int[] Data = new int[S];
            for (int i = 0; i < Data.Length; i++)
                Data[i] = i;

            byte[] Data2 = new byte[Data.Length * 4];


            fixed (int* src = Data) fixed (byte* tgt = Data2)
            {
                byte* srcptr = (byte*)src;
                byte* tgtptr = (byte*)tgt;

                for (int i = 0; i < Data2.Length; i++, srcptr++, tgtptr++)
                    *tgtptr = *srcptr;
            }

            for (int i = 0; i < N; i++)
            {
                using (FileStream aFile = new FileStream($"D:\\file{i}.dat", System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    for (long j = 0; j < B; j++)
                    {
                        byte[] ReadData = new byte[S * 4];
                        aFile.Read(ReadData, 0, ReadData.Length);
                    }
                }
            }
        }
    }
}

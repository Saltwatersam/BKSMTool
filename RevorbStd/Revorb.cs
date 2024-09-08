// This file is part of RevorbStd.
// Copyright (c) 2018 Naomi
// 
// Licensed under the MIT License. You may obtain a copy of the License at
// https://opensource.org/licenses/MIT
// 
// Modifications by Saltwatersam for integration in BKSMTool.

using System.Runtime.InteropServices;
using static RevorbStd.Native;
using System;
using System.IO;

namespace RevorbStd
{
    public class Revorb
    {
        public static unsafe RevorbStream Jiggle(Stream fi)
        {
            byte[] raw = new byte[fi.Length];
            long pos = fi.Position;
            fi.Position = 0;
            fi.Read(raw, 0, raw.Length);
            fi.Position = pos;
            IntPtr ptr = Marshal.AllocHGlobal(4096);

            REVORB_FILE output = new REVORB_FILE
            {
                start = ptr,
                size = 4096
            };

            try
            {
                fixed (byte* rawPtr = raw)
                {
                    REVORB_FILE input = new REVORB_FILE
                    {
                        start = (IntPtr)rawPtr,
                        size = raw.Length
                    };

                    input.cursor = input.start;

                    output.cursor = output.start;

                    int result = revorb(ref input, ref output);

                    if (result != REVORB_ERR_SUCCESS)
                    {
                        throw new Exception($"Expected success, got {result} -- refer to RevorbStd.Native");
                    }

                    return new RevorbStream(output);
                }
            }
            catch
            {
                if (output.start != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(output.start);
                    output.start = IntPtr.Zero;
                }

                throw;
            }
        }

        public unsafe class RevorbStream : UnmanagedMemoryStream
        {
            private REVORB_FILE revorbFile;

            public RevorbStream(REVORB_FILE revorbFile) : base((byte*)revorbFile.start.ToPointer(), revorbFile.size)
            {
                this.revorbFile = revorbFile;
            }

            public override void Close()
            {
                base.Close();
                Marshal.FreeHGlobal(revorbFile.start);
            }
        }
    }
}
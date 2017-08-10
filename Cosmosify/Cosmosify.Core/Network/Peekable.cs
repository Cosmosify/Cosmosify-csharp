using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Cosmosify.Core.Base;

namespace Cosmosify.Core.Network
{
    internal interface IPeekable
    {
        int OriginalRead(byte[] buffer, int offset, int count);

        int OriginalReadByte();

        void Peek(byte[] buffer, int offset, int count);
    }


    internal class Peekable
    {
        private byte[] _buffer = new byte[0];
        private int _bufferPos = 0;

        private readonly IPeekable _peekable;

        public Peekable(IPeekable peekable)
        {
            this._peekable = peekable;
        }


        public unsafe int Read(byte[] buffer, int offset, int count)
        {
            //
            // Parameter check
            //
            (buffer, offset, count).CheckValidation();


            int bufCount = this._buffer.Length - this._bufferPos;

            if (bufCount == 0)
            {
                return this._peekable.OriginalRead(buffer, offset, count);
            }


            int copyCount = (bufCount <= count) ? bufCount : count;

            if (copyCount > 0)
            {
                fixed (byte* pStart = &this._buffer[this._bufferPos])
                {
                    Marshal.Copy((IntPtr)pStart, buffer, offset, copyCount);
                }
                this._bufferPos += copyCount;
            }

            return copyCount;
        }



        public int ReadByte()
        {
            if (this._bufferPos < this._buffer.Length)
            {
                return (int)this._buffer[this._bufferPos++];
            }
            else
            {
                return this._peekable.OriginalReadByte();
            }
        }


        public unsafe void Peek(byte[] buffer, int offset, int count)
        {
            //
            // Parameter check
            //
            (buffer, offset, count).CheckValidation();

            // TODO: Recover to a consistent state if any exception is thrown in Read()

            int bufCount = this._buffer.Length - this._bufferPos;

            if (count > bufCount)
            {
                byte[] newBuffer = new byte[count];

                if (this._bufferPos < this._buffer.Length)
                {
                    fixed (byte* pStart = &this._buffer[this._bufferPos])
                    {
                        Marshal.Copy((IntPtr)pStart, newBuffer, 0, bufCount);
                    }
                }

                while (bufCount < count)
                {
                    // TODO: Handle the occational situation where Read() reaches the end & returns 0
                    int tmp = this._peekable.OriginalRead(newBuffer, bufCount, count - bufCount);
                    bufCount += tmp;
                }
                Debug.Assert(bufCount == count);

                this._buffer = newBuffer;
                this._bufferPos = 0;
            }


            Debug.Assert(count <= bufCount);
            fixed (byte* pStart = &this._buffer[this._bufferPos])
            {
                Marshal.Copy((IntPtr)pStart, buffer, offset, count);
            }
        }
    }
}

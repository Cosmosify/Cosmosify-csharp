using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Cosmosify.Core.Base
{
    public class PeekableSslStream : SslStream
    {
        private byte[] _buffer = new byte[0];
        private int _bufferPos = 0;


        public override unsafe int Read(byte[] buffer, int offset, int count)
        {
            // TODO: Parameter check

            int bufCount = this._buffer.Length - this._bufferPos;

            if (bufCount == 0)
            {
                return base.Read(buffer, offset, count);
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


        public override int ReadByte()
        {
            if (this._bufferPos < this._buffer.Length)
            {
                return (int)this._buffer[this._bufferPos++];
            }
            else
            {
                return base.ReadByte();
            }
        }


        public unsafe void Peek(byte[] buffer, int offset, int count)
        {
            // TODO: Parameter check

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
                    int tmp = base.Read(newBuffer, bufCount, count - bufCount);
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


        public PeekableSslStream(Stream innerStream) 
            : base(innerStream)
        {
        }

        public PeekableSslStream(Stream innerStream, bool leaveInnerStreamOpen)
            : base(innerStream, leaveInnerStreamOpen)
        {
        }

        public PeekableSslStream(
            Stream innerStream,
            bool leaveInnerStreamOpen,
            RemoteCertificateValidationCallback userCertificateValidationCallback)
            : base(innerStream,
                leaveInnerStreamOpen,
                userCertificateValidationCallback)
        {
        }

        public PeekableSslStream(
            Stream innerStream,
            bool leaveInnerStreamOpen,
            RemoteCertificateValidationCallback userCertificateValidationCallback,
            LocalCertificateSelectionCallback userCertificateSelectionCallback)
            : base(innerStream,
                leaveInnerStreamOpen,
                userCertificateValidationCallback,
                userCertificateSelectionCallback)
        {
        }

        public PeekableSslStream(
            Stream innerStream,
            bool leaveInnerStreamOpen,
            RemoteCertificateValidationCallback userCertificateValidationCallback,
            LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy)
            : base(innerStream,
                leaveInnerStreamOpen,
                userCertificateValidationCallback,
                userCertificateSelectionCallback,
                encryptionPolicy)
        {
        }
    }
}

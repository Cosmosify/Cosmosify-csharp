using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Cosmosify.Core.Base;

namespace Cosmosify.Core.Network
{
    public class PeekableNetworkStream : NetworkStream, IPeekable
    {
        private readonly Peekable _peekable;


        public override int Read(byte[] buffer, int offset, int count)
        {
            return this._peekable.Read(buffer, offset, count);
        }

        int IPeekable.OriginalRead(byte[] buffer, int offset, int count)
        {
            return base.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return this._peekable.ReadByte();
        }

        int IPeekable.OriginalReadByte()
        {
            return base.ReadByte();
        }

        public void Peek(byte[] buffer, int offset, int count)
        {
            this._peekable.Peek(buffer, offset, count);
        }



        public PeekableNetworkStream(Socket socket, FileAccess access, bool ownsSocket) 
            : base(socket, access, ownsSocket)
        {
            this._peekable = new Peekable(this);
        }
    }
}

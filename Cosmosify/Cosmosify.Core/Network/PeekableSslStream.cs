using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Runtime.InteropServices;
using Cosmosify.Core.Base;

namespace Cosmosify.Core.Network
{
    public class PeekableSslStream : SslStream, IPeekable
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







        public PeekableSslStream(Stream innerStream) 
            : base(innerStream)
        {
            this._peekable = new Peekable(this);
        }

        public PeekableSslStream(Stream innerStream, bool leaveInnerStreamOpen)
            : base(innerStream, leaveInnerStreamOpen)
        {
            this._peekable = new Peekable(this);
        }

        public PeekableSslStream(
            Stream innerStream,
            bool leaveInnerStreamOpen,
            RemoteCertificateValidationCallback userCertificateValidationCallback)
            : base(innerStream,
                leaveInnerStreamOpen,
                userCertificateValidationCallback)
        {
            this._peekable = new Peekable(this);
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
            this._peekable = new Peekable(this);
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
            this._peekable = new Peekable(this);
        }

    }
}

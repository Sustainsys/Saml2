using Microsoft.IdentityModel.Tokens;
using System;

namespace Sustainsys.Saml2.Metadata.Tokens
{
    public abstract class BinaryKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        private byte[] identificationData;
        private bool cloneBuffer;

        protected BinaryKeyIdentifierClause(
            string clauseType,
            byte[] identificationData,
            bool cloneBuffer,
            byte[] derivationNonce,
            int derivationLength) :
            base(clauseType, derivationNonce, derivationLength)
        {
            if (identificationData == null)
            {
                throw new ArgumentNullException(nameof(identificationData));
            }
            this.cloneBuffer = cloneBuffer;
            this.identificationData = cloneBuffer ? identificationData.CloneByteArray() : identificationData;
        }

        protected BinaryKeyIdentifierClause(string clauseType, byte[] identificationData, bool cloneBuffer) :
            this(clauseType, identificationData, cloneBuffer, null, 0)
        {
        }

        public byte[] GetBuffer()
        {
            return identificationData.CloneByteArray();
        }

        public byte[] GetRawBuffer()
        {
            return cloneBuffer ? identificationData.CloneByteArray() : identificationData;
        }

        public bool Matches(byte[] data, int offset)
        {
            if (data.Length - offset != identificationData.Length)
            {
                return false;
            }
            for (int i = 0; i < identificationData.Length; ++i)
            {
                if (data[i + offset] != identificationData[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool Matches(byte[] data)
        {
            return Matches(data, 0);
        }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            return keyIdentifierClause is BinaryKeyIdentifierClause otherClause &&
                Matches(otherClause.identificationData);
        }
    }
}
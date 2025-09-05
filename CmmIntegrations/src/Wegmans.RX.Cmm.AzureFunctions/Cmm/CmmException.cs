using System;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm
{

    [Serializable]
    public class CmmException : Exception
    {
        public CmmException() { }
        public CmmException(string message) : base(message) { }
        public CmmException(string message, Exception inner) : base(message, inner) { }
        protected CmmException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

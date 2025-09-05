using System;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute
{
    [Serializable]
    public class AstuteException : Exception
    {
        public AstuteException() { }
        public AstuteException(string message) : base(message) { }
        public AstuteException(string message, Exception inner) : base(message, inner) { }
        protected AstuteException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

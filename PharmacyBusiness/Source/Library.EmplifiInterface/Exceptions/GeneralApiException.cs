using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.EmplifiInterface.Exceptions
{
    public class GeneralApiException : Exception
    {
        public GeneralApiException(string? message) : base(message)
        {
        }
    }
}

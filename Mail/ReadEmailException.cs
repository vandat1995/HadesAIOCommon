using System;
using System.Collections.Generic;
using System.Text;

namespace HadesAIOCommon.Mail
{
    public class ReadEmailException : Exception
    {
        public ReadEmailException()
        {

        }
        public ReadEmailException(string msg) : base(msg)
        {

        }
    }
}

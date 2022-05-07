using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tops.FRM
{
    class BizCheckException:Exception
    {
        public string CheckField{get;set;}
        public BizCheckException(string message, string checkField)
            : base(message)
        {
            CheckField = checkField;
        }
    }
}

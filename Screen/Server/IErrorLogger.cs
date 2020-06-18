using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    interface IErrorLogger //记录错误
    {
        void HandleException(Exception ex);
    }
}

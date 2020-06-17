using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    interface IErrorLogger
    {
        void HandleException(Exception ex);
    }
}

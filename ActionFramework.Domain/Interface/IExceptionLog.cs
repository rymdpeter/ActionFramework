﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Domain.Interface
{
    public interface IExceptionLog
    {
        string StackTrace { get; set; }
        string ExceptionMessage { get; set; }
        string Source { get; set; }
    }
}

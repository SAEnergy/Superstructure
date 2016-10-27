﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Components.Logging
{
    public interface ILogMessageFilter : IFilter
    {
        void FilterAdd(IFilter filter);
        void FilterClear();
    }
}

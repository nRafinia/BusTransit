using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Common.Base
{
    public class BaseFilter<T> 
    {
        public Expression<Func<T, bool>> Filter { get; set; }
        public Expression<Func<T, object>> Order  { get; set; }

        public int PageIndex { get; set; } = 0;

        public int PageSize { get; set; } = 10;
    } 
}

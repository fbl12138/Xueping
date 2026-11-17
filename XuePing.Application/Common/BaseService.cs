using System;
using System.Collections.Generic;
using System.Text;

namespace XuePing.Application.Common
{
    public abstract class BaseService
    {
        protected readonly IXuePingDbContext DbContext;
        protected BaseService(IXuePingDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Services
{
    public interface ICacheService
    {
        object GetCachedObject(object key);

        void RemoveCachedObject(object key);

        void AddCachedObject(object key, object value);
    }
}

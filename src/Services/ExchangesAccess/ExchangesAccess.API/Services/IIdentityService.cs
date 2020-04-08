using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();

        string GetUserName();
    }
}

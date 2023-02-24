using BookStore.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Interfaces
{
    public interface ITokenManager
    {
        Task<string> GenerateAccessTokenAsync(long accountId, ClientRequestInfo clientInfo);
        Task<long> GetAccountIdByAccessTokenAsync(HttpRequest request);
        Task<long> GetAccountIdByAccessTokenAsync(string accessToken);
        string GenerateRefreshToken();
        string GetHash(string input);
        Task<string> GenerateKeyTokenValidateAsync(long accountId, string email);
        long ReadKeyTokenValidate(string accountInfoTxtRaw, ref string email);
    }
}

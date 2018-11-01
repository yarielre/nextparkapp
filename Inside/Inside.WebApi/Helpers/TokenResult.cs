using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Inside.WebApi.Helpers
{
    public class TokenResult
    {
        public string AuthToken { get; set; }
        public int  UserId { get; set; }

        public TokenResult(string token,int id)
        {
            AuthToken = token;
            UserId = id;
        }

        public TokenResult(string token)
        {
            AuthToken = token;
        }
    }
}

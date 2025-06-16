using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSN.Application.TokenConstants;
public static class TokenConstants
{
    public const string TokenHeader = "Authorization";
    public const string TokenPrefix = "Bearer ";

    public const string UserIdClaim = "UserIdClaim";
    public const string UserRoleClaim = "UserRoleClaim";

    public const int TokenDurationInHours = 1;
}
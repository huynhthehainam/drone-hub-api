using System;

namespace MiSmart.Infrastructure.Constants
{
    public class Keys
    {
        public const String AuthHeaderKey = "Authorization";
        public const String JWTPrefixKey = "Bearer";
        public const String JWTAuthKey = "Auth";
        public const String JWTRoleKey = "RoleMiSmart";
        public const String JWTAdminKey = "AdminMiSmart";
        public const String JWTUserTypeKey = "UserTypeMiSmart";
        public const String AllowedOrigin = "AllowedOrigin";
        public const String IdentityClaim = "Auth";
    }
}
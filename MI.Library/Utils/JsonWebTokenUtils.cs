using MI.Library.Interface.Common;
using MI.Library.Interface.Enum;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MI.Library.Utils
{
    /// <summary>
    ///     Jwt相关工具类。该版本用于新版的Jwt
    /// </summary>
    public static class JsonWebTokenUtils
    {
        public static ClaimsPrincipal GetJwtPrincipal(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                return handler.ValidateToken(token, GetTokenValidationParameters(), out _);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static TokenValidationParameters GetTokenValidationParameters(SecurityKey key)
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                AuthenticationType = Constants.Authorization.Scheme.Current,
                NameClaimType = "name",
                RoleClaimType = "role"
            };
        }

        /// <summary>
        ///     获取当前环境的Jwt验证参数
        /// </summary>
        /// <returns></returns>
        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return GetTokenValidationParameters(GetSecurityKey());
        }


        /// <summary>
        ///     获取当前环境的SecurityKey
        /// </summary>
        /// <returns></returns>
        public static SecurityKey GetSecurityKey()
        {
            var jwkJson = GetSecurityKeyJson();

            return GetSecurityKey(jwkJson);
        }

        /// <summary>
        ///     根据jwk获取SecurityKey
        /// </summary>
        /// <param name="jwkJson">jwk（Json格式）</param>
        /// <returns></returns>
        public static SecurityKey GetSecurityKey(string jwkJson)
        {
            return new JsonWebKey(jwkJson);
        }

        private static string GetSecurityKeyJson()
        {
            var env = EnvironmentConfiguration.Environment;

            switch (env)
            {
                case EnvironmentType.Product:
                    return
                        "{\"kty\":\"RSA\",\"use\":\"sig\",\"kid\":\"c7ea11d2c61eb7e2c5f80fa427c60b5f\",\"e\":\"AQAB\",\"n\":\"udc2Y_lAOozzsZIKSKsmDRzIEv5utuHKSDrdP89sAEG9HovHhRQCeJePU_6VMw0LnUYte9o5PXhtolcYpL8zpuUctC3N3NqsM4wS1Td5zaaWN3weIRyQ3eCHj76mamiUXBse0PH6Jz9Rw-Pf9qLH1gJXbCaUFVJdCQa8XgBv4ejNbNou9xQzXwh2wepp-SNGrAuHvs_5mW68mVr_-K8Dsv3tmb3pUxOdyuDVen5_9nPr4igiU2UDROP57hAjgjpjzjsSBISy26VlhzpKQHexQMcXb8ZD-FQqKujwLnWoPsIRj4PotyzItKjPNC41B_-lhTqboIpMcd0YQQIdT7bLzQ\",\"alg\":\"RS256\"}";
                case EnvironmentType.Demo:
                    return "{\"kty\":\"RSA\",\"use\":\"sig\",\"kid\":\"4c552f7516fe22b08bf1c5caf9026508\",\"e\":\"AQAB\",\"n\":\"y6D9srIUvcRzZtGtB2tj8G2Dxgzoukme17SsiDISmkzDe9Y9OYOzbp0bOyDe4i9hAv3-p-qLSyhlSaAe7X5i5EG29hmytsmVt4ZIA-H-mruYMGNnAc1xHL2Z6mGyfWFqNMzR0Dre9-nYK9yLGlEjBiwbTX67I_5knMLfskyjmMDs2sZuz3t5ks2u7ZkzT7-FgtKkSB9jJbz1nT9YaJdKzJEoDeW3RrlOgXgQEEcW-VBToBe8ekkcvomv6E1PXmGYFzYEPNUMHvtc_q98VI9XlTEfcv0z8s1lhXbAZZ6M2bEqrDuwjfqks5kJm73wMTS3EIY52yTSrbhRSPpOmIlzzQ\",\"alg\":\"RS256\"}";
                case EnvironmentType.Default:
                    return "{\"kty\":\"RSA\",\"use\":\"sig\",\"kid\":\"23afe9e610fbe46309085ea4c3dac4b3\",\"e\":\"AQAB\",\"n\":\"0Fo6BmVmyAvb7HDsdgxXTvuvc_W_VEjXyOJI3kLBc_qmZXaEhKiqpuA548O83l7CgfsYVr6E0MekgP3GPr8upzNvCdPfOfU9RZeyI7nKNLx354TkeFS7ed2DKB09w5t-Odbr5PQ_y1Wg8aW-Jt-Z6dkHbtaXgQXQQRRGkENFt9LjolzCC94UTrRV-khNEW3uvL5-nP3bNgxX-xJtX5enl6-E_VMHDTcA4bWwLoKGsmoWZWKa42Yw5y5s398bcL6o8O3kAPtpLIRTO2OBdlS702R2rBmtjaE9NInFoVMZ8FAgSYB82Oqi-GY-GNnlSBg-dviIJZwO30Avwsw3bFBRrQ\",\"alg\":\"RS256\"}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

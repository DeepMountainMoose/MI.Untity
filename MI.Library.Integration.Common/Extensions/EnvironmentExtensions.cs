using Com.Ctrip.Framework.Apollo.Enums;
using MI.Library.Interface.Enum;

namespace MI.Library.Integration.Common.Extensions
{
    internal static class EnvironmentExtensions
    {
        public static Env ToApolloEnv(this EnvironmentType environmentType)
        {
            switch (environmentType)
            {
                case EnvironmentType.Product:
                    return Env.Pro;
                case EnvironmentType.Demo:
                    return Env.Fat;
                default:
                    return Env.Dev;
            }
        }
    }
}

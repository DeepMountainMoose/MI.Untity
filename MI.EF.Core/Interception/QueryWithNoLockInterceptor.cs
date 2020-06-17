using System.Data.Common;
using System.Text.RegularExpressions;

namespace MI.EF.Core.Interception
{
    internal static class QueryWithNoLockInterceptor /*: IDbCommandInterceptor*/
    {
        private static readonly Regex WithNoLockRegex = new Regex("(?<tableAlias>(FROM|JOIN) \\[\\w+] AS \\[\\w+](?! WITH \\(NOLOCK\\)))", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static void QueryWithNoLock(DbCommand command)
        {
            command.CommandText = WithNoLockRegex.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
        }
    }
}

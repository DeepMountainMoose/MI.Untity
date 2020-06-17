using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace MI.EF.Core.Interception
{
    internal static class InsertWithScopeIdentityInterceptor
    {
        private static readonly Regex IdentityScopeRegex = new Regex("SELECT \\[PKID\\]\r\nFROM \\[dbo\\]\\.\\[[\\s\\S]+\\]\r\nWHERE @@ROWCOUNT > 0 AND \\[PKID\\] = scope_identity", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        public static void ReaderExecuting(DbCommand command)
        {
            if (IdentityScopeRegex.IsMatch(command.CommandText))
            {
                command.CommandText = IdentityScopeRegex.Replace(command.CommandText, "SELECT scope_identity") + " AS [PKID]";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace MI.EF.Core.Interception
{
    public class DisableDangerCommandInterceptor
    {
        private static readonly List<string> DangerCommands = new List<string>
        {
            "DROP",
            "DELETE FROM",
            "TRUNCATE"
        };

        public static void CheckCommand(DbCommand command)
        {
            if (DangerCommands.Any(_ => command.CommandText.IndexOf(_, StringComparison.InvariantCultureIgnoreCase) >= 0))
            {
                var innerException = new DbContextException(0, command.CommandText);

                throw new DbContextException(0, innerException, "当前语句包含高危操作");
            }
        }
    }
}

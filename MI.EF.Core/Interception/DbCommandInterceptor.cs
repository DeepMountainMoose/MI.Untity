using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MI.EF.Core.Interception
{
    public class DbCommandInterceptor : IObserver<DiagnosticListener>
    {
        private readonly DBRelationalEvent _dbCommandInterceptor = new DBRelationalEvent();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener listener)
        {
            if (listener.Name == DbLoggerCategory.Name)
            {
                listener.Subscribe(_dbCommandInterceptor);
            }
        }
    }

    public class DBRelationalEvent : IObserver<KeyValuePair<string, object>>
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Key == RelationalEventId.CommandExecuting.Name)
            {
                var command = ((CommandEventData)value.Value).Command;

                DisableDangerCommandInterceptor.CheckCommand(command);
                QueryWithNoLockInterceptor.QueryWithNoLock(command);
                //InsertWithScopeIdentityInterceptor.ReaderExecuting(command);
            }

        }

    }
}

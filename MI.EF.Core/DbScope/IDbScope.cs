using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.EF.Core.DbScope
{
    public interface IDbScope
    {
        void Execute(Action action);

        Task ExecuteAsync(Func<Task> action);

        TResult Execute<TResult>(Func<TResult> func);

        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func);
    }
}

using MI.Component.Core.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.EF.Core.DbScope
{
    public class DbRetryScope : IDbScope
    {
        private static readonly List<int> SqlErrorCodes = new List<int>
        {
            0,
            -1,
            -2,
            64,
            109,
            233,
            955,
            4060,
            10054,
            10061,
            16804,
            17142,
            6005
        };

        private readonly DbRetryScopeConfig retryConfig;

        public DbRetryScope()
            : this(new DbRetryScopeConfig())
        { }

        public DbRetryScope(DbRetryScopeConfig retryConfig)
        {
            this.retryConfig = retryConfig;
        }

        public void Execute(Action action)
        {
            ParameterChecker.CheckNull(action, "action");
            var retriedCount = 0;
            while (true)
            {
                try
                {
                    action();

                    break;
                }
                catch (Exception ex)
                {
                    if (!this.HandlerException(ex, ref retriedCount))
                    {
                        throw;
                    }
                }

                Thread.Sleep(100);
            }
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            ParameterChecker.CheckNull(action, "action");

            var retriedCount = 0;
            while (true)
            {
                try
                {
                    await action();

                    break;
                }
                catch (Exception ex)
                {
                    if (!this.HandlerException(ex, ref retriedCount))
                    {
                        throw;
                    }
                }

                Thread.Sleep(100);
            }
        }

        public TResult Execute<TResult>(Func<TResult> func)
        {
            ParameterChecker.CheckNull(func, "func");
            var retriedCount = 0;

            TResult result;
            while (true)
            {
                try
                {
                    result = func();

                    break;
                }
                catch (Exception ex)
                {
                    if (!this.HandlerException(ex, ref retriedCount))
                    {
                        throw;
                    }
                }

                Thread.Sleep(100);
            }

            return result;
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func)
        {
            ParameterChecker.CheckNull(func, "func");
            var retriedCount = 0;

            TResult result;
            while (true)
            {
                try
                {
                    result = await func();

                    break;
                }
                catch (Exception ex)
                {
                    if (!this.HandlerException(ex, ref retriedCount))
                    {
                        throw;
                    }
                }

                Thread.Sleep(100);
            }

            return result;
        }

        private bool HandlerException(Exception ex, ref int retriedCount)
        {
            if (ex == null)
            {
                return false;
            }

            var sourceException = ex;
            var sqlException = sourceException as SqlException;
            while (sqlException == null && sourceException != null && sourceException.InnerException != null)
            {
                sourceException = sourceException.InnerException;

                sqlException = sourceException as SqlException;
            }

            if (sqlException == null)
            {
                return false;
            }

            if (!SqlErrorCodes.Contains(sqlException.Number))
            {
                return false;
            }

            return retriedCount++ < this.retryConfig.MaximumRetries;
        }
    }
}

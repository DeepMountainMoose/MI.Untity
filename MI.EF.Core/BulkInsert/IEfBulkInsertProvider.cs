using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MI.EF.Core.BulkInsert
{
    public interface IEfBulkInsertProvider
    {
        DbContext Context { get; }
        IDbConnection GetConnection();
        void Run<T>(IEnumerable<T> entioties);
        IEfBulkInsertProvider SetContext(DbContext context);
    }
}

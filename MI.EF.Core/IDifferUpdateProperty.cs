using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public interface IDifferUpdateProperty<TEntity>
        where TEntity : class
    {
        string Property { get; }

        object Value { get; }

        bool IsDiffer { get; }

        string UpdateConstraint { get; }

        NumberCondition Condition { get; }
    }
}

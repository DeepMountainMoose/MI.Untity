using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MI.EF.Core
{
    public class DifferUpdateProperty<TEntity,TValue>:UpdateProperty<TEntity,TValue>,IDifferUpdateProperty<TEntity>
        where TEntity:class
        where TValue:struct
    {
        public DifferUpdateProperty(Expression<Func<TEntity,TValue>> propertyExpression,TValue value)
            :this(propertyExpression,value,string.Empty)
        { }

        public DifferUpdateProperty(Expression<Func<TEntity,TValue>> propertyExpression,TValue value,string updateConstraint)
            :base(propertyExpression,value,true,updateConstraint)
        { }

        public DifferUpdateProperty(Expression<Func<TEntity,TValue>> propertyExpression,TValue value,NumberCondition condition)
            :base(propertyExpression,value,true,condition)
        { }
    }
}

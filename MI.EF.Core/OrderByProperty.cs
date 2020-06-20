using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MI.EF.Core
{
    public class OrderByProperty<TOrderByEntity, TOrderByKey>
    {
        public Expression<Func<TOrderByEntity, TOrderByKey>> OrderByKey { get; private set; }
        public bool OrderByDesc { get; private set; } = false;
        private OrderByProperty() { }

        public static OrderByProperty<TOrderByEntity,TOrderByKey> Asc(Expression<Func<TOrderByEntity, TOrderByKey>> orderByKey)
        {
            return new OrderByProperty<TOrderByEntity, TOrderByKey>()
            {
                OrderByKey = orderByKey,
                OrderByDesc = false
            };
        }

        public static OrderByProperty<TOrderByEntity, TOrderByKey> Desc(Expression<Func<TOrderByEntity,TOrderByKey>> orderByKey)
        {
            return new OrderByProperty<TOrderByEntity, TOrderByKey>()
            {
                OrderByKey = orderByKey,
                OrderByDesc = true,
            };
        }
    }
}

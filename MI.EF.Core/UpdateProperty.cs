using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MI.EF.Core
{
    public class UpdateProperty<TEntity, TValue> : IDifferUpdateProperty<TEntity>
        where TEntity : class
    {
        public string Property { get; private set; }

        public object Value { get; private set; }

        public bool IsDiffer { get; private set; }

        public string UpdateConstraint { get; private set; }

        public NumberCondition Condition { get; set; }

        public int ConditionValue { get; set; }

        public UpdateProperty(Expression<Func<TEntity,TValue>> propertyExpression,TValue value,bool isDiffer,string updateConstraint)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if(memberExpression==null)
            {
                throw new DbContextException(0, $"{propertyExpression.Body}无法无法获得{typeof(TEntity).Name}的属性值");
            }

            this.Property = memberExpression.Member.Name;
            var ColumnAttributeData = memberExpression.Member.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Equals(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)));
            if (ColumnAttributeData != null && ColumnAttributeData.ConstructorArguments.Count > 0)
            {
                this.Property = ColumnAttributeData.ConstructorArguments[0].Value.ToString();
            }

            this.Value = value;
            this.IsDiffer = isDiffer;
            this.UpdateConstraint = updateConstraint;
            this.Condition = NumberCondition.Null;
            this.ConditionValue = 0;
        }

        public UpdateProperty(string property, TValue value, bool isDiffer, string updateConstraint)
        {
            this.Property = property;
            this.Value = value;
            this.IsDiffer = isDiffer;
            this.UpdateConstraint = updateConstraint;
            this.Condition = NumberCondition.Null;
            this.ConditionValue = 0;
        }

        public UpdateProperty(Expression<Func<TEntity, TValue>> propertyExpression, TValue value)
            : this(propertyExpression, value, false, string.Empty)
        {

        }

        public UpdateProperty(Expression<Func<TEntity, TValue>> propertyExpression, TValue value, bool isDiffer, NumberCondition condition)
            : this(propertyExpression, value, isDiffer, string.Empty)
        {
            this.Condition = condition;
            setPatch();
        }

        public UpdateProperty(string property, TValue value, bool isDiffer, NumberCondition condition)
            : this(property, value, isDiffer, string.Empty)
        {
            this.Condition = condition;
            setPatch();
        }

        private void setPatch()
        {
            switch (this.Condition)
            {
                case NumberCondition.Null:
                    break;
                case NumberCondition.Larger:
                    this.UpdateConstraint = $">{this.ConditionValue}";
                    break;
                case NumberCondition.LargerOrEqual:
                    this.UpdateConstraint = $">={this.ConditionValue}";
                    break;
                case NumberCondition.Less:
                    this.UpdateConstraint = $"<{this.ConditionValue}";
                    break;
                case NumberCondition.LessOrEquual:
                    this.UpdateConstraint = $"<={this.ConditionValue}";
                    break;
                case NumberCondition.Equal:
                    this.UpdateConstraint = $"={this.ConditionValue}";
                    break;
                case NumberCondition.NotEqual:
                    this.UpdateConstraint = $"<>{this.ConditionValue}";
                    break;
                default:
                    break;
            }
        }
    }
}

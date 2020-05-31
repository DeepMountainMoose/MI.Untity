using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MI
{
    public class SuccessfulLazy<T>:IWrapper<T>
    {
        private readonly Func<T> _valueFactory;
        private T _value;
        private bool _initialized;

        /// <summary>Initializes a new instance of the <see cref="SuccessfulLazy{T}"/> class.</summary>
        /// <param name="valueFactory">The value factory.</param>
        public SuccessfulLazy([NotNull] Func<T> valueFactory) => _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));

        /// <summary>
        /// Gets a value indicating whether this instance is value created.
        /// </summary>
        /// <value><c>true</c> if this instance is value created; otherwise, <c>false</c>.</value>
        public bool IsValueCreated => _initialized;

        /// <summary>如果创建value时报错则会一直创建。</summary>
        public T Value
        {
            get
            {
                object syncLock = this;
                return LazyInitializer.EnsureInitialized(ref _value, ref _initialized, ref syncLock, _valueFactory);
            }
        }
    }
}

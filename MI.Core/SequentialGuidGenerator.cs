using System;
using System.Security.Cryptography;

namespace MI.Core
{
    /// <summary>
    ///     Implements <see cref="IGuidGenerator" /> by creating sequential Guids.
    ///     This code is taken from jhtodd/SequentialGuid
    ///     https://github.com/jhtodd/SequentialGuid/blob/master/SequentialGuid/Classes/SequentialGuid.cs
    ///     <para>
    ///         实现<see cref="IGuidGenerator" />用于产生有序的Guid
    ///         本代码参考自
    ///         https://github.com/jhtodd/SequentialGuid/blob/master/SequentialGuid/Classes/SequentialGuid.cs
    ///     </para>
    /// </summary>
    public class SequentialGuidGenerator : IGuidGenerator
    {
        /// <summary>
        ///     Database type to generate GUIDs.
        ///     <para>生成GUID的数据库类型</para>
        /// </summary>
        public enum SequentialGuidDatabaseType
        {
            /// <summary>
            ///     SqlServer
            /// </summary>
            SqlServer,

            /// <summary>
            ///     Oracle
            /// </summary>
            Oracle,

            /// <summary>
            ///     MySql
            /// </summary>
            MySql,

            /// <summary>
            ///     PostgreSql
            /// </summary>
            PostgreSql
        }

        /// <summary>
        ///     Describes the type of a sequential GUID value.
        ///     <para>
        ///         生成有序GUID的数据类型
        ///     </para>
        /// </summary>
        public enum SequentialGuidType
        {
            /// <summary>
            ///     The GUID should be sequential when formatted using the
            ///     <see cref="Guid.ToString()" /> method.
            /// </summary>
            SequentialAsString,

            /// <summary>
            ///     The GUID should be sequential when formatted using the
            ///     <see cref="Guid.ToByteArray" /> method.
            /// </summary>
            SequentialAsBinary,

            /// <summary>
            ///     The sequential portion of the GUID should be located at the end
            ///     of the Data4 block.
            /// </summary>
            SequentialAtEnd
        }

        private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

        /// <summary>
        ///     Prevents a default instance of the <see cref="SequentialGuidGenerator" /> class from being created.
        ///     Use <see cref="Instance" />.
        /// </summary>
        private SequentialGuidGenerator()
        {
            DatabaseType = SequentialGuidDatabaseType.SqlServer;
        }

        /// <summary>
        ///     Gets the singleton <see cref="SequentialGuidGenerator" /> instance.
        /// </summary>
        public static SequentialGuidGenerator Instance { get; } = new SequentialGuidGenerator();

        /// <summary>
        ///     Database Type
        ///     <para>
        ///         数据库类型
        ///     </para>
        /// </summary>
        public SequentialGuidDatabaseType DatabaseType { get; set; }

        /// <summary>
        ///     Creates a GUID.
        ///     <para>产生一个GUID</para>
        /// </summary>
        public Guid Create()
        {
            return Create(DatabaseType);
        }

        /// <summary>
        ///     Creates a GUID.
        ///     <para>产生一个GUID</para>
        /// </summary>
        /// <param name="databaseType"></param>
        /// <returns></returns>
        public Guid Create(SequentialGuidDatabaseType databaseType)
        {
            switch (databaseType)
            {
                case SequentialGuidDatabaseType.SqlServer:
                    return Create(SequentialGuidType.SequentialAtEnd);
                case SequentialGuidDatabaseType.Oracle:
                    return Create(SequentialGuidType.SequentialAsBinary);
                case SequentialGuidDatabaseType.MySql:
                case SequentialGuidDatabaseType.PostgreSql:
                    return Create(SequentialGuidType.SequentialAsString);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///     Creates a GUID.
        ///     <para>产生一个GUID</para>
        /// </summary>
        /// <param name="guidType"></param>
        /// <returns></returns>
        public Guid Create(SequentialGuidType guidType)
        {
            // We start with 16 bytes of cryptographically strong random data.
            var randomBytes = new byte[10];
            Rng.GetBytes(randomBytes);

            var timestamp = DateTime.UtcNow.Ticks / 10000L;

            var timestampBytes = BitConverter.GetBytes(timestamp);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            var guidBytes = new byte[16];

            switch (guidType)
            {
                case SequentialGuidType.SequentialAsString:
                case SequentialGuidType.SequentialAsBinary:

                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                    if (guidType == SequentialGuidType.SequentialAsString && BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(guidBytes, 0, 4);
                        Array.Reverse(guidBytes, 4, 2);
                    }

                    break;

                case SequentialGuidType.SequentialAtEnd:

                    Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                    Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                    break;
            }

            return new Guid(guidBytes);
        }
    }
}

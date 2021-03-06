﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library
{
    public class Constants
    {
        public static class Apollo
        {
            public const string ClusterEnvironment = "CLUSTER_ENVIRONMENT";

            public const string DefaultNamespace = "EHI.Front";

            public static class Aes
            {
                public const string Key = "7hf^$Hd*g3@#!fd4";

                public const string Iv = "a1rg35Dew47f4ffk";
            }

            public static class Database
            {
                public const string AppId = "100000";

                public const string Namespace = "MI.DBConfig";
            }
        }

        public static class Authorization
        {
            public static class Scheme
            {
                public const string Past = "OldEhiJwtBearer";

                public const string Current = "EhiJwtBearer";
            }

            public const string CookieName = "ehi_l_a";
        }
    }
}

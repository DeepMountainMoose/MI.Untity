using MI.EF.Core;

namespace MI.Core.Test
{
    public partial class MIContext : DbContextBase
    {
        public MIContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
    }
}

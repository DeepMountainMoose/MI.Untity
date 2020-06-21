using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MI.EF.Core.MySql.Test
{
    public class SlideShowImg
    {
        [Key]
        public int Id { get; set; }

        public string imgName { get; set; }

        public int ProductId { get; set; }

        public string LinkPage { get; set; }

        public bool pushHome { get; set; }
    }

    public partial class MIContext
    {
        public virtual DbSet<SlideShowImg> SlideShowImg { get; set; }
    }
}

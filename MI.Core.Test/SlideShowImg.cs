using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Core.Test
{
    [Table("SlideShowImg")]
    public class SlideShowImg
    {
        [Key]
        [Column("id")]
        public int PKID { get; set; }

        [Column("imgName")]
        public string ImgName { get; set; }

        public int ProductID { get; set; }

        public string LinkPage { get; set; }

        [Column("pushHome")]
        public bool PushHome { get; set; }
    }

    public partial class MIContext
    {
        public virtual DbSet<SlideShowImg> SlideShowImg { get; set; }
    }
}

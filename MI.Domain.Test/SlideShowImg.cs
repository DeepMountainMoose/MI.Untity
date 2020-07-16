using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MI.Domain.Test
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
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace News.Models
{
    public class Image
    {
        [Key]
        public Int32 Id { get; set; }

        public byte[] ImageSmall { get; set; }
        public byte[] ImageLarge { get; set; }

        [Required]
        [DisplayName("Article")]
        public Int32 ArticleId { get; set; }

        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace News.Models
{
    public class Article
    {
        public Article()
        {
            this.Author = HttpContext.Current.User.Identity.Name;
        }

        [Key]
        public Int32 Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(200)]
        public String Title { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [MaxLength(1000)]
        public String Summary { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public String Content { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PublishedAt { get; set; }

        [Required]
        public String Author { get; set; }

        [Required]
        public Boolean isLead { get; set; }

        public virtual ICollection<Image> Images { get; set; }
    }
}
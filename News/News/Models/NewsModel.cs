using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace News.Models
{
    public class NewsModel : DbContext
    {
        public NewsModel()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Image> Images { get; set; }
    }
}
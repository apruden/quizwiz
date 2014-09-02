namespace QuizWiz.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public class ConfigContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        public ConfigContext()
            : base("DefaultConnection")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Setting> Settings { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace jamescms.Models
{
    public class TextContext : DbContext, ITextContext
    {
        public TextContext() :
            base("DefaultConnection")
        {
            
        }

        public virtual IDbSet<TextWall> TextWalls { get; set; }
        public virtual IDbSet<Text> Texts { get; set; }
        public virtual IDbSet<Tag> Tags { get; set; }

    }

    public interface ITextContext
    {
        public IDbSet<TextWall> TextWalls { get; set; }
        public IDbSet<Text> Texts { get; set; }
        public IDbSet<Tag> Tags{ get; set; }
    }

    [Table("TextWalls")]
    public class TextWall
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TextWallId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [Table("Texts")]
    public class Text
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TextId { get; set; }            
        public string Title { get; set; }
        public string Article { get; set; }
        public DateTime Posted { get; set; }
        public DateTime Updated { get; set; }
    }

    [Table("Tags")]
    public class Tag
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }
        public string Name { get; set; }
    }

    public class Comment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public string 
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace jamescms.Models
{
    #region Interface

    public interface ITextContext
    {
        public IDbSet<TextWall> TextWalls { get; set; }
        public IDbSet<Text> Texts { get; set; }
        public IDbSet<Tag> Tags { get; set; }
        public IDbSet<Comment> Comments { get; set; }
    }

    #endregion Interface

    #region Model Context

    public class TextContext : DbContext, ITextContext
    {
        public TextContext() :
            base("DefaultConnection")
        {
            
        }

        public virtual IDbSet<TextWall> TextWalls { get; set; }
        public virtual IDbSet<Text> Texts { get; set; }
        public virtual IDbSet<Tag> Tags { get; set; }
        public virtual IDbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Text>()
                .HasMany(d => d.Tags)
                .WithMany(d => d.Texts)
                .Map(d =>
                    {
                        d.ToTable("Texts_Tags");
                        d.MapLeftKey("TextId");
                        d.MapRightKey("TagId");
                    });

            modelBuilder.Entity<Comment>()
                .HasMany(d => d.ChildComments)
                .WithMany()
                .Map(d =>
                    {
                        d.ToTable("ChildCommentRelationships");
                        d.MapLeftKey("CommentParentId");
                        d.MapRightKey("CommentId");
                    });
        }
    }

    #endregion Model Context

    #region Text Model Classes

    [Table("TextWalls")]
    public class TextWall : Entity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
    }

    [Table("Texts")]
    public class Text : Entity
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        public string Article { get; set; }
        [Required]
        public DateTime Posted { get; set; }
        [Required]
        public DateTime Updated { get; set; }

        public virtual IList<Tag> Tags { get; set; }
    }

    [Table("Tags")]
    public class Tag
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual IList<Text> Texts { get; set; }
    }

    public class Comment
    {
        [Required]
        [StringLength(2000)]
        public string Comment { get; set; }
        public int? ParentCommentId { get; set; }
        public int UserId { get; set; }
        public DateTime Posted { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }

        public virtual IList<Comment> ChildComments { get; set; }
    }

    #endregion Text Model Classes
}

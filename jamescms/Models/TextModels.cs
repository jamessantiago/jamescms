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
        IDbSet<TextWall> TextWalls { get; set; }
        IDbSet<Text> Texts { get; set; }
        IDbSet<Tag> Tags { get; set; }
        IDbSet<Comment> Comments { get; set; }
    }

    #endregion Interface

    #region Model Context

    public class TextContext : DbContext, ITextContext
    {
        public TextContext() :
            base("ApplicationConnection")
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
    [FullTextIndex("FTIX_Texts", "Title, Article")]
    public class Text : Entity
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        [Index("IX_Text_UrlTitle", true)]
        [StringLength(100)]
        public string UrlTitle { get; set; }
        [Required]        
        public string Article { get; set; }
        [Required]
        [Index("IX_Text_Posted", false, true)]
        public DateTime Posted { get; set; }
        [Required]
        public DateTime Updated { get; set; }

        public virtual IList<Comment> Comments { get; set; }
        public virtual IList<Tag> Tags { get; set; }
        
    }

    [Table("Tags")]
    public class Tag : Entity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual IList<Text> Texts { get; set; }
    }

    [Table("Comments")]
    public class Comment : Entity
    {
        [Required]
        [StringLength(2000)]
        public string CommentText { get; set; }
        public int? ParentCommentId { get; set; }
        [Required]
        [Index("IX_Comment_UserId")]
        public int UserId { get; set; }
        [Required]
        public DateTime Posted { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }

        public virtual Text Text {get; set;}
        public virtual IList<Comment> ChildComments { get; set; }
    }

    #endregion Text Model Classes
}

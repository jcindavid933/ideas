using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace brightideas.Models
{
    public class Post
    {
        [Key]
        public int PostId {get;set;}
        [Required]
        public string Content {get;set;}
        public DateTime created_at {get;set;}
        public DateTime updated_at {get;set;}

        public int UserId{get;set;}
        [ForeignKey("UserId")]
        public User user {get;set;}
        public List<Like> Like {get;set;}

    }
}
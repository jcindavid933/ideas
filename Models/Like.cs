using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace brightideas.Models
{
    public class Like
    {
        [Key]
        public int LikeId {get;set;}
        [Required]
        public DateTime created_at {get;set;}
        public int UserId{get;set;}
        [ForeignKey("UserId")]
        public User user {get;set;}
        public int PostId{get;set;}
        [ForeignKey("PostId")]
        public Post post {get;set;}

    }
}
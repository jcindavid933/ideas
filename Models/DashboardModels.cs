using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace brightideas.Models
{
    public class DashboardModels
    {
        public List<Post> allPosts {get; set;}
        public User User {get; set;}
    }
}
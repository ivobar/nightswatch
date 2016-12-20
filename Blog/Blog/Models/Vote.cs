using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }

        public bool HasVoted { get; set; }

        public bool HasVotedPositive { get; set; }

        public bool HasVotedNegative { get; set; }


    }
}
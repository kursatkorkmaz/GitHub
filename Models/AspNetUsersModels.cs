﻿using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExamEase.Models
{
    public class AspNetUsers : IdentityUser<string>
    {
        //extra column
        public DateTime? LockoutEndDateUtc { get; set; }
    }
}




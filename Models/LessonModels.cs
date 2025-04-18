using ExamEase.Data;
using ExamEase.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ExamEase.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }
        [MaxLength(1000)]
        public string LessonName { get; set; }
        public int LessonCatId { get; set; }

        public int LessonCatOrnek { get; set; }
        public int LessonCatYuzde { get; set; }
        public int LessonCatTest { get; set; }

        public int LessonCatSoru { get; set; }

        public int LessonCatSira { get; set; }

    }

   





   


    public class LessonListing
    {
        public List<LessonListing> Listing { get; set; }
       
    }

  




}

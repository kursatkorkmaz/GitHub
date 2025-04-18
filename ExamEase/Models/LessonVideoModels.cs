using ExamEase.Data;
using ExamEase.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExamEase.Models
{
    public class LessonVideo
    {
        [Key]
        public int LessonVideoId { get; set; }
        public int LessonVideoVideoId { get; set; }
        public int LessonVideoCatId { get; set; }

        public int LessonVideoLessonId { get; set; }
    }


}

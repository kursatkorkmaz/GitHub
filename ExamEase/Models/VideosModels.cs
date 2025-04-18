using ExamEase.Data;
using ExamEase.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExamEase.Models
{
    public class Videos
    {
        [Key]
        public int Video_id { get; set; }
        [MaxLength(500)]
        public string Video_Adi { get; set; }
        [MaxLength(1500)]
        public string Video_Path { get; set; }
        [MaxLength(50)]
        public string Video_sure { get; set; }
    }


}

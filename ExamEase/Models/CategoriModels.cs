using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ExamEase.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ExamEase.Models
{
    public class Categori
    {
        [Key]
        public int CatId { get; set; }
        public string CatAciklama { get; set; }
        public string CarDurum { get; set; }
    }


    public class CategoriViewModel
    {
        public int CatId { get; set; }
       
        public string CatAciklama { get; set; }

        public string CarDurum { get; set; }
    
        public List<SelectListItem> GenderSelectList { get; set; }
    }




    public class CategoriListing
    {
        public List<CategoriViewModel> Listing { get; set; }
        public string StudentExamStatus { get; set; }
    }

    public class CategoriResultViewModel
    {
        public int CatId { get; set; }

        public string CatAciklama { get; set; }

        public string CarDurum { get; set; }
    }


}

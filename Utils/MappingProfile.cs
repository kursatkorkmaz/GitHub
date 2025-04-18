using AutoMapper;
using ExamEase.Models;
using System.IO.Packaging;

namespace ExamEase.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StudentAnswer, StudentAnswerCloned>();
        }
    }

}

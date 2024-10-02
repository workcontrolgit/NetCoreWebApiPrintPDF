namespace NetCoreWebApiPrintPDF.WebApi.Models
{
    public class CertificateViewModel
    {
        public string StudentName { get; set; }
        public string CoachName { get; set; }
        public DateTime EvaluationDate { get; set; }
        public List<SkillGrade> SkillGrades { get; set; }
    }
}
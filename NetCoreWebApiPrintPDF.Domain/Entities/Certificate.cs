namespace NetCoreWebApiPrintPDF.Domain.Entities
{
    public class Certificate
    {
        public string StudentName { get; set; }
        public string CoachName { get; set; }
        public DateTime EvaluationDate { get; set; }
        public List<SkillGrade> SkillGrades { get; set; }
    }
}
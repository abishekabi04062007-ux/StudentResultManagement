namespace StudentResultManagement.Services
{
    public class ResultCalculationService
    {
        public int CalculateTotal(int internalMark, int externalMark)
        {
            return internalMark + externalMark;
        }

        public string CalculateGrade(int total)
        {
            if (total >= 90 && total <= 100) return "A+";
            if (total >= 80 && total <= 89) return "A";
            if (total >= 70 && total <= 79) return "B+";
            if (total >= 60 && total <= 69) return "B";
            if (total >= 50 && total <= 59) return "C";
            if (total >= 40 && total <= 49) return "D";
            return "F";
        }

        public string CalculateResultStatus(int total)
        {
            return total >= 40 ? "PASS" : "FAIL";
        }

        public double CalculatePercentage(int totalMarks, int maxMarks)
        {
            if (maxMarks == 0) return 0;
            return Math.Round((double)totalMarks / maxMarks * 100, 2);
        }
    }
}

/// <summary>
/// Shared grade ordering for save / end-screen comparisons.
/// </summary>
public static class GradeRanking
{
    public enum Grade
    {
        D = 1,
        C = 2,
        B = 3,
        A = 4,
        S = 5,
        SPlus = 6
    }

    public static Grade GetGradeValue(string gradeString)
    {
        return gradeString switch
        {
            "S+" => Grade.SPlus,
            "S" => Grade.S,
            "A" => Grade.A,
            "B" => Grade.B,
            "C" => Grade.C,
            "D" => Grade.D,
            _ => Grade.D
        };
    }

    public static bool IsAchievedGradeBetter(string gradeAchieved, string gradeCheck)
    {
        return GetGradeValue(gradeAchieved) > GetGradeValue(gradeCheck);
    }
}

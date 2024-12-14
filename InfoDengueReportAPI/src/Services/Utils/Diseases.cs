namespace InfoDengueReportAPI.Services.Utils;
public static class Diseases
{
    public static List<string> GetDiseases()
        {
            return new List<string> { "dengue", "zika", "chikungunya" };
        }
}
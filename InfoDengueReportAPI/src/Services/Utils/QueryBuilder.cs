namespace InfoDengueReportAPI.Services.Utils;
public class QueryBuilder
{
    public static string Build(string geoCode, string disease, int ewStart, int ewEnd, int eyStart, int eyEnd)
    {
        return $"geocode={geoCode}&disease={disease}&format=csv&ew_start={ewStart}&ew_end={ewEnd}&ey_start={eyStart}&ey_end={eyEnd}";
    }
}
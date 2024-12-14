namespace InfoDengueReportAPI.Services.Utils;
public static class GeoCodes
{
    public static List<string> GetGeoCodes()
        {
            return new List<string>
            {
                GeocodeTranslator.GetGeoCode("rio_de_janeiro"),
                GeocodeTranslator.GetGeoCode("sao_paulo")
            };
        }
}

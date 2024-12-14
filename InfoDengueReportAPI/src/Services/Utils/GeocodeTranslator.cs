namespace InfoDengueReportAPI.Services.Utils;
public static class GeocodeTranslator
{
    public static string GetMunicipio(string geocode) => geocode switch
    {
        "3304557" => "rio_de_janeiro",
        "3550308" => "sao_paulo",
        _ => throw new ArgumentException($"Geocode {geocode} não encontrado.")
    };

    public static string GetGeoCode(string municipio) => municipio switch
    {
        "rio_de_janeiro" => "3304557",
        "sao_paulo" => "3550308",
        _ => throw new ArgumentException($"Geocode {municipio} não encontrado.")
    };
}
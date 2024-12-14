namespace InfoDengueReportAPI.Services.Utils;
public static class GeocodeTranslator
{
    public static string GetMunicipio(string geocode) => geocode switch
    {
        "3304557" => "Rio de Janeiro",
        "3550308" => "São Paulo",
        _ => throw new ArgumentException($"Geocode {geocode} não encontrado.")
    };

    public static string GetGeoCode(string municipio) => municipio switch
    {
        "rio_de_janeiro" => "3304557",
        "sao_paulo" => "3550308",
        _ => throw new ArgumentException($"Geocode {municipio} não encontrado.")
    };
}
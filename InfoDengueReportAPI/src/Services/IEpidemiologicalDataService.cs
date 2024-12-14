using InfoDengueReportAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoDengueReportAPI.Services
{
    public interface IEpidemiologicalDataService
    {
        Task<List<Relatorio>> GetAllEpidemiologicalDataAsync();
        Task<List<Relatorio>> GetEpidemiologicalDataByGeocodeAsync(string geoCode, string municipio);
        Task<Dictionary<string, Dictionary<string, double>>> GetTotalCasesInRJandSPAsync();
        Task<List<Relatorio>> GetCustomEpidemiologicalDataAsync(string geoCode, string disease, int ewStart, int ewEnd, int eyStart, int eyEnd, string municipio);
    }
}
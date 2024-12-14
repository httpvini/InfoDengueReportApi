namespace InfoDengueReportAPI.Services
{
    public interface IEpidemiologicalDataService
    {
        Task<List<string>> GetAllEpidemiologicalDataAsync();
        Task<List<string>> GetEpidemiologicalDataByGeocodeAsync(string geoCode);
        Task<List<List<int>>> GetTotalCasesInRJandSPAsync();
        Task<List<int>> GetTotalCasesByGeocodeAsync(string geoCode);
        Task<string> GetCustomEpidemiologicalDataAsync(string geoCode, string disease, int ewStart, int ewEnd, int eyStart, int eyEnd);
    }
}

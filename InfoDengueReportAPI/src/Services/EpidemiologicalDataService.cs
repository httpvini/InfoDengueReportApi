using InfoDengueReportAPI.Services.Utils;
using Newtonsoft.Json;

namespace InfoDengueReportAPI.Services
{
    public class EpidemiologicalDataService : IEpidemiologicalDataService
    {
        private readonly IApiClient _apiClient;
        private readonly IGeocodeTranslator _geocodeTranslator;
        private const string ApiUrl = "https://info.dengue.mat.br/api/alertcity";
        private const int EwStart = 1;
        private const int EwEnd = 52;
        private const int EyStart = 1980;
        private const int EyEnd = 2024;

        public EpidemiologicalDataService(IApiClient apiClient, IGeocodeTranslator geocodeTranslator)
        {
            _apiClient = apiClient;
            _geocodeTranslator = geocodeTranslator;
        }

        public async Task<List<string>> GetAllEpidemiologicalDataAsync()
        {
            var diseases = Diseases.GetDiseases();
            var geoCodes = GeoCodes.GetGeoCodes();

            var queryParamsList = CreateQueryParams(diseases, geoCodes);

            var tasks = queryParamsList
                .Select(queryParams => _apiClient.GetAsync($"{ApiUrl}?{queryParams}"));

            var responses = await Task.WhenAll(tasks);
            return responses.ToList();
        }

        public async Task<List<string>> GetEpidemiologicalDataByGeocodeAsync(string geoCode)
        {
            var diseases = Diseases.GetDiseases();

            var queryParamsList = diseases
                .Select(disease => QueryBuilder.Build(geoCode, disease, EwStart, EwEnd, EyStart, EyEnd));

            var tasks = queryParamsList
                .Select(queryParams => _apiClient.GetAsync($"{ApiUrl}?{queryParams}"));

            var responses = await Task.WhenAll(tasks);
            return responses.ToList();
        }

        public async Task<List<List<int>>> GetTotalCasesInRJandSPAsync()
        {
            var geoCodes = GeoCodes.GetGeoCodes();

            var tasks = geoCodes
                .Select(geoCode => GetTotalCasesByGeocodeAsync(geoCode));

            var totalCases = await Task.WhenAll(tasks);
            return totalCases.ToList();
        }

        public async Task<List<int>> GetTotalCasesByGeocodeAsync(string geoCode)
        {
            var diseases = Diseases.GetDiseases();

            var tasks = diseases
                .Select(async disease =>
                {
                    var queryParams = QueryBuilder.Build(geoCode, disease, EwStart, EwEnd, EyStart, EyEnd);
                    var data = await _apiClient.GetAsync($"{ApiUrl}?{queryParams}");
                    var result = ParseResult(data);
                    return result.totalCases;
                });

            var totalCases = await Task.WhenAll(tasks);
            return ((IEnumerable<dynamic>)totalCases).Select(x => (int)x).ToList();
        }

        public async Task<string> GetCustomEpidemiologicalDataAsync(string geoCode, string disease, int ewStart, int ewEnd, int eyStart, int eyEnd)
        {
            var queryParams = QueryBuilder.Build(geoCode, disease, ewStart, ewEnd, eyStart, eyEnd);
            var response = await _apiClient.GetAsync($"{ApiUrl}?{queryParams}");
            return response;
        }

        private dynamic ParseResult(string data)
        {
            return JsonConvert.DeserializeObject(data);
        }

        private IEnumerable<string> CreateQueryParams(IEnumerable<string> diseases, IEnumerable<string> geoCodes)
        {
            return from disease in diseases
                   from geoCode in geoCodes
                   select QueryBuilder.Build(geoCode, disease, EwStart, EwEnd, EyStart, EyEnd);
        }
    }
}

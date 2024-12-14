using InfoDengueReportAPI.Models;
using InfoDengueReportAPI.Services.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfoDengueReportAPI.Services
{
    public class EpidemiologicalDataService : IEpidemiologicalDataService
    {
        private readonly IApiClient _apiClient;
        private const string ApiUrl = "https://info.dengue.mat.br/api/alertcity";
        private const int EwStart = 1;
        private const int EwEnd = 52;
        private const int EyStart = 2023;
        private const int EyEnd = 2024;

        public EpidemiologicalDataService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Relatorio>> GetAllEpidemiologicalDataAsync()
        {
            var diseases = Diseases.GetDiseases();
            var geoCodes = GeoCodes.GetGeoCodes();
            var relatorios = new List<Relatorio>();

            foreach (var geoCode in geoCodes)
            {
                foreach (var disease in diseases)
                {
                    var queryParams = QueryBuilder.Build(geoCode, disease, EwStart, EwEnd, EyStart, EyEnd);
                    var data = await _apiClient.GetAsync($"{ApiUrl}?{queryParams}");
                    Console.WriteLine($"Response do método GetAllEpidemiologicalDataAsync - --- --- -- -- -- - -- {data}");
                    if (!string.IsNullOrEmpty(data))
                    {

                        var dadosEpidemiologicos = RelatorioParser.ParseEpidemiologicalData(data);
                        if (dadosEpidemiologicos != null)
                        {
                            var relatoriosParse = RelatorioParser.ConverterParaRelatorio(dadosEpidemiologicos, disease, geoCode, GeocodeTranslator.GetMunicipio(geoCode));
                            relatorios.AddRange(relatoriosParse);
                        }
                    }
                }
            }

            return relatorios;
        }


        public async Task<List<Relatorio>> GetEpidemiologicalDataByGeocodeAsync(string geoCode, string municipio)
        {
            var diseases = Diseases.GetDiseases();
            var relatorios = new List<Relatorio>();
            foreach (var disease in diseases)
            {
                var queryParams = QueryBuilder.Build(geoCode, disease, EwStart, EwEnd, EyStart, EyEnd);
                var data = await _apiClient.GetAsync($"{ApiUrl}?{queryParams}");

                 if (!string.IsNullOrEmpty(data))
                    {
                        var dadosEpidemiologicos = RelatorioParser.ParseEpidemiologicalData(data);
                        if (dadosEpidemiologicos != null)
                        {
                            var relatoriosDaDoenca = RelatorioParser.ConverterParaRelatorio(dadosEpidemiologicos, disease, geoCode, municipio);
                            relatorios.AddRange(relatoriosDaDoenca);
                        }
                    }
            }
            return relatorios;
        }

        public async Task<Dictionary<string, Dictionary<string, double>>> GetTotalCasesInRJandSPAsync()
        {
            var geoCodes = GeoCodes.GetGeoCodes();
            var totalCasesByMunicipality = new Dictionary<string, Dictionary<string, double>>();

            foreach (var geoCode in geoCodes)
            {
                foreach (var disease in Diseases.GetDiseases())
                {
                    var queryParams = QueryBuilder.Build(geoCode, disease, EwStart, EwEnd, EyStart, EyEnd);
                    var data = await _apiClient.GetAsync($"{ApiUrl}?{queryParams}");

                    if (!string.IsNullOrEmpty(data))
                    {
                        var dadosEpidemiologicos = RelatorioParser.ParseEpidemiologicalData(data);

                        if (dadosEpidemiologicos != null)
                        {
                            var municipio = GeocodeTranslator.GetMunicipio(geoCode);
                            var relatoriosDaDoenca = RelatorioParser.ConverterParaRelatorio(dadosEpidemiologicos, disease, geoCode, municipio);

                            double totalEstimatedCases = relatoriosDaDoenca.Sum(relatorio => relatorio.quantidadeDeCasosEstimados);

                            if (!totalCasesByMunicipality.ContainsKey(municipio))
                            {
                                totalCasesByMunicipality.Add(municipio, new Dictionary<string, double>());
                            }

                            if (totalCasesByMunicipality[municipio].ContainsKey(disease))
                            {
                                totalCasesByMunicipality[municipio][disease] += totalEstimatedCases;
                            }
                            else
                            {
                                totalCasesByMunicipality[municipio].Add(disease, totalEstimatedCases);
                            }
                        }
                    }
                }
            }

            return totalCasesByMunicipality;
        }

       public async Task<List<Relatorio>> GetCustomEpidemiologicalDataAsync(string geoCode, string disease, int ewStart, int ewEnd, int eyStart, int eyEnd, string municipio)
        {
            var queryParams = QueryBuilder.Build(geoCode, disease, ewStart, ewEnd, eyStart, eyEnd);
            var data = await _apiClient.GetAsync($"{ApiUrl}?{queryParams}");
            var relatorios = new List<Relatorio>();

             if (!string.IsNullOrEmpty(data))
                    {
                         var dadosEpidemiologicos = RelatorioParser.ParseEpidemiologicalData(data);
                         if (dadosEpidemiologicos != null)
                        {
                           var relatoriosDaDoenca = RelatorioParser.ConverterParaRelatorio(dadosEpidemiologicos, disease, geoCode, municipio);
                             relatorios.AddRange(relatoriosDaDoenca);
                        }

                    }

            return relatorios;
        }
    }
}
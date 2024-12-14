using InfoDengueReportAPI.Models;
using InfoDengueReportAPI.Services.Utils;
using Microsoft.EntityFrameworkCore;

namespace InfoDengueReportAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly DbContext _dbContext;
        private readonly IEpidemiologicalDataService _dataService;
        private readonly ILogger<ReportService> _logger;

        public ReportService(DbContext dbContext, IEpidemiologicalDataService dataService, ILogger<ReportService> logger) 
        {
            _dbContext = dbContext;
            _dataService = dataService;
            _logger = logger;
        }

        public async Task<Solicitante> SaveSolicitanteAsync(string nome, string cpf)
        {   
            _logger.LogInformation("Verificando se Solicitante já existe");
            var solicitante = await _dbContext.Set<Solicitante>()
                .FirstOrDefaultAsync(s => s.Cpf == cpf);

            if (solicitante == null)
            {
                solicitante = new Solicitante { Nome = nome, Cpf = cpf };
                await _dbContext.Set<Solicitante>().AddAsync(solicitante);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Solicitante '{nome}' cadastrado com CPF '{cpf}'.");
            }

            return solicitante;
        }

        public async Task<Relatorio> SaveRelatorioAsync(string descricao, string arbovirose, int semanaInicio, int semanaFim, string codigoIBGE, string municipio, int solicitanteId)
        {
            _logger.LogInformation("Salvando Relatório");
            var relatorio = new Relatorio
            {
                Descricao = descricao,
                Arbovirose = arbovirose,
                SemanaInicio = semanaInicio,
                SemanaFim = semanaFim,
                CodigoIBGE = codigoIBGE,
                Municipio = municipio,
                SolicitanteId = solicitanteId
            };

            await _dbContext.Set<Relatorio>().AddAsync(relatorio);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Relatório '{descricao}' salvo com SolicitanteID '{solicitanteId}'.");

            return relatorio;
        }

        public async Task<List<string>> GetRelatorioFromApiAsync(string geoCode)
        {
            _logger.LogInformation($"Buscando dados epidemiológicos para geocode '{geoCode}'");
            return await _dataService.GetEpidemiologicalDataByGeocodeAsync(geoCode);
        }

        public async Task<Relatorio> ProcessRelatorioAsync(string nome, string cpf, string tipoBusca, string? geoCode = null, string? disease = null, int? ewStart = null, int? ewEnd = null, int? eyStart = null, int? eyEnd = null)
        {
            var solicitante = await SaveSolicitanteAsync(nome, cpf);

            string descricao = "";
            List<string> rawData = new();

            switch (tipoBusca.ToLower())
            {
                case "geocode":
                    _logger.LogInformation($"Buscando relatório por geocode '{geoCode}");
                    if (geoCode == null) throw new ArgumentException("Geocode não fornecido para busca por geocode.");
                    rawData = await GetRelatorioFromApiAsync(geoCode);
                    descricao = $"Dados epidemiológicos para o município com geocode {geoCode}";
                    break;

                case "total-casos-rj-sp":
                    _logger.LogInformation("Buscando total de casos Rj e Sp");
                    var totalCasesRJSP = await _dataService.GetTotalCasesInRJandSPAsync();
                    rawData = totalCasesRJSP.SelectMany(tc => tc.Select(c => c.ToString())).ToList();
                    descricao = "Total de casos epidemiológicos nos municípios do Rio de Janeiro e São Paulo";
                    break;

                case "arbovirose":
                    _logger.LogInformation($"Buscando relatório customizado - gecode: '{geoCode} / arbovirose: '{disease}");
                    if (geoCode == null || disease == null) throw new ArgumentException("Geocode ou arbovirose não fornecidos para busca por arbovirose.");
                    rawData.Add(await _dataService.GetCustomEpidemiologicalDataAsync(geoCode, disease, ewStart ?? 1, ewEnd ?? 52, eyStart ?? 1980, eyEnd ?? 2024));
                    descricao = $"Dados epidemiológicos para arbovirose {disease} no município {geoCode} entre as semanas {ewStart} e {ewEnd} de {eyStart} a {eyEnd}";
                    break;

                case "municipios-rj-sp":
                    _logger.LogInformation("Buscando relatório total de dados de casos Rj e Sp");
                    var allDataRJSP = await _dataService.GetAllEpidemiologicalDataAsync();
                    rawData = allDataRJSP;
                    descricao = "Dados epidemiológicos completos para os municípios do Rio de Janeiro e São Paulo";
                    break;

                default:
                    throw new ArgumentException("Tipo de busca inválido.");
            }

            var relatorio = await SaveRelatorioAsync(
                descricao: descricao,
                arbovirose: disease ?? "",
                semanaInicio: ewStart ?? 0,
                semanaFim: ewEnd ?? 0,
                codigoIBGE: geoCode ?? "",
                municipio: geoCode != null ? GeocodeTranslator.GetMunicipio(geoCode) : "",
                solicitanteId: solicitante.Id
            );

            return relatorio;
        }

        public async Task<List<Solicitante>> GetSolicitantesAsync()
        {
            return await _dbContext.Set<Solicitante>().ToListAsync();
        }

        public async Task<List<Relatorio>> GetRelatoriosAsync()
        {
            return await _dbContext.Set<Relatorio>().ToListAsync();
        }
    }
}

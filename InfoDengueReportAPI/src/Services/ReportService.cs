using InfoDengueReportAPI.Models;
using Microsoft.EntityFrameworkCore;
using InfoDengueReportAPI.Data;
using InfoDengueReportAPI.Services.Utils;

namespace InfoDengueReportAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly InfoDengueContext _dbContext;
        private readonly IEpidemiologicalDataService _dataService;
        private readonly ILogger<ReportService> _logger;

        public ReportService(InfoDengueContext dbContext, IEpidemiologicalDataService dataService, ILogger<ReportService> logger)
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

        public async Task<Relatorio> SaveRelatorioAsync(Relatorio relatorio)
        {
            _logger.LogInformation("Salvando Relatório");

            await _dbContext.Set<Relatorio>().AddAsync(relatorio);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Relatório salvo com SolicitanteID '{relatorio.SolicitanteId}'.");

            return relatorio;
        }

        public async Task<List<Relatorio>> GetRelatorioFromApiAsync(string geoCode, string municipio)
        {
            _logger.LogInformation($"Buscando dados epidemiológicos para geocode '{geoCode}'");
            return await _dataService.GetEpidemiologicalDataByGeocodeAsync(geoCode, municipio);
        }

        public async Task<List<Relatorio>> ProcessRelatorioAsync(string nome, string cpf, string tipoBusca, string? geoCode = null, string? disease = null, int? ewStart = null, int? ewEnd = null, int? eyStart = null, int? eyEnd = null, string? municipio = null)
        {
            var solicitante = await SaveSolicitanteAsync(nome, cpf);
            List<Relatorio> relatorios = new List<Relatorio>();
            string descricao = "";

            switch (tipoBusca.ToLower())
            {
                case "geocode":
                    _logger.LogInformation($"Buscando relatório por geocode '{geoCode}'");
                    if (geoCode == null) throw new ArgumentException("Geocode ou municipio não fornecidos para busca por geocode.");
                    relatorios = await GetRelatorioFromApiAsync(geoCode, GeocodeTranslator.GetMunicipio(geoCode));
                    descricao = $"Dados epidemiológicos para o município {municipio} com geocode {geoCode}";

                    break;

                case "total-casos-rj-sp":
                    _logger.LogInformation("Buscando total de casos Rj e Sp por Arbovirose");
                    var totalCasesByMunicipalityAndDisease = await _dataService.GetTotalCasesInRJandSPAsync();

                    foreach (var municipioKvp in totalCasesByMunicipalityAndDisease)
                    {
                        municipio = municipioKvp.Key;
                        var totalCasesByDisease = municipioKvp.Value;

                        foreach (var diseaseKvp in totalCasesByDisease)
                        {
                            disease = diseaseKvp.Key;
                            var totalCases = diseaseKvp.Value;

                            relatorios.Add(new Relatorio
                            {
                                Municipio = municipio,
                                quantidadeDeCasosEstimados = totalCases,
                                Descricao = descricao,
                                SolicitanteId = solicitante.Id,
                                Arbovirose = disease, // Agora inclui a arbovirose
                                CodigoIBGE = GeocodeTranslator.GetGeoCode(municipio),
                                DataSolicitacao = DateTime.Now,
                                SemanaFim = 0,
                                SemanaInicio = 0
                            });
                        }
                    }
                    descricao = "Total de casos epidemiológicos nos municípios do Rio de Janeiro e São Paulo, discriminados por Arbovirose";
                    break;

                case "arbovirose":
                    _logger.LogInformation($"Buscando relatório customizado - arbovirose: '{disease}'");
                    if (disease == null) throw new ArgumentException("arbovirose não fornecido para busca por arbovirose.");

                    relatorios = await _dataService.GetCustomEpidemiologicalDataAsync(geoCode, disease, ewStart ?? 1, ewEnd ?? 52, eyStart ?? 2024, eyEnd ?? 2024, municipio);

                    if (relatorios != null && relatorios.Any())
                    {
                        var relatoriosAgrupados = relatorios
                        .GroupBy(r => new { r.Municipio, r.Arbovirose })
                        .Select(g => new TotalCasesByMunicipilatyDto
                        {
                            Municipio = g.Key.Municipio,
                            Doenca = g.Key.Arbovirose,
                            TotalCasos = g.Sum(r => r.quantidadeDeCasosEstimados)
                        })
                        .ToList();
        
                        relatorios = relatoriosAgrupados.Select(dto => new Relatorio
                        {
                            Municipio = dto.Municipio,
                            Arbovirose = dto.Doenca,
                            quantidadeDeCasosEstimados = dto.TotalCasos,
                            CodigoIBGE = GeocodeTranslator.GetGeoCode(dto.Municipio),
                            DataSolicitacao = DateTime.Now,
                            SemanaFim = 0,
                            SemanaInicio = 0
                        }).ToList();
                    }
                    else
                    {
                        return new List<Relatorio>();
                    }
                    descricao = $"Dados epidemiológicos agrupados por município e arbovirose para {disease}";
                    break;

                case "municipios-rj-sp":
                    _logger.LogInformation("Buscando relatório total de dados de casos Rj e Sp");
                    relatorios = await _dataService.GetAllEpidemiologicalDataAsync();
                    descricao = "Dados epidemiológicos completos para os municípios do Rio de Janeiro e São Paulo";
                    break;

                default:
                    throw new ArgumentException("Tipo de busca inválido.");
            }


            Relatorio? relatorioSalvo = null;
            foreach (var relatorio in relatorios)
            {
                relatorio.Descricao = descricao;
                relatorio.SolicitanteId = solicitante.Id;
                relatorioSalvo = await SaveRelatorioAsync(relatorio);
            }
           
            return relatorios;
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
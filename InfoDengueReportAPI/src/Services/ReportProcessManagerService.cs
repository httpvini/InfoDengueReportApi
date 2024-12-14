using InfoDengueReportAPI.Models;

namespace InfoDengueReportAPI.Services
{
    public class ReportProcessManagerService : IReportProcessManagerService
    {
        private readonly IReportService _relatorioService;

        public ReportProcessManagerService(IReportService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        public async Task<Relatorio> ProcessReportAsync(
            string nomeSolicitante, 
            string cpfSolicitante, 
            string tipoBusca, 
            string? geoCode = null, 
            string? disease = null, 
            int? ewStart = null, 
            int? ewEnd = null, 
            int? eyStart = null, 
            int? eyEnd = null)
        {
            try
            {

                var relatorio = await _relatorioService.ProcessRelatorioAsync(
                    nome: nomeSolicitante,
                    cpf: cpfSolicitante,
                    tipoBusca: tipoBusca,
                    geoCode: geoCode,
                    disease: disease,
                    ewStart: ewStart,
                    ewEnd: ewEnd,
                    eyStart: eyStart,
                    eyEnd: eyEnd
                );

                return relatorio;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Solicitante>> GetAllSolicitantesAsync()
        {
            try
            {
                Console.WriteLine("Buscando todos os solicitantes...");
                return await _relatorioService.GetSolicitantesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar solicitantes: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Relatorio>> GetAllRelatoriosAsync()
        {
            try
            {
                Console.WriteLine("Buscando todos os relatórios...");
                return await _relatorioService.GetRelatoriosAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar relatórios: {ex.Message}");
                throw;
            }
        }
    }
}

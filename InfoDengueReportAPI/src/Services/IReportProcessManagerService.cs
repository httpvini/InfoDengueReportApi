using InfoDengueReportAPI.Models;

namespace InfoDengueReportAPI.Services
{
    public interface IReportProcessManagerService
    {
        Task<List<Relatorio>> ProcessReportAsync(
            string nomeSolicitante, 
            string cpfSolicitante, 
            string tipoBusca, 
            string? geoCode = null, 
            string? disease = null, 
            int? ewStart = null, 
            int? ewEnd = null, 
            int? eyStart = null, 
            int? eyEnd = null);

        Task<List<Solicitante>> GetAllSolicitantesAsync();
        Task<List<Relatorio>> GetAllRelatoriosAsync();
    }
}

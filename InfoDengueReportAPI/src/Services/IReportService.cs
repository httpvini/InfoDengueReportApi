using InfoDengueReportAPI.Models;

namespace InfoDengueReportAPI.Services
{
    public interface IReportService
    {
        Task<Solicitante> SaveSolicitanteAsync(string nome, string cpf);
        Task<Relatorio> SaveRelatorioAsync(string descricao, string arbovirose, int semanaInicio, int semanaFim, string codigoIBGE, string municipio, int solicitanteId);
        Task<List<string>> GetRelatorioFromApiAsync(string geoCode);
        Task<Relatorio> ProcessRelatorioAsync(string nome, string cpf, string tipoBusca, string? geoCode = null, string? disease = null, int? ewStart = null, int? ewEnd = null, int? eyStart = null, int? eyEnd = null);
        Task<List<Solicitante>> GetSolicitantesAsync();
        Task<List<Relatorio>> GetRelatoriosAsync();
    }
}

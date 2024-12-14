using InfoDengueReportAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoDengueReportAPI.Services
{
    public interface IReportService
    {
        Task<Solicitante> SaveSolicitanteAsync(string nome, string cpf);
        Task<Relatorio> SaveRelatorioAsync(Relatorio relatorio);
        Task<List<Relatorio>> GetRelatorioFromApiAsync(string geoCode, string municipio);
        Task<List<Relatorio>> ProcessRelatorioAsync(string nome, string cpf, string tipoBusca, string? geoCode = null, string? disease = null, int? ewStart = null, int? ewEnd = null, int? eyStart = null, int? eyEnd = null, string? municipio = null);
        Task<List<Solicitante>> GetSolicitantesAsync();
        Task<List<Relatorio>> GetRelatoriosAsync();
    }
}
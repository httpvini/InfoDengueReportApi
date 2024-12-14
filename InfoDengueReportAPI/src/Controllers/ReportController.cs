using InfoDengueReportAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfoDengueReportAPI.Controllers
{
    [ApiController]
    [Route("api/report")]
    public class ReportController : ControllerBase
    {
        private readonly IReportProcessManagerService _reportProcessManagerService;

        public ReportController(IReportProcessManagerService reportProcessManagerService)
        {
            _reportProcessManagerService = reportProcessManagerService;
        }

        [HttpGet("epidemiological-data/rj-sp")]
        public async Task<IActionResult> GetEpidemiologicalDataForRJAndSP([FromQuery] string nomeSolicitante, [FromQuery] string cpfSolicitante)
        {
            try
            {
                var relatorio = await _reportProcessManagerService.ProcessReportAsync(
                    nomeSolicitante, 
                    cpfSolicitante, 
                    tipoBusca: "municipios-rj-sp"
                );
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar dados: {ex.Message}");
            }
        }

        [HttpGet("epidemiological-data/by-ibge-code")]
        public async Task<IActionResult> GetEpidemiologicalDataByIBGECode([FromQuery] string codigoIBGE, [FromQuery] string nomeSolicitante, [FromQuery] string cpfSolicitante)
        {
            try
            {
                var relatorio = await _reportProcessManagerService.ProcessReportAsync(
                    nomeSolicitante, 
                    cpfSolicitante, 
                    tipoBusca: "by-ibge-code", 
                    geoCode: codigoIBGE
                );
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar dados: {ex.Message}");
            }
        }

        [HttpGet("total-cases/rj-sp")]
        public async Task<IActionResult> GetTotalCasesForRJAndSP([FromQuery] string nomeSolicitante, [FromQuery] string cpfSolicitante)
        {
            try
            {
                var relatorio = await _reportProcessManagerService.ProcessReportAsync(
                    nomeSolicitante, 
                    cpfSolicitante, 
                    tipoBusca: "total-casos-rj-sp"
                );
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar dados: {ex.Message}");
            }
        }

        [HttpGet("total-cases/by-disease")]
        public async Task<IActionResult> GetTotalCasesByDisease([FromQuery] string arbovirose, [FromQuery] string nomeSolicitante, [FromQuery] string cpfSolicitante)
        {
            try
            {
                var relatorio = await _reportProcessManagerService.ProcessReportAsync(
                    nomeSolicitante, 
                    cpfSolicitante, 
                    tipoBusca: "total-casos-by-disease", 
                    disease: arbovirose
                );
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar dados: {ex.Message}");
            }
        }

        [HttpGet("solicitantes")]
        public async Task<IActionResult> GetSolicitantes()
        {
            try
            {
                var solicitantes = await _reportProcessManagerService.GetAllSolicitantesAsync();
                return Ok(solicitantes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar solicitantes: {ex.Message}");
            }
        }
    }
}

using InfoDengueReportAPI.Models;
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
                    tipoBusca: "geocode",
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
        public async Task<ActionResult<List<TotalCasesByMunicipilatyDto>>> GetTotalCasesForRJAndSP(
        [FromQuery] string nomeSolicitante, 
        [FromQuery] string cpfSolicitante)
        {
            try
            {
                var relatorios = await _reportProcessManagerService.ProcessReportAsync(
                    nomeSolicitante,
                    cpfSolicitante,
                    tipoBusca: "total-casos-rj-sp"
                );

                if (relatorios == null || relatorios.Count == 0)
                {
                    return NotFound("Nenhum relatório encontrado.");
                }

                var totalCasosPorMunicipioEDoenca = relatorios
                    .GroupBy(r => new { r.Municipio, r.Arbovirose })
                    .Select(g => new TotalCasesByMunicipilatyDto
                    {
                        Municipio = g.Key.Municipio,
                        Doenca = g.Key.Arbovirose,
                        TotalCasos = g.Sum(r => r.quantidadeDeCasosEstimados)
                    })
                    .ToList();

                return Ok(totalCasosPorMunicipioEDoenca);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [HttpGet("total-cases/by-arbovirose")]
        public async Task<ActionResult<List<TotalCasesByMunicipilatyDto>>> GetTotalCasesByArbovirose(
            [FromQuery] string arbovirose,
            [FromQuery] string nomeSolicitante,
            [FromQuery] string cpfSolicitante
        )
        {
            try
            {
                var relatorios = await _reportProcessManagerService.ProcessReportAsync(
                    nomeSolicitante,
                    cpfSolicitante,
                    tipoBusca: "arbovirose",
                    disease: arbovirose
                );
    
                if (relatorios == null || !relatorios.Any())
                {
                    return NotFound("Nenhum relatório encontrado.");
                }

                var relatoriosDto = relatorios.Select(r => new TotalCasesByMunicipilatyDto
                {
                    Municipio = r.Municipio,
                    Doenca = r.Arbovirose,
                    TotalCasos = r.quantidadeDeCasosEstimados
                }).ToList();

                return Ok(relatoriosDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor.");
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

using Microsoft.AspNetCore.Mvc;
using InfoDengueReportAPI.Models;

namespace InfoDengueReportAPI.Controllers
{
    [Route("api/solicitante")]
    [ApiController]
    public class RequesterController : ControllerBase
    {
        private static List<Solicitante> solicitantes = new List<Solicitante>();

        [HttpGet]
        public ActionResult<IEnumerable<Solicitante>> GetAll()
        {
            return Ok(solicitantes);
        }

        [HttpGet("{id}")]
        public ActionResult<Solicitante> GetById(int id)
        {
            var solicitante = solicitantes.FirstOrDefault(s => s.Id == id);
            if (solicitante == null)
                return NotFound();

            return Ok(solicitante);
        }

        [HttpPost]
        public ActionResult<Solicitante> Create([FromBody] Solicitante solicitante)
        {
            solicitante.Id = solicitantes.Count > 0 ? solicitantes.Max(s => s.Id) + 1 : 1;
            solicitantes.Add(solicitante);

            return CreatedAtAction(nameof(GetById), new { id = solicitante.Id }, solicitante);
        }

        [HttpPut("{id}")]
        public ActionResult<Solicitante> Update(int id, [FromBody] Solicitante solicitante)
        {
            var existingSolicitante = solicitantes.FirstOrDefault(s => s.Id == id);
            if (existingSolicitante == null)
                return NotFound();

            existingSolicitante.Nome = solicitante.Nome;
            existingSolicitante.Cpf = solicitante.Cpf;

            return Ok(existingSolicitante);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var solicitante = solicitantes.FirstOrDefault(s => s.Id == id);
            if (solicitante == null)
                return NotFound();

            solicitantes.Remove(solicitante);
            return NoContent();
        }
    }
}
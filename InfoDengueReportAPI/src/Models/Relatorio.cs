namespace InfoDengueReportAPI.Models
{
    public class Relatorio
    {
        internal string Descricao;

        public int Id { get; set; }
        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
        public string Arbovirose { get; set; }
        public int SemanaInicio { get; set; }
        public int SemanaFim { get; set; }
        public string CodigoIBGE { get; set; }
        public string Municipio { get; set; }
        public int SolicitanteId { get; set; }
        public Solicitante Solicitante { get; set; }
    }
}
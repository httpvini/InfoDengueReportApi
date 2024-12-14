namespace InfoDengueReportAPI.Models
{
    public class DadosEpidemiologicos
    {
        public long data_iniSE { get; set; }
        public int SE { get; set; }
        public double casos_est { get; set; }
        public int casos { get; set; }
        public double p_rt1 { get; set; }
        public double p_inc100k { get; set; }
        public int nivel { get; set; }
        public long id { get; set; }
        public double Rt { get; set; }
        public double pop { get; set; }
        public double? tempmin { get; set; } // Nullable para campos que podem ser nulos
        public double? umidmax { get; set; }
        public int receptivo { get; set; }
        public int transmissao { get; set; }
        public int nivel_inc { get; set; }
        public double? umidmed { get; set; }
        public double? umidmin { get; set; }
        public double? tempmed { get; set; }
        public double? tempmax { get; set; }
        public int notif_accum_year { get; set; }

    }
}
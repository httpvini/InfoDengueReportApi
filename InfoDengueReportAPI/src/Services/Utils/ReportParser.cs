using Newtonsoft.Json;
using System.Collections.Generic;
using InfoDengueReportAPI.Models;

namespace InfoDengueReportAPI.Models
{
    public class RelatorioParser
    {
        public static List<DadosEpidemiologicos> ParseEpidemiologicalData(string jsonResponse)
        {
            try
            {
                //Usando Newtonsoft.Json para desserializar direto para a lista.
                var dados = JsonConvert.DeserializeObject<List<DadosEpidemiologicos>>(jsonResponse);
                return dados;
            }
            catch (JsonReaderException ex)
            {
                // Tratar erros de parsing JSON
                Console.WriteLine($"Erro ao analisar JSON: {ex.Message}");
                return new List<DadosEpidemiologicos>(); // Retorna uma lista vazia em caso de erro
            }
        }

        public static List<Relatorio> ConverterParaRelatorio(List<DadosEpidemiologicos> dadosEpidemiologicos, string arbovirose, string codigoIBGE, string municipio)
        {
             var relatorios = new List<Relatorio>();
            foreach (var dado in dadosEpidemiologicos)
            {
                var relatorio = new Relatorio
                {
                    Arbovirose = arbovirose,
                    CodigoIBGE = codigoIBGE,
                    Municipio = municipio,
                    SemanaInicio = dado.SE,
                    SemanaFim = dado.SE,
                    quantidadeDeCasosEstimados = dado.casos_est,
                    notificacoes = dado.casos
                };
                relatorios.Add(relatorio);
            }
            return relatorios;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.IO;
using Npgsql;
using Npgsql.TypeHandlers;

namespace Guardian_Sascar
{
    class IntegraTecClass
    {

        class TDadosTerminal
        {
            public int TermCodigo;
            // Número do terminal
            public string NumeroTerminal;
            // Versão do terminal
            public int Versao;
            // Ativo para Gerenciamento de Risco
            public bool Ativo;
            // Ativo para posicionar no webservice
            public bool AtivoWS;
            // Data da ultima atualizacao de status
            public DateTime DataUltimaAtualizacao;
            // Código do veiculo
            public int TermOrasCodigo;

            public int UltimaVelocidade;
            public int UltimaRPM;
            public Int64 UltimaOdm;
        }        

        class TDadosVersao
        {
            public string Descricao_Versao;
            public string Versao;
            public int Tecnologia;
            public int Comunicacao;
            public int Satelite;
            public int GPRS;
            public string Risco;
            public string Logistico;
            public string Mensagem;
        }

        DataTable tbReferencia = new DataTable();
        private void CriatbReferencia()
        {
            tbReferencia.Columns.Add("Desc",typeof(string));
            tbReferencia.Columns.Add("Lat", typeof(double));
            tbReferencia.Columns.Add("Log", typeof(double));
        }

        NpgsqlConnection pgsqlConnection = null;
        string connString = null;

        private void CriaConexao(string usuarioDB, string senhaDB, string host, string porta, string nomeBD)
        {
            connString = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                                          usuarioDB, porta, usuarioDB, senhaDB, nomeBD);
            connString = connString + "Pooling = true; MinPoolSize = 1; MaxPoolSize = 30; ConnectionLifeTime = 20;";
            pgsqlConnection = new NpgsqlConnection(connString);
        }

        public void AbreConexao()
        {
            try
            {
                pgsqlConnection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao abrir conexão com o banco de dados! ",ex);
            }
        }

        public void FechaConexao() //.:: Fecha e destroi o objeto de conexão ::.
        {
            try
            {
                pgsqlConnection.Close();
                pgsqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao fechar conexão com o banco de dados! ", ex);
            }
        }
    }

}

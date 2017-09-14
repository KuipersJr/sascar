using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;

namespace Guardian_Sascar
{
    public class threadObtemPacotes
    {
        private RichTextBox rxLog;
        DataSet ds_veiculos = new DataSet();        
        DataTable dt_veiculos = new DataTable();

        private int hoursbetween(DateTime dtaIni, DateTime dtaFim )
        {
            TimeSpan tempo = dtaFim.Subtract(dtaIni);
            return tempo.Hours; 
        }

        public void addLog(string msg)        {           
 
            if (msg != string.Empty)
            {
                rxLog.AppendText(DateTime.Now.ToString("dd/MM/yy hh:mm:ss") + " -> " + msg + "\n");
                if (rxLog.Lines.Count() >= 1000)
                {
                    rxLog.Clear();
                }
                rxLog.Update();
            }
        }

        private string obtVeiculo(string usuariows, string senhaws, int idVeiculo)
        {
            addLog("Obtendo Veiculo...");

            srSascar.SasIntegraWS wsSascar = new srSascar.SasIntegraWSClient();
            srSascar.obterVeiculosRequest veiculosrq = new srSascar.obterVeiculosRequest();
            srSascar.obterVeiculosResponse veiculosrp = new srSascar.obterVeiculosResponse();

            veiculosrq.idVeiculo = idVeiculo;
            veiculosrq.quantidade = 1;
            veiculosrq.usuario = usuariows;
            veiculosrq.senha = senhaws;
            try
            {
                veiculosrp = wsSascar.obterVeiculos(veiculosrq);
                if (veiculosrp == null)
                {
                    return string.Empty; 
                }
                else
                if (veiculosrp.@return.Length > -1)
                {
                    string sPlaca = veiculosrp.@return[0].placa.ToString();
                    return sPlaca;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }

        private void obtemVeiculos(string usuariows, string senhaws)
        {
            addLog("Obtendo Veiculos...");    
            dt_veiculos.Columns.Add("idVeiculo", typeof(Int64));
            dt_veiculos.Columns.Add("placa", typeof(string));

            srSascar.SasIntegraWS wsSascar = new srSascar.SasIntegraWSClient();
            srSascar.obterVeiculosRequest veiculosrq = new srSascar.obterVeiculosRequest();
            srSascar.obterVeiculosResponse veiculosrp = new srSascar.obterVeiculosResponse();

            int idVeiculo = 0;
            int QtdeVeicul = 0;

            while (idVeiculo <= 0 || veiculosrq.idVeiculo != idVeiculo) 
            {              
                veiculosrq.idVeiculo = idVeiculo;
                veiculosrq.quantidade = 5000;
                veiculosrq.usuario = usuariows;
                veiculosrq.senha = senhaws;
                try
                {
                    veiculosrp = wsSascar.obterVeiculos(veiculosrq);   // tratar se não obter retorno 
                    if (veiculosrp == null)
                    {
                        return;
                    }
                    addLog("Veiculos carregados " + veiculosrp.@return.Length.ToString());
                    Application.DoEvents();
                    if (veiculosrp.@return.Length > -1)
                    {
                        idVeiculo = veiculosrp.@return[veiculosrp.@return.Length - 1].idVeiculo;
                        DataRow drDados;
                        for (int i = 0; i <= veiculosrp.@return.Length -1 ; i++)
                        {
                            if (obtemPlacaVeiculo(veiculosrp.@return[i].idVeiculo.ToString()) == string.Empty)
                            {
                                QtdeVeicul++; 
                                drDados = dt_veiculos.NewRow();
                                drDados["idVeiculo"] = veiculosrp.@return[i].idVeiculo;
                                drDados["placa"] = veiculosrp.@return[i].placa;
                                dt_veiculos.Rows.Add(drDados);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    addLog("Erro ao obtemVeiculos " + ex.Message);
                    return;
                }
            }
            addLog("Veiculos carregados " + QtdeVeicul.ToString());
        }

        private string obtemPlacaVeiculo(string sidVeiculo)
        {
            string idv = string.Empty;
            
            string sfind = string.Empty;
            sfind = "idVeiculo = '" + sidVeiculo + "'";
            try
            {
                DataRow[] idVeic = dt_veiculos.Select(sfind);
                if (idVeic.Length > 0)
                {
                    idv = idVeic[0].ItemArray[1].ToString();
                }
            }
            catch
            {
                idv = string.Empty;
            }                
            
            return idv;
        }

        public void obtemPosicoesJson(string usuariows, string senhaws)
        {
            int iDLL = 1;
            
            string dirpacote = "/SASCAR_PACOTES/SASCAR_D"; 
            string sPath = Directory.GetCurrentDirectory();

            if (!Directory.Exists(sPath + "/SASCAR_PACOTES"))
            {
                Directory.CreateDirectory(sPath + "/SASCAR_PACOTES");
            }

            addLog("Obtendo os Pacotes..."); 

            srSascar.SasIntegraWS wsSascar = new srSascar.SasIntegraWSClient();
            srSascar.obterPacotePosicoesJSONRequest posicoesJsonRq = new srSascar.obterPacotePosicoesJSONRequest();
            srSascar.obterPacotePosicoesJSONResponse posicoesJsonRp = new srSascar.obterPacotePosicoesJSONResponse();

            posicoesJsonRq.usuario = usuariows;
            posicoesJsonRq.senha = senhaws;
            posicoesJsonRq.quantidade = 5000;

            posicoesJsonRp = wsSascar.obterPacotePosicoesJSON(posicoesJsonRq);
            XmlDocument xmldoc = new XmlDocument();
            string xmlHeader = @"{'?xml': {'@version': '1.0','@encoding': 'UTF-8'},'PosicoesResponse': {'return': ";
            int qtdArq = 0;
            for (int i = 0; i <= posicoesJsonRp.@return.Length - 1; i++)
            {
                string sJson = string.Empty;                
                if (qtdArq == 0)
                {                    
                    sJson = xmlHeader + posicoesJsonRp.@return[i].ToString();
                    xmldoc = (XmlDocument)JsonConvert.DeserializeXmlNode(sJson);
                    XmlNode nPlaca = xmldoc.SelectSingleNode("//return/idVeiculo");
                    string sxml = xmldoc.InnerText;
                    //
                    string sPlaca = obtemPlacaVeiculo(nPlaca.InnerText);
                    if (sPlaca == string.Empty)
                    {
                        int id = Convert.ToInt32(nPlaca.InnerText);
                        sPlaca = obtVeiculo(usuariows, senhaws, id);
                    }
                    nPlaca.InnerText = sPlaca;
                }
                else
                {
                    sJson = @"{'return':" + posicoesJsonRp.@return[i].ToString();
                    XmlDocument xmldocI = (XmlDocument)JsonConvert.DeserializeXmlNode(sJson);
                    XmlNode nPlaca = xmldocI.SelectSingleNode("//return/idVeiculo");
                    string sPlaca = obtemPlacaVeiculo(nPlaca.InnerText);
                    
                    if (sPlaca == string.Empty)
                    {
                      int id = Convert.ToInt32(nPlaca.InnerText);
                      sPlaca = obtVeiculo(usuariows, senhaws, id);
                    }
                    nPlaca.InnerText = sPlaca;

                    XmlNodeList parantNode = xmldoc.GetElementsByTagName("PosicoesResponse");
                    XmlNode xmelement = xmldoc.CreateNode("element", "return", "");
                    xmelement.InnerXml = xmldocI.ChildNodes[0].InnerXml;
                    parantNode[0].AppendChild(xmelement);                                    

                    try
                    {
                        
                        addLog(xmldoc.OuterXml.ToString());
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        addLog(xmelement.InnerText + " " + ex.Message);
                    }
                }

                // tratar se não buscar placa
                
                qtdArq++;

                if (qtdArq == 10 || i == posicoesJsonRp.@return.Length -1)
                {
                    if (iDLL == 3) // mudar iDLL = quantidade de thread de consumo
                    {
                        iDLL = 0;
                    }
                    iDLL++;

                    //nome do arquivo 
                    string sPathFile = sPath + dirpacote + iDLL.ToString("000") + "_POSICAO_" + DateTime.Now.ToString("ddHHmmss") + ".xml";
                    TextWriter twriter = File.CreateText(sPathFile);
                    xmldoc.Save(twriter);
                    qtdArq = 0;
                }
                
            }
            addLog("Pacotes " + qtdArq.ToString() + " Recebidos"); 
            
        }

        public void obtemPosicoesXml()
        {
        }

        private volatile bool _stopThread;
        public void StopThread()
        {
            _stopThread = true;
        }

        public void Execute(string usuariotec, string senhatec,ref RichTextBox rtxtLog)
        {
            rxLog = rtxtLog;  
            rtxtLog = rxLog;
            DateTime tmVeiculos = DateTime.Now.AddHours(-1);
            while (!_stopThread)
            {
                if (hoursbetween(tmVeiculos, DateTime.Now) == 1) // EXECUTA A CADA UMA HORA
                {
                    obtemVeiculos(usuariotec, senhatec); 
                    tmVeiculos = DateTime.Now;
                }

                obtemPosicoesJson(usuariotec, senhatec);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace Guardian_Sascar
{   
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
            _FrmPrincipal = this;
        }

        public static Form _FrmPrincipal;

        public void atualizaLogBaixa(string message)
        {
            rtxtLogBaixa.AppendText(DateTime.Now.ToString("dd/MM/yy hh:mm:ss") + "-> " + message);
            if (rtxtLogBaixa.Lines.Count() >= 1000)
            {
                rtxtLogBaixa.Clear();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            threadObtemPacotes ws = new threadObtemPacotes();
            ws.Execute("buonny", "bu0nny", ref rtxtLogBaixa);


          
        //   ws.obtemVeiculos();

        //   string idVeiculo = ws.obtemPlacaVeiculo("14283");

//           MessageBox.Show(idVeiculo);
           
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {

        }
    }
}


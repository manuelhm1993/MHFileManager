using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MHFileManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.comboOpciones.SelectedIndex = 0;
        }

        private void SetDirectoryPath(Button btnClick)
        {
            // Esta sintaxis de using permite que un objeto que usa recursos del SO se libere con Dispose() automáticamente
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                // Lookup table o mapeo de objetos
                Dictionary<string, Dictionary<bool, Control>> lookup = new Dictionary<string, Dictionary<bool, Control>>
                {
                    { "btnOrigen", new Dictionary<bool, Control> { { false, txtOrigen } } },
                    { "btnDestino", new Dictionary<bool, Control> { { true, txtDestino } } }
                };


                dialogo.Description = "Selecciona la carpeta de origen";
                dialogo.ShowNewFolderButton = lookup[btnClick.Name].Keys.ElementAt(0);

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    lookup[btnClick.Name].Values.ElementAt(0).Text = dialogo.SelectedPath;
                }
            }
        }

        private void origen_Click(object sender, EventArgs e)
        {
            SetDirectoryPath((Button)sender);
        }

        private void btnDestino_Click(object sender, EventArgs e)
        {
            SetDirectoryPath((Button)sender);
        }
    }
}

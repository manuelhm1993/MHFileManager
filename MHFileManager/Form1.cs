using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        }

        #region Mis Métodos
        private void SetDirectoryPath(Button btnClick)
        {
            // Esta sintaxis de using permite que un objeto que usa recursos del SO se libere con Dispose() automáticamente
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                // Lookup table o mapeo de objetos
                Dictionary<string, Dictionary<bool, Control>> lookup = new Dictionary<string, Dictionary<bool, Control>>()
                {
                    { "btnOrigen", new Dictionary<bool, Control>() { { false, txtOrigen } } },
                    { "btnDestino", new Dictionary<bool, Control>() { { true, txtDestino } } }
                };


                dialogo.Description = $"Selecciona la carpeta de { (btnClick.Name.Equals("btnOrigen") ? "origen" : "destino") }";
                dialogo.ShowNewFolderButton = lookup[btnClick.Name].Keys.ElementAt(0);

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    TextBox input = (TextBox)lookup[btnClick.Name].Values.ElementAt(0);

                    input.Text = dialogo.SelectedPath;
                    input.Enabled = true;
                }
            }
        }

        private void SetDirectoryPath()
        {
            // Esta sintaxis de using permite que un objeto que usa recursos del SO se libere con Dispose() automáticamente
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Selecciona la carpeta de destino";
                dialogo.ShowNewFolderButton = true; // Esto permite crear nuevas carpetas

                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    txtDestino.Text = dialogo.SelectedPath;
                    txtDestino.Enabled = true;
                }
            }
        }

        private string getDirectorio(string[] files)
        {
            string path = files[0];

            for(int i = (path.Length - 1); i >=0; i--)
            {
                if (path[i] == '\\')
                {
                    path = path.Substring(0, i);
                    break;
                }
            }

            return path;
        }

        private void SetFilePath(bool multiSelect)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.Multiselect = multiSelect;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtOrigen.Text = multiSelect ? getDirectorio(dialog.FileNames) : dialog.FileName;
                    txtOrigen.Enabled = true;
                }
            }
        }

        private void Reset()
        {
            // Formulario
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Botones
            this.btnAceptar.Enabled = false;
            this.btnAceptar.Text = "Aceptar";

            // TextBox
            this.txtOrigen.Enabled = false;
            this.txtDestino.Enabled = false;

            this.txtOrigen.Text = "Seleccione un archivo o carpeta";
            this.txtDestino.Text = "Seleccione una carpeta";

            // Combos
            this.comboOpciones.SelectedIndex = 0;

            // Radios
            carpeta.Checked = true;
        }

        private void Copiar(string rutaArchivoOrigen, string rutaArchivoDestino)
        {
            // EjecutarAccion el archivo
            File.Copy(rutaArchivoOrigen, rutaArchivoDestino, true); // true para sobrescribir si existe
        }

        private void Mover(string rutaArchivoOrigen, string rutaArchivoDestino)
        {
            // EjecutarAccion el archivo
            Directory.Move(rutaArchivoOrigen, rutaArchivoDestino);
        }

        private void EjecutarAccion(string directorioOrigen, string directorioDestino, string accion)
        {
            // Crear el directorio de destino si no existe
            if (!Directory.Exists(directorioDestino))
            {
                Directory.CreateDirectory(directorioDestino);
            }

            // Obtener todos los archivos en el directorio de origen
            string[] archivos = Directory.GetFiles(directorioOrigen);

            // Obtener todos los subdirectorios en el directorio de origen
            string[] subdirectorios = Directory.GetDirectories(directorioOrigen);

            foreach (string archivo in archivos)
            {
                // Calcular la ruta completa del archivo de origen y destino
                string rutaArchivoOrigen = archivo;
                string rutaArchivoDestino = Path.Combine(directorioDestino, Path.GetFileName(archivo));

                try
                {
                    if (accion.Equals("Copiar"))
                    {
                        Copiar(rutaArchivoOrigen, rutaArchivoDestino);
                    }
                    else if (accion.Equals("Mover"))
                    {
                        Mover(rutaArchivoOrigen, rutaArchivoDestino);
                    }
                    else
                    {
                        return;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error procesando '{rutaArchivoOrigen}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            foreach (string subdirectorio in subdirectorios)
            {
                // Calcular la ruta completa del subdirectorio de origen y destino
                string rutaSubdirectorioOrigen = subdirectorio;
                string rutaSubdirectorioDestino = Path.Combine(directorioDestino, Path.GetFileName(subdirectorio));

                // EjecutarAccion el subdirectorio de forma recursiva
                EjecutarAccion(rutaSubdirectorioOrigen, rutaSubdirectorioDestino, accion);
            }

            MessageBox.Show("La acción fue completada exitosamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Reset();
        }
        #endregion Mis Métodos

        #region Eventos
        private void Form1_Load(object sender, EventArgs e)
        {
            Reset();
        }

        private void origen_Click(object sender, EventArgs e)
        {
            if (this.carpeta.Checked)
            {
                SetDirectoryPath((Button)sender);
            }
            else
            {
                SetFilePath(this.archivos.Checked);
            }
        }

        private void btnDestino_Click(object sender, EventArgs e)
        {
            SetDirectoryPath();
        }

        private void comboOpciones_SelectedIndexChanged(object sender, EventArgs e)
        {
            string texto = ((ComboBox)sender).SelectedItem.ToString();

            if (texto.Equals("Seleccione una opción"))
            {
                btnAceptar.Text = "Aceptar";
                btnAceptar.Enabled = false;
            }
            else
            {
                btnAceptar.Text = texto;
                btnAceptar.Enabled = (txtOrigen.Enabled && txtDestino.Enabled);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string accion = ((Button)sender).Text;
            string origen = txtOrigen.Text;
            string destino = txtDestino.Text;

            MessageBox.Show($"Acción: {accion}\nOrigen: {origen}\nDestino: {destino}", "Debug");

            EjecutarAccion(origen, destino, accion);
        }
        #endregion Eventos
    }
}

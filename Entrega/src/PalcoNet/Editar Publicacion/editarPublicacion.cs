﻿using PalcoNet.Editar_Publicacion.SectoresUtils;
using PalcoNet.Generar_Publicacion;
using PalcoNet.PublicacionesUtils;
using PalcoNet.Validadores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PalcoNet.Editar_Publicacion
{
    public partial class EditarPublicacion : Form
    {
        private Publicacion publicacion;
        private FuncionFormPublicacion funcion;
        public List<SectorNumerado> sectoresNumerados { get; set; }
        public List<SectorSinNumerar> sectoresSinNumerar { get; set; }

        public EditarPublicacion(FuncionFormPublicacion funcion)
        {
            InitializeComponent();
            this.funcion = funcion;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            foreach (TextBox t in TodosLosTextbox())
            {
                t.Text = "";
            }
        }

        private List<TextBox> TodosLosTextbox()
        {
            return this.Controls.OfType<TextBox>().ToList<TextBox>();
        }

        private void CargarGrados()
        {
            comboGrado.DropDownStyle = ComboBoxStyle.DropDownList;
            comboGrado.DataSource = Grados.Todos();
            comboGrado.ValueMember = "Id";
            comboGrado.DisplayMember = "Show";
        }

        private void CargarRubros()
        {
            comboRubro.DropDownStyle = ComboBoxStyle.DropDownList;
            comboRubro.DataSource = Rubro.Todos();
            comboRubro.ValueMember = "Id";
            comboRubro.DisplayMember = "Descripcion";
        }

        public void LlenateCon(Publicacion publicacion)
        {
            fechaEspectaculo.Value = publicacion.fechaPublicacion.GetValueOrDefault(Contexto.FechaActual);
            
            txtCiudad.Text = publicacion.ciudad;
            txtLocalidad.Text = publicacion.localidad;
            txtCalle.Text = publicacion.calle;
            txtNroCalle.Text = publicacion.nroCalle;
            txtCodPostal.Text = publicacion.codigoPostal;
            txtDescripción.Text = publicacion.descripcion;

            Console.WriteLine("Seteando rubros y grados");
            comboRubro.SelectedItem = publicacion.rubro;
            comboGrado.SelectedItem = publicacion.grado;

            this.CargarSectoresDe(publicacion.id);

            this.publicacion = publicacion;
        }

        public void CargarSectoresDe(long publicacionId)
        {
            this.sectoresSinNumerar = Sectores.FindSectoresSinNumerarByPublicacionId(publicacionId);
            this.sectoresNumerados = Sectores.FindSectoresNumeradosByPublicacionId(publicacionId);
            this.RefreshSectores();
        }

        public void CargarSectoresVacios()
        {
            this.sectoresNumerados = new List<SectorNumerado>();
            this.sectoresSinNumerar = new List<SectorSinNumerar>();
            this.RefreshSectores();
        }

        private void RefreshSectores(){
            var bindingSource = new BindingSource(this.sectoresNumerados, null);
            numeradosDataGrid.DataSource = bindingSource;
            numeradosDataGrid.Columns["numerado"].Visible = false;

            var bindingSource2 = new BindingSource(this.sectoresSinNumerar, null);
            noNumeradasDataGrid.DataSource = bindingSource2;
            noNumeradasDataGrid.Columns["numerado"].Visible = false;
        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                GuardarBorrador();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show("No se pudo guardar");
            }
        }

        private void LlenarPublicacion()
        {
            publicacion.fechaEspectaculo = fechaEspectaculo.Value;
            publicacion.grado = comboGrado.SelectedItem as Grado;
            publicacion.rubro = comboRubro.SelectedItem as Rubro;
            publicacion.ciudad = txtCiudad.Text;
            publicacion.localidad = txtLocalidad.Text;
            publicacion.calle = txtCalle.Text;
            publicacion.nroCalle = txtNroCalle.Text;
            publicacion.codigoPostal = txtCodPostal.Text;
            publicacion.descripcion = txtDescripción.Text;
        }

        private void GuardarBorrador()
        {
            publicacion = publicacion == null ? new Publicacion() : publicacion;
            publicacion.estado = new Borrador();
            LlenarPublicacion();
            funcion.GuardarBorrador(this, publicacion);
        }

        private void btnPublicar_Click(object sender, EventArgs e)
        {
            try
            {
                ValidarTodo();
                GuardarBorrador();
                DialogResult dialogResult = MessageBox.Show("¿Desea realizar esta publicación para varias fechas?",
                    "Publicación por lotes", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                    PublicarUnaVez();
                else
                {
                    var form = new PublicacionMultiFechaForm(this.SectoresPublicacion(), this.publicacion);
                    form.fechas.Add(this.fechaEspectaculo.Value);
                    form.ShowDialog();
                }
                Close();
            }
            catch (UserInputException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "\n" + ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show("Ha ocurrido un error");
            }
        }

        private void ValidarTodo()
        {
            foreach (TextBox t in TodosLosTextbox())
            {
                if (String.IsNullOrWhiteSpace(t.Text))
                    throw new UserInputException("Debe completar todos los campos");
            }
            if (new ValidadorNumerico().IsInvalid(txtNroCalle.Text))
                throw new UserInputException("El nro de calle debe ser un número");
            if (comboGrado.SelectedItem == null)
                throw new UserInputException("Debe seleccionar un grado para su publicación");
            if (comboRubro.SelectedItem == null)
                throw new UserInputException("Debe seleccionar un rubro para su publicación");
            if (fechaEspectaculo.Value < Contexto.FechaActual)
                throw new UserInputException("La fecha del espectáculo debe ser posterior a la actual");
        }

        private void PublicarUnaVez()
        {
            publicacion.Publicarse(this.SectoresPublicacion());
            Close();
            MessageBox.Show("Se ha publicado el evento");
        }

        private void EditarPublicacion_Load(object sender, EventArgs e)
        {
            CargarGrados();
            CargarRubros();
            funcion.Setup(this);
            fechaEspectaculo.MinDate = fechaEspectaculo.Value < Contexto.FechaActual ? fechaEspectaculo.Value : Contexto.FechaActual;
        }

        private void btnNuevoNumerado_Click(object sender, EventArgs e)
        {
            var form = new SectorNumeradoForm();
            var result = form.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.sectoresNumerados.Add(form.Sector);
                this.RefreshSectores();
            }
        }

        private void btnBorrarNumerado_Click(object sender, EventArgs e)
        {
            try
            {
                var sectorSeleccionado = (SectorNumerado)numeradosDataGrid.CurrentRow.DataBoundItem;
                this.sectoresNumerados.Remove(sectorSeleccionado);
                this.RefreshSectores();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No seleccionó un sector numerado");
            }
        }

        private void btnNuevoSinNumerar_Click(object sender, EventArgs e)
        {
            var form = new SectorSinNumerarForm();
            var result = form.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.sectoresSinNumerar.Add(form.Sector);
                this.RefreshSectores();
            }
        }

        private void btnBorrarSinNumerar_Click(object sender, EventArgs e)
        {
            try
            {
                var sectorSeleccionado = (SectorSinNumerar)noNumeradasDataGrid.CurrentRow.DataBoundItem;
                this.sectoresSinNumerar.Remove(sectorSeleccionado);
                this.RefreshSectores();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No se seleccionó un sector sin numerar");
            }
        }

        public void GuardarSectores()
        {
            this.SectoresPublicacion().ForEach(s => s.PersistiteParaUn(this.publicacion.GetIdEspectaculo()));
        }

        public List<Sector> SectoresPublicacion()
        {
            return this.sectoresNumerados.Concat<Sector>(this.sectoresSinNumerar).ToList();
        }
    }
}
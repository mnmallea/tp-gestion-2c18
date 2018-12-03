﻿using PalcoNet.PublicacionesUtils;
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
            fechaPublicacion.Text = publicacion.fechaPublicacion != null ? publicacion.fechaPublicacion.ToString() : "-";
            fechaVencimiento.Text = publicacion.fechaVencimiento != null ? publicacion.fechaVencimiento.ToString() : "-";
            fechaEspectaculo.Value = (DateTime)publicacion.fechaPublicacion;
            
            txtCiudad.Text = publicacion.ciudad;
            txtLocalidad.Text = publicacion.localidad;
            txtCalle.Text = publicacion.calle;
            txtNroCalle.Text = publicacion.nroCalle;
            txtCodPostal.Text = publicacion.codigoPostal;
            txtDescripción.Text = publicacion.descripcion;

            Console.WriteLine("Seteando rubros y grados");
            comboRubro.SelectedItem = publicacion.rubro;
            comboGrado.SelectedItem = publicacion.grado;

            var lista = new BindingList<Ubicacion>(publicacion.Ubicaciones);
            var bindingSource = new BindingSource(lista, null);

            this.ubicacionesDataGrid.DataSource = bindingSource;
            this.ubicacionesDataGrid.Columns["id"].Visible = false;

            this.publicacion = publicacion;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            publicacion.estado = new Borrador();
            Persistir();
        }

        private void Persistir()
        {
            funcion.Guardar(this, publicacion);
        }

        private void btnPublicar_Click(object sender, EventArgs e)
        {
            publicacion.estado = new Publicado();
            Persistir();
        }

        private void EditarPublicacion_Load(object sender, EventArgs e)
        {
            CargarGrados();
            CargarRubros();
            funcion.Setup(this);
        }
    }
}

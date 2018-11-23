﻿using PalcoNet.LoginUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PalcoNet
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void logIn_Load(object sender, EventArgs e)
        {

        }

        private void btnLimpiarLogIn_Click(object sender, EventArgs e)
        {
            username.Text = "";
            password.Text = "";
        }

        private void btnIngresarLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                var idUser = ValidadorLogin.ValidarLogin(username.Text, password.Text);
                Contexto.idUsuarioLogueado = idUser;
                Console.WriteLine("SE LOGUEO : " + idUser);
                var funcionalidades = Funcionalidades.findFuncionalidadesByUsuarioId(idUser);
                if (funcionalidades.Count == 1)
                {
                    throw new NotImplementedException("debería dejarte acceder a su funcion");
                }
                else
                {
                    var formSelector = new SelectorFuncionalidadForm();
                    formSelector.Funcionalidades = funcionalidades;
                    formSelector.Show();
                }
                
            }
            catch (ProcedureException ex)
            {
                MessageBox.Show(ex.GetSqlErrorMessage(), "Error al iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void crearUsuario_Click(object sender, EventArgs e)
        {
            new Registro_de_Usuario.CrearUsuarioForm().Show();
        }
    }
}
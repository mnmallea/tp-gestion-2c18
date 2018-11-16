﻿using PalcoNet.LoginUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalcoNet.Abm_Rol
{
    class Rol
    {
        public short id {get; set; }
        public List<Funcionalidad> funcionalidades {get; set; }
        public String nombre {get; set; }
        public Boolean habilitado {get; set; }

        public Rol(String nombre, List<Funcionalidad> funcionalidades) {
            this.nombre = nombre;
            this.funcionalidades = funcionalidades;
            this.habilitado = true;
        }

        private Rol() { }

        public static Rol traerDe(DataRow dr)
        {
            var rol = new Rol();
            rol.id = Convert.ToInt16(dr["id_rol"]);
            rol.nombre = dr["nombre"].ToString();
            rol.habilitado = Convert.ToBoolean(dr["habilitado"]);
            rol.funcionalidades = Funcionalidades.findFuncionalidadesByRolId(rol.id);
            return rol;
        }

        public override string ToString()
        {
            return String.Format("Rol({0}, {1}, {2})", id, nombre, habilitado);
        }

        public void eliminate(){
            DataBase.GetInstance().procedure("eliminar_rol", new Parametro("id_rol", id));
        }
    }
}

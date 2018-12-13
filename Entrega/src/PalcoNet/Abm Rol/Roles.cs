﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalcoNet.Abm_Rol
{
    public class Roles
    {
        public static List<Rol> TraerTodos(){
            String sql = "select * from COMPUMUNDOHIPERMEGARED.Rol";
            var dt = DataBase.GetInstance().Query(sql);
            var roles = new List<Rol>();
            foreach (DataRow dr in dt.Rows) {
                roles.Add(Rol.TraerDe(dr));
            }
            return roles;
        }

        public static Rol Cliente
        {
            get
            {
                return RolByName("CLIENTE");
            }
        }

        public static Rol Administrador
        {
            get
            {
                return RolByName("ADMINISTRADOR");
            }
        }

        public static Rol Empresa
        {
            get
            {
                return RolByName("EMPRESA");
            }
        }

        private static Rol RolByName(string p)
        {
            String sql = String.Format("select * from COMPUMUNDOHIPERMEGARED.Rol where nombre like '{0}'",p);
            var dt = DataBase.GetInstance().Query(sql);
            return Rol.TraerDe(dt.Rows[0]);
        }

    }
}
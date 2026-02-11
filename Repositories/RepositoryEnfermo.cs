using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace MvcCoreProceduresEF.Repositories
{
    #region STORES PROCEDURES
    //    create procedure SP_ALL_ENFERMOS
    //as
    //    select* from ENFERMO
    //go

    //create procedure SP_FIND_ENFERMO
    //(@inscripcion nvarchar(50))
    //as
    //    select* from ENFERMO where INSCRIPCION = @inscripcion
    //go

    //create procedure SP_DELETE_ENFERMO
    //(@inscripcion nvarchar(50))
    //as
    //    delete from ENFERMO where INSCRIPCION = @inscripcion
    //go
    //    create procedure SP_INSERT_ENFERMO(@apellido nvarchar(50),@direccion nvarchar(50),@fecha_nac datetime, @s nvarchar(50),@nss nvarchar(50)) 
    //as
    //    declare @inscripcion nvarchar(50)
    //    select @inscripcion = MAX(INSCRIPCION) + 1 from ENFERMO

    //    insert into ENFERMO(INSCRIPCION, APELLIDO, DIRECCION, FECHA_NAC, S, NSS) VALUES(@inscripcion, @apellido, @direccion, @fecha_nac, @s, @nss)
    //go
    #endregion
    public class RepositoryEnfermo
    {

        private EnfermosContext context;

        public RepositoryEnfermo(EnfermosContext context)
        {
            this.context = context;
        }

        public async Task<List<Enfermo>> GetEnfermosAsync()
        {
            //NECESITAMOS UN COMMAND
            //VAMOS A USAR USING

            using (DbCommand command = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_ALL_ENFERMOS";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sql;
                //ABRIMOS LA CONEXION A PARTIR DEL COMMAND
                await command.Connection.OpenAsync();
                //EJECUTAMOS NUESTRO READER
                DbDataReader reader = await command.ExecuteReaderAsync();
                //MAPEAR DATOS MANUALMENTE
                List<Enfermo> enfermos = new List<Enfermo>();
                while (await reader.ReadAsync())
                {
                    Enfermo enfermo = new Enfermo();
                    {
                        enfermo.Inscripcion = reader["INSCRIPCION"].ToString();
                        enfermo.Apellido = reader["APELLIDO"].ToString();
                        enfermo.Direccion = reader["DIRECCION"].ToString();
                        enfermo.FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString());
                        enfermo.Genero = reader["S"].ToString();
                        enfermo.Nss = reader["NSS"].ToString();
                    };
                    enfermos.Add(enfermo); 
                }
                await reader.CloseAsync();
                await command.Connection.CloseAsync();
                return enfermos;

            }
        }

        public async Task<Enfermo> FindEnfermoAsyc (string inscripcion)
        {
            //PARA  LLAMAR A UN PROCEDIMINETO QUE CONTIENE PARAMETROS
            //LA LLAMADA SE REALIZA MEDIANTE EL NOMBRE DEL PROCEDURE
            //Y CADA PARAMETRO A CONTINUCACION EN LA DECALRACION DEL SQL : SP_PROCEDURE @PAM1 , @PAM2
            string sql = "SP_FIND_ENFERMO @inscripcion";
            SqlParameter pamIns = new SqlParameter("@inscripcion",inscripcion);
            //SI LOS DATOS QUE DEVUELVE EL PROCEDURE ESTAN MAPEADOS CON UN MODEL , PODEMOS UTILIZAR UN METODO FROM
            //SQLRAW PARA RECUPERAR DIRECTAMENTE EL MODEL/S 
            //NO PODEMOS CONSULTAR Y EXTRAER A LA VEZ CON LINQ , SE DEBE REALIZAR SIEMPRE EN 2 PASOS
            var consulta = this.context.Enfermos.FromSqlRaw(sql, pamIns);
            //DEBEMOS UILIZAR AsNumerable() PARA EXTRAER LOS DATOS
            Enfermo enfermo = await consulta.ToAsyncEnumerable().FirstOrDefaultAsync();
            return enfermo;
            
        }

        public async Task DeleteEnfermoAsync(string inscripcion)
        {
            //PARA  LLAMAR A UN PROCEDIMINETO QUE CONTIENE PARAMETROS
            //LA LLAMADA SE REALIZA MEDIANTE EL NOMBRE DEL PROCEDURE
            //Y CADA PARAMETRO A CONTINUCACION EN LA DECALRACION DEL SQL : SP_PROCEDURE @PAM1 , @PAM2
            string sql = "SP_DELETE_ENFERMO";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            using(DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(pamIns);
                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }
        }
        public async Task DeleteEnfermoRawAsync(string inscripcion)
        {
            //PARA  LLAMAR A UN PROCEDIMINETO QUE CONTIENE PARAMETROS
            //LA LLAMADA SE REALIZA MEDIANTE EL NOMBRE DEL PROCEDURE
            //Y CADA PARAMETRO A CONTINUCACION EN LA DECALRACION DEL SQL : SP_PROCEDURE @PAM1 , @PAM2
            string sql = "SP_DELETE_ENFERMO @inscripcion";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamIns);
        }

        public async Task CreateEnfermoAsync(string apellido,string direccion,DateTime fecha_nac,string s,string nss)
        {
            string sql = "SP_INSERT_ENFERMO @apellido,@direccion,@fecha_nac,@s,@nss";
            SqlParameter pamApellido = new SqlParameter("@apellido", apellido);
            SqlParameter pamDireccion = new SqlParameter("@direccion", direccion);
            SqlParameter pamFechaNac = new SqlParameter("@fecha_nac", fecha_nac);
            SqlParameter pamS = new SqlParameter("@s", s);
            SqlParameter pamNss = new SqlParameter("@nss", nss);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamApellido, pamDireccion, pamFechaNac, pamS, pamNss);

        }
        


    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System;
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
    }
}

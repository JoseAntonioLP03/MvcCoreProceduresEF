using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Data;
using System.Data.Common;

namespace MvcCoreProceduresEF.Repositories
{
    public class RepositoryDoctor
    {
        private EnfermosContext context;

        public RepositoryDoctor(EnfermosContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> GetEspecialidadesAsync()
        {
            //NECESITAMOS UN COMMAND
            //VAMOS A USAR USING

            using (DbCommand command = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_FIND_ESPECIALIDADES";
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sql;
                //ABRIMOS LA CONEXION A PARTIR DEL COMMAND
                await command.Connection.OpenAsync();
                //EJECUTAMOS NUESTRO READER
                DbDataReader reader = await command.ExecuteReaderAsync();
                //MAPEAR DATOS MANUALMENTE
                List<string> especialidades = new List<string>();
                while (await reader.ReadAsync())
                {
                    
                    string especialidad =reader["ESPECIALIDAD"].ToString();

                    especialidades.Add(especialidad);
                    
                }
                await reader.CloseAsync();
                await command.Connection.CloseAsync();
                return especialidades;

            }
        }

        public async Task IncrementarSalarioRawAsync(string especialidad , int incremento)
        {
            string sql = "SP_INCREMENTAR_SALARIO @especialidad , @incremento";
            SqlParameter pamEspecialidad = new SqlParameter("@especialidad", especialidad);
            SqlParameter pamIncremento = new SqlParameter("@incremento", incremento);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamEspecialidad, pamIncremento);

        }
        public async Task IncrementarSalarioEntityFrameWorkAsync(string especialidad, int incremento)
        {
            var consulta= from datos in this.context.Doctores
                                    where datos.Especialidad == especialidad
                                    select datos;
            List<Doctor> doctores = await consulta.ToListAsync();
            foreach (Doctor doc in doctores)
            {
                doc.Salario = doc.Salario + incremento;
            }
            await this.context.SaveChangesAsync();
         }

        public async Task<List<Doctor>> MostrarDoctoresEspecialidadAsync(string especialidad)
        {

            using (DbCommand command = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_DOCTORES_POR_ESPECIALIDAD";
                SqlParameter pamEspecialidad = new SqlParameter("@especialidad", especialidad);
                command.Parameters.Add(pamEspecialidad);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = sql;
                await command.Connection.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();
                List<Doctor> doctores = new List<Doctor>();
                while (await reader.ReadAsync())
                {
                    Doctor doctor = new Doctor()
                    {
                        Hospital_Cod = int.Parse(reader["HOSPITAL_COD"].ToString()),
                        Doctor_No = int.Parse(reader["DOCTOR_NO"].ToString()),
                        Apellido = reader["APELLIDO"].ToString(),
                        Salario = int.Parse(reader["SALARIO"].ToString()),
                        Especialidad = reader["ESPECIALIDAD"].ToString()
                    };
                    doctores.Add(doctor);
                }
                await reader.CloseAsync();
                await command.Connection.CloseAsync();
                return doctores;

            }
        }
    }
}

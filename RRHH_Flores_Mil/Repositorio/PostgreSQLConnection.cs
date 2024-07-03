using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RRHH_Flores_Mil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RRHH_Flores_Mil.Repositorio
{
    public class PostgreSQLConnection
    {
        private readonly string _connectionString = "Host=ruby.db.elephantsql.com;Username=uhgqrqkk;Password=TPXLD7XduKMp3E47_gUO-7-PdHVYLAbh;Database=uhgqrqkk";

        public async Task<List<User>> Login(string usr, string pwd)
        {
            var list = new List<User>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("select u.usuario, u.password, u.rol, u.email, concat(u.nombres, ' ', u.apellidos), u.codigo, u.fecha_contrato, u.sucursal, u.dias_pendientes_vacaciones " +
                    "from usuario u where u.usuario = @usr and u.password = @pwd", conn))
                {
                    cmd.Parameters.AddWithValue("@usr", usr);
                    cmd.Parameters.AddWithValue("@pwd", pwd);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new User
                            {
                                Usuario = reader.GetString(0),
                                Rol = reader.GetString(2),
                                Contraseña = reader.GetString(1),
                                Email = reader.GetString(3),
                                NombreCompleto = reader.GetString(4),
                                Codigo = reader.GetInt32(5),
                                FechaContrato = reader.GetDateTime(6),
                                Sucursal = reader.GetString(7),
                                DiasPendientes = reader.GetInt32(8)
                            });
                        }                            
                    }
                    await conn.CloseAsync();
                }
            }
            return list;
        }

        public async Task<List<string[]>> ConsultarJefeSucursal(string usr, string pwd)
        {
            var list = new List<User>();
            List<int> codigoJefes = new List<int>();
            List<string[]> listOfArrays = new List<string[]>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("select sv.jefe_sucursal from usuario u " +
                    "inner join solicitud_vacaciones sv ON u.codigo = sv.usuario " +
                    "where u.usuario = @usr and u.password = @pwd", conn))
                {
                    cmd.Parameters.AddWithValue("@usr", usr);
                    cmd.Parameters.AddWithValue("@pwd", pwd);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            codigoJefes.Add(reader.GetInt16(0));
                        }
                    }
                }

                if (codigoJefes.Count > 0)
                {
                    // Crear una lista de parámetros
                    var parameterNames = new List<string>();
                    for (int i = 0; i < codigoJefes.Count; i++)
                    {
                        parameterNames.Add($"@id{codigoJefes[i]}");
                    }
                    var inClause = string.Join(",", parameterNames);
                    var query = $"select u.codigo, concat(u.nombres,' ', u.apellidos) from usuario u where u.codigo IN ({inClause})";
                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        // Asignar valores a los parámetros
                        for (int i = 0; i < codigoJefes.Count; i++)
                        {
                            command.Parameters.AddWithValue(parameterNames[i], codigoJefes[i]);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                listOfArrays.Add(new string[] { reader.GetInt32(0).ToString(), reader.GetString(1) });
                            }
                        }
                    }
                    
                }
                await conn.CloseAsync();
            }
            return listOfArrays;
        }

        public async Task<List<AprobarVacaciones>> DetallesEmpleadoVacaciones(string estado)
        {
            var list = new List<AprobarVacaciones>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("" +
                    "select sv.codigo, sv.usuario, sv.jefe_sucursal, sv.fecha_inicio, sv.fecha_fin, sv.fecha_reingreso, " +
                    "sv.estado_solicitud, sv.observaciones, concat(u.nombres, ' ', u.apellidos) " +
                    "from solicitud_vacaciones sv inner join usuario u " +
                    "on sv.usuario = u.codigo " +
                    "where sv.estado_solicitud = 'pendiente'", conn))
                {
                    cmd.Parameters.AddWithValue("@estadoSolicitud", estado);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new AprobarVacaciones
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(8),
                                FechaInicio = reader.GetDateTime(3),
                                FechaFin = reader.GetDateTime(4),
                                Aprobado = reader.GetString(6).Equals("aprobado") ? true : false,
                                Observacion = reader.GetString(7)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public async Task<List<string>> ConsultarEmailJefe(int codigo)
        {
            List<string> email = new List<string>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("" +
                    "select u.email, concat(u.nombres, ' ', u.apellidos) from usuario u where u.codigo = @codigo", conn))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            email.Add(reader.GetString(0));
                            email.Add(reader.GetString(1));
                        }
                    }
                }

                using (var cmd = new NpgsqlCommand("" +
                    "select * from utils u", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            email.Add(reader.GetString(0));
                        }
                    }                    
                }
                await conn.CloseAsync();
            }
            return email;
        }

        public async Task<string> InsertarSolicitud(int codigo, int cjefe, DateTime fechaInicio, DateTime fechaFin,
            DateTime fechaReingreso, int dias, string observacion, string tipo)
        {
            string Estado = string.Empty;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("" +
                    "INSERT INTO solicitud_vacaciones (estado, usuario, jefe_sucursal, fecha_inicio, fecha_fin, fecha_reingreso, dias_tomados, estado_solicitud, observaciones, aceptacion_uso_datos_personales, tipo) " +
                    "VALUES('0', @usuario,@jefe, @fechaInicio, @fechaFin,@fechaReingreso, @dias, 'pendiente',@observacion, @auP, @tipo)", conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", codigo);
                    cmd.Parameters.AddWithValue("@jefe", cjefe);
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin.Date);
                    cmd.Parameters.AddWithValue("@fechaReingreso", fechaReingreso.Date);
                    cmd.Parameters.AddWithValue("@dias", dias);
                    cmd.Parameters.AddWithValue("@observacion", observacion);
                    cmd.Parameters.AddWithValue("@auP", true);
                    cmd.Parameters.AddWithValue("@tipo", tipo);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Estado = reader.GetString(0);
                        }
                    }
                    await conn.CloseAsync();
                }                
            }
            return Estado;
        }

        public async Task<List<string>> AprobarRechazarSolicitud(int codigo, string decision, string observacion)
        {
            List<string> email = new List<string>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();                
                using (var cmd = new NpgsqlCommand("" +
                    "update solicitud_vacaciones set estado_solicitud = @decision, observaciones = @observacion where codigo = @codigo", conn))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@observacion", observacion);
                    cmd.Parameters.AddWithValue("@decision", decision);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            email.Add(reader.GetString(0));
                            email.Add(reader.GetString(1));
                        }
                    }
                }
                await conn.CloseAsync();
            }
            return email;
        }

        public async Task<List<string>> SolicitudAproEmpl(int codigo)
        {
            List<string> datosSolicitud = new List<string>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("" +
                    "select concat(u.nombres, ' ', u.apellidos), u.email, sv.fecha_inicio , sv.fecha_fin, sv.tipo  " +
                    "from solicitud_vacaciones sv inner join usuario u ON (sv.usuario = u.codigo) " +
                    "where sv.codigo  = @codigo", conn))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            datosSolicitud.Add(reader.GetString(0));
                            datosSolicitud.Add(reader.GetString(1));
                            datosSolicitud.Add(reader.GetDateTime(2).ToString("yyyy-MM-dd"));
                            datosSolicitud.Add(reader.GetDateTime(3).ToString("yyyy-MM-dd"));
                            datosSolicitud.Add(reader.GetString(4));
                        }
                    }
                }

                using (var cmd = new NpgsqlCommand("" +
                    "select concat(u.nombres, ' ', u.apellidos), u.email " +
                    "from solicitud_vacaciones sv inner join usuario u ON (sv.jefe_sucursal = u.codigo)" +
                    "where sv.codigo  = @codigo", conn))
                {
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            datosSolicitud.Add(reader.GetString(0));
                            datosSolicitud.Add(reader.GetString(1));
                        }
                    }
                    await conn.CloseAsync();
                }
            }
            return datosSolicitud;
        }

        public async Task<List<string>> SolicitudAproRRH()
        {
            List<string> datosSolicitud = new List<string>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("select concat(u.nombres, ' ', u.apellidos), u.email from usuario u where rol like 'Gerente RRHH'", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            datosSolicitud.Add(reader.GetString(0));
                            datosSolicitud.Add(reader.GetString(1));
                        }
                    }
                }
            }
            return datosSolicitud;
        }

    }
}

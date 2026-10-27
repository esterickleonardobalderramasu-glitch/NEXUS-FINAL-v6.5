using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace NexusVideojuegoEngine
{
    public class GestorBD
    {
        // Conexión directa a SQL Server Express
        private readonly string connectionString = 
            @"Server=.\SQLEXPRESS;Database=NexusV65DB;Integrated Security=True;TrustServerCertificate=True;";

        /// <summary>
        /// Comprueba la conexión activa con SQL Server.
        /// </summary>
        public bool ProbarConexion()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de conexión BD: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Persistencia transaccional ACID en SQL Server.
        /// </summary>
        public bool GuardarPartidaTransaccional(Cadete cadete, int energiaCore)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string queryCadete = @"
                        IF EXISTS (SELECT 1 FROM Cadetes WHERE CadeteID = @ID)
                            UPDATE Cadetes SET Vida = @Vida, XP = @XP WHERE CadeteID = @ID;
                        ELSE
                            INSERT INTO Cadetes (CadeteID, Nombre, Rango, Vida, VidaMax, XP)
                            VALUES (@ID, @Nombre, @Rango, @Vida, @VidaMax, @XP);";

                    using (SqlCommand cmdCadete = new SqlCommand(queryCadete, connection, transaction))
                    {
                        cmdCadete.Parameters.AddWithValue("@ID", cadete.CadeteID);
                        cmdCadete.Parameters.AddWithValue("@Nombre", cadete.Nombre);
                        cmdCadete.Parameters.AddWithValue("@Rango", cadete.Rango);
                        cmdCadete.Parameters.AddWithValue("@Vida", cadete.Vida);
                        cmdCadete.Parameters.AddWithValue("@VidaMax", cadete.VidaMax);
                        cmdCadete.Parameters.AddWithValue("@XP", cadete.XP);
                        cmdCadete.ExecuteNonQuery();
                    }

                    string queryBitacora = @"
                        INSERT INTO BitacoraEventos (FechaHora, Descripcion, NivelEnergia)
                        VALUES (@Fecha, @Desc, @Energia);";

                    using (SqlCommand cmdBitacora = new SqlCommand(queryBitacora, connection, transaction))
                    {
                        cmdBitacora.Parameters.AddWithValue("@Fecha", DateTime.Now);
                        cmdBitacora.Parameters.AddWithValue("@Desc", $"Partida guardada para el cadete {cadete.Nombre}");
                        cmdBitacora.Parameters.AddWithValue("@Energia", energiaCore);
                        cmdBitacora.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error en transacción SQL: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
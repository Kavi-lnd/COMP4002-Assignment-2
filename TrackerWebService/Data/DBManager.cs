using System.Data.SQLite;
using TrackerWebService.Models;

namespace TrackerWebService.Data
{
    public class DBManager
    {
        private static readonly string ConnectionString =
            "Data Source=tracker.db;Version=3;";

        public static void Initialize()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Peers (
                    PeerId TEXT PRIMARY KEY,
                    IPAddress TEXT NOT NULL,
                    WcfPort INTEGER NOT NULL,
                    LastHeartbeat TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS CompletedJobs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PeerId TEXT NOT NULL,
                    JobId TEXT NOT NULL,
                    CompletedAt TEXT NOT NULL,
                    FOREIGN KEY (PeerId) REFERENCES Peers(PeerId)
                );";

            command.ExecuteNonQuery();
        }


        public static bool RegisterPeer(
            string peerId,
            string ipAddress,
            int wcfPort,
            string lastHeartbeat)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT INTO Peers
            (PeerId, IPAddress, WcfPort, LastHeartbeat)
        VALUES
            (@PeerId, @IPAddress, @WcfPort, @LastHeartbeat);";

            command.Parameters.AddWithValue("@PeerId", peerId);
            command.Parameters.AddWithValue("@IPAddress", ipAddress);
            command.Parameters.AddWithValue("@WcfPort", wcfPort);
            command.Parameters.AddWithValue("@LastHeartbeat", lastHeartbeat);

            return command.ExecuteNonQuery() == 1;
        }


        public static List<Peer> GetAllPeers()
        {
            var peers = new List<Peer>();

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT PeerId, IPAddress, WcfPort, LastHeartbeat
        FROM Peers;";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                peers.Add(new Peer
                {
                    PeerId = reader.GetString(0),
                    IPAddress = reader.GetString(1),
                    WcfPort = reader.GetInt32(2),
                    LastHeartbeat = reader.GetString(3)
                });
            }

            return peers;
        }



        public static bool UpdateHeartbeat(string peerId, string lastHeartbeat)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
        UPDATE Peers
        SET LastHeartbeat = @LastHeartbeat
        WHERE PeerId = @PeerId;";

            command.Parameters.AddWithValue("@LastHeartbeat", lastHeartbeat);
            command.Parameters.AddWithValue("@PeerId", peerId);

            return command.ExecuteNonQuery() == 1;
        }


        public static int RemoveStalePeers()
        {
            string cutoff = DateTime.UtcNow.AddSeconds(-30).ToString("O");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
        DELETE FROM Peers
        WHERE LastHeartbeat < @Cutoff;";

            command.Parameters.AddWithValue("@Cutoff", cutoff);

            return command.ExecuteNonQuery();
        }


        public static bool RecordCompletedJob(string peerId, string jobId)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT INTO CompletedJobs (PeerId, JobId, CompletedAt)
        VALUES (@PeerId, @JobId, @CompletedAt);";

            command.Parameters.AddWithValue("@PeerId", peerId);
            command.Parameters.AddWithValue("@JobId", jobId);
            command.Parameters.AddWithValue(
                "@CompletedAt", DateTime.UtcNow.ToString("O"));

            return command.ExecuteNonQuery() == 1;
        }


        public static int GetCompletedJobCount(string peerId)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT COUNT(*)
        FROM CompletedJobs
        WHERE PeerId = @PeerId;";

            command.Parameters.AddWithValue("@PeerId", peerId);

            return Convert.ToInt32(command.ExecuteScalar());
        }



        public static List<object> GetPeersWithJobCounts()
        {
            var results = new List<object>();

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT p.PeerId, p.IPAddress, p.WcfPort,
                       p.LastHeartbeat, COUNT(c.Id) AS JobCount
                FROM Peers p
                LEFT JOIN CompletedJobs c ON p.PeerId = c.PeerId
                GROUP BY p.PeerId, p.IPAddress, p.WcfPort,
                         p.LastHeartbeat;";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                results.Add(new
                {
                    PeerId = reader.GetString(0),
                    IPAddress = reader.GetString(1),
                    WcfPort = reader.GetInt32(2),
                    LastHeartbeat = reader.GetString(3),
                    CompletedJobCount = reader.GetInt32(4)
                });
            }

            return results;
        }

    }
}

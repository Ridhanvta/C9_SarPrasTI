using System;
using System.Data.SqlClient;
using SatprasDesktopApp.Config;
namespace Script {
    class Program {
        static void Main() {
            try {
                using(var conn = DatabaseConfig.GetConnection()) {
                    if (conn == null) { Console.WriteLine("No conn"); return; }
                    var cmd = new SqlCommand("SELECT top 1 * FROM [dbo].[vwEvaluasiHabisPakai]", conn);
                    using(var r = cmd.ExecuteReader()){
                        for(int i=0; i<r.FieldCount; i++) {
                            Console.WriteLine(r.GetName(i));
                        }
                    }
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}

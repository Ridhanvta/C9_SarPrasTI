using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SatprasDesktopApp.Config
{
    public static class DAL
    {
        // 1. URUSAN KONEKSI (Connection Management)
        
        public static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "localhost"; // Fallback jika tidak ditemukan IP v4
        }

        private static string PromptForServerIP()
        {
            string newIp = "";
            using (Form prompt = new Form())
            {
                prompt.Width = 400;
                prompt.Height = 220;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Koneksi Database Gagal";
                prompt.StartPosition = FormStartPosition.CenterScreen;
                prompt.MaximizeBox = false;

                Label textLabel = new Label() { Left = 20, Top = 20, Width = 350, Height = 40, Text = "Gagal terhubung ke Database.\nSilakan masukkan IP Address Laptop Server:" };
                TextBox inputBox = new TextBox() { Left = 20, Top = 70, Width = 340 };
                Button confirmation = new Button() { Text = "Simpan & Konek", Left = 260, Width = 100, Top = 120, DialogResult = DialogResult.OK };
                
                confirmation.Click += (sender, e) => { prompt.Close(); };
                
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(confirmation);
                prompt.AcceptButton = confirmation;

                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    newIp = inputBox.Text.Trim();
                }
            }
            return newIp;
        }

        private static string GetStoredIP()
        {
            string path = System.IO.Path.Combine(Application.UserAppDataPath, "server_ip.txt");
            if (System.IO.File.Exists(path))
            {
                return System.IO.File.ReadAllText(path).Trim();
            }
            return GetLocalIPAddress(); // Default ke laptop sendiri
        }

        private static void SaveStoredIP(string ipAddress)
        {
            string path = System.IO.Path.Combine(Application.UserAppDataPath, "server_ip.txt");
            System.IO.File.WriteAllText(path, ipAddress);
        }

        public static string GetDynamicConnectionString(string ipAddress, int timeout = 15)
        {
            // Tidak menggunakan App.config sama sekali.
            if (ipAddress == "localhost" || ipAddress == "127.0.0.1" || ipAddress == ".")
            {
                // Koneksi Lokal (Laptop Server): Gunakan Windows Authentication & Shared Memory (sangat stabil)
                return $"Server=.\\SQLEXPRESS;Database=satprasDB_new;Trusted_Connection=True;Connection Timeout={timeout};";
            }
            else
            {
                // Koneksi Jaringan (Laptop Client / UTM Mac): 
                // Wajib menggunakan SQL Server Authentication agar terbebas dari penolakan Windows Authentication beda mesin.
                // Gunakan format IP,1433 untuk memotong keharusan adanya SQL Browser.
                return $"Server={ipAddress},1433;Database=satprasDB_new;User Id=client_sarpras;Password=Client123!;Connection Timeout={timeout};";
            }
        }

        public static SqlConnection GetConnection()
        {
            // 1. Coba konek ke titik (.) (Laptop Server) terlebih dahulu via Shared Memory.
            // Jika berhasil, berarti aplikasi sedang dijalankan di laptop pembuatnya.
            try
            {
                string localConnString = GetDynamicConnectionString(".", 3);
                SqlConnection localConn = new SqlConnection(localConnString);
                localConn.Open();
                return localConn;
            }
            catch
            {
                // Gagal konek ke localhost. Berarti ini kemungkinan dijalankan di laptop Klien (teman Anda).
            }

            // 2. Baca IP yang tersimpan untuk Klien
            string currentIp = GetStoredIP();
            
            try
            {
                string connString = GetDynamicConnectionString(currentIp);
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();
                return conn;
            }
            catch (Exception)
            {
                // Jika gagal juga, munculkan form pop-up untuk meminta IP Server
                string newIp = PromptForServerIP();
                if (!string.IsNullOrEmpty(newIp))
                {
                    SaveStoredIP(newIp);
                    try
                    {
                        string newConnString = GetDynamicConnectionString(newIp);
                        SqlConnection newConn = new SqlConnection(newConnString);
                        newConn.Open();
                        return newConn;
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show("Masih gagal terhubung ke " + newIp + ".\nPastikan Firewall Server mengizinkan Port 1433 dan SQL Service menyala.\n\nDetail: " + ex2.Message, "Koneksi Terputus", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                return null;
            }
        }

        // 2. HELPER UNIVERSAL UNTUK MEMBANTU EKSEKUSI
        private static void AttachParameters(SqlCommand cmd, SqlParameter[] parameters)
        {
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
        }

        public static DataTable ExecuteQuery(string queryOrSp, SqlParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            using (SqlConnection conn = GetConnection())
            {
                if (conn == null) return null;
                using (SqlCommand cmd = new SqlCommand(queryOrSp, conn))
                {
                    cmd.CommandType = cmdType;
                    AttachParameters(cmd, parameters);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string queryOrSp, SqlParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            using (SqlConnection conn = GetConnection())
            {
                if (conn == null) return 0;
                using (SqlCommand cmd = new SqlCommand(queryOrSp, conn))
                {
                    cmd.CommandType = cmdType;
                    AttachParameters(cmd, parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string queryOrSp, SqlParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            using (SqlConnection conn = GetConnection())
            {
                if (conn == null) return null;
                using (SqlCommand cmd = new SqlCommand(queryOrSp, conn))
                {
                    cmd.CommandType = cmdType;
                    AttachParameters(cmd, parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        // --- SPECIFIC METHODS (DITAMBAHKAN SECARA BERTAHAP) ---

        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = "";
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    if (conn != null && conn.State == ConnectionState.Open) return true;
                    errorMessage = "Koneksi null atau tertutup.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // ===============================================
        // MODUL AUTENTIKASI (loginForm & loginPage)
        // ===============================================
        public static bool AuthenticateUser(string email, string password)
        {
            using (SqlConnection conn = GetConnection())
            {
                if (conn == null) return false;

                using (SqlCommand cmd = new SqlCommand("[master].[sp_AuthenticateUser]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // ===============================================
        // MODUL KELOLA BARANG (kelolaBarang.cs)
        // ===============================================
        public static DataTable GetBarangData(string keyword)
        {
            string query = @"SELECT * FROM [dbo].[vw_DataBarangMaster] 
                             WHERE [Nama Barang] LIKE @kw OR Merk LIKE @kw OR [ID/Kode Barang] LIKE @kw";
            SqlParameter[] pars = { new SqlParameter("@kw", "%" + keyword + "%") };
            return ExecuteQuery(query, pars);
        }

        public static DataTable GetMerkData()
        {
            string query = "SELECT * FROM [dbo].[vw_DataMerk] ORDER BY namaMerk";
            return ExecuteQuery(query);
        }

        private static int GetOrCreateMerk(string namaMerk, SqlConnection conn, SqlTransaction trans)
        {
            namaMerk = namaMerk.Trim();
            string checkQ = "SELECT idMerk FROM master.merk WHERE namaMerk = @nama";
            using (var cmd = new SqlCommand(checkQ, conn, trans))
            {
                cmd.Parameters.AddWithValue("@nama", namaMerk);
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }
            using (var cmd = new SqlCommand("[master].[sp_InsertMerk]", conn, trans))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nama", namaMerk);
                return (int)cmd.ExecuteScalar();
            }
        }

        private static string GenerateIdBarangOtomatis(string namaMerk, int tipeBarang, SqlConnection conn, SqlTransaction trans)
        {
            string cleanMerk = namaMerk.Trim().Replace(" ", "").ToUpper();
            string singkatanMerk = cleanMerk.Length >= 3 ? cleanMerk.Substring(0, 3) : cleanMerk.PadRight(3, 'X');
            string codePrefix = $"TI-{singkatanMerk}-{tipeBarang}-";

            string countQuery = "SELECT COUNT(*) FROM master.barang WHERE idBarang LIKE @prefix + '%'";
            int runningNumber = 1;
            using (SqlCommand cmd = new SqlCommand(countQuery, conn, trans))
            {
                cmd.Parameters.AddWithValue("@prefix", codePrefix);
                runningNumber = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            }
            return $"{codePrefix}{runningNumber.ToString("D3")}";
        }

        private static int GetOrCreateGedung(string namaGedung, SqlConnection conn, SqlTransaction trans)
        {
            if (string.IsNullOrWhiteSpace(namaGedung)) return 0;
            string query = "SELECT idGedung FROM master.gedung WHERE namaGedung = @nama";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@nama", namaGedung.Trim());
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }
            using (var cmd = new SqlCommand("[master].[sp_InsertGedung]", conn, trans))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@namaGedung", namaGedung.Trim());
                return (int)cmd.ExecuteScalar();
            }
        }

        private static int GetOrCreateRuangan(string namaRuangan, int idGedung, SqlConnection conn, SqlTransaction trans)
        {
            if (string.IsNullOrWhiteSpace(namaRuangan) || idGedung <= 0) return 0;
            string query = "SELECT idRuangan FROM master.ruangan WHERE namaRuangan = @nama AND idGedung = @idGedung";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@nama", namaRuangan.Trim());
                cmd.Parameters.AddWithValue("@idGedung", idGedung);
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }
            using (var cmd = new SqlCommand("[master].[sp_InsertRuangan]", conn, trans))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idGedung", idGedung);
                cmd.Parameters.AddWithValue("@namaRuangan", namaRuangan.Trim());
                return (int)cmd.ExecuteScalar();
            }
        }

        public static string SimpanBarangMaster(bool isEditMode, string originalIdBarang, string namaBarang, string namaMerk, int stokInput, int tipeInput, string satuanInput)
        {
            using (var conn = GetConnection())
            {
                if (conn == null) throw new Exception("Koneksi DB gagal.");
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int resolvedIdMerk = GetOrCreateMerk(namaMerk, conn, transaction);
                        string finalIdBarang = isEditMode ? originalIdBarang : GenerateIdBarangOtomatis(namaMerk, tipeInput, conn, transaction);

                        using (var cmd = new SqlCommand("[master].[sp_ManageBarang]", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Action", isEditMode ? "UPDATE" : "INSERT");
                            cmd.Parameters.AddWithValue("@idBarang", finalIdBarang);
                            cmd.Parameters.AddWithValue("@namaBarang", namaBarang);
                            cmd.Parameters.AddWithValue("@idMerk", resolvedIdMerk);
                            cmd.Parameters.AddWithValue("@stok", stokInput);
                            cmd.Parameters.AddWithValue("@satuan", satuanInput);
                            cmd.Parameters.AddWithValue("@tipeBarang", tipeInput);
                            cmd.Parameters.AddWithValue("@isiKonversi", 1);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return finalIdBarang;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // ===============================================
        // MODUL DETAIL BARANG (FormDetailBarang.cs)
        // ===============================================

        public static DataTable GetComboBoxBarangAset()
        {
            string query = @"
                SELECT 
                    b.idBarang, 
                    b.namaBarang + '/' + ISNULL(m.namaMerk, 'No Merk') AS displayBarang 
                FROM master.barang b
                LEFT JOIN master.merk m ON b.idMerk = m.idMerk
                WHERE b.tipeBarang = 1";
            return ExecuteQuery(query);
        }

        public static DataTable GetComboBoxGedung()
        {
            return ExecuteQuery("SELECT * FROM [dbo].[vw_DataGedung]");
        }

        public static DataTable GetComboBoxRuangan(int idGedung)
        {
            string query = "SELECT idRuangan, namaRuangan FROM [dbo].[vw_DataRuangan] WHERE idGedung = @idGedung";
            SqlParameter[] pars = { new SqlParameter("@idGedung", idGedung) };
            return ExecuteQuery(query, pars);
        }

        public static DataTable GetDetailBarangData(string keyword)
        {
            string query = @"
                SELECT 
                    db.idDetailBarang AS [ID Detail],
                    b.namaBarang AS [Nama Aset],
                    m.namaMerk AS [Merk],
                    db.spesifikasi AS [Spesifikasi],
                    db.satuan AS [Satuan],
                    g.namaGedung AS [Gedung],
                    r.namaRuangan AS [Lokasi Ruangan],
                    db.tglMasuk AS [Tanggal Masuk],
                    s.tahunAjaran AS [Semester],
                    b.idBarang, g.idGedung, r.idRuangan, b.tipeBarang
                FROM [transaction].detailBarang db
                LEFT JOIN [master].barang b ON db.idBarang = b.idBarang
                LEFT JOIN [master].merk m ON b.idMerk = m.idMerk 
                LEFT JOIN [master].ruangan r ON db.idRuangan = r.idRuangan
                LEFT JOIN [master].gedung g ON r.idGedung = g.idGedung
                LEFT JOIN [master].semester s ON db.idSemesterMasuk = s.idSemester";

            if (!string.IsNullOrEmpty(keyword))
            {
                query += " WHERE b.namaBarang LIKE @kw OR db.idDetailBarang LIKE @kw OR db.spesifikasi LIKE @kw OR m.namaMerk LIKE @kw";
                SqlParameter[] pars = { new SqlParameter("@kw", "%" + keyword + "%") };
                return ExecuteQuery(query, pars);
            }
            return ExecuteQuery(query);
        }

        public static void SimpanDetailBarang(bool isEditMode, string originalIdDetail, string selectedIdBarang, int jumlahLoop, int idRuangan, string spesifikasi, string satuanFinal)
        {
            using (var conn = GetConnection())
            {
                if (conn == null) throw new Exception("Koneksi DB gagal.");

                int idSemesterMasuk = 0;
                if (!isEditMode)
                {
                    int tahunSekarang = DateTime.Now.Year;
                    int bulanSekarang = DateTime.Now.Month;
                    string tipeSemester = bulanSekarang >= 7 ? "Ganjil" : "Genap";
                    string tahunAjaran = bulanSekarang >= 7 ? $"{tahunSekarang}/{tahunSekarang + 1}" : $"{tahunSekarang - 1}/{tahunSekarang}";
                    string currentSemesterText = $"{tahunAjaran} ({tipeSemester})";

                    string qSemester = "SELECT idSemester FROM master.semester WHERE tahunAjaran = @tahunAjaran";
                    using (SqlCommand cmdSem = new SqlCommand(qSemester, conn))
                    {
                        cmdSem.Parameters.AddWithValue("@tahunAjaran", currentSemesterText);
                        object result = cmdSem.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            throw new Exception($"Semester Aktif ({currentSemesterText}) belum terdaftar di database!\nSilakan daftarkan terlebih dahulu di menu Semester.");
                        }
                        idSemesterMasuk = Convert.ToInt32(result);
                    }
                }

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        if (isEditMode)
                        {
                            using (var cmd = new SqlCommand("[transaction].[sp_UpdateDetailBarang]", conn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idRuangan", idRuangan);
                                cmd.Parameters.AddWithValue("@spesifikasi", spesifikasi);
                                cmd.Parameters.AddWithValue("@satuan", satuanFinal);
                                cmd.Parameters.AddWithValue("@idDetailBarang", originalIdDetail);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < jumlahLoop; i++)
                            {
                                string countQuery = "SELECT COUNT(*) FROM [transaction].detailBarang WHERE idBarang = @idB";
                                int nextSequence = 1;
                                using (SqlCommand cmdCount = new SqlCommand(countQuery, conn, transaction))
                                {
                                    cmdCount.Parameters.AddWithValue("@idB", selectedIdBarang);
                                    nextSequence = Convert.ToInt32(cmdCount.ExecuteScalar()) + 1;
                                }

                                string finalIdDetail = $"{selectedIdBarang}-{nextSequence.ToString("D3")}";

                                using (var cmdIn = new SqlCommand("[transaction].[sp_InsertDetailBarang]", conn, transaction))
                                {
                                    cmdIn.CommandType = CommandType.StoredProcedure;
                                    cmdIn.Parameters.AddWithValue("@idDetailBarang", finalIdDetail);
                                    cmdIn.Parameters.AddWithValue("@idBarang", selectedIdBarang);
                                    cmdIn.Parameters.AddWithValue("@idRuangan", idRuangan);
                                    cmdIn.Parameters.AddWithValue("@spesifikasi", spesifikasi);
                                    cmdIn.Parameters.AddWithValue("@satuan", satuanFinal);
                                    cmdIn.Parameters.AddWithValue("@idSemesterMasuk", idSemesterMasuk);
                                    cmdIn.Parameters.AddWithValue("@tglMasuk", DateTime.Now);
                                    cmdIn.Parameters.AddWithValue("@statusAset", 1);
                                    cmdIn.ExecuteNonQuery();
                                }
                            }

                            string qStock = "UPDATE master.barang SET stok = stok + @totalLoop WHERE idBarang = @barang";
                            using (var cmdStok = new SqlCommand(qStock, conn, transaction))
                            {
                                cmdStok.Parameters.AddWithValue("@totalLoop", jumlahLoop);
                                cmdStok.Parameters.AddWithValue("@barang", selectedIdBarang);
                                cmdStok.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void ImportDataBarang(DataTable dtExcel)
        {
            using (var conn = GetConnection())
            {
                if (conn == null) throw new Exception("Koneksi DB gagal.");
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string qSemester = "SELECT TOP 1 idSemester FROM master.semester WHERE isAktif = 1 ORDER BY idSemester DESC";
                        int idSemesterMasuk = 0;
                        using (SqlCommand cmdSem = new SqlCommand(qSemester, conn, transaction))
                        {
                            object semObj = cmdSem.ExecuteScalar();
                            if (semObj != null && semObj != DBNull.Value) idSemesterMasuk = Convert.ToInt32(semObj);
                        }
                        if (idSemesterMasuk == 0) throw new Exception("Tidak ada semester aktif. Import dibatalkan.");

                        System.Collections.Generic.Dictionary<string, string> cacheMasterBarang = new System.Collections.Generic.Dictionary<string, string>();
                        int detailCounter = 1;

                        foreach (DataRow row in dtExcel.Rows)
                        {
                            string namaBarang = row["NamaBarang"].ToString().Trim();
                            string merk = row["Merk"].ToString().Trim();
                            string spek = row["Spesifikasi"].ToString().Trim();
                            string gedung = row["Gedung"].ToString().Trim();
                            string ruangan = row["Ruangan"].ToString().Trim();
                            int tipeBarang = row["TipeBarang"].ToString() == "Aset" ? 1 : 0;
                            string satuan = row["Satuan"].ToString().Trim();
                            int isiKonversi = 1;

                            if (string.IsNullOrEmpty(namaBarang) || string.IsNullOrEmpty(merk)) continue;

                            int idMerk = GetOrCreateMerk(merk, conn, transaction);
                            string masterKey = $"{namaBarang}_{idMerk}";
                            string idBarang = "";

                            if (cacheMasterBarang.ContainsKey(masterKey))
                            {
                                idBarang = cacheMasterBarang[masterKey];
                            }
                            else
                            {
                                idBarang = GenerateIdBarangOtomatis(merk, tipeBarang, conn, transaction);
                                int stokAwal = tipeBarang == 0 ? 0 : dtExcel.Select($"NamaBarang = '{namaBarang}' AND Merk = '{merk}'").Length;

                                using (var cmdM = new SqlCommand("[master].[sp_ManageBarang]", conn, transaction))
                                {
                                    cmdM.CommandType = CommandType.StoredProcedure;
                                    cmdM.Parameters.AddWithValue("@Action", "INSERT");
                                    cmdM.Parameters.AddWithValue("@idBarang", idBarang);
                                    cmdM.Parameters.AddWithValue("@namaBarang", namaBarang);
                                    cmdM.Parameters.AddWithValue("@idMerk", idMerk);
                                    cmdM.Parameters.AddWithValue("@stok", stokAwal);
                                    cmdM.Parameters.AddWithValue("@tipeBarang", tipeBarang);
                                    cmdM.Parameters.AddWithValue("@satuan", satuan);
                                    cmdM.Parameters.AddWithValue("@isiKonversi", isiKonversi);
                                    cmdM.ExecuteNonQuery();
                                }
                                cacheMasterBarang.Add(masterKey, idBarang);
                                detailCounter = 1;
                            }

                            if (tipeBarang == 1)
                            {
                                string idDetail = $"{idBarang}-{detailCounter.ToString("D3")}";
                                detailCounter++;

                                int idGedung = GetOrCreateGedung(gedung, conn, transaction);
                                int idRuangan = GetOrCreateRuangan(ruangan, idGedung, conn, transaction);

                                using (var cmdD = new SqlCommand("[transaction].[sp_InsertDetailBarang]", conn, transaction))
                                {
                                    cmdD.CommandType = CommandType.StoredProcedure;
                                    cmdD.Parameters.AddWithValue("@idDetailBarang", idDetail);
                                    cmdD.Parameters.AddWithValue("@idBarang", idBarang);
                                    cmdD.Parameters.AddWithValue("@idRuangan", idRuangan);
                                    cmdD.Parameters.AddWithValue("@spesifikasi", spek);
                                    cmdD.Parameters.AddWithValue("@satuan", satuan);
                                    cmdD.Parameters.AddWithValue("@idSemesterMasuk", idSemesterMasuk);
                                    cmdD.Parameters.AddWithValue("@tglMasuk", DateTime.Now);
                                    cmdD.Parameters.AddWithValue("@statusAset", 1);
                                    cmdD.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        // ===============================================
        // MODUL PERMINTAAN BARANG (permintaanBarang.cs)
        // ===============================================

        public static DataTable GetDaftarRuangan()
        {
            return ExecuteQuery("SELECT * FROM [dbo].[vwDaftarRuangan]");
        }

        public static DataTable GetSemesterAktif(string currentSemesterText)
        {
            string query = "SELECT idSemester, tahunAjaran FROM [master].[semester] WHERE tahunAjaran = @active";
            SqlParameter[] pars = { new SqlParameter("@active", currentSemesterText) };
            return ExecuteQuery(query, pars);
        }

        public static DataTable GetKatalogBarangHabisPakai(string keyword)
        {
            string query = @"SELECT idBarang AS [ID], namaBarang AS [Nama Barang], stok AS [Total Stok (Pcs)], 
                                    satuan AS [Satuan Master], isiKonversi AS [Isi Per Satuan]
                             FROM [master].[barang] WHERE tipeBarang = 0";

            if (!string.IsNullOrEmpty(keyword))
            {
                query += " AND (namaBarang LIKE @kw OR idBarang LIKE @kw)";
                SqlParameter[] pars = { new SqlParameter("@kw", "%" + keyword + "%") };
                return ExecuteQuery(query, pars);
            }
            return ExecuteQuery(query);
        }

        public static DataTable GetRiwayatTransaksi(string keyword)
        {
            string query = "SELECT * FROM [dbo].[vwDataTransaksi]";

            if (!string.IsNullOrEmpty(keyword))
            {
                query += " WHERE [Barang] LIKE @kw OR [Peminta] LIKE @kw";
                SqlParameter[] pars = { new SqlParameter("@kw", "%" + keyword + "%") };
                return ExecuteQuery(query, pars);
            }
            return ExecuteQuery(query);
        }

        public static void SimpanPermintaanBarang(string idBarang, int idRuangan, string namaPeminta, int jumlahRequested, string activeSemesterText)
        {
            using (var conn = GetConnection())
            {
                if (conn == null) throw new Exception("Koneksi DB gagal.");
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string qSemester = "SELECT idSemester FROM [master].[semester] WHERE tahunAjaran = @TA";
                        int idSemesterAktif = 0;
                        using (SqlCommand cmdSem = new SqlCommand(qSemester, conn, transaction))
                        {
                            cmdSem.Parameters.AddWithValue("@TA", activeSemesterText);
                            object result = cmdSem.ExecuteScalar();
                            if (result != null) idSemesterAktif = Convert.ToInt32(result);
                            else throw new Exception($"Tahun akademik '{activeSemesterText}' belum terdaftar di master data semester!");
                        }

                        using (SqlCommand cmd = new SqlCommand("sp_SavePermintaanBarang", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@idPB", DBNull.Value);
                            cmd.Parameters.AddWithValue("@idB", idBarang);
                            cmd.Parameters.AddWithValue("@idR", idRuangan);
                            cmd.Parameters.AddWithValue("@nama", namaPeminta);
                            cmd.Parameters.AddWithValue("@jml", jumlahRequested);
                            cmd.Parameters.AddWithValue("@smt", idSemesterAktif);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        // ===============================================
        // MODUL MAINTENANCE (maintenancePage.cs)
        // ===============================================

        public static DataTable GetKaryawanData()
        {
            return ExecuteQuery("SELECT idKaryawan, namaKaryawan FROM [master].[karyawan]");
        }

        public static DataTable GetDetailBarangAsset(string keyword)
        {
            string query = @"SELECT * FROM [dbo].[vwDetailBarangAsset]";

            if (!string.IsNullOrEmpty(keyword))
            {
                query += " WHERE b.namaBarang LIKE @kw OR db.spesifikasi LIKE @kw OR r.namaRuangan LIKE @kw OR db.idDetailBarang LIKE @kw";
                SqlParameter[] pars = { new SqlParameter("@kw", "%" + keyword + "%") };
                return ExecuteQuery(query, pars);
            }
            return ExecuteQuery(query);
        }

        public static DataTable GetMaintenanceHistory(string keyword)
        {
            string query = @"SELECT * FROM [dbo].[vwMaintenanceHistory]";

            if (!string.IsNullOrEmpty(keyword))
            {
                query += " WHERE [Aset] LIKE @kw OR [Petugas] LIKE @kw OR [Lokasi] LIKE @kw";
                SqlParameter[] pars = { new SqlParameter("@kw", "%" + keyword + "%") };
                return ExecuteQuery(query, pars);
            }
            return ExecuteQuery(query);
        }

        public static void SimpanMaintenance(string idKaryawan, string idDetailBarang, string idBarang, DateTime tglCek, int kondisi, string kerusakan, string tindakLanjut, int idSemester)
        {
            using (var conn = GetConnection())
            {
                if (conn == null) throw new Exception("Koneksi DB gagal.");

                using (SqlCommand cmd = new SqlCommand("[dbo].[sp_SaveMaintenance]", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idM", DBNull.Value); // Mutlak selalu INSERT log baru
                    cmd.Parameters.AddWithValue("@idK", idKaryawan);
                    cmd.Parameters.AddWithValue("@idD", idDetailBarang);
                    cmd.Parameters.AddWithValue("@idB", idBarang);
                    cmd.Parameters.AddWithValue("@tgl", tglCek);
                    cmd.Parameters.AddWithValue("@kon", kondisi);
                    cmd.Parameters.AddWithValue("@ker", kerusakan);
                    cmd.Parameters.AddWithValue("@tin", tindakLanjut);
                    cmd.Parameters.AddWithValue("@smt", idSemester);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        // ===============================================
        // MODUL SEMESTER (formSemester.cs)
        // ===============================================

        public static DataTable GetSemesterData(string keyword)
        {
            string query = "SELECT * FROM master.vw_DataSemester";
            if (!string.IsNullOrEmpty(keyword))
            {
                query += " WHERE [Tahun Ajaran] LIKE @keyword";
                SqlParameter[] pars = { new SqlParameter("@keyword", "%" + keyword + "%") };
                return ExecuteQuery(query, pars);
            }
            return ExecuteQuery(query);
        }

        public static void TambahSemester(string tahunAjaran)
        {
            using (var conn = GetConnection())
            {
                if (conn == null) throw new Exception("Koneksi DB gagal.");

                string checkQuery = "SELECT COUNT(*) FROM [master].[semester] WHERE [tahunAjaran] = @tahunAjaran";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@tahunAjaran", tahunAjaran);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        throw new Exception($"Semester '{tahunAjaran}' sudah terdaftar di database!");
                    }
                }

                using (SqlCommand cmd = new SqlCommand("master.sp_ManageSemester", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@tahunAjaran", tahunAjaran);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // ===============================================
        // MODUL KARYAWAN (karyawan.cs)
        // ===============================================

        public static DataTable GetKaryawanGridData(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return ExecuteQuery("SELECT * FROM master.vw_DataKaryawan");
            }
            else
            {
                SqlParameter[] pars = {
                    new SqlParameter("@Action", "SEARCH"),
                    new SqlParameter("@namaKaryawan", keyword)
                };
                return ExecuteQuery("master.sp_ManageKaryawan", pars, CommandType.StoredProcedure);
            }
        }

        public static void ManageKaryawan(string action, string idKaryawan, string namaKaryawan)
        {
            using (var conn = GetConnection())
            {
                if (conn == null) throw new Exception("Koneksi DB gagal.");

                using (SqlCommand cmd = new SqlCommand("master.sp_ManageKaryawan", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);
                    
                    if (action == "INSERT" || action == "UPDATE")
                    {
                        cmd.Parameters.AddWithValue("@namaKaryawan", namaKaryawan);
                    }
                    if (action == "UPDATE" || action == "DELETE")
                    {
                        cmd.Parameters.AddWithValue("@idKaryawan", idKaryawan);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }
        // ===============================================
        // MODUL GRAFIK (grafik.cs)
        // ===============================================

        public static DataTable GetAllSemesters()
        {
            return ExecuteQuery("SELECT * FROM [dbo].[vw_SemesterReport]");
        }

        public static DataTable GetStokAktualBarang()
        {
            string query = "SELECT * FROM [dbo].[vw_StokAktualBarang]";
            return ExecuteQuery(query);
        }
        // ===============================================
        // MODUL LAPORAN (ReportPage.cs)
        // ===============================================

        public static DataTable GetSemesterReportData()
        {
            return ExecuteQuery("SELECT * FROM [dbo].[vw_SemesterReport]");
        }

        public static DataTable GetProcurementAlertData()
        {
            string query = "SELECT * FROM [dbo].[vw_ProcurementAlert]";
            return ExecuteQuery(query);
        }

        public static DataTable GetReplacementAlertData()
        {
            string query = "SELECT * FROM [dbo].[vw_ReplacementAlert]";
            return ExecuteQuery(query);
        }

        public static DataTable GetLaporanData(string tipe, int idSemester, int angkaBulan)
        {
            string query = "SELECT [Tanggal Transaksi], [Nama Barang], [Lokasi Ruangan], [Nama Peminta], [Jumlah Diberikan] FROM [dbo].[vw_LaporanPermintaanBarang] WHERE idSemester = @idSemester";

            if (tipe.ToLower() == "bulanan" && angkaBulan > 0)
            {
                query += " AND BulanTransaksi = @bulan";
                SqlParameter[] pars = { 
                    new SqlParameter("@idSemester", idSemester),
                    new SqlParameter("@bulan", angkaBulan)
                };
                return ExecuteQuery(query, pars);
            }
            else
            {
                SqlParameter[] pars = { new SqlParameter("@idSemester", idSemester) };
                return ExecuteQuery(query, pars);
            }
        }

        public static DataTable GetMaintenanceData(string tipe, int idSemester, int angkaBulan)
        {
            string query = "SELECT [Tanggal Pengecekan], [Nama Aset], [Lokasi], [Petugas], [Kondisi Akhir], [Detail Kerusakan], [Tindak Lanjut] FROM [dbo].[vw_LaporanMaintenance] WHERE idSemester = @idSemester";

            if (tipe.ToLower() == "bulanan" && angkaBulan > 0)
            {
                query += " AND BulanTransaksi = @bulan";
                SqlParameter[] pars = { 
                    new SqlParameter("@idSemester", idSemester),
                    new SqlParameter("@bulan", angkaBulan)
                };
                return ExecuteQuery(query, pars);
            }
            else
            {
                SqlParameter[] pars = { new SqlParameter("@idSemester", idSemester) };
                return ExecuteQuery(query, pars);
            }
        }
        public static DataTable GetReportStokData(int? idSemesterFilter)
        {
            if (idSemesterFilter.HasValue)
            {
                string query = @"
                SELECT DISTINCT v.[Nama Barang], v.[Jumlah Saat Ini], v.[Jenis Barang], v.[Satuan] 
                FROM [dbo].[vw_LaporanStok] v
                INNER JOIN [transaction].[permintaanBarang] p ON v.idBarang = p.idBarang
                WHERE p.idSemester = @idSemester";
                SqlParameter[] pars = { new SqlParameter("@idSemester", idSemesterFilter.Value) };
                return ExecuteQuery(query, pars);
            }
            return ExecuteQuery("SELECT [Nama Barang], [Jumlah Saat Ini], [Jenis Barang], [Satuan] FROM [dbo].[vw_LaporanStok]");
        }
        public static DataTable GetReportEvaluasiData(int? idSemester)
        {
            string query = @"
                SELECT DISTINCT 
                    v.NamaBarang, 
                    v.TotalKeluar, 
                    v.FrekuensiDiminta AS [jumlah diminta], 
                    v.FrekuensiRestock AS [jumlah restock], 
                    v.RataRataJmlRestock, 
                    v.StatusEvaluasi AS [analisa]
                FROM [dbo].[vwEvaluasiHabisPakai] v
                INNER JOIN [master].[barang] b ON v.NamaBarang = b.namaBarang ";

            if (idSemester.HasValue)
            {
                query += @"
                INNER JOIN [transaction].[permintaanBarang] p ON b.idBarang = p.idBarang
                WHERE p.idSemester = @idSemester";
                SqlParameter[] pars = { new SqlParameter("@idSemester", idSemester.Value) };
                return ExecuteQuery(query, pars);
            }
            return ExecuteQuery(query);
        }
        public static DataTable GetCetakStokSummary(int? idSemesterFilter)
        {
            if (idSemesterFilter.HasValue)
            {
                SqlParameter[] pars = { new SqlParameter("@idSemester", idSemesterFilter.Value) };
                return ExecuteQuery("[dbo].[sp_CetakStokSummary]", pars, CommandType.StoredProcedure);
            }
            return ExecuteQuery("[dbo].[sp_CetakStokSummary]", null, CommandType.StoredProcedure);
        }

        public static DataTable GetCetakStokDetail(int? idSemesterFilter)
        {
            if (idSemesterFilter.HasValue)
            {
                SqlParameter[] pars = { new SqlParameter("@idSemester", idSemesterFilter.Value) };
                return ExecuteQuery("[dbo].[sp_CetakStokDetail]", pars, CommandType.StoredProcedure);
            }
            return ExecuteQuery("[dbo].[sp_CetakStokDetail]", null, CommandType.StoredProcedure);
        }
    }
}

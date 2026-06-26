IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[master].[LogPerubahanStok]') AND type in (N'U'))
BEGIN
    CREATE TABLE master.LogPerubahanStok (
        idLog INT IDENTITY(1,1) PRIMARY KEY,
        idBarang VARCHAR(50),
        namaBarang VARCHAR(255),
        stokLama INT,
        stokBaru INT,
        jenisAktivitas VARCHAR(50),
        waktuPerubahan DATETIME DEFAULT GETDATE(),
        userDB VARCHAR(100) DEFAULT SYSTEM_USER
    );
END
GO

-- =====================================================================
-- 3. TRIGGERS UNTUK KEAMANAN DAN AUDIT
-- =====================================================================
CREATE OR ALTER TRIGGER trg_AuditStokBarang
ON master.barang
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON; 
    
    IF UPDATE(stok)
    BEGIN
        INSERT INTO master.LogPerubahanStok (idBarang, namaBarang, stokLama, stokBaru, jenisAktivitas)
        SELECT 
            i.idBarang, 
            i.namaBarang, 
            d.stok AS stokLama, 
            i.stok AS stokBaru,
            CASE 
                WHEN i.stok > d.stok THEN 'Penambahan Stok Masuk'
                WHEN i.stok < d.stok THEN 'Pengurangan Stok Keluar'
                ELSE 'Update Tanpa Perubahan Stok'
            END
        FROM inserted i
        JOIN deleted d ON i.idBarang = d.idBarang
        WHERE i.stok <> d.stok;
    END
END;
GO

CREATE OR ALTER TRIGGER trg_PreventDeleteMaintenance
ON [transaction].[maintenance]
INSTEAD OF DELETE
AS
BEGIN
    RAISERROR ('Akses Ditolak (Database Level): Riwayat Maintenance bersifat permanen dan DILARANG KERAS untuk dihapus!', 16, 1);
    ROLLBACK TRANSACTION;
END;
GO

-- =====================================================================
-- 4. VIEWS UNTUK MEMBERSIHKAN C# DAL (DATA ACCESS LAYER)
-- =====================================================================
CREATE OR ALTER VIEW [dbo].[vw_DataBarangMaster] AS
SELECT b.idBarang AS [ID/Kode Barang], b.namaBarang AS [Nama Barang], b.stok AS [Sisa Stok],
       m.namaMerk AS Merk, b.tipeBarang AS tipeBarang,
       b.satuan AS Satuan, b.isiKonversi AS Konversi 
FROM master.barang b 
INNER JOIN master.merk m ON b.idMerk = m.idMerk;
GO

CREATE OR ALTER VIEW [dbo].[vw_DataMerk] AS
SELECT idMerk, namaMerk FROM master.merk;
GO

CREATE OR ALTER VIEW [dbo].[vw_DataGedung] AS
SELECT idGedung, namaGedung FROM master.gedung;
GO

CREATE OR ALTER VIEW [dbo].[vw_DataRuangan] AS
SELECT idRuangan, namaRuangan, idGedung FROM master.ruangan;
GO

CREATE OR ALTER VIEW [dbo].[vw_StokAktualBarang] AS
SELECT namaBarang, stok AS StokAktual FROM master.barang;
GO

CREATE OR ALTER VIEW [dbo].[vw_SemesterReport] AS
SELECT idSemester, tahunAjaran FROM master.semester;
GO

CREATE OR ALTER VIEW [dbo].[vw_ProcurementAlert] AS
SELECT namaBarang AS [Nama Barang], stok AS [Sisa Stok], 'SEGERA BELI' AS [Rekomendasi]
FROM master.barang 
WHERE tipeBarang = 0 AND stok < 20;
GO

CREATE OR ALTER VIEW [dbo].[vw_ReplacementAlert] AS
SELECT 
    b.namaBarang AS [Nama Asset], 
    r.namaRuangan AS [Lokasi],
    COUNT(m.idMaintenance) AS [Total Frekuensi Rusak]
FROM [transaction].maintenance m
JOIN [transaction].detailBarang db ON m.idDetailBarang = db.idDetailBarang
JOIN [master].barang b ON db.idBarang = b.idBarang
JOIN [master].ruangan r ON db.idRuangan = r.idRuangan
WHERE m.kondisi = 0
GROUP BY b.namaBarang, r.namaRuangan
HAVING COUNT(m.idMaintenance) >= 2;
GO

CREATE OR ALTER VIEW [dbo].[vw_LaporanPermintaanBarang] AS
SELECT 
    pb.tglPermintaan AS [Tanggal Transaksi],
    b.namaBarang AS [Nama Barang],
    r.namaRuangan AS [Lokasi Ruangan],
    pb.namaPeminta AS [Nama Peminta],
    pb.jumlah AS [Jumlah Diberikan],
    pb.idSemester,
    MONTH(pb.tglPermintaan) AS [BulanTransaksi]
FROM [transaction].permintaanBarang pb
JOIN [master].barang b ON pb.idBarang = b.idBarang
JOIN [master].ruangan r ON pb.idRuangan = r.idRuangan;
GO

CREATE OR ALTER VIEW [dbo].[vw_LaporanMaintenance] AS
SELECT 
    m.tglCek AS [Tanggal Pengecekan],
    b.namaBarang AS [Nama Aset],
    r.namaRuangan AS [Lokasi],
    k.namaKaryawan AS [Petugas],
    CASE WHEN m.kondisi = 1 THEN 'BAIK' ELSE 'RUSAK' END AS [Kondisi Akhir],
    ISNULL(m.kerusakan, '-') AS [Detail Kerusakan],
    ISNULL(m.tindakLanjut, '-') AS [Tindak Lanjut],
    m.idSemester,
    MONTH(m.tglCek) AS [BulanTransaksi]
FROM [transaction].maintenance m
JOIN [transaction].detailBarang db ON m.idDetailBarang = db.idDetailBarang
JOIN [master].barang b ON db.idBarang = b.idBarang
JOIN [master].ruangan r ON db.idRuangan = r.idRuangan
JOIN [master].karyawan k ON m.idKaryawan = k.idKaryawan;
GO

CREATE OR ALTER VIEW [dbo].[vw_LaporanStok] AS
SELECT DISTINCT 
    b.namaBarang AS [Nama Barang],
    b.stok AS [Jumlah Saat Ini],
    CASE 
        WHEN b.tipeBarang = 0 THEN 'Barang Habis Pakai'
        WHEN b.tipeBarang = 1 THEN 'Aset Tetap'
        ELSE 'Tidak Diketahui'
    END AS [Jenis Barang],
    b.satuan AS [Satuan],
    b.idBarang,
    b.idMerk
FROM master.barang b;
GO

CREATE OR ALTER VIEW [dbo].[vwEvaluasiHabisPakai] AS
SELECT 
    b.namaBarang AS NamaBarang,
    ISNULL(SUM(p.jumlah), 0) AS TotalKeluar,
    COUNT(p.idPermintaanBarang) AS FrekuensiDiminta,
    (SELECT COUNT(*) FROM master.LogPerubahanStok l WHERE l.idBarang = b.idBarang AND l.stokBaru > l.stokLama) AS FrekuensiRestock,
    ISNULL((SELECT AVG(l.stokBaru - l.stokLama) FROM master.LogPerubahanStok l WHERE l.idBarang = b.idBarang AND l.stokBaru > l.stokLama), 0) AS RataRataJmlRestock,
    CASE 
        WHEN ISNULL(SUM(p.jumlah), 0) > 50 THEN 'Sangat Laris (Perlu Restock Rutin)'
        WHEN ISNULL(SUM(p.jumlah), 0) > 10 THEN 'Normal (Aman)'
        ELSE 'Jarang Dipakai (Kurangi Pembelian)'
    END AS StatusEvaluasi
FROM master.barang b
LEFT JOIN [transaction].permintaanBarang p ON b.idBarang = p.idBarang
WHERE b.tipeBarang = 0
GROUP BY b.idBarang, b.namaBarang;
GO

-- =====================================================================
-- 5. STORED PROCEDURES
-- =====================================================================
CREATE OR ALTER PROCEDURE [master].[sp_AuthenticateUser]
    @Email VARCHAR(50),
    @Password VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) AS IsValid 
    FROM [master].[users] 
    WHERE email = @Email AND password = @Password;
END
GO

CREATE OR ALTER PROCEDURE [master].[sp_InsertMerk]
    @namaMerk VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [master].[merk] (namaMerk) 
    OUTPUT INSERTED.idMerk 
    VALUES (@namaMerk);
END
GO

CREATE OR ALTER PROCEDURE [master].[sp_InsertGedung]
    @namaGedung VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [master].[gedung] (namaGedung) 
    OUTPUT INSERTED.idGedung 
    VALUES (@namaGedung);
END
GO

CREATE OR ALTER PROCEDURE [master].[sp_InsertRuangan]
    @idGedung INT,
    @namaRuangan VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [master].[ruangan] (idGedung, namaRuangan) 
    OUTPUT INSERTED.idRuangan 
    VALUES (@idGedung, @namaRuangan);
END
GO

CREATE OR ALTER PROCEDURE [master].[sp_ManageBarang]
    @Action VARCHAR(10),
    @idBarang VARCHAR(50),
    @namaBarang VARCHAR(100) = NULL,
    @idMerk INT = NULL,
    @stok INT = 0,
    @tipeBarang BIT = NULL,
    @satuan VARCHAR(20) = NULL,
    @isiKonversi INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [master].[barang] (idBarang, namaBarang, idMerk, stok, tipeBarang, satuan, isiKonversi)
        VALUES (@idBarang, @namaBarang, @idMerk, @stok, @tipeBarang, @satuan, @isiKonversi);
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [master].[barang] 
        SET namaBarang = @namaBarang, idMerk = @idMerk, stok = @stok, satuan = @satuan, tipeBarang = @tipeBarang
        WHERE idBarang = @idBarang;
    END
END
GO

CREATE OR ALTER PROCEDURE [transaction].[sp_InsertDetailBarang]
    @idDetailBarang VARCHAR(50),
    @idBarang VARCHAR(50),
    @idRuangan INT,
    @spesifikasi VARCHAR(500) = NULL,
    @satuan VARCHAR(20),
    @idSemesterMasuk INT = NULL,
    @tglMasuk DATE = NULL,
    @statusAset BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [transaction].[detailBarang] 
    (idDetailBarang, idBarang, idRuangan, spesifikasi, satuan, idSemesterMasuk, tglMasuk, statusAset) 
    VALUES 
    (@idDetailBarang, @idBarang, @idRuangan, @spesifikasi, @satuan, @idSemesterMasuk, @tglMasuk, @statusAset);
END
GO

CREATE OR ALTER PROCEDURE [transaction].[sp_UpdateDetailBarang]
    @idDetailBarang VARCHAR(50),
    @idRuangan INT,
    @spesifikasi VARCHAR(500) = NULL,
    @satuan VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [transaction].[detailBarang] 
    SET idRuangan = @idRuangan, spesifikasi = @spesifikasi, satuan = @satuan 
    WHERE idDetailBarang = @idDetailBarang;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CetakStokSummary]
    @idSemester INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @idSemester IS NULL
    BEGIN
        SELECT 
            b.namaBarang AS [NamaBarang],
            b.stok AS [Stok],
            CASE WHEN b.tipeBarang = 0 THEN 'Barang Habis Pakai' ELSE 'Aset Tetap' END AS [JenisBarang],
            b.satuan AS [Satuan]
        FROM [master].[barang] b;
    END
    ELSE
    BEGIN
        SELECT 
            b.namaBarang AS [NamaBarang],
            b.stok AS [Stok],
            CASE WHEN b.tipeBarang = 0 THEN 'Barang Habis Pakai' ELSE 'Aset Tetap' END AS [JenisBarang],
            b.satuan AS [Satuan]
        FROM [master].[barang] b
        WHERE EXISTS (
            SELECT 1 FROM [transaction].[permintaanBarang] p 
            WHERE p.idBarang = b.idBarang AND p.idSemester = @idSemester
        );
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CetakStokDetail]
    @idSemester INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @idSemester IS NULL
    BEGIN
        SELECT 
            b.namaBarang AS [NamaBarang],
            r.namaRuangan AS [Ruangan],
            CASE 
                WHEN m_latest.kondisi = 1 THEN 'Baik'
                WHEN m_latest.kondisi = 0 THEN 'Rusak'
                ELSE 'Belum Dicek'
            END AS [Kondisi],
            CASE 
                WHEN db.spesifikasi IS NOT NULL THEN db.spesifikasi 
                ELSE '-' 
            END AS [Spesifikasi],
            COUNT(db.idDetailBarang) AS [Jumlah]
        FROM [master].[barang] b
        INNER JOIN [transaction].[detailBarang] db ON b.idBarang = db.idBarang
        INNER JOIN [master].[ruangan] r ON db.idRuangan = r.idRuangan
        LEFT JOIN (
            SELECT idDetailBarang, kondisi
            FROM (
                SELECT idDetailBarang, kondisi, 
                       ROW_NUMBER() OVER(PARTITION BY idDetailBarang ORDER BY tglCek DESC, idMaintenance DESC) as rn
                FROM [transaction].[maintenance]
            ) tmp WHERE rn = 1
        ) m_latest ON db.idDetailBarang = m_latest.idDetailBarang
        WHERE b.tipeBarang = 1 
        GROUP BY b.namaBarang, r.namaRuangan, m_latest.kondisi, db.spesifikasi;
    END
    ELSE
    BEGIN
        SELECT 
            b.namaBarang AS [NamaBarang],
            r.namaRuangan AS [Ruangan],
            CASE 
                WHEN m_latest.kondisi = 1 THEN 'Baik'
                WHEN m_latest.kondisi = 0 THEN 'Rusak'
                ELSE 'Belum Dicek'
            END AS [Kondisi],
            CASE 
                WHEN db.spesifikasi IS NOT NULL THEN db.spesifikasi 
                ELSE '-' 
            END AS [Spesifikasi],
            COUNT(db.idDetailBarang) AS [Jumlah]
        FROM [master].[barang] b
        INNER JOIN [transaction].[detailBarang] db ON b.idBarang = db.idBarang
        INNER JOIN [master].[ruangan] r ON db.idRuangan = r.idRuangan
        LEFT JOIN (
            SELECT idDetailBarang, kondisi
            FROM (
                SELECT idDetailBarang, kondisi, 
                       ROW_NUMBER() OVER(PARTITION BY idDetailBarang ORDER BY tglCek DESC, idMaintenance DESC) as rn
                FROM [transaction].[maintenance]
            ) tmp WHERE rn = 1
        ) m_latest ON db.idDetailBarang = m_latest.idDetailBarang
        WHERE b.tipeBarang = 1 
        AND EXISTS (
            SELECT 1 FROM [transaction].[permintaanBarang] p 
            WHERE p.idBarang = b.idBarang AND p.idSemester = @idSemester
        )
        GROUP BY b.namaBarang, r.namaRuangan, m_latest.kondisi, db.spesifikasi;
    END
END
GO


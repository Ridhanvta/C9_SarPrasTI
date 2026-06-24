-- 1. BUAT TABEL LOG TERLEBIH DAHULU
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
GO

-- 2. BUAT TRIGGER AUDIT STOK BARANG
CREATE TRIGGER trg_AuditStokBarang
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

-- 3. BUAT TRIGGER PENCEGAH PENGHAPUSAN RIWAYAT MAINTENANCE
CREATE TRIGGER trg_PreventDeleteMaintenance
ON [transaction].[maintenance]
INSTEAD OF DELETE
AS
BEGIN
    RAISERROR ('Akses Ditolak (Database Level): Riwayat Maintenance bersifat permanen dan DILARANG KERAS untuk dihapus!', 16, 1);
    ROLLBACK TRANSACTION;
END;
GO

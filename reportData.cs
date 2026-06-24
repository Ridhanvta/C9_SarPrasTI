namespace ManajemenSarPras
{
    // Class untuk Halaman 1 (Summary Global)
    public class ReportSummary
    {
        public string NamaBarang { get; set; }
        public int Stok { get; set; }
        public string JenisBarang { get; set; }
        public string Satuan { get; set; }
    }

    // Class untuk Halaman 2 (Detail Aset Tetap berdasarkan Ruangan & Kondisi)
    public class ReportDetail
    {
        public string NamaBarang { get; set; }
        public string Ruangan { get; set; }
        public string Kondisi { get; set; }
        public int Jumlah { get; set; } // Biar ketahuan berapa yg baik, berapa yg rusak
        public string Spesifikasi { get; set; } // Menambahkan properti Spesifikasi

    }
}
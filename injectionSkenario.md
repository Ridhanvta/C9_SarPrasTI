Langkah 1: Inisiasi Keadaan Normal (Normal Behavior)

Buka aplikasi dan arahkan ke Form Detail Barang.

Pada kotak teks Cari Detail Barang, ketikkan satu huruf atau kata kunci spesifik (contoh: ketik huruf x atau kata rusak).

Hasil: Sistem Live Search berjalan normal. DataGridView hanya akan menampilkan baris data yang memiliki unsur huruf/kata tersebut. Sistem terlihat berfungsi sebagaimana mestinya.

Langkah 2: Eksekusi Serangan (The Exploit)

Saat kotak pencarian masih berisi huruf/kata tadi, klik tombol "Testing SQL Injection".

Tombol ini dirancang sebagai pemicu (trigger) yang secara diam-diam mengganti input pencarian dengan payload mematikan: Bocor%' OR 1=1 -- dan mengeksekusinya menggunakan metode String Concatenation (kode rentan).

Hasil: Sebuah MessageBox Peringatan akan muncul di layar, memperlihatkan kepada audiens bentuk query mentah (raw query) yang berhasil disuntikkan dan masuk ke mesin SQL Server.

Langkah 3: Dampak Kerusakan (The Bypass Impact)

Tutup MessageBox tersebut dan perhatikan tabel DataGridView.

Hasil: Terjadi Data Leak (Kebocoran Data) Masif. Tabel yang awalnya hanya menampilkan sedikit data hasil pencarian, kini memuntahkan SELURUH isi database sarpras. Filter pencarian telah berhasil di-bypass (ditembus) sepenuhnya oleh payload eksternal.

Langkah 4: Restorasi Arsitektur (The Fix)

Klik tombol "Kembalikan Data (Mode Aman)".

Hasil: Sistem membersihkan cache pencarian, memutuskan koneksi query kotor, dan mengembalikan mekanisme pembacaan data menggunakan View dan Stored Procedure asli yang diamankan dengan Parameterized Query. Sistem kembali kebal.
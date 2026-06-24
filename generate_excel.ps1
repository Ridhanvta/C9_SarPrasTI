$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false
$workbook = $excel.Workbooks.Add()
$worksheet = $workbook.Worksheets.Item(1)

$headers = @("NamaBarang", "Merk", "TipeBarang", "Satuan", "IsiKonversi", "StokHabisPakai", "IDDetailBarang", "Spesifikasi", "NamaGedung", "NamaRuangan")
for ($i = 0; $i -lt $headers.Length; $i++) {
    $worksheet.Cells.Item(1, $i + 1) = $headers[$i]
    $worksheet.Cells.Item(1, $i + 1).Font.Bold = $true
}

# Contoh 1: Barang Habis Pakai (Tipe = 0)
$worksheet.Cells.Item(2, 1) = "Kertas A4"
$worksheet.Cells.Item(2, 2) = "Sinar Dunia"
$worksheet.Cells.Item(2, 3) = "0"
$worksheet.Cells.Item(2, 4) = "Rim"
$worksheet.Cells.Item(2, 5) = "500"
$worksheet.Cells.Item(2, 6) = "25"
$worksheet.Cells.Item(2, 7) = ""
$worksheet.Cells.Item(2, 8) = ""
$worksheet.Cells.Item(2, 9) = ""
$worksheet.Cells.Item(2, 10) = ""

# Contoh 2: Barang Aset/Maintenance (Tipe = 1)
$worksheet.Cells.Item(3, 1) = "Proyektor LCD"
$worksheet.Cells.Item(3, 2) = "Epson"
$worksheet.Cells.Item(3, 3) = "1"
$worksheet.Cells.Item(3, 4) = "Unit"
$worksheet.Cells.Item(3, 5) = "1"
$worksheet.Cells.Item(3, 6) = ""
$worksheet.Cells.Item(3, 7) = "PRJ-EPS-001"
$worksheet.Cells.Item(3, 8) = "3000 Lumens, HDMI"
$worksheet.Cells.Item(3, 9) = "Gedung Pusat"
$worksheet.Cells.Item(3, 10) = "Ruang Rapat 1"

$worksheet.Columns.AutoFit()

$filePath = "C:\Users\INFINIX\source\repos\C9_SarPrasTI_new\Template_Import_Barang.xlsx"
if (Test-Path $filePath) { Remove-Item $filePath }
$workbook.SaveAs($filePath)
$excel.Quit()
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel)

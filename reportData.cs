using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManajemenSarPras
{
    // Cukup satu class ini aja, pastikan R-nya besar dan ada kata public
    public class ReportData
    {
        public string NamaBarang { get; set; }
        public int Stok { get; set; }
        public string JenisBarang { get; set; }
        public string Satuan { get; set; }
    }
}
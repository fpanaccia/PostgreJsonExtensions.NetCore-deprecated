using System;
using System.Collections.Generic;
using System.Text;

namespace TestStudio.DTO
{
    public class Jason
    {
        public string Str { get; set; }
        public Fecha Fecha { get; set; }
        public decimal Num { get; set; }
        public bool Logico { get; set; }
    }

    public class Fecha
    {
        public DateTime Date { get; set; }
    }
}

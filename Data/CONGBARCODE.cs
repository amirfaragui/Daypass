using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;

namespace DayPass.Data
{
    public class CONGBARCODE
    {
        [Key]
        public string CEPAN {  get; set; }
        public decimal MAXEXIT { get; set; }

        public decimal NBEXIT { get; set; }
        public DateTime? DTEXPIRE { get; set; }
        public DateTime? DTCREAT { get; set; }
        public decimal DAYVALD { get; set; }
        public string CTRACK { get; set; }
        public DateTime? LASTEXIT {  get; set; }
        public DateTime? LASTENTRY { get; set; }
        public CONGBARCODE()
        {
        }

    }
}

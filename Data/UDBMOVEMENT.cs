using System;
using System.ComponentModel.DataAnnotations;

namespace DayPass.Data
{
    public class UDBMOVEMENT
    {
        [Key]
        public decimal LGLOBALID { get; set; }
       
        public decimal SDEVICE { get; set; }
        public DateTime? TACTIONTIME { get; set; }
       
        public string CEPAN { get; set; }
      
        public UDBMOVEMENT()
        {
        }
    }
}

using System.ComponentModel.DataAnnotations;
namespace DayPass.Data
{
    public class DEVINFO
    {
        [Key]
        public decimal SROUTINSTANCEID { get; set; }
        public string CDESC { get; set; }
        public DEVINFO()
        { }

    }
}

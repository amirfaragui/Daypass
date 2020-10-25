using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ValueCards.Data
{
  public class Tenant
  {
    [Key]
    public Guid Id { get; set; }
    [StringLength(80)]
    public string Name { get; set; }
    [StringLength(256)]
    public string Address { get; set; }
    [StringLength(48)]
    public string Contact { get; set; }
    [StringLength(20)]
    public string Telephone { get; set; }
    [StringLength(64)]
    public string Email { get; set; }

    [StringLength(32)]
    public string QRCodeSeed { get; set; }

    [StringLength(48)]
    public string DataLabel1 { get; set; }

    [StringLength(48)]
    public string DataLabel2 { get; set; }

    [StringLength(48)]
    public string DataLabel3 { get; set; }

    [StringLength(48)]
    public string DataLabel4 { get; set; }

    [StringLength(48)]
    public string CheckInLabel { get; set; }

    [StringLength(48)]
    public string CheckOutLabel { get; set; }

    [StringLength(48)]
    public string ReservationLabel { get; set; }


    [StringLength(256)]
    public string PassPhrase { get; set; }

    public decimal? UnitPrice { get; set; }

    public bool AllowMultipleIssue { get; set; }

    public byte[] LogoImage { get; set; }

    public virtual ApplicationUser User { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; }
    public virtual ICollection<AmountThreshold> AmountThresholds { get; set; }
    public virtual ICollection<QuantityThreshold> QuantityThresholds { get; set; }

    [Timestamp]
    public byte[] timestamp { get; set; }

    public Tenant()
    {
      Id = Guid.NewGuid();
      Reservations = new HashSet<Reservation>();
      AmountThresholds = new HashSet<AmountThreshold>();
      QuantityThresholds = new HashSet<QuantityThreshold>();
    }
  }
}

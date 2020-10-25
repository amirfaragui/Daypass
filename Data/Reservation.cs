using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValueCards.Data
{
  public class Reservation
  {
    [Key]
    public Guid Id { get; set; }
    [ForeignKey("Tenant")]
    public Guid TenantId { get; set; }
    [JsonIgnore]
    public virtual Tenant Tenant { get; set; }

    public DateTimeOffset TimeCreated { get; set; }
    public DateTimeOffset StartValidityDate { get; set; }
    public DateTimeOffset EndValidityDate { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long SequenceNumber { get; set; }

    [StringLength(48)]
    public string GuestName { get; set; }

    [StringLength(64)]
    public string DataField1 { get; set; }

    [StringLength(64)]
    public string DataField2 { get; set; }

    [StringLength(64)]
    public string DataField3 { get; set; }

    [StringLength(64)]
    public string DataField4 { get; set; }

    [StringLength(48)]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [StringLength(512)]
    public string QRCode { get; set; }

    public decimal? Price { get; set; }

    [Timestamp]
    public byte[] timestamp { get; set; }
  }
}

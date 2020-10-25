using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValueCards.Data
{
  public class AmountThreshold
  {
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Tenant")]
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; }

    public ThresholdTerm Term { get; set; }

    public decimal? Amount { get; set; }

    [Timestamp]
    public byte[] timestamp { get; set; }

    public AmountThreshold()
    {
      Id = Guid.NewGuid();
    }
  }
}

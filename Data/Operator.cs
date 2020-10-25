using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ValueCards.Data
{
  public class Operator
  {
    [Key]
    public Guid Id { get; set; }
    [StringLength(64)]
    public string UserName { get; set; }

    [Timestamp]
    public byte[] timestamp { get; set; }

    public virtual  ApplicationUser User { get; set; }

    public bool Approved { get; set; }

    public Operator()
    {
      Id = Guid.NewGuid();
    }
  }


}

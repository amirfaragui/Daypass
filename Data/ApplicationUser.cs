using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValueCards.Data
{
  public enum UserType
  {
    OPERATOR = 0,
    TENANT = 1,
  }

  public class ApplicationUser: IdentityUser<Guid>
  {
    [PersonalData]
    public UserType Type { get; set; }


    [ForeignKey("Operator")]
    public Guid? OperatorId { get; set; }
    public virtual Operator Operator { get; set; }

    [ForeignKey("Tenant")]
    public Guid? TenantId { get; set; }
    public virtual Tenant Tenant { get; set; }

    public bool Active { get; set; }
  }

  //public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
  //{
  //  public void Configure(EntityTypeBuilder<ApplicationUser> builder)
  //  {
  //    builder.HasOne(e => e.Operator).WithOne().HasForeignKey<ApplicationUser>(c => c.OperatorId);
  //  }
  //}
}

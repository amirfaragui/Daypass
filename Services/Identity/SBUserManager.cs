using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValueCards.Models;

namespace ValueCards.Services.Identity
{
  public class SBUserManager: UserManager<SBUser>
  {
    public SBUserManager(IUserStore<SBUser> store, 
                         IOptions<IdentityOptions> optionsAccessor,
                         IPasswordHasher<SBUser> passwordHasher, 
                         IEnumerable<IUserValidator<SBUser>> userValidators,
                         IEnumerable<IPasswordValidator<SBUser>> passwordValidators,
                         ILookupNormalizer keyNormalizer, 
                         IdentityErrorDescriber errors,
                         IServiceProvider services,
                         ILogger<SBUserManager> logger) 
      :base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {

    }
  }
}

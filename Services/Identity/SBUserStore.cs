using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValueCards.Models;

namespace ValueCards.Services.Identity
{
  public class UserStore<TUser> : IUserStore<TUser> where TUser : SBUser
  {    
    private readonly IMemoryCache _cache;

    public UserStore(IMemoryCache cache)
    {
      _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public HashSet<TUser> Users
    {
      get
      {
        var users = _cache.Get("users") as HashSet<TUser>;
        if(users == null)
        {
          users = new HashSet<TUser>();
          _cache.Set("users", users, TimeSpan.FromHours(1));
        }
        return users;
      }
    }

    public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
      var users = Users;
      if (users.Add(user))
      {
        _cache.Set("users", users, TimeSpan.FromHours(1));
        return Task.FromResult(IdentityResult.Success);
      }
      return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "100", Description = "User already exists" }));
    }

    public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
      var users = Users;
      if (users.Remove(user))
      {
        _cache.Set("users", users, TimeSpan.FromHours(1));
        return Task.FromResult(IdentityResult.Success);
      }
      return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "101", Description = "User does not exists" }));
    }

    public void Dispose()
    {
    }

    public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
      var user = Users.FirstOrDefault(i => i.Id == userId);
      return Task.FromResult(user);
    }

    public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
      var user = Users.FirstOrDefault(i => i.NormalizedUserName == normalizedUserName);
      if(user == null)
      {
        user = Activator.CreateInstance<TUser>();
        user.Id = normalizedUserName;
        user.UserName = normalizedUserName;
        user.NormalizedUserName = normalizedUserName;
        
        if(await CreateAsync(user, cancellationToken) != IdentityResult.Success)
        {
          user = null;
        }
      }
      return user;
    }

    public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
      return Task.FromResult(user.NormalizedUserName);
    }

    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
      return Task.FromResult(user.Id);
    }

    public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
      return Task.FromResult(user.UserName);
    }

    public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
    {
      user.NormalizedUserName = normalizedName;
      return Task.CompletedTask;
    }

    public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
    {
      user.UserName = userName;
      return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
      var match = await FindByIdAsync(user.Id, cancellationToken);
      if(match!=null)
      {
        match.NormalizedEmail = user.NormalizedEmail;
        match.NormalizedUserName = user.NormalizedUserName;
        match.Password = user.Password;
        match.UserName = user.UserName;
        match.Email = user.Email;
        _cache.Set("users", Users, TimeSpan.FromHours(1));

        return IdentityResult.Success;
      }

      return IdentityResult.Failed(new IdentityError() { Code = "101", Description = "User does not exists" });

    }
  }

  public class SBUserStore: UserStore<SBUser>
  {
    public SBUserStore(IMemoryCache cache) : base(cache)
    {      
    }
  }
}
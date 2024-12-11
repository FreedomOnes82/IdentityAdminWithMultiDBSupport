using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Framework.Core.Blazor.Admin.MySql.Auth.Models;
using Framework.Core.Blazor.Admin.MySql.Auth.Providers;
using Dapper;
using Framework.Core.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Graph;
using Framework.Core.Blazor.Admin.MySql;

namespace Framework.Core.Blazor.Admin.MySql.Auth.Providers
{
    internal class RolesProvider : Base
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RolesProvider(IDatabaseConnectionFactory databaseConnectionFactory, IDbConnectionAccessor connectionAccessor, IDataTablePrefixProvider dataTablePrefixProvider)
            : base(connectionAccessor, dataTablePrefixProvider)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            int flag = 0;
            var repository = GetRepository<ApplicationRole>();
            await repository.InsertAsync(role);
            if (!string.IsNullOrEmpty(role.Name))
            {
                var res = await FindByNameAsync(role.NormalizedName);
                if (role.Claims != null && role.Claims.Any() && res != null)
                {
                    var RcRepository = GetRepository<AspNetRoleClaim>();
                    foreach (var claim in role.Claims)
                    {
                        await RcRepository.InsertAsync(new AspNetRoleClaim { RoleId = res.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
                    }
                    return IdentityResult.Success;
                }
                flag = res != null ? 1 : 0;
            }
            return flag == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"Error inserting role with name {role.Name}."
            });
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role)
        {
            var repository = GetRepository<ApplicationRole>();
            var isValidGuid = Guid.TryParse(role.Id, out var roleGuid);
            if (!isValidGuid)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = nameof(CreateAsync),
                    Description = $"The Role which called {role.Name} is incorrect."
                });
            }
            await repository.UpdateAsync(role);
            var newRole = await FindByIdAsync(roleGuid);
            if (newRole.Name != role.Name || newRole.NormalizedName != role.NormalizedName
            || newRole.ConcurrencyStamp != role.ConcurrencyStamp)
                return IdentityResult.Failed(new IdentityError
                {
                    Code = nameof(CreateAsync),
                    Description = $"The Role which called {role.Name} update failed."
                });
            var RcRepository = GetRepository<AspNetRoleClaim>();
            var oldClaims = (await RcRepository.GetAllAsync()).Where(x => x.RoleId == role.Id).ToList();
            if (oldClaims.Any())
            {
                foreach (var claim in oldClaims)
                {
                    await RcRepository.DeleteAsync(claim);
                }
            }

            if (role.Claims != null)
                foreach (var claim in role.Claims)
                {
                    await RcRepository.InsertAsync(new AspNetRoleClaim { RoleId = newRole.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
                }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role)
        {
            var repository = GetRepository<ApplicationRole>();
            await repository.DeleteAsync(new ApplicationRole { Id = role.Id });
            return IdentityResult.Success;
        }

        public async Task<ApplicationRole> FindByIdAsync(Guid roleId)
        {
            var repository = GetRepository<ApplicationRole>();
            var res = await repository.GetByKeyAsync(new ApplicationRole { Id = roleId.ToString() });
            return res;
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName)
        {
            var repository = GetRepository<ApplicationRole>();
            var res = await repository.GetAllAsync();
            return res.FirstOrDefault(x => x.NormalizedName == normalizedRoleName);
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync()
        {
            var repository = GetRepository<ApplicationRole>();
            var res = await repository.GetAllAsync();
            return res;
        }
    }
}

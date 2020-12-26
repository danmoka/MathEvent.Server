using MathEventWebApi.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Data
{
    public class ServerContext : IdentityDbContext<ApplicationUser>
    {
        public ServerContext(DbContextOptions<ServerContext> opt) : base(opt)
        {

        }

    }
}

﻿using Microsoft.EntityFrameworkCore;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Infrastructure;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<BuyingList> BuyingLists => Set<BuyingList>();
    public DbSet<BuyingListItem> BuyingListItems => Set<BuyingListItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    //}

}

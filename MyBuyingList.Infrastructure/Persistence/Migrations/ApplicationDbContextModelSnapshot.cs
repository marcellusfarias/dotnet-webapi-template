﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyBuyingList.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyBuyingList.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MyBuyingList.Domain.Entities.BuyingList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("NOW()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<int>("GroupId")
                        .HasColumnType("integer")
                        .HasColumnName("group_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_buying_lists");

                    b.HasIndex("CreatedBy")
                        .HasDatabaseName("ix_buying_lists_created_by");

                    b.HasIndex("GroupId")
                        .HasDatabaseName("ix_buying_lists_group_id");

                    b.ToTable("buying_lists", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CreatedBy = 1,
                            GroupId = 1,
                            Name = "Default"
                        });
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.BuyingListItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<int>("BuyingListId")
                        .HasColumnType("integer")
                        .HasColumnName("buying_list_id");

                    b.Property<bool>("Completed")
                        .HasColumnType("boolean")
                        .HasColumnName("completed");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("description");

                    b.HasKey("Id")
                        .HasName("pk_buying_list_items");

                    b.HasIndex("BuyingListId")
                        .HasDatabaseName("ix_buying_list_items_buying_list_id");

                    b.ToTable("buying_list_items", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BuyingListId = 1,
                            Completed = false,
                            Description = "Default"
                        });
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("NOW()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("group_name");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_at");

                    b.Property<int?>("LastModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("last_modified_by");

                    b.HasKey("Id")
                        .HasName("pk_groups");

                    b.HasIndex("CreatedBy")
                        .HasDatabaseName("ix_groups_created_by");

                    b.HasIndex("GroupName")
                        .IsUnique()
                        .HasDatabaseName("ix_groups_group_name");

                    b.ToTable("groups", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            CreatedBy = 1,
                            GroupName = "Default"
                        });
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.Policy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_policies");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_policies_name");

                    b.ToTable("policies", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "CreateUser"
                        },
                        new
                        {
                            Id = 2,
                            Name = "UpdateUser"
                        },
                        new
                        {
                            Id = 3,
                            Name = "DeleteUser"
                        },
                        new
                        {
                            Id = 4,
                            Name = "GetAllUsers"
                        },
                        new
                        {
                            Id = 5,
                            Name = "BuyingListGet"
                        },
                        new
                        {
                            Id = 6,
                            Name = "BuyingListCreate"
                        },
                        new
                        {
                            Id = 7,
                            Name = "BuyingListUpdate"
                        },
                        new
                        {
                            Id = 8,
                            Name = "BuyingListDelete"
                        },
                        new
                        {
                            Id = 9,
                            Name = "GroupGet"
                        },
                        new
                        {
                            Id = 10,
                            Name = "GroupCreate"
                        },
                        new
                        {
                            Id = 11,
                            Name = "GroupUpdate"
                        },
                        new
                        {
                            Id = 12,
                            Name = "GroupDelete"
                        });
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("PK_roles_id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_roles_name");

                    b.ToTable("roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Administrator"
                        },
                        new
                        {
                            Id = 2,
                            Name = "RegularUser"
                        });
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.RolePolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<int>("PolicyId")
                        .HasColumnType("integer")
                        .HasColumnName("policy_id");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_role_policies");

                    b.HasAlternateKey("RoleId", "PolicyId")
                        .HasName("ak_role_policies_role_id_policy_id");

                    b.HasIndex("PolicyId")
                        .HasDatabaseName("ix_role_policies_policy_id");

                    b.ToTable("role_policies", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            PolicyId = 1,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 2,
                            PolicyId = 2,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 3,
                            PolicyId = 3,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 4,
                            PolicyId = 4,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 5,
                            PolicyId = 5,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 6,
                            PolicyId = 6,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 7,
                            PolicyId = 7,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 8,
                            PolicyId = 8,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 9,
                            PolicyId = 9,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 10,
                            PolicyId = 10,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 11,
                            PolicyId = 11,
                            RoleId = 1
                        },
                        new
                        {
                            Id = 12,
                            PolicyId = 12,
                            RoleId = 1
                        });
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasColumnName("active")
                        .HasDefaultValueSql("FALSE");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("NOW()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("password");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("PK_users_id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasDatabaseName("ix_users_user_name");

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Active = true,
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "marcelluscfarias@gmail.com",
                            Password = "123",
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_roles");

                    b.HasAlternateKey("RoleId", "UserId")
                        .HasName("ak_user_roles_role_id_user_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_roles_user_id");

                    b.ToTable("user_roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            RoleId = 1,
                            UserId = 1
                        });
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.BuyingList", b =>
                {
                    b.HasOne("MyBuyingList.Domain.Entities.User", "UserCreated")
                        .WithMany("BuyingListCreatedBy")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_buying_lists_user_user_created_id");

                    b.HasOne("MyBuyingList.Domain.Entities.Group", "Group")
                        .WithMany("BuyingLists")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_buying_lists_group_group_id");

                    b.Navigation("Group");

                    b.Navigation("UserCreated");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.BuyingListItem", b =>
                {
                    b.HasOne("MyBuyingList.Domain.Entities.BuyingList", "BuyingList")
                        .WithMany("Items")
                        .HasForeignKey("BuyingListId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_buying_list_items_buying_lists_buying_list_id");

                    b.Navigation("BuyingList");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.Group", b =>
                {
                    b.HasOne("MyBuyingList.Domain.Entities.User", "User")
                        .WithMany("GroupsCreatedBy")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_groups_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.RolePolicy", b =>
                {
                    b.HasOne("MyBuyingList.Domain.Entities.Policy", "Policy")
                        .WithMany("RolePolicies")
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_role_policies_policies_policy_id");

                    b.HasOne("MyBuyingList.Domain.Entities.Role", "Role")
                        .WithMany("RolePolicies")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_role_policies_roles_role_id");

                    b.Navigation("Policy");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.UserRole", b =>
                {
                    b.HasOne("MyBuyingList.Domain.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_roles_roles_role_id");

                    b.HasOne("MyBuyingList.Domain.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_roles_users_user_id");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.BuyingList", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.Group", b =>
                {
                    b.Navigation("BuyingLists");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.Policy", b =>
                {
                    b.Navigation("RolePolicies");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.Role", b =>
                {
                    b.Navigation("RolePolicies");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("MyBuyingList.Domain.Entities.User", b =>
                {
                    b.Navigation("BuyingListCreatedBy");

                    b.Navigation("GroupsCreatedBy");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}

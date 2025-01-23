﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PMS.Models;

#nullable disable

namespace PMS.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PMS.Models.Lease", b =>
                {
                    b.Property<int>("LeaseID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LeaseID"));

                    b.Property<string>("LeaseAgreementFilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LeaseDuration")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LeaseEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LeaseStartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LeaseStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Pending");

                    b.Property<int?>("TenantID")
                        .HasColumnType("int");

                    b.Property<bool?>("TermsAndConditions")
                        .HasColumnType("bit");

                    b.Property<int?>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("LeaseID");

                    b.HasIndex("TenantID");

                    b.HasIndex("UnitId");

                    b.ToTable("Leases");
                });

            modelBuilder.Entity("PMS.Models.LeaseDetails", b =>
                {
                    b.Property<int>("LeaseDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LeaseDetailsId"));

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ContactNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CurrentAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmployerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmploymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LeaseID")
                        .HasColumnType("int");

                    b.Property<decimal?>("MonthlyIncome")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("LeaseDetailsId");

                    b.HasIndex("LeaseID")
                        .IsUnique()
                        .HasFilter("[LeaseID] IS NOT NULL");

                    b.ToTable("LeaseDetails");
                });

            modelBuilder.Entity("PMS.Models.MaintenanceRequest", b =>
                {
                    b.Property<int>("RequestID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequestID"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RequestStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Pending");

                    b.Property<DateTime?>("ResolutionDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StaffID")
                        .HasColumnType("int");

                    b.Property<int?>("TenantID")
                        .HasColumnType("int");

                    b.Property<int?>("UnitID")
                        .HasColumnType("int");

                    b.HasKey("RequestID");

                    b.HasIndex("StaffID");

                    b.HasIndex("TenantID");

                    b.HasIndex("UnitID");

                    b.ToTable("MaintenanceRequests");
                });

            modelBuilder.Entity("PMS.Models.Payment", b =>
                {
                    b.Property<int>("PaymentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentID"));

                    b.Property<decimal?>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("LeaseID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Pending");

                    b.HasKey("PaymentID");

                    b.HasIndex("LeaseID");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("PMS.Models.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("PMS.Models.PropertyManager", b =>
                {
                    b.Property<int>("ManagerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ManagerID"));

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ManagerID");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("PropertyManagers");
                });

            modelBuilder.Entity("PMS.Models.Request", b =>
                {
                    b.Property<int>("RequestID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequestID"));

                    b.Property<DateTime?>("CompletedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RequestDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RequestDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RequestStartDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RequestStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Pending");

                    b.Property<string>("RequestType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StaffID")
                        .HasColumnType("int");

                    b.Property<int?>("TenantID")
                        .HasColumnType("int");

                    b.HasKey("RequestID");

                    b.HasIndex("StaffID");

                    b.HasIndex("TenantID");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("PMS.Models.Staff", b =>
                {
                    b.Property<int>("StaffID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StaffID"));

                    b.Property<bool>("IsVacant")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<TimeOnly?>("ShiftEndTime")
                        .HasColumnType("time");

                    b.Property<TimeOnly?>("ShiftStartTime")
                        .HasColumnType("time");

                    b.Property<string>("StaffRole")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("StaffID");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("Staffs");
                });

            modelBuilder.Entity("PMS.Models.Tenant", b =>
                {
                    b.Property<int>("TenantID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TenantID"));

                    b.Property<bool>("IsActualTenant")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicturePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("TenantID");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("PMS.Models.Unit", b =>
                {
                    b.Property<int>("UnitID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UnitID"));

                    b.Property<string>("AvailabilityStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Available");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NumberOfBathrooms")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfBedrooms")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfFloors")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfGarages")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfUnits")
                        .HasColumnType("int");

                    b.Property<decimal?>("PricePerMonth")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("SecurityDeposit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Town")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnitName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnitOwner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnitStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Active");

                    b.Property<string>("UnitType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ZipCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UnitID");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("PMS.Models.UnitImage", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImageId"));

                    b.Property<string>("FilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("ImageId");

                    b.HasIndex("UnitId");

                    b.ToTable("UnitImages");
                });

            modelBuilder.Entity("PMS.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("ConfirmPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("TermsAndConditions")
                        .HasColumnType("bit");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PMS.Models.Lease", b =>
                {
                    b.HasOne("PMS.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID");

                    b.HasOne("PMS.Models.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId");

                    b.Navigation("Tenant");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("PMS.Models.LeaseDetails", b =>
                {
                    b.HasOne("PMS.Models.Lease", null)
                        .WithOne("LeaseDetails")
                        .HasForeignKey("PMS.Models.LeaseDetails", "LeaseID");
                });

            modelBuilder.Entity("PMS.Models.MaintenanceRequest", b =>
                {
                    b.HasOne("PMS.Models.Staff", "Staff")
                        .WithMany("MaintenanceRequests")
                        .HasForeignKey("StaffID");

                    b.HasOne("PMS.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID");

                    b.HasOne("PMS.Models.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitID");

                    b.Navigation("Staff");

                    b.Navigation("Tenant");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("PMS.Models.Payment", b =>
                {
                    b.HasOne("PMS.Models.Lease", "Lease")
                        .WithMany()
                        .HasForeignKey("LeaseID");

                    b.Navigation("Lease");
                });

            modelBuilder.Entity("PMS.Models.PropertyManager", b =>
                {
                    b.HasOne("PMS.Models.User", "User")
                        .WithOne("PropertyManager")
                        .HasForeignKey("PMS.Models.PropertyManager", "UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PMS.Models.Request", b =>
                {
                    b.HasOne("PMS.Models.Staff", "Staff")
                        .WithMany()
                        .HasForeignKey("StaffID");

                    b.HasOne("PMS.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID");

                    b.Navigation("Staff");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("PMS.Models.Staff", b =>
                {
                    b.HasOne("PMS.Models.User", "User")
                        .WithOne("Staff")
                        .HasForeignKey("PMS.Models.Staff", "UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PMS.Models.Tenant", b =>
                {
                    b.HasOne("PMS.Models.User", "User")
                        .WithOne("Tenant")
                        .HasForeignKey("PMS.Models.Tenant", "UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PMS.Models.UnitImage", b =>
                {
                    b.HasOne("PMS.Models.Unit", "Unit")
                        .WithMany("Images")
                        .HasForeignKey("UnitId");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("PMS.Models.Lease", b =>
                {
                    b.Navigation("LeaseDetails");
                });

            modelBuilder.Entity("PMS.Models.Staff", b =>
                {
                    b.Navigation("MaintenanceRequests");
                });

            modelBuilder.Entity("PMS.Models.Unit", b =>
                {
                    b.Navigation("Images");
                });

            modelBuilder.Entity("PMS.Models.User", b =>
                {
                    b.Navigation("PropertyManager");

                    b.Navigation("Staff");

                    b.Navigation("Tenant");
                });
#pragma warning restore 612, 618
        }
    }
}

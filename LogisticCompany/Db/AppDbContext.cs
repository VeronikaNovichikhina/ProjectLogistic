using System;
using System.Collections.Generic;
using LogisticCompany.Domain.Entities.Employee;
using LogisticCompany.Domain.Entities.Location;
using LogisticCompany.Domain.Entities.Orders;
using LogisticCompany.Domain.Entities.Tracking;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace LogisticCompany.Db;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientType> ClientTypes { get; set; }

    public virtual DbSet<CompanyClient> CompanyClients { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<DeliveryType> DeliveryTypes { get; set; }

    public virtual DbSet<IndividualClient> IndividualClients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }


    public virtual DbSet<ParcelTemplate> ParcelTemplates { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<StatusDelivery> StatusDeliveries { get; set; }

    public virtual DbSet<Town> Towns { get; set; }

    public virtual DbSet<Tracking> Trackings { get; set; }

    public virtual DbSet<TransportType> TransportTypes { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<DeliveryTariff> DeliveryTariffs { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchesId).HasName("PRIMARY");

            entity.ToTable("branches");

            entity.HasIndex(e => e.TownId, "FK_branches_town");

            entity.Property(e => e.BranchesId).HasColumnName("branches_ID");
            entity.Property(e => e.AddressBranches)
                .HasMaxLength(255)
                .HasColumnName("address_branches")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.NameBranches)
                .HasMaxLength(100)
                .HasColumnName("name_branches")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PhoneBranches)
                .HasMaxLength(20)
                .HasColumnName("phone_branches")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.TownId).HasColumnName("town_ID");

            entity.HasOne(d => d.Town).WithMany(p => p.Branches)
                .HasForeignKey(d => d.TownId)
                .HasConstraintName("FK_branches_town");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientsId).HasName("PRIMARY");

            entity.ToTable("clients");

            entity.HasIndex(e => e.ClientTypeId, "client_type_id");

            entity.Property(e => e.ClientsId).HasColumnName("clients_ID");
            entity.Property(e => e.ClientTypeId).HasColumnName("client_type_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.ClientType).WithMany(p => p.Clients)
                .HasForeignKey(d => d.ClientTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("clients_ibfk_1");
        });

        modelBuilder.Entity<ClientType>(entity =>
        {
            entity.HasKey(e => e.ClientTypeId).HasName("PRIMARY");

            entity.ToTable("client_types");

            entity.HasIndex(e => e.TypeName, "type_name").IsUnique();

            entity.Property(e => e.ClientTypeId).HasColumnName("client_type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<CompanyClient>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PRIMARY");

            entity.ToTable("company_clients");

            entity.HasIndex(e => e.ClientsId, "FK_company_clients_clients");

            entity.Property(e => e.CompanyId).HasColumnName("company_ID");
            entity.Property(e => e.ClientsId).HasColumnName("clients_ID");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasColumnName("company_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.Inn)
                .HasMaxLength(20)
                .HasColumnName("INN")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.Clients).WithMany(p => p.CompanyClients)
                .HasForeignKey(d => d.ClientsId)
                .HasConstraintName("FK_company_clients_clients");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PRIMARY");

            entity.ToTable("country");

            entity.Property(e => e.CountryId).HasColumnName("country_ID");
            entity.Property(e => e.CountryName)
                .HasMaxLength(100)
                .HasColumnName("country_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<DeliveryType>(entity =>
        {
            entity.HasKey(e => e.DeliveryTypeId).HasName("PRIMARY");

            entity.ToTable("delivery_type");

            entity.Property(e => e.DeliveryTypeId).HasColumnName("delivery_type_ID");
            entity.Property(e => e.NameDeliveryType)
                .HasMaxLength(50)
                .HasColumnName("name_delivery_type")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<IndividualClient>(entity =>
        {
            entity.HasKey(e => e.IndividualId).HasName("PRIMARY");

            entity.ToTable("individual_clients");

            entity.HasIndex(e => e.ClientsId, "FK_individual_clients_clients");

            entity.Property(e => e.IndividualId).HasColumnName("individual_ID");
            entity.Property(e => e.ClientsId).HasColumnName("clients_ID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PassportDateOfIssue).HasColumnName("passport_date_of_issue");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(20)
                .HasColumnName("passport_number")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PatronymicName)
                .HasMaxLength(50)
                .HasColumnName("patronymic_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.Clients).WithMany(p => p.IndividualClients)
                .HasForeignKey(d => d.ClientsId)
                .HasConstraintName("FK_individual_clients_clients");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrdersId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.PickupBranchesId, "FK_orders_branches");

            entity.HasIndex(e => e.ClientsId, "FK_orders_clients");

            entity.HasIndex(e => e.DeliveryTypeId, "FK_orders_deliveryType");

            entity.HasIndex(e => e.DestinationTownId, "FK_orders_destinationTown");

            entity.HasIndex(e => e.OriginTownId, "FK_orders_originTown");

            entity.HasIndex(e => e.TemplateId, "FK_orders_templates");

            entity.HasIndex(e => e.TransportTypeId, "FK_orders_transportType");

            entity.Property(e => e.OrdersId).HasColumnName("orders_ID");
            entity.Property(e => e.ClientsId).HasColumnName("clients_ID");
            entity.Property(e => e.CourierDestAddress)
                .HasMaxLength(80)
                .HasColumnName("courier_dest_address")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.DeliveryTypeId).HasColumnName("delivery_type_ID");
            entity.Property(e => e.DescriptionParcel)
                .HasMaxLength(255)
                .HasColumnName("description_parcel")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.DestinationBranchId).HasColumnName("destination_branch_ID");
            entity.Property(e => e.DestinationTownId).HasColumnName("destination_town_ID");
            entity.Property(e => e.FirstRecepientName)
                .HasMaxLength(50)
                .HasColumnName("first_recepient_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.HeightCm)
                .HasPrecision(10, 2)
                .HasColumnName("height_cm");
            entity.Property(e => e.LastRecepientName)
                .HasMaxLength(50)
                .HasColumnName("last_recepient_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.LengthCm)
                .HasPrecision(10, 2)
                .HasColumnName("length_cm");
            entity.Property(e => e.MiddleRecepientName)
                .HasMaxLength(50)
                .HasColumnName("middle_recepient_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.OriginTownId).HasColumnName("origin_town_ID");
            entity.Property(e => e.PhoneRecepient)
                .HasMaxLength(20)
                .HasColumnName("phone_recepient")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.PickupBranchesId).HasColumnName("pickup_branches_ID");
            entity.Property(e => e.TemplateId).HasColumnName("template_id");
            entity.Property(e => e.TransportTypeId).HasColumnName("transport_type_ID");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");
            entity.Property(e => e.WidthCm)
                .HasPrecision(10, 2)
                .HasColumnName("width_cm");

            entity.HasOne(d => d.Clients).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientsId)
                .HasConstraintName("FK_orders_clients");

            entity.HasOne(d => d.DeliveryType).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DeliveryTypeId)
                .HasConstraintName("FK_orders_deliveryType");

            entity.HasOne(d => d.DestinationTown).WithMany(p => p.OrderDestinationTowns)
                .HasForeignKey(d => d.DestinationTownId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_destinationTown");

            entity.HasOne(d => d.OriginTown).WithMany(p => p.OrderOriginTowns)
                .HasForeignKey(d => d.OriginTownId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_originTown");

            entity.HasOne(d => d.PickupBranches).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PickupBranchesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_branches");

            entity.HasOne(d => d.Template).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("FK_orders_templates");

            entity.HasOne(d => d.TransportType).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TransportTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_transportType");
            entity.Property(e => e.CreatedDate)
              .HasDefaultValueSql("CURRENT_TIMESTAMP")
              .HasColumnType("datetime")
              .HasColumnName("create_Date");
            entity.Property(e => e.OrderNumber)
              .HasMaxLength(20)
              .HasColumnName("order_number");
            entity.Property(e => e.CalculatedPrice)
              .HasPrecision(10, 2)
             .HasColumnName("calculate_price");
        });


        modelBuilder.Entity<ParcelTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PRIMARY");

            entity.ToTable("parcel_templates");

            entity.Property(e => e.TemplateId).HasColumnName("template_id");
            entity.Property(e => e.HeightCm)
                .HasColumnName("height_cm");
            entity.Property(e => e.LengthCm)
                .HasColumnName("length_cm");
            entity.Property(e => e.MaxWeight)
                .HasColumnName("max_weight");
            
            entity.Property(e => e.TemplateName)
                .HasMaxLength(100)
                .HasColumnName("template_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.WidthCm)
                .HasColumnName("width_cm");

            
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentsId).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.PaymentMethodId, "FK_payments_method");

            entity.HasIndex(e => e.OrdersId, "FK_payments_orders");

            entity.Property(e => e.PaymentsId).HasColumnName("payments_ID");
           
            entity.Property(e => e.OrdersId).HasColumnName("orders_ID");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.Amount)
               .HasMaxLength(80)
               .HasColumnName("amount");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_ID");

            entity.HasOne(d => d.Orders).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrdersId)
                .HasConstraintName("FK_payments_orders");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_payments_method");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PRIMARY");

            entity.ToTable("payment_methods");

            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_ID");
            entity.Property(e => e.MethodName)
                .HasMaxLength(50)
                .HasColumnName("method_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<StatusDelivery>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PRIMARY");

            entity.ToTable("status_delivery");

            entity.Property(e => e.StatusId).HasColumnName("status_ID");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<Town>(entity =>
        {
            entity.HasKey(e => e.TownId).HasName("PRIMARY");

            entity.ToTable("town");

            entity.HasIndex(e => e.CountryId, "FK_town_country");

            entity.Property(e => e.TownId).HasColumnName("town_ID");
            entity.Property(e => e.CountryId).HasColumnName("country_ID");
            entity.Property(e => e.TownName)
                .HasMaxLength(100)
                .HasColumnName("town_name")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");

            entity.HasOne(d => d.Country).WithMany(p => p.Towns)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_town_country");
        });

        modelBuilder.Entity<Tracking>(entity =>
        {
            entity.HasKey(e => e.TrackingsId).HasName("PRIMARY");

            entity.ToTable("trackings");

            entity.HasIndex(e => e.BranchesId, "FK_trackings_branches");

            entity.HasIndex(e => e.OrdersId, "FK_trackings_orders");

            entity.HasIndex(e => e.StatusId, "FK_trackings_status");

            entity.Property(e => e.TrackingsId).HasColumnName("trackings_ID");
            entity.Property(e => e.BranchesId).HasColumnName("branches_ID");
            entity.Property(e => e.LocationTrackings)
                .HasMaxLength(255)
                .HasColumnName("location_trackings")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
            entity.Property(e => e.OrdersId).HasColumnName("orders_ID");
            entity.Property(e => e.StatusId).HasColumnName("status_ID");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("update_date");

            entity.HasOne(d => d.Branches).WithMany(p => p.Trackings)
                .HasForeignKey(d => d.BranchesId)
                .HasConstraintName("FK_trackings_branches");

            entity.HasOne(d => d.Orders).WithMany(p => p.Trackings)
                .HasForeignKey(d => d.OrdersId)
                .HasConstraintName("FK_trackings_orders");

            entity.HasOne(d => d.Status).WithMany(p => p.Trackings)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_trackings_status");
        });

        modelBuilder.Entity<TransportType>(entity =>
        {
            entity.HasKey(e => e.TransportTypeId).HasName("PRIMARY");

            entity.ToTable("transport_type");

            entity.Property(e => e.TransportTypeId).HasColumnName("transport_type_ID");
            entity.Property(e => e.NameTransportType)
                .HasMaxLength(50)
                .HasColumnName("name_transport_type")
                .UseCollation("utf8mb3_general_ci")
                .HasCharSet("utf8mb3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'User'")
                .HasColumnType("enum('Admin','User')");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Phone).IsUnique();

          
            // Связь с Branch
            entity.HasOne(e => e.Branch)
                  .WithMany(b => b.Employees)
                  .HasForeignKey(e => e.BranchId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Связь с User
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DeliveryTariff>(entity =>
        {
            entity.HasKey(e => e.TariffId).HasName("PRIMARY");

           
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

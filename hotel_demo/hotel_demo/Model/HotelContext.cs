using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace hotel_demo.Model;

public partial class HotelContext : DbContext
{
    public HotelContext()
    {
    }

    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookService> BookServices { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Cleaningschedule> Cleaningschedules { get; set; }

    public virtual DbSet<Employeer> Employeers { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Hoterlroom> Hoterlrooms { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Roomcleaningschedule> Roomcleaningschedules { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=root;database=hotel", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Bookid).HasName("PRIMARY");

            entity.ToTable("book");

            entity.HasIndex(e => e.Employeerid, "employeerid");

            entity.HasIndex(e => e.Guestid, "guestid");

            entity.HasIndex(e => e.Roomid, "roomid");

            entity.Property(e => e.Bookid).HasColumnName("bookid");
            entity.Property(e => e.Bookingdate).HasColumnName("bookingdate");
            entity.Property(e => e.Cardstatus)
                .HasMaxLength(15)
                .HasColumnName("cardstatus");
            entity.Property(e => e.Dateofentry)
                .HasColumnType("datetime")
                .HasColumnName("dateofentry");
            entity.Property(e => e.Departuredate)
                .HasColumnType("datetime")
                .HasColumnName("departuredate");
            entity.Property(e => e.Employeerid).HasColumnName("employeerid");
            entity.Property(e => e.Guestid).HasColumnName("guestid");
            entity.Property(e => e.Payment).HasColumnName("payment");
            entity.Property(e => e.Roomid).HasColumnName("roomid");

            entity.HasOne(d => d.Employeer).WithMany(p => p.Books)
                .HasForeignKey(d => d.Employeerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("book_ibfk_3");

            entity.HasOne(d => d.Guest).WithMany(p => p.Books)
                .HasForeignKey(d => d.Guestid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("book_ibfk_1");

            entity.HasOne(d => d.Room).WithMany(p => p.Books)
                .HasForeignKey(d => d.Roomid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("book_ibfk_2");
        });

        modelBuilder.Entity<BookService>(entity =>
        {
            entity.HasKey(e => e.IdbookService).HasName("PRIMARY");

            entity.ToTable("book_service");

            entity.HasIndex(e => e.Bookid, "bookid");

            entity.HasIndex(e => e.Serviceid, "serviceid");

            entity.Property(e => e.IdbookService).HasColumnName("idbook_service");
            entity.Property(e => e.Bookid).HasColumnName("bookid");
            entity.Property(e => e.Serviceid).HasColumnName("serviceid");

            entity.HasOne(d => d.Book).WithMany(p => p.BookServices)
                .HasForeignKey(d => d.Bookid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("book_service_ibfk_2");

            entity.HasOne(d => d.Service).WithMany(p => p.BookServices)
                .HasForeignKey(d => d.Serviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("book_service_ibfk_1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Idcategory).HasName("PRIMARY");

            entity.ToTable("category");

            entity.Property(e => e.Idcategory).HasColumnName("idcategory");
            entity.Property(e => e.Cost)
                .HasPrecision(10)
                .HasColumnName("cost");
            entity.Property(e => e.Name)
                .HasMaxLength(55)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Cleaningschedule>(entity =>
        {
            entity.HasKey(e => e.Idschedule).HasName("PRIMARY");

            entity.ToTable("cleaningschedule");

            entity.HasIndex(e => e.Staffid, "staffid");

            entity.Property(e => e.Idschedule).HasColumnName("idschedule");
            entity.Property(e => e.Dayofweek)
                .HasMaxLength(12)
                .HasColumnName("dayofweek");
            entity.Property(e => e.Month)
                .HasMaxLength(20)
                .HasColumnName("month");
            entity.Property(e => e.Staffid).HasColumnName("staffid");

            entity.HasOne(d => d.Staff).WithMany(p => p.Cleaningschedules)
                .HasForeignKey(d => d.Staffid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cleaningschedule_ibfk_1");
        });

        modelBuilder.Entity<Employeer>(entity =>
        {
            entity.HasKey(e => e.Idemployeer).HasName("PRIMARY");

            entity.ToTable("employeers");

            entity.HasIndex(e => e.Roleid, "roleid");

            entity.Property(e => e.Idemployeer).HasColumnName("idemployeer");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Entrycount).HasColumnName("entrycount");
            entity.Property(e => e.Isblocked).HasColumnName("isblocked");
            entity.Property(e => e.LastEntry).HasColumnName("last entry");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(25)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Role).WithMany(p => p.Employeers)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employeers_ibfk_1");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.Idfloor).HasName("PRIMARY");

            entity.ToTable("floor");

            entity.Property(e => e.Idfloor).HasColumnName("idfloor");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.Idguest).HasName("PRIMARY");

            entity.ToTable("guest");

            entity.Property(e => e.Idguest).HasColumnName("idguest");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Passport)
                .HasMaxLength(15)
                .HasColumnName("passport");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Hoterlroom>(entity =>
        {
            entity.HasKey(e => e.Idroom).HasName("PRIMARY");

            entity.ToTable("hoterlroom");

            entity.HasIndex(e => e.Categoryid, "categoryid");

            entity.HasIndex(e => e.Floorid, "floorid");

            entity.HasIndex(e => e.Statusid, "statusid");

            entity.Property(e => e.Idroom).HasColumnName("idroom");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Cost)
                .HasPrecision(10)
                .HasColumnName("cost");
            entity.Property(e => e.Floorid).HasColumnName("floorid");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.Statusid).HasColumnName("statusid");

            entity.HasOne(d => d.Category).WithMany(p => p.Hoterlrooms)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("hoterlroom_ibfk_2");

            entity.HasOne(d => d.Floor).WithMany(p => p.Hoterlrooms)
                .HasForeignKey(d => d.Floorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("hoterlroom_ibfk_3");

            entity.HasOne(d => d.Status).WithMany(p => p.Hoterlrooms)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("hoterlroom_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Idrole).HasName("PRIMARY");

            entity.ToTable("role");

            entity.Property(e => e.Idrole).HasColumnName("idrole");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Roomcleaningschedule>(entity =>
        {
            entity.HasKey(e => e.Idroomcleaningschedule).HasName("PRIMARY");

            entity.ToTable("roomcleaningschedule");

            entity.HasIndex(e => e.Cleaningschefduleid, "cleaningschefduleid");

            entity.HasIndex(e => e.Roomid, "roomid");

            entity.Property(e => e.Idroomcleaningschedule).HasColumnName("idroomcleaningschedule");
            entity.Property(e => e.Cleaningschefduleid).HasColumnName("cleaningschefduleid");
            entity.Property(e => e.Cleaningstatus)
                .HasMaxLength(50)
                .HasColumnName("cleaningstatus");
            entity.Property(e => e.Roomid).HasColumnName("roomid");

            entity.HasOne(d => d.Cleaningschefdule).WithMany(p => p.Roomcleaningschedules)
                .HasForeignKey(d => d.Cleaningschefduleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roomcleaningschedule_ibfk_1");

            entity.HasOne(d => d.Room).WithMany(p => p.Roomcleaningschedules)
                .HasForeignKey(d => d.Roomid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roomcleaningschedule_ibfk_2");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Idservice).HasName("PRIMARY");

            entity.ToTable("service");

            entity.Property(e => e.Idservice).HasColumnName("idservice");
            entity.Property(e => e.Cost)
                .HasPrecision(10)
                .HasColumnName("cost");
            entity.Property(e => e.Descriptiom)
                .HasColumnType("text")
                .HasColumnName("descriptiom");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Idstaff).HasName("PRIMARY");

            entity.ToTable("staff");

            entity.Property(e => e.Idstaff).HasColumnName("idstaff");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Idstatus).HasName("PRIMARY");

            entity.ToTable("status");

            entity.Property(e => e.Idstatus).HasColumnName("idstatus");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Idtask).HasName("PRIMARY");

            entity.ToTable("task");

            entity.HasIndex(e => e.Idroomcleaning, "idroomcleaning");

            entity.Property(e => e.Idtask).HasColumnName("idtask");
            entity.Property(e => e.Idroomcleaning).HasColumnName("idroomcleaning");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.IdroomcleaningNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.Idroomcleaning)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("task_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

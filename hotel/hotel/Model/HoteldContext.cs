using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace hotel.Model;

public partial class HoteldContext : DbContext
{
    public HoteldContext()
    {
    }

    public HoteldContext(DbContextOptions<HoteldContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookService> BookServices { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Clint> Clints { get; set; }

    public virtual DbSet<ClintGuest> ClintGuests { get; set; }

    public virtual DbSet<CommHotel> CommHotels { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<PhotoCategory> PhotoCategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<StatusRoom> StatusRooms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=root;database=hoteld", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Idbook).HasName("PRIMARY");

            entity.ToTable("book");

            entity.HasIndex(e => e.ClientId, "client_id_idx");

            entity.HasIndex(e => e.EmployeeId, "employee_id_idx");

            entity.HasIndex(e => e.RoomId, "room_id_idx");

            entity.Property(e => e.Idbook).HasColumnName("idbook");
            entity.Property(e => e.BookingDate).HasColumnName("booking_date");
            entity.Property(e => e.CheckInDate).HasColumnName("check_in_date");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DepartureDate).HasColumnName("departure_date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Payment).HasColumnName("payment");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.StatusBook).HasColumnName("status_book");

            entity.HasOne(d => d.Client).WithMany(p => p.Books)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("client_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.Books)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_id");

            entity.HasOne(d => d.Room).WithMany(p => p.Books)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("room_id");
        });

        modelBuilder.Entity<BookService>(entity =>
        {
            entity.HasKey(e => e.IdbookService).HasName("PRIMARY");

            entity.ToTable("book_service");

            entity.HasIndex(e => e.BookId, "book_id_idx");

            entity.HasIndex(e => e.ServiceId, "service_id_idx");

            entity.Property(e => e.IdbookService).HasColumnName("idbook_service");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.Book).WithMany(p => p.BookServices)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("book_id");

            entity.HasOne(d => d.Service).WithMany(p => p.BookServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_id");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Idcategory).HasName("PRIMARY");

            entity.ToTable("category");

            entity.Property(e => e.Idcategory).HasColumnName("idcategory");
            entity.Property(e => e.Cost)
                .HasPrecision(10)
                .HasColumnName("cost");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.NumOfRoom).HasColumnName("num_of_room");
        });

        modelBuilder.Entity<Clint>(entity =>
        {
            entity.HasKey(e => e.Idclint).HasName("PRIMARY");

            entity.ToTable("clint");

            entity.Property(e => e.Idclint).HasColumnName("idclint");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.NumberPass)
                .HasMaxLength(6)
                .HasColumnName("number_pass");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.SeriaPass)
                .HasMaxLength(4)
                .HasColumnName("seria_pass");
        });

        modelBuilder.Entity<ClintGuest>(entity =>
        {
            entity.HasKey(e => e.IdclintGuest).HasName("PRIMARY");

            entity.ToTable("clint_guest");

            entity.HasIndex(e => e.Clientid, "idd_client_idx");

            entity.HasIndex(e => e.GuestIt, "idguest_idx");

            entity.Property(e => e.IdclintGuest).HasColumnName("idclint_guest");
            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.GuestIt).HasColumnName("guest_it");

            entity.HasOne(d => d.Client).WithMany(p => p.ClintGuests)
                .HasForeignKey(d => d.Clientid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idd_client");

            entity.HasOne(d => d.GuestItNavigation).WithMany(p => p.ClintGuests)
                .HasForeignKey(d => d.GuestIt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idguest");
        });

        modelBuilder.Entity<CommHotel>(entity =>
        {
            entity.HasKey(e => e.IdcommHotel).HasName("PRIMARY");

            entity.ToTable("comm_hotel");

            entity.HasIndex(e => e.IdClient, "clintidd_idx");

            entity.HasIndex(e => e.IdComm, "commid_idx");

            entity.HasIndex(e => e.IdHotel, "hotel_id_idx");

            entity.Property(e => e.IdcommHotel).HasColumnName("idcomm_hotel");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.IdComm).HasColumnName("id_comm");
            entity.Property(e => e.IdHotel).HasColumnName("id_hotel");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.CommHotels)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("clintidd");

            entity.HasOne(d => d.IdHotelNavigation).WithMany(p => p.CommHotels)
                .HasForeignKey(d => d.IdHotel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("hotel_id");

            entity.HasOne(d => d.IdCommentNavigation).WithMany(p => p.CommHotels)
            .HasForeignKey(d => d.IdComm)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("commid");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => new { e.Idcomment, e.Comment1 })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("comment");

            entity.Property(e => e.Idcomment)
                .ValueGeneratedOnAdd()
                .HasColumnName("idcomment");
            entity.Property(e => e.Comment1)
                .HasMaxLength(150)
                .HasColumnName("comment");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Stars).HasColumnName("stars");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Idemployee).HasName("PRIMARY");

            entity.ToTable("employee");

            entity.HasIndex(e => e.Idhotel, "idhotel_idx");

            entity.HasIndex(e => e.Idrole, "idrole_idx");

            entity.Property(e => e.Idemployee).HasColumnName("idemployee");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Idhotel).HasColumnName("idhotel");
            entity.Property(e => e.Idrole).HasColumnName("idrole");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .HasColumnName("phone_number");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.IdhotelNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Idhotel)
                .HasConstraintName("idhotel");

            entity.HasOne(d => d.IdroleNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Idrole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idrole");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Idfavorite).HasName("PRIMARY");

            entity.ToTable("favorite");

            entity.HasIndex(e => e.Idcategory, "idcategory_idx");

            entity.HasIndex(e => e.Idclint, "idclint_idx");

            entity.Property(e => e.Idfavorite).HasColumnName("idfavorite");
            entity.Property(e => e.Idcategory).HasColumnName("idcategory");
            entity.Property(e => e.Idclint).HasColumnName("idclint");

            entity.HasOne(d => d.IdcategoryNavigation).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.Idcategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idcategory");

            entity.HasOne(d => d.IdclintNavigation).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.Idclint)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idclint");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.Idfloor).HasName("PRIMARY");

            entity.ToTable("floor");

            entity.Property(e => e.Idfloor).HasColumnName("idfloor");
            entity.Property(e => e.Name)
                .HasMaxLength(15)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.Idguest).HasName("PRIMARY");

            entity.ToTable("guest");

            entity.Property(e => e.Idguest).HasColumnName("idguest");
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(15)
                .HasColumnName("document_number");
            entity.Property(e => e.DocumentType).HasColumnName("document_type");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Idhotel).HasName("PRIMARY");

            entity.ToTable("hotel");

            entity.Property(e => e.Idhotel).HasColumnName("idhotel");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(45)
                .HasColumnName("city");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .HasColumnName("phone_number");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.Stars).HasColumnName("stars");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Idphoto).HasName("PRIMARY");

            entity.ToTable("photo");

            entity.Property(e => e.Idphoto).HasColumnName("idphoto");
            entity.Property(e => e.Photo1).HasColumnName("photo");
        });

        modelBuilder.Entity<PhotoCategory>(entity =>
        {
            entity.HasKey(e => e.IdphotoCategory).HasName("PRIMARY");

            entity.ToTable("photo_category");

            entity.HasIndex(e => e.CategoryId, "category_id_idx");

            entity.HasIndex(e => e.PhotoId, "photo_id_idx");

            entity.Property(e => e.IdphotoCategory).HasColumnName("idphoto_category");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.PhotoId).HasColumnName("photo_id");

            entity.HasOne(d => d.Category).WithMany(p => p.PhotoCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("category_id");

            entity.HasOne(d => d.Photo).WithMany(p => p.PhotoCategories)
                .HasForeignKey(d => d.PhotoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("photo_id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Idrole).HasName("PRIMARY");

            entity.ToTable("role");

            entity.Property(e => e.Idrole).HasColumnName("idrole");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Idroom).HasName("PRIMARY");

            entity.ToTable("room");

            entity.HasIndex(e => e.Floorid, "floorid_idx");

            entity.HasIndex(e => e.Hotelid, "hotelid_idx");

            entity.HasIndex(e => e.IdCategory, "id_category_idx");

            entity.HasIndex(e => e.StatusId, "status_id_idx");

            entity.Property(e => e.Idroom).HasColumnName("idroom");
            entity.Property(e => e.Floorid).HasColumnName("floorid");
            entity.Property(e => e.Hotelid).HasColumnName("hotelid");
            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .HasColumnName("name");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Floor).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.Floorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("floorid");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.Hotelid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("hotelid");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("id_category");

            entity.HasOne(d => d.Status).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("status_id");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Idservice).HasName("PRIMARY");

            entity.ToTable("service");

            entity.Property(e => e.Idservice).HasColumnName("idservice");
            entity.Property(e => e.Cost)
                .HasPrecision(10)
                .HasColumnName("cost");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StatusRoom>(entity =>
        {
            entity.HasKey(e => e.IdstatusRoom).HasName("PRIMARY");

            entity.ToTable("status_room");

            entity.Property(e => e.IdstatusRoom).HasColumnName("idstatus_room");
            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

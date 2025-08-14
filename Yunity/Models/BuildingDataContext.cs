using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Yunity.Models;

public partial class BuildingDataContext : DbContext
{
    public BuildingDataContext()
    {
    }

    public BuildingDataContext(DbContextOptions<BuildingDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<BdList> BdLists { get; set; }

    public virtual DbSet<BdManager> BdManagers { get; set; }

    public virtual DbSet<BdMember> BdMembers { get; set; }

    public virtual DbSet<BdPermission> BdPermissions { get; set; }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Calendar> Calendars { get; set; }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<CompanyAccount> CompanyAccounts { get; set; }

    public virtual DbSet<CompanyLoginHistory> CompanyLoginHistories { get; set; }

    public virtual DbSet<CompanyProfile> CompanyProfiles { get; set; }

    public virtual DbSet<CompanyServiceArea> CompanyServiceAreas { get; set; }

    public virtual DbSet<CsAppointmentRecord> CsAppointmentRecords { get; set; }

    public virtual DbSet<CsOrderPhoto> CsOrderPhotos { get; set; }

    public virtual DbSet<CsProduct> CsProducts { get; set; }

    public virtual DbSet<CsProductPhoto> CsProductPhotos { get; set; }

    public virtual DbSet<DoorNo> DoorNos { get; set; }

    public virtual DbSet<EcpayOrder> EcpayOrders { get; set; }

    public virtual DbSet<GetPack> GetPacks { get; set; }

    public virtual DbSet<ManageFee> ManageFees { get; set; }

    public virtual DbSet<NearStore> NearStores { get; set; }

    public virtual DbSet<NearStoreCoordinate> NearStoreCoordinates { get; set; }

    public virtual DbSet<NearStoreWithBd> NearStoreWithBds { get; set; }

    public virtual DbSet<PublicArea> PublicAreas { get; set; }

    public virtual DbSet<PublicAreaReserve> PublicAreaReserves { get; set; }

    public virtual DbSet<SendPack> SendPacks { get; set; }

    public virtual DbSet<ServerHistory> ServerHistories { get; set; }

    public virtual DbSet<TManagerInfo> TManagerInfos { get; set; }

    public virtual DbSet<TManagerLoginHistory> TManagerLoginHistories { get; set; }

    public virtual DbSet<TSystemInfo> TSystemInfos { get; set; }

    public virtual DbSet<TusersInfo> TusersInfos { get; set; }

    public virtual DbSet<UserAreaReserve> UserAreaReserves { get; set; }

    public virtual DbSet<VendorCoordinate> VendorCoordinates { get; set; }

    public virtual DbSet<VisitorRecord> VisitorRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=BuildingData;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<BdList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BD_List__3213E83FFA9898B7");

            entity.ToTable("BD_List");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BdAddr)
                .HasMaxLength(100)
                .HasColumnName("BD_Addr");
            entity.Property(e => e.BdName)
                .HasMaxLength(50)
                .HasColumnName("BD_Name");
            entity.Property(e => e.BdNo)
                .HasMaxLength(50)
                .HasColumnName("BD_No");
            entity.Property(e => e.BdState).HasColumnName("BD_State");
            entity.Property(e => e.ContractEnd)
                .HasColumnType("datetime")
                .HasColumnName("Contract_End");
            entity.Property(e => e.ContractStart)
                .HasColumnType("datetime")
                .HasColumnName("Contract_Start");
            entity.Property(e => e.FirstContact)
                .HasMaxLength(50)
                .HasColumnName("First_Contact");
            entity.Property(e => e.FstContactTel)
                .HasMaxLength(50)
                .HasColumnName("Fst_Contact_Tel");
            entity.Property(e => e.HouseCount)
                .HasMaxLength(50)
                .HasColumnName("House_Count");
            entity.Property(e => e.SecContactTel)
                .HasMaxLength(50)
                .HasColumnName("Sec_Contact_Tel");
            entity.Property(e => e.SecondContact)
                .HasMaxLength(50)
                .HasColumnName("Second_Contact");
        });

        modelBuilder.Entity<BdManager>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BD_Manag__3213E83F66976C38");

            entity.ToTable("BD_Manager");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BdId).HasColumnName("BD_ID");
            entity.Property(e => e.ManagerId).HasColumnName("Manager_ID");
        });

        modelBuilder.Entity<BdMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BD_Membe__3213E83F02924CCC");

            entity.ToTable("BD_Member");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BdId).HasColumnName("BD_ID");
            entity.Property(e => e.DoorNo)
                .HasMaxLength(50)
                .HasColumnName("Door_No");
            entity.Property(e => e.DoorNoId).HasColumnName("DoorNo_ID");
            entity.Property(e => e.UserId).HasColumnName("User_ID");
        });

        modelBuilder.Entity<BdPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BD_Permi__3213E83F1EC1CA94");

            entity.ToTable("BD_Permissions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BdId).HasColumnName("BD_ID");
            entity.Property(e => e.CommunityService).HasColumnName("Community_Service");
            entity.Property(e => e.ManageFee).HasColumnName("Manage_Fee");
            entity.Property(e => e.NearbyStore).HasColumnName("Nearby_Store");
            entity.Property(e => e.PublicAreaReserve).HasColumnName("Public_Area_Reserve");
            entity.Property(e => e.ReceivePackage).HasColumnName("Receive_Package");
            entity.Property(e => e.SentPackage).HasColumnName("Sent_Package");
            entity.Property(e => e.VisitorRecord).HasColumnName("Visitor_Record");
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Board__3214EC0724AF0F3E");

            entity.ToTable("Board");

            entity.Property(e => e.BdId).HasColumnName("BD_Id");
            entity.Property(e => e.Info).HasMaxLength(500);
            entity.Property(e => e.ManagerId).HasColumnName("Manager_Id");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OpenTime)
                .HasColumnType("datetime")
                .HasColumnName("Open_Time");
            entity.Property(e => e.Photo).HasMaxLength(100);
        });

        modelBuilder.Entity<Calendar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Calendar__3214EC07320EFAB0");

            entity.ToTable("Calendar");

            entity.Property(e => e.BdId).HasColumnName("BD_Id");
            entity.Property(e => e.EventEnd)
                .HasColumnType("datetime")
                .HasColumnName("Event_End");
            entity.Property(e => e.EventName)
                .HasMaxLength(50)
                .HasColumnName("Event_Name");
            entity.Property(e => e.EventStart)
                .HasColumnType("datetime")
                .HasColumnName("Event_Start");
            entity.Property(e => e.Info).HasMaxLength(100);
            entity.Property(e => e.LogTime)
                .HasColumnType("datetime")
                .HasColumnName("Log_Time");
            entity.Property(e => e.ManagerId).HasColumnName("Manager_Id");
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chat_roo__3214EC076CA626BA");

            entity.ToTable("Chat_room");

            entity.Property(e => e.MangerId)
                .HasMaxLength(500)
                .HasColumnName("Manger_Id");
            entity.Property(e => e.MsgText)
                .HasMaxLength(500)
                .HasColumnName("Msg_Text");
            entity.Property(e => e.MsgTime)
                .HasColumnType("datetime")
                .HasColumnName("Msg_Time");
            entity.Property(e => e.RoomId)
                .HasMaxLength(500)
                .HasColumnName("Room_Id");
            entity.Property(e => e.UserId)
                .HasMaxLength(500)
                .HasColumnName("User_Id");
        });

        modelBuilder.Entity<CompanyAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Company___3214EC27DAF9E516");

            entity.ToTable("Company_Account");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ComAccount)
                .HasMaxLength(50)
                .HasColumnName("Com_Account");
            entity.Property(e => e.ComName)
                .HasMaxLength(100)
                .HasColumnName("Com_Name");
            entity.Property(e => e.ComPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Com_Password");
            entity.Property(e => e.ComPermissions).HasColumnName("Com_Permissions");
            entity.Property(e => e.ComStatus).HasColumnName("Com_Status");
            entity.Property(e => e.FAspUserId)
                .HasMaxLength(500)
                .HasDefaultValue("none")
                .HasColumnName("fAspUserId");
        });

        modelBuilder.Entity<CompanyLoginHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Company___3214EC271EDE10C3");

            entity.ToTable("Company_Login_History");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ComIpAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Com_IP_Address");
            entity.Property(e => e.ComLoginStatus)
                .HasMaxLength(20)
                .HasColumnName("Com_Login_Status");
            entity.Property(e => e.ComLoginTime)
                .HasColumnType("datetime")
                .HasColumnName("Com_Login_Time");
            entity.Property(e => e.ComLogoutTime)
                .HasColumnType("datetime")
                .HasColumnName("Com_Logout_Time");
            entity.Property(e => e.CompanyId).HasColumnName("Company_ID");
        });

        modelBuilder.Entity<CompanyProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Company___3214EC2792857A45");

            entity.ToTable("Company_Profile", tb => tb.HasTrigger("trg_UpdateComModifyTime"));

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ComAddress)
                .HasMaxLength(100)
                .HasColumnName("Com_Address");
            entity.Property(e => e.ComBusinessHours)
                .HasMaxLength(50)
                .HasColumnName("Com_Business_Hours");
            entity.Property(e => e.ComContractEndDate)
                .HasColumnType("datetime")
                .HasColumnName("Com_Contract_End_Date");
            entity.Property(e => e.ComContractStartDate)
                .HasColumnType("datetime")
                .HasColumnName("Com_Contract_Start_Date");
            entity.Property(e => e.ComEmail)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Com_Email");
            entity.Property(e => e.ComId).HasColumnName("Com_ID");
            entity.Property(e => e.ComModifyTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Com_Modify_Time");
            entity.Property(e => e.ComPerson)
                .HasMaxLength(50)
                .HasColumnName("Com_Person");
            entity.Property(e => e.ComPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Com_Phone");
            entity.Property(e => e.ComRegistrationNumber).HasColumnName("Com_Registration_Number");
            entity.Property(e => e.ComServerItem)
                .HasMaxLength(50)
                .HasColumnName("Com_Server_Item");
        });

        modelBuilder.Entity<CompanyServiceArea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Company___3214EC27B60BC8D8");

            entity.ToTable("Company_Service_Area");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ComId).HasColumnName("Com_ID");
        });

        modelBuilder.Entity<CsAppointmentRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CS_Appoi__3213E83F33A260C0");

            entity.ToTable("CS_Appointment_Record");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArNotes).HasColumnName("ar_notes");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .HasColumnName("customer_name");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.FinishDate)
                .HasColumnType("datetime")
                .HasColumnName("finish_date");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ServiceLocation)
                .HasMaxLength(255)
                .HasColumnName("service_location");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<CsOrderPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CS_Order__3213E83FB3839335");

            entity.ToTable("CS_Order_Photo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
        });

        modelBuilder.Entity<CsProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CS_Produ__3213E83F168AA24D");

            entity.ToTable("CS_Product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComId).HasColumnName("com_id");
            entity.Property(e => e.PCategory)
                .HasMaxLength(50)
                .HasColumnName("p_category");
            entity.Property(e => e.PDescription).HasColumnName("p_description");
            entity.Property(e => e.PName)
                .HasMaxLength(100)
                .HasColumnName("p_name");
            entity.Property(e => e.PPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("p_price");
            entity.Property(e => e.PStatus)
                .HasMaxLength(50)
                .HasColumnName("p_status");
            entity.Property(e => e.PStock).HasColumnName("p_stock");
        });

        modelBuilder.Entity<CsProductPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CS_Produ__3213E83F7739D61E");

            entity.ToTable("CS_Product_Photo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
        });

        modelBuilder.Entity<DoorNo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Door_No__3214EC078C7A0060");

            entity.ToTable("Door_No");

            entity.Property(e => e.BdId).HasColumnName("BD_Id");
            entity.Property(e => e.CarPark).HasColumnName("Car_Park");
            entity.Property(e => e.DoorNo1)
                .HasMaxLength(50)
                .HasColumnName("Door_No");
            entity.Property(e => e.MotorPark).HasColumnName("Motor_Park");
            entity.Property(e => e.SquareFeet)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Square_Feet");
        });

        modelBuilder.Entity<EcpayOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EcpayOrd__3214EC072777EEFE");

            entity.ToTable("EcpayOrder");

            entity.Property(e => e.MemberId).HasMaxLength(50);
            entity.Property(e => e.MerchantTradeNo).HasMaxLength(50);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentType).HasMaxLength(50);
            entity.Property(e => e.PaymentTypeChargeFee).HasMaxLength(50);
            entity.Property(e => e.RtnMsg).HasMaxLength(255);
            entity.Property(e => e.TradeDate).HasMaxLength(50);
            entity.Property(e => e.TradeNo).HasMaxLength(50);
        });

        modelBuilder.Entity<GetPack>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Get_Pack__3214EC07BFC6A422");

            entity.ToTable("Get_Pack");

            entity.Property(e => e.BdId).HasColumnName("BD_Id");
            entity.Property(e => e.DoorNoId).HasColumnName("Door_No_Id");
            entity.Property(e => e.GetUser)
                .HasMaxLength(50)
                .HasColumnName("Get_User");
            entity.Property(e => e.LogTime)
                .HasColumnType("datetime")
                .HasColumnName("Log_Time");
            entity.Property(e => e.Logistic).HasMaxLength(50);
            entity.Property(e => e.ManagerId).HasColumnName("Manager_Id");
            entity.Property(e => e.PackNo)
                .HasMaxLength(50)
                .HasColumnName("Pack_No");
            entity.Property(e => e.PackPhoto)
                .HasMaxLength(100)
                .HasColumnName("Pack_Photo");
            entity.Property(e => e.PickTime)
                .HasColumnType("datetime")
                .HasColumnName("Pick_Time");
            entity.Property(e => e.PickUser)
                .HasMaxLength(50)
                .HasColumnName("Pick_User");
        });

        modelBuilder.Entity<ManageFee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Manage_F__3214EC07E6267D22");

            entity.ToTable("Manage_Fee");

            entity.Property(e => e.CarPrice).HasColumnName("Car_Price");
            entity.Property(e => e.DoorNoId).HasColumnName("Door_No_Id");
            entity.Property(e => e.FeeEnd)
                .HasColumnType("datetime")
                .HasColumnName("Fee_End");
            entity.Property(e => e.FeeName)
                .HasMaxLength(50)
                .HasColumnName("Fee_Name");
            entity.Property(e => e.LogTime).HasColumnType("datetime");
            entity.Property(e => e.MerchantTradeNo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MotorPrice).HasColumnName("Motor_Price");
            entity.Property(e => e.PayTime)
                .HasColumnType("datetime")
                .HasColumnName("Pay_Time");
            entity.Property(e => e.PayType).HasColumnName("Pay_Type");
        });

        modelBuilder.Entity<NearStore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Near_Sto__3214EC079DE1F209");

            entity.ToTable("Near_Store", tb => tb.HasTrigger("trg_UpdateTimeOnNearStoreUpdate"));

            entity.Property(e => e.Addr).HasMaxLength(100);
            entity.Property(e => e.Info).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.NsPhone)
                .HasMaxLength(50)
                .HasColumnName("NS_Phone");
            entity.Property(e => e.OpenTime)
                .HasMaxLength(100)
                .HasColumnName("Open_Time");
            entity.Property(e => e.Photo).HasMaxLength(100);
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<NearStoreCoordinate>(entity =>
        {
            entity.HasKey(e => e.NearStoreCoordinatesId).HasName("PK__NearStor__1B1DCEB27BDDDCA5");

            entity.Property(e => e.NearStoreCoordinatesId).HasColumnName("NearStoreCoordinatesID");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.NearStoreId).HasColumnName("NearStoreID");
        });

        modelBuilder.Entity<NearStoreWithBd>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Near_Store_withBD");

            entity.Property(e => e.BdId).HasColumnName("BD_Id");
            entity.Property(e => e.NearStoreId).HasColumnName("Near_Store_Id");
            entity.Property(e => e.NearStoreWithBdId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Near_Store_withBD_ID");
            entity.Property(e => e.State).HasDefaultValue(0);
        });

        modelBuilder.Entity<PublicArea>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Public_Area");

            entity.Property(e => e.Icont)
                .HasMaxLength(100)
                .HasColumnName("icont");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PaId).HasColumnName("Pa_Id");
            entity.Property(e => e.PaName)
                .HasMaxLength(100)
                .HasColumnName("Pa_Name");
        });

        modelBuilder.Entity<PublicAreaReserve>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Public_A__3214EC07E2B07185");

            entity.ToTable("Public_Area_Reserve");

            entity.Property(e => e.AreaInfo)
                .HasMaxLength(500)
                .HasColumnName("Area_Info");
            entity.Property(e => e.BdId).HasColumnName("Bd_Id");
            entity.Property(e => e.CloseTime)
                .HasPrecision(0)
                .HasColumnName("Close_Time");
            entity.Property(e => e.DeductUnit).HasColumnName("Deduct_Unit");
            entity.Property(e => e.Icon).HasColumnName("icon");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OpenTime)
                .HasPrecision(0)
                .HasColumnName("Open_Time");
            entity.Property(e => e.Photo)
                .HasMaxLength(100)
                .HasColumnName("photo");
            entity.Property(e => e.UseTime)
                .HasMaxLength(50)
                .HasColumnName("Use_Time");
            entity.Property(e => e.UseTimeUnit)
                .HasColumnType("decimal(3, 1)")
                .HasColumnName("Use_Time_Unit");
        });

        modelBuilder.Entity<SendPack>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Send_Pac__3214EC07204B8305");

            entity.ToTable("Send_Pack");

            entity.Property(e => e.GetTel)
                .HasMaxLength(50)
                .HasColumnName("Get_Tel");
            entity.Property(e => e.GetUser)
                .HasMaxLength(50)
                .HasColumnName("Get_User");
            entity.Property(e => e.Logistic).HasMaxLength(50);
            entity.Property(e => e.ManagerId).HasColumnName("Manager_Id");
            entity.Property(e => e.PackPhoto)
                .HasMaxLength(100)
                .HasColumnName("Pack_Photo");
            entity.Property(e => e.PickLogisticTime)
                .HasColumnType("datetime")
                .HasColumnName("Pick_Logistic_Time");
            entity.Property(e => e.PickTime)
                .HasColumnType("datetime")
                .HasColumnName("Pick_Time");
            entity.Property(e => e.PickUser)
                .HasMaxLength(50)
                .HasColumnName("Pick_User");
            entity.Property(e => e.SendAddr)
                .HasMaxLength(100)
                .HasColumnName("Send_Addr");
            entity.Property(e => e.UserId).HasColumnName("User_Id");
        });

        modelBuilder.Entity<ServerHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Server_H__3214EC274377EB4A");

            entity.ToTable("Server_History");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CompanyId).HasColumnName("Company_ID");
            entity.Property(e => e.ServiceDate)
                .HasColumnType("datetime")
                .HasColumnName("Service_Date");
            entity.Property(e => e.ServiceFeedback)
                .HasMaxLength(200)
                .HasColumnName("Service_Feedback");
            entity.Property(e => e.ServiceStatus)
                .HasMaxLength(50)
                .HasColumnName("Service_Status");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .HasColumnName("Service_Type");
            entity.Property(e => e.ServiceWho)
                .HasMaxLength(50)
                .HasColumnName("Service_Who");
        });

        modelBuilder.Entity<TManagerInfo>(entity =>
        {
            entity.HasKey(e => e.FId).HasName("PK__tManager__D9F8227CEADAC4B1");

            entity.ToTable("tManager_Info");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FAccount)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fAccount");
            entity.Property(e => e.FAspUserId)
                .HasMaxLength(500)
                .HasDefaultValue("none")
                .HasColumnName("fAspUserId");
            entity.Property(e => e.FBuildingId).HasColumnName("fBuilding_id");
            entity.Property(e => e.FEmail)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fEmail");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fName");
            entity.Property(e => e.FPassword)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fPassword");
            entity.Property(e => e.FPhone)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fPhone");
            entity.Property(e => e.IsApproved)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
        });

        modelBuilder.Entity<TManagerLoginHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tManager__3214EC27A7F22B97");

            entity.ToTable("tManager_Login_History");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FIpAddress)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fIP_address");
            entity.Property(e => e.FLoginStatus)
                .HasMaxLength(20)
                .HasDefaultValue("none")
                .HasColumnName("fLogin_status");
            entity.Property(e => e.FLoginTime)
                .HasColumnType("datetime")
                .HasColumnName("fLogin_time");
            entity.Property(e => e.FLogoutTime)
                .HasColumnType("datetime")
                .HasColumnName("fLogout_time");
            entity.Property(e => e.FManId).HasColumnName("fMan_id");
        });

        modelBuilder.Entity<TSystemInfo>(entity =>
        {
            entity.HasKey(e => e.FId).HasName("PK__tSystem___D9F8227C3BE3696F");

            entity.ToTable("tSystem_Info");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FAccount)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fAccount");
            entity.Property(e => e.FEmail)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fEmail");
            entity.Property(e => e.FImage)
                .HasMaxLength(100)
                .HasDefaultValue("none")
                .HasColumnName("fImage");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fName");
            entity.Property(e => e.FPassword)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fPassword");
            entity.Property(e => e.FPhone)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fPhone");
        });

        modelBuilder.Entity<TusersInfo>(entity =>
        {
            entity.HasKey(e => e.FId).HasName("PK__tusers_I__D9F8227C61CD62CC");

            entity.ToTable("tusers_Info");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FAccount)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fAccount");
            entity.Property(e => e.FAspUserId)
                .HasMaxLength(500)
                .HasDefaultValue("none")
                .HasColumnName("fAspUserId");
            entity.Property(e => e.FBuildingId).HasColumnName("fBuilding_id");
            entity.Property(e => e.FEmail)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fEmail");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fName");
            entity.Property(e => e.FPassword)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fPassword");
            entity.Property(e => e.FPhone)
                .HasMaxLength(15)
                .HasDefaultValue("none")
                .HasColumnName("fPhone");
            entity.Property(e => e.FUserAddress)
                .HasMaxLength(50)
                .HasDefaultValue("none")
                .HasColumnName("fUser_address");
            entity.Property(e => e.QrCord).HasColumnName("QR_Cord");
        });

        modelBuilder.Entity<UserAreaReserve>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User_Are__3214EC07168C981D");

            entity.ToTable("User_Area_Reserve");

            entity.Property(e => e.AreaId).HasColumnName("Area_Id");
            entity.Property(e => e.DoorNoId).HasColumnName("Door_No_Id");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("End_Time");
            entity.Property(e => e.ReserveTime)
                .HasColumnType("datetime")
                .HasColumnName("Reserve_Time");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("Start_Time");
            entity.Property(e => e.UserId).HasColumnName("User_Id");
        });

        modelBuilder.Entity<VendorCoordinate>(entity =>
        {
            entity.HasKey(e => e.VendorCoordinatesId).HasName("PK__VendorCo__D81DF3962E8E6FFF");

            entity.Property(e => e.VendorCoordinatesId).HasColumnName("VendorCoordinatesID");
            entity.Property(e => e.CompanyProfileId).HasColumnName("CompanyProfileID");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
        });

        modelBuilder.Entity<VisitorRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Visitor___3214EC0792815389");

            entity.ToTable("Visitor_Record");

            entity.Property(e => e.BdId).HasColumnName("Bd_Id");
            entity.Property(e => e.DoorNoId).HasColumnName("Door_No_Id");
            entity.Property(e => e.ManagerId).HasColumnName("Manager_Id");
            entity.Property(e => e.VisitReason)
                .HasMaxLength(100)
                .HasColumnName("Visit_Reason");
            entity.Property(e => e.VisitTime)
                .HasColumnType("datetime")
                .HasColumnName("Visit_Time");
            entity.Property(e => e.VisitorName)
                .HasMaxLength(50)
                .HasColumnName("Visitor_Name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

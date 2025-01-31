USE [master]
GO
/****** Object:  Database [TravelDB]    Script Date: 1/7/2025 3:22:27 PM ******/
CREATE DATABASE [TravelDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TravelDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TravelDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TravelDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TravelDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TravelDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TravelDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TravelDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TravelDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TravelDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TravelDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TravelDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [TravelDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TravelDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TravelDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TravelDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TravelDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TravelDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TravelDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TravelDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TravelDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TravelDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TravelDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TravelDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TravelDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TravelDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TravelDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TravelDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TravelDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TravelDB] SET RECOVERY FULL 
GO
ALTER DATABASE [TravelDB] SET  MULTI_USER 
GO
ALTER DATABASE [TravelDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TravelDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TravelDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TravelDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TravelDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TravelDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'TravelDB', N'ON'
GO
ALTER DATABASE [TravelDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [TravelDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TravelDB]
GO
/****** Object:  Table [dbo].[Accommodations]    Script Date: 1/7/2025 3:22:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accommodations](
	[AccommodationId] [int] IDENTITY(1,1) NOT NULL,
	[AccommodationName] [nvarchar](100) NOT NULL,
	[AccommodationType] [nvarchar](100) NOT NULL,
	[AccommodationLocation] [nvarchar](100) NOT NULL,
	[AccommodationDescription] [nvarchar](255) NOT NULL,
	[UserId] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AccommodationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Activities]    Script Date: 1/7/2025 3:22:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Activities](
	[ActivityId] [int] IDENTITY(1,1) NOT NULL,
	[ActivityName] [nvarchar](100) NOT NULL,
	[ActivityType] [nvarchar](100) NOT NULL,
	[ActivityDescription] [nvarchar](255) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingsAccommodations]    Script Date: 1/7/2025 3:22:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingsAccommodations](
	[BookingId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[AccommodationId] [int] NOT NULL,
	[CheckInDate] [date] NOT NULL,
	[CheckOutDate] [date] NOT NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingsActivities]    Script Date: 1/7/2025 3:22:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingsActivities](
	[BookingsActivitiesId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ActivityId] [int] NOT NULL,
	[StartHour] [time](7) NOT NULL,
	[StopHour] [time](7) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BookingsActivitiesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewsAccommodations]    Script Date: 1/7/2025 3:22:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewsAccommodations](
	[ReviewId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[AccommodationId] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[Comment] [nvarchar](1000) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewsActivities]    Script Date: 1/7/2025 3:22:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewsActivities](
	[ReviewId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ActivityId] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[Comment] [nvarchar](1000) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 1/7/2025 3:22:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[UserPassword] [nvarchar](255) NOT NULL,
	[UserType] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[FullName]  AS (([FirstName]+' ')+[LastName]),
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ReviewsAccommodations] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewsActivities] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Accommodations]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BookingsAccommodations]  WITH CHECK ADD FOREIGN KEY([AccommodationId])
REFERENCES [dbo].[Accommodations] ([AccommodationId])
GO
ALTER TABLE [dbo].[BookingsAccommodations]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BookingsActivities]  WITH CHECK ADD FOREIGN KEY([ActivityId])
REFERENCES [dbo].[Activities] ([ActivityId])
GO
ALTER TABLE [dbo].[BookingsActivities]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ReviewsAccommodations]  WITH CHECK ADD FOREIGN KEY([AccommodationId])
REFERENCES [dbo].[Accommodations] ([AccommodationId])
GO
ALTER TABLE [dbo].[ReviewsAccommodations]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ReviewsActivities]  WITH CHECK ADD FOREIGN KEY([ActivityId])
REFERENCES [dbo].[Activities] ([ActivityId])
GO
ALTER TABLE [dbo].[ReviewsActivities]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ReviewsAccommodations]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
ALTER TABLE [dbo].[ReviewsActivities]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
USE [master]
GO
ALTER DATABASE [TravelDB] SET  READ_WRITE 
GO

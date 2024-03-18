USE [master]
GO
/****** Object:  Database [PackageDelivery]    Script Date: 3/29/2024 1:05:27 PM ******/
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PackageDelivery')
BEGIN
    -- Use single-user mode to drop the database to disconnect any active connections
    ALTER DATABASE [PackageDelivery] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [PackageDelivery]
END
GO

CREATE DATABASE [PackageDelivery]
 CONTAINMENT = NONE
 ON  PRIMARY
( NAME = N'PackageDelivery', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\PackageDelivery.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON
( NAME = N'PackageDelivery_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\PackageDelivery_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [PackageDelivery] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
BEGIN
    EXEC [PackageDelivery].[dbo].[sp_fulltext_database] @action = 'enable'
END
GO
ALTER DATABASE [PackageDelivery] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [PackageDelivery] SET ANSI_NULLS OFF
GO
ALTER DATABASE [PackageDelivery] SET ANSI_PADDING OFF
GO
ALTER DATABASE [PackageDelivery] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [PackageDelivery] SET ARITHABORT OFF
GO
ALTER DATABASE [PackageDelivery] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [PackageDelivery] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [PackageDelivery] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [PackageDelivery] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [PackageDelivery] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [PackageDelivery] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [PackageDelivery] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [PackageDelivery] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [PackageDelivery] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [PackageDelivery] SET  ENABLE_BROKER
GO
ALTER DATABASE [PackageDelivery] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [PackageDelivery] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [PackageDelivery] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [PackageDelivery] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [PackageDelivery] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [PackageDelivery] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [PackageDelivery] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [PackageDelivery] SET RECOVERY FULL
GO
ALTER DATABASE [PackageDelivery] SET  MULTI_USER
GO
ALTER DATABASE [PackageDelivery] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [PackageDelivery] SET DB_CHAINING OFF
GO
ALTER DATABASE [PackageDelivery] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF )
GO
ALTER DATABASE [PackageDelivery] SET TARGET_RECOVERY_TIME = 60 SECONDS
GO
ALTER DATABASE [PackageDelivery] SET DELAYED_DURABILITY = DISABLED
GO
ALTER DATABASE [PackageDelivery] SET ACCELERATED_DATABASE_RECOVERY = OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'PackageDelivery', N'ON'
GO
ALTER DATABASE [PackageDelivery] SET QUERY_STORE = ON
GO
ALTER DATABASE [PackageDelivery] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PackageDelivery]
GO

/****** Object:  User [packageDelivery]    Script Date: 3/29/2024 1:05:27 PM ******/
CREATE USER [packageDelivery] FOR LOGIN [packageDelivery] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [packageDelivery]
GO
/****** Object:  Table [dbo].[ADDR_TBL]    Script Date: 3/29/2024 1:05:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ADDR_TBL](
	[ID_CLM] [int] IDENTITY(1,1) NOT NULL,
	[STR] [char](300) NULL,
	[CT_ST] [char](200) NULL,
	[ZP] [char](5) NULL,
	[DLVR] [int] NULL,
 CONSTRAINT [PK_ADDR_TBL] PRIMARY KEY CLUSTERED
(
	[ID_CLM] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DLVR_TBL]    Script Date: 3/29/2024 1:05:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DLVR_TBL](
	[NMB_CLM] [int] IDENTITY(1,1) NOT NULL,
	[CSTM] [int] NULL,
	[STS] [char](1) NULL,
	[ESTM_CLM] [float] NULL,
	[PRD_LN_1] [int] NULL,
	[PRD_LN_1_AMN] [char](2) NULL,
	[PRD_LN_2] [int] NULL,
	[PRD_LN_2_AMN] [char](2) NULL,
 CONSTRAINT [PK_DLVR_TBL] PRIMARY KEY CLUSTERED
(
	[NMB_CLM] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DLVR_TBL2]    Script Date: 3/29/2024 1:05:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DLVR_TBL2](
	[NMB_CLM] [int] NOT NULL,
	[PRD_LN_3] [int] NULL,
	[PRD_LN_3_AMN] [char](2) NULL,
	[PRD_LN_4] [int] NULL,
	[PRD_LN_4_AMN] [char](2) NULL,
 CONSTRAINT [PK_DLVR_TBL2] PRIMARY KEY CLUSTERED
(
	[NMB_CLM] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[P_TBL]    Script Date: 3/29/2024 1:05:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[P_TBL](
	[NMB_CLM] [int] IDENTITY(1,1) NOT NULL,
	[NM_CLM] [char](100) NULL,
	[ADDR1] [int] NULL,
	[ADDR2] [int] NULL,
 CONSTRAINT [PK_P_TBL] PRIMARY KEY CLUSTERED
(
	[NMB_CLM] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PRD_TBL]    Script Date: 3/29/2024 1:05:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PRD_TBL](
	[NMB_CM] [int] IDENTITY(1,1) NOT NULL,
	[NM_CLM] [char](100) NULL,
	[DSC_CLM] [char](500) NULL,
	[WT] [float] NULL,
	[WT_KG] [float] NULL
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ADDR_TBL] ON

INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (1, N'1234 Main St                                                                                                                                                                                                                                                                                                ', N'Washington DC                                                                                                                                                                                           ', N'22200', NULL)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (2, N'2345 2nd St                                                                                                                                                                                                                                                                                                 ', N'Washington DC                                                                                                                                                                                           ', N'22201', NULL)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (3, N'8338 3rd St                                                                                                                                                                                                                                                                                                 ', N'Arlington VA                                                                                                                                                                                            ', N'22202', NULL)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (11, N'1234 Main St                                                                                                                                                                                                                                                                                                ', N'Washington DC                                                                                                                                                                                           ', N'22200', 8)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (12, N'1321 S Eads St                                                                                                                                                                                                                                                                                              ', N'Arlingron VA                                                                                                                                                                                            ', N'22202', 9)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (13, N'Hello Word                                                                                                                                                                                                                                                                                                  ', N'Mat ebal                                                                                                                                                                                                ', N'22200', 10)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (14, N'Hello word                                                                                                                                                                                                                                                                                                  ', N'This is my city                                                                                                                                                                                         ', N'12312', 11)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (15, N'Mini soft                                                                                                                                                                                                                                                                                                   ', N'LA CA                                                                                                                                                                                                   ', N'1234 ', 12)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (16, N'Mini Soft 12345                                                                                                                                                                                                                                                                                             ', N'Tatarstan Astana                                                                                                                                                                                        ', N'12345', 13)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (17, N'My Address                                                                                                                                                                                                                                                                                                  ', N'California love                                                                                                                                                                                         ', N'1234 ', 14)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (18, N'Redmond, Somewhere in the US                                                                                                                                                                                                                                                                                ', N'City State                                                                                                                                                                                              ', N'82139', 15)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (19, N'aarstarstarst                                                                                                                                                                                                                                                                                               ', N'a b                                                                                                                                                                                                     ', N'12341', 16)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (20, N'Work it make it do it makes us                                                                                                                                                                                                                                                                              ', N'Harder better faster stronger                                                                                                                                                                           ', N'12345', 17)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (21, N'Now now now now that don''t kill                                                                                                                                                                                                                                                                             ', N'Can only make me stronger                                                                                                                                                                               ', N'12345', 18)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (22, N'Kwik e-wat???                                                                                                                                                                                                                                                                                               ', N'Dagestand Brotishka                                                                                                                                                                                     ', N'13245', 19)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (23, N'Mini Sota                                                                                                                                                                                                                                                                                                   ', N'Makro sota                                                                                                                                                                                              ', N'12345', 20)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (24, N'Kwik e-wattt??                                                                                                                                                                                                                                                                                              ', N'Urzik istan                                                                                                                                                                                             ', N'12345', 21)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (25, N'NE macro soft but apple                                                                                                                                                                                                                                                                                     ', N'Where is apple                                                                                                                                                                                          ', N'12345', 22)
INSERT [dbo].[ADDR_TBL] ([ID_CLM], [STR], [CT_ST], [ZP], [DLVR]) VALUES (26, N'I pull up and say shit                                                                                                                                                                                                                                                                                      ', N'Gotabraclet onmynecklace                                                                                                                                                                                ', N'12345', 23)
SET IDENTITY_INSERT [dbo].[ADDR_TBL] OFF
GO
SET IDENTITY_INSERT [dbo].[DLVR_TBL] ON

INSERT [dbo].[DLVR_TBL] ([NMB_CLM], [CSTM], [STS], [ESTM_CLM], [PRD_LN_1], [PRD_LN_1_AMN], [PRD_LN_2], [PRD_LN_2_AMN]) VALUES (20, 1, N'R', 120.2, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[DLVR_TBL] OFF
GO
SET IDENTITY_INSERT [dbo].[P_TBL] ON

INSERT [dbo].[P_TBL] ([NMB_CLM], [NM_CLM], [ADDR1], [ADDR2]) VALUES (1, N'Macro Soft                                                                                          ', 1, NULL)
INSERT [dbo].[P_TBL] ([NMB_CLM], [NM_CLM], [ADDR1], [ADDR2]) VALUES (2, N'Devices for Everyone                                                                                ', 2, NULL)
INSERT [dbo].[P_TBL] ([NMB_CLM], [NM_CLM], [ADDR1], [ADDR2]) VALUES (3, N'Kwik-E-Mart                                                                                         ', 3, NULL)
SET IDENTITY_INSERT [dbo].[P_TBL] OFF
GO
SET IDENTITY_INSERT [dbo].[PRD_TBL] ON

INSERT [dbo].[PRD_TBL] ([NMB_CM], [NM_CLM], [DSC_CLM], [WT], [WT_KG]) VALUES (1, N'Best Pizza Ever                                                                                     ', N'Your favorite cheese pizza made with classic marinara sauce topped with mozzarella cheese.                                                                                                                                                                                                                                                                                                                                                                                                                          ', 2, NULL)
INSERT [dbo].[PRD_TBL] ([NMB_CM], [NM_CLM], [DSC_CLM], [WT], [WT_KG]) VALUES (2, N'myPhone                                                                                             ', NULL, NULL, 0.5)
INSERT [dbo].[PRD_TBL] ([NMB_CM], [NM_CLM], [DSC_CLM], [WT], [WT_KG]) VALUES (3, N'Couch                                                                                               ', N'Made with a sturdy wood frame and upholstered in touchable and classic linen, this fold-down futon provides a stylish seating solution along with an extra space for overnight guests.                                                                                                                                                                                                                                                                                                                              ', 83.5, NULL)
INSERT [dbo].[PRD_TBL] ([NMB_CM], [NM_CLM], [DSC_CLM], [WT], [WT_KG]) VALUES (4, N'TV Set                                                                                              ', NULL, NULL, 7)
INSERT [dbo].[PRD_TBL] ([NMB_CM], [NM_CLM], [DSC_CLM], [WT], [WT_KG]) VALUES (5, N'Fridge                                                                                              ', NULL, NULL, 34)
SET IDENTITY_INSERT [dbo].[PRD_TBL] OFF
GO
USE [master]
GO
ALTER DATABASE [PackageDelivery] SET  READ_WRITE
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ALittleLeafDecor')
BEGIN
    CREATE DATABASE [ALittleLeafDecor];
END
GO
USE [ALittleLeafDecor]
GO
/****** Object:  Table [dbo].[Address_List]    Script Date: 21/10/2025 5:52:50 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address_List](
	[adrs_id] [int] IDENTITY(1,1) NOT NULL,
	[id_user] [bigint] NOT NULL,
	[adrs_fullname] [nvarchar](255) NOT NULL,
	[adrs_address] [nvarchar](255) NOT NULL,
	[adrs_phone] [nvarchar](10) NOT NULL,
	[adrs_isDefault] [bit] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[adrs_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bill]    Script Date: 21/10/2025 5:52:50 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bill](
	[bill_id] [int] IDENTITY(1,1) NOT NULL,
	[id_user] [bigint] NOT NULL,
	[id_adrs] [int] NOT NULL,
	[date_created] [date] NOT NULL,
	[total_amount] [int] NOT NULL,
	[payment_method] [nvarchar](50) NOT NULL,
	[payment_status] [nvarchar](50) NOT NULL,
	[is_confirmed] [bit] NOT NULL,
	[shipping_status] [nvarchar](50) NOT NULL,
	[note] [nvarchar](255) NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[bill_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bill_Detail]    Script Date: 21/10/2025 5:52:50 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bill_Detail](
	[bill_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[id_bill] [int] NOT NULL,
	[id_product] [int] NOT NULL,
	[quantity] [int] NOT NULL,
	[unit_price] [int] NOT NULL,
	[total_price] [int] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[bill_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 21/10/2025 5:52:50 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[category_id] [int] IDENTITY(1,1) NOT NULL,
	[category_name] [nvarchar](255) NOT NULL,
	[category_parent_id] [int] NULL,
	[category_img] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[category_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 21/10/2025 5:52:50 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[product_id] [int] IDENTITY(1,1) NOT NULL,
	[id_category] [int] NOT NULL,
	[product_name] [nvarchar](255) NOT NULL,
	[product_price] [int] NOT NULL,
	[product_description] [nvarchar](max) NULL,
	[quantity_in_stock] [int] NOT NULL,
	[is_onSale] [bit] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
 CONSTRAINT [PK__Product__47027DF5D9D271A0] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product_Images]    Script Date: 21/10/2025 5:52:50 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product_Images](
	[img_id] [int] IDENTITY(1,1) NOT NULL,
	[id_product] [int] NOT NULL,
	[img_name] [nvarchar](255) NOT NULL,
	[is_primary] [bit] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[img_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 21/10/2025 5:52:50 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[user_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_email] [nvarchar](255) NOT NULL,
	[user_password] [nvarchar](255) NOT NULL,
	[user_fullname] [nvarchar](255) NOT NULL,
	[user_sex] [bit] NOT NULL,
	[user_birthday] [date] NOT NULL,
	[user_isActive] [bit] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NOT NULL,
	[user_role] [varchar](20) NOT NULL,
 CONSTRAINT [PK__User__B9BE370FE2FFFB58] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Address_List] ON 

INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1040, 35, N'La Hoành Nghiệp', N'439/41 Hồ Học Lãm, P. An Lạc, Q. Bình Tân, TP. Hồ Chí Minh', N'0832963381', 0, CAST(N'2024-12-18T01:30:30.903' AS DateTime), CAST(N'2025-09-24T16:59:29.290' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1041, 36, N'Nguyễn Xuân Thương', N'335 An Dương Vương, Q5', N'0933273851', 1, CAST(N'2024-12-18T01:32:17.137' AS DateTime), CAST(N'2024-12-18T01:39:01.933' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1042, 37, N'Hạ Phụng Việt', N'Xã An Tây, Thị xã Bến Cát, Bình Dương', N'0936223455', 1, CAST(N'2024-12-18T01:32:52.380' AS DateTime), CAST(N'2024-12-18T01:39:10.317' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1043, 38, N'Hồ Phương Quyên', N'Đường ở Q8, Phường 11, Quận 8,Thành phố Hồ Chí Minh', N'0833275832', 1, CAST(N'2024-12-18T01:33:37.857' AS DateTime), CAST(N'2024-12-18T01:39:16.570' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1044, 39, N'Cồ Bích Hà', N'Xã Đông Thành, Thị xã Bình Minh, Vĩnh Long', N'0901229983', 1, CAST(N'2024-12-18T01:35:07.250' AS DateTime), CAST(N'2024-12-18T01:39:27.717' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1045, 35, N'Nghiệp', N'335 An Dương Vương, Q5', N'0832963381', 1, CAST(N'2024-12-18T02:10:06.897' AS DateTime), CAST(N'2025-09-24T16:59:29.290' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1046, 40, N'Nhã Thi', N'439 Hậu Giang, Q6', N'N/A', 1, CAST(N'2024-12-18T02:22:23.380' AS DateTime), CAST(N'2024-12-18T02:22:23.380' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1047, 41, N'Nhã Lệ', N'41 Hồ Học Lãm, An Lạc, Bình Tân', N'N/A', 1, CAST(N'2024-12-18T02:23:32.287' AS DateTime), CAST(N'2024-12-18T02:23:32.287' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1048, 42, N'Phạm Minh Toàn', N'Đường ở Q8, Phường 11, Quận 8,Thành phố Hồ Chí Minh', N'N/A', 1, CAST(N'2024-12-18T02:27:47.437' AS DateTime), CAST(N'2024-12-18T02:27:47.437' AS DateTime))
INSERT [dbo].[Address_List] ([adrs_id], [id_user], [adrs_fullname], [adrs_address], [adrs_phone], [adrs_isDefault], [created_at], [updated_at]) VALUES (1049, 43, N'Hồ Phương Quyên', N'Đường ở Q8, Phường 11, Quận 8,Thành phố Hồ Chí Minh', N'N/A', 1, CAST(N'2024-12-18T09:47:07.800' AS DateTime), CAST(N'2024-12-18T09:47:07.800' AS DateTime))
SET IDENTITY_INSERT [dbo].[Address_List] OFF
GO
SET IDENTITY_INSERT [dbo].[Bill] ON 

INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (7, 35, 1040, CAST(N'2024-06-12' AS Date), 270000, N'online', N'paid', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:06:29.527' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (8, 35, 1040, CAST(N'2024-10-20' AS Date), 4550000, N'cod', N'paid', 1, N'not_fulfilled', N'giao gấp cho em nhe', CAST(N'2024-12-18T02:51:17.463' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (9, 35, 1040, CAST(N'2024-03-08' AS Date), 2405000, N'online', N'pending', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:03:32.000' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (10, 35, 1040, CAST(N'2024-12-18' AS Date), 345000, N'online', N'pending', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:04:57.520' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (11, 39, 1044, CAST(N'2024-04-27' AS Date), 1095000, N'cod', N'paid', 1, N'fulfilled', NULL, CAST(N'2024-12-18T02:07:05.260' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (12, 39, 1044, CAST(N'2024-04-30' AS Date), 2535000, N'cod', N'paid', 1, N'fulfilled', NULL, CAST(N'2024-12-18T02:53:10.930' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (13, 39, 1044, CAST(N'2024-05-11' AS Date), 604000, N'cod', N'pending', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:03:53.270' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (14, 38, 1043, CAST(N'2024-06-12' AS Date), 1860000, N'online', N'paid', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:06:41.577' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (15, 38, 1043, CAST(N'2024-03-18' AS Date), 2850000, N'cod', N'pending', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:04:01.633' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (16, 38, 1043, CAST(N'2024-06-23' AS Date), 1050000, N'cod', N'pending', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:04:11.270' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (17, 37, 1042, CAST(N'2024-11-10' AS Date), 2355000, N'online', N'paid', 1, N'fulfilled', NULL, CAST(N'2024-12-18T02:06:56.260' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (18, 37, 1042, CAST(N'2024-11-10' AS Date), 525000, N'cod', N'pending', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:04:34.250' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (19, 37, 1042, CAST(N'2024-12-18' AS Date), 2640000, N'cod', N'pending', 1, N'not_fulfilled', NULL, CAST(N'2024-12-18T02:02:00.647' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (20, 35, 1040, CAST(N'2024-12-18' AS Date), 1560000, N'cod', N'pending', 1, N'not_fulfilled', N'giao gấp cho em nhe', CAST(N'2024-12-18T02:09:23.823' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (21, 43, 1049, CAST(N'2024-12-18' AS Date), 3450000, N'cod', N'pending', 1, N'not_fulfilled', N'giao cho lẹ nha', CAST(N'2024-12-18T09:48:15.843' AS DateTime))
INSERT [dbo].[Bill] ([bill_id], [id_user], [id_adrs], [date_created], [total_amount], [payment_method], [payment_status], [is_confirmed], [shipping_status], [note], [updated_at]) VALUES (22, 35, 1040, CAST(N'2025-10-21' AS Date), 345000, N'cod', N'pending', 1, N'not_fulfilled', N'cho em 10k', CAST(N'2025-10-21T17:31:16.647' AS DateTime))
SET IDENTITY_INSERT [dbo].[Bill] OFF
GO
SET IDENTITY_INSERT [dbo].[Bill_Detail] ON 

INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (7, 7, 165, 1, 270000, 270000, CAST(N'2024-12-18T01:56:20.513' AS DateTime), CAST(N'2024-12-18T01:56:20.513' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (8, 8, 10, 3, 870000, 2610000, CAST(N'2024-12-18T01:58:24.250' AS DateTime), CAST(N'2024-12-18T01:58:24.250' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (9, 8, 4, 1, 1370000, 1370000, CAST(N'2024-12-18T01:58:24.263' AS DateTime), CAST(N'2024-12-18T01:58:24.263' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (10, 8, 5, 1, 570000, 570000, CAST(N'2024-12-18T01:58:24.267' AS DateTime), CAST(N'2024-12-18T01:58:24.267' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (11, 9, 11, 2, 385000, 770000, CAST(N'2024-12-18T01:58:43.083' AS DateTime), CAST(N'2024-12-18T01:58:43.083' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (12, 9, 1, 3, 545000, 1635000, CAST(N'2024-12-18T01:58:43.087' AS DateTime), CAST(N'2024-12-18T01:58:43.087' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (13, 10, 3, 1, 345000, 345000, CAST(N'2024-12-18T01:58:52.320' AS DateTime), CAST(N'2024-12-18T01:58:52.320' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (14, 11, 3, 1, 345000, 345000, CAST(N'2024-12-18T01:59:44.763' AS DateTime), CAST(N'2024-12-18T01:59:44.763' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (15, 11, 5, 1, 570000, 570000, CAST(N'2024-12-18T01:59:44.773' AS DateTime), CAST(N'2024-12-18T01:59:44.773' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (16, 11, 234, 1, 180000, 180000, CAST(N'2024-12-18T01:59:44.777' AS DateTime), CAST(N'2024-12-18T01:59:44.777' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (17, 12, 174, 3, 65000, 195000, CAST(N'2024-12-18T01:59:58.970' AS DateTime), CAST(N'2024-12-18T01:59:58.970' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (18, 12, 111, 3, 780000, 2340000, CAST(N'2024-12-18T01:59:58.983' AS DateTime), CAST(N'2024-12-18T01:59:58.983' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (19, 13, 141, 3, 28000, 84000, CAST(N'2024-12-18T02:00:15.317' AS DateTime), CAST(N'2024-12-18T02:00:15.317' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (20, 13, 172, 1, 520000, 520000, CAST(N'2024-12-18T02:00:15.327' AS DateTime), CAST(N'2024-12-18T02:00:15.327' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (21, 14, 114, 1, 720000, 720000, CAST(N'2024-12-18T02:00:41.930' AS DateTime), CAST(N'2024-12-18T02:00:41.930' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (22, 14, 104, 3, 250000, 750000, CAST(N'2024-12-18T02:00:41.930' AS DateTime), CAST(N'2024-12-18T02:00:41.930' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (23, 14, 77, 1, 390000, 390000, CAST(N'2024-12-18T02:00:41.933' AS DateTime), CAST(N'2024-12-18T02:00:41.933' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (24, 15, 164, 3, 370000, 1110000, CAST(N'2024-12-18T02:00:55.800' AS DateTime), CAST(N'2024-12-18T02:00:55.800' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (25, 15, 10, 2, 870000, 1740000, CAST(N'2024-12-18T02:00:55.803' AS DateTime), CAST(N'2024-12-18T02:00:55.803' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (26, 16, 142, 3, 350000, 1050000, CAST(N'2024-12-18T02:01:08.617' AS DateTime), CAST(N'2024-12-18T02:01:08.617' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (27, 17, 107, 3, 530000, 1590000, CAST(N'2024-12-18T02:01:39.863' AS DateTime), CAST(N'2024-12-18T02:01:39.863' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (28, 17, 184, 1, 480000, 480000, CAST(N'2024-12-18T02:01:39.867' AS DateTime), CAST(N'2024-12-18T02:01:39.867' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (29, 17, 189, 1, 285000, 285000, CAST(N'2024-12-18T02:01:39.867' AS DateTime), CAST(N'2024-12-18T02:01:39.867' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (30, 18, 106, 3, 175000, 525000, CAST(N'2024-12-18T02:01:50.947' AS DateTime), CAST(N'2024-12-18T02:01:50.947' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (31, 19, 78, 3, 880000, 2640000, CAST(N'2024-12-18T02:02:00.650' AS DateTime), CAST(N'2024-12-18T02:02:00.650' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (32, 20, 172, 3, 520000, 1560000, CAST(N'2024-12-18T02:09:23.983' AS DateTime), CAST(N'2024-12-18T02:09:23.983' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (33, 21, 3, 10, 345000, 3450000, CAST(N'2024-12-18T09:48:15.923' AS DateTime), CAST(N'2024-12-18T09:48:15.923' AS DateTime))
INSERT [dbo].[Bill_Detail] ([bill_detail_id], [id_bill], [id_product], [quantity], [unit_price], [total_price], [created_at], [updated_at]) VALUES (34, 22, 3, 1, 345000, 345000, CAST(N'2025-10-21T17:31:16.810' AS DateTime), CAST(N'2025-10-21T17:31:16.810' AS DateTime))
SET IDENTITY_INSERT [dbo].[Bill_Detail] OFF
GO
SET IDENTITY_INSERT [dbo].[Category] ON 

INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (1, N'Bếp', 0, N'bep.jpg', CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (2, N'Picnic Day', 0, N'picnicday.webp', CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (3, N'Cốc, Ly', 0, N'cocly.jpg', CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (4, N'Trá Chiều, Café', 0, N'trachieucafe.jpg', CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (5, N'Thiền Định An Yên', 0, N'thiendinhanyen.webp', CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (6, N'Mùi Hương', 0, N'muihuong.webp', CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (7, N'Trang Trí Nhà Cửa', 0, N'trangtrinhacua.jpg', CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (8, N'Đồ Cá Nhân', 0, N'docanhan.webp', CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (9, N'Xoong, Nồi, Chảo', 1, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (10, N'Giá, Kệ Đa Năng', 1, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (11, N'Phụ Kiện Bếp', 1, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (12, N'Thìa, Đũa', 1, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (13, N'Bát, Đĩa', 1, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (14, N'Bình Nước', 1, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (15, N'Cốc', 3, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (16, N'Ly', 3, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (17, N'Bộ Ấm Trà', 4, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (18, N'Trà Cụ', 4, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (19, N'Tea Party', 4, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (20, N'Café Tỉnh Táo', 4, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (21, N'Trầm', 5, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (22, N'Set Đốt Trầm', 5, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (23, N'Vòng, Đá', 5, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (24, N'Nến Thơm', 6, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (25, N'Tinh Dầu, Túi Thơm', 6, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (26, N'Khăn Trải Bàn, Thảm', 7, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (27, N'Bình Cắm Hoa', 7, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (28, N'Túi, Giỏ, Hộp Đựng', 7, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (29, N'Tượng Décor', 7, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (30, N'Khác', 7, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (31, N'Kẹp Tóc', 8, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (32, N'Thư Giãn', 8, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (33, N'Bình Nước', 8, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (34, N'Túi Tote', 8, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (35, N'Book Corner', 8, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
INSERT [dbo].[Category] ([category_id], [category_name], [category_parent_id], [category_img], [created_at], [updated_at]) VALUES (36, N'Warm & Cozy', 8, NULL, CAST(N'2024-10-12T17:32:03.000' AS DateTime), CAST(N'2024-10-12T17:32:03.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[Category] OFF
GO
SET IDENTITY_INSERT [dbo].[Product] ON 

INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (1, 9, N'Ấm Tráng Men Mickey', 545000, NULL, 0, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T10:09:31.790' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (2, 9, N'Ấm Tráng Men Daisy', 740000, N'Kích thước:
- Đường kính miệng: 8.5cm
- Đường kính đáy: 12cm
- Cao: 12 cm
- Dung tích: 1.5L
- Trọng lượng:1kg
                                    ', 93, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:29:48.903' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (3, 9, N'Bếp Nướng Cồn Tròn', 345000, N'- Chất liệu: Hợp kim nhôm, gỗ sồi
- Kích thước: 
+ Nhỏ: 16 x 10 cm (đường kính x chiều cao)
+ Lớn: 21 x 10 cm (đường kính x chiều cao)
                                    ', 83, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2025-10-21T17:31:16.873' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (4, 9, N'Bếp Picnic Bag', 1370000, N'Bếp dễ lắp đặt, dễ sử dụng và dễ vệ sinh.
Bếp sử dụng than.
Được làm bằng thép chất lượng và lưới thép mạ crôm, không gỉ, không ăn mòn, bền.
Kích thước nhỏ gọn, tiện dụng.
Kích thước: xem hình ảnh để nhìn rõ kích thước.', 93, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T01:58:24.263' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (5, 9, N'Chảo DILL Xanh Bơ (19,5cm)', 570000, N'- Chất liệu: bakelite, hợp kim nhôm tiêu chuẩn Châu Âu, thủy tinh, chống dính greblon của Đức
- Dùng được trên bếp ga, bếp hồng ngoại
- Thân nồi được làm bằng vật liệu kháng khuẩn, chống bám bẩn, dễ lau chù
- Đáy nồi có lưới tổng hợp năng lượng, làm nóng đều và nhanh
- Tay cầm nồi và nắp chống nóng
- Nắp nồi làm bằng kính cường lực
                                    ', 97, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T01:59:44.773' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (6, 9, N'Chảo Đỏ (24cm)', 980000, N'- Chất liệu: Hợp nhôm phủ chống dính gốm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:32:28.287' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (7, 9, N'Chảo Gang Korea', 550000, N'🏕Chiếc chảo Gang đang Hot trong giới Camping và Picnic Hàn Quốc, mình thì thấy dùng trong bếp gia đình tiện, gọn gàng, dễ vệ sinh lắm...
🍅Quan trọng là nướng đồ siêu ngon, đồ ăn xém cạnh, giòn rụm mà không cháy khét. Có ai lúc nào, mùa nào cũng thèm đồ nướng không?????
                                    ', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:32:48.920' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (8, 9, N'Khuôn Bánh Vỏ Sò', 340000, NULL, 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T09:50:14.433' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (9, 9, N'Máy Nướng YIDPU', 645000, N'- Kích thước: 22 x 13,5 x 8 cm (chiều dài x chiều rộng x chiều cao)
- Công suất: 650W
- Mỗi máy nướng gồm có 1 thân máy và 2 bộ khay nướng.
                                    ', 94, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:33:10.440' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (10, 9, N'Máy Vắt Cam', 870000, N'- Kích thước: 17.6x20cm (cao x rộng)
- Thể tích: 400ml
- Công suất: 30W', 95, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:00:55.803' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (11, 9, N'Nồi Đất Nung', 385000, N'- Chất liệu: Đất Nung
- Kích thước: 
+ Nồi cao: 20 x 7cm (đường kính x chiều cao)
+ Nồi thấp: 22 x 5cm (đường kính x chiều cao)      ', 95, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T01:58:43.083' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (12, 9, N'Nồi DILL Xanh Bơ (17,2cm)', 790000, N'- Chất liệu: bakelite, hợp kim nhôm tiêu chuẩn Châu Âu, thủy tinh, chống dính greblon của Đức
- Dùng được trên bếp ga, bếp hồng ngoại
- Thân nồi được làm bằng vật liệu kháng khuẩn, chống bám bẩn, dễ lau chùi
- Đáy nồi có lưới tổng hợp năng lượng, làm nóng đều và nhanh
- Tay cầm nồi và nắp chống nóng
- Nắp nồi làm bằng kính cường lực', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:34:48.257' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (13, 9, N'Nồi Hokua', 1450000, N'- Chất Liệu: Nhôm
- Kích Thước: 23,6 x 11,8cm
- Trọng lượng: 445g
- Dung tích: 1,9L
- Bề mặt được xử lý bằng màng oxit hóa anot axit oxalic, không chỉ cải thiện khả năng chống ăn mòn của bề mặt nhôm mà còn tăng độ bền của nồi
- Khả năng dẫn nhiệt tốt giúp tiết kiệm thời gian đun nấu
* Lưu ý: Không sử dụng với bếp từ', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:35:26.010' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (14, 10, N'Bàn Cafe Di Động Phong Cách Bắc Âu', 1350000, N'- Sử dụng vật liệu chất lượng cao
- Chất liệu nhựa ABS thân thiện với môi trường nhập khẩu (không mùi, bền, chống chịu thời tiết tốt)
- Tay cầm bằng gỗ sồi chất lượng cao, ổn định và bền bỉ, khung kim loại\r\n\n
- Bánh xe linh hoạt, con lăn trơn tru được trang bị cuộn dây chống mài mòn
- Có 5 màu
- Ord trong tháng 7 giảm còn 1280k, hàng về sau 7-15 ngày
- Sản phẩm được đóng gói trong thùng carton kích thước 45*16*60cm                       ', 96, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:36:04.577' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (15, 10, N'Giá đĩa 3 ngăn', 295000, N'', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (16, 10, N'Kệ Đa Năng 2 Tầng 395', 395000, N'- Chất liệu: Kim loại
- Có thể tháo lắp
- Kích thước: (dài x rộng x cao)
+ Cả kệ: 30.3 x 25 x 29.5 cm
+ Tầng trên: 27 x  25 x 5.4 cm
+ Tầng dưới: 30.3 x 25 x 2 cm
+ Khoảng cách giữa 2 tầng: 22.1 cm', 96, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:36:57.490' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (17, 10, N'Kệ Ngôi Nhà Đa Năng', 230000, N'Căn bếp nhỏ của bạn sẽ gọn gàng, ngăn nắp hơn với kệ đa năng.
- Chất liệu: Sắt kim loại, Phun sơn tĩnh điện
- Lưu trữ đa năng, giá đỡ thớt, nắp nồi...
- Với nhiều ngăn giúp chúng ta dễ dàng lấy và cất dụng cụ
- Chiều cao khách nhau giữa các ngăn giúp kẹp chặt và cố định nắp nồi, chảo, đĩa..
- Vách ngăn nhỏ phía trước có thể dùng để cá dụng cụ nhà bếp như thìa canh..., dễ dàng lấy ra trong quá trình nấu ăn ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:37:35.723' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (18, 10, N'Kệ Puff', 245000, N'- Chất liệu: Sắt sơn tĩnh điện
- Kệ có thể điều chỉnh chiều dài từ 40 - 72.5cm
- Kích thước: 40 - 72.5 x 20 x 15 cm (dài x rộng x cao)', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:37:55.240' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (19, 10, N'Kệ Tre Lắp Ghép', 840000, N'- Chất liệu: Gỗ tự nhiên
- Gồm 4 ô gỗ rời, có thể lắp ghép theo nhiều cách
- Kích thước: (dài x rộng x cao)
+ Ô thứ nhất: 25 x 9.3 x 6 cm
+ Ô thứ hai:   28 x 9.3 x 15.8 cm
+ Ô thứ ba:    32 x 9.3 x 21 cm
+ Ô thứ tư:     36 x 9.3 x 25 cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:38:52.200' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (20, 11, N'Ấm Siêu Tốc Mickey Mouse', 1450000, N'- Công suất cao 1850w, sôi nhanh trong 5 phút
- Được uỷ quyền bởi Disney
- Nước đun sôi tốt cho sức khoẻ
- Chất liệu tiếp xúc với nước được làm bằng thép ko gỉ 304 an toàn trong thực phẩm
- Lớp lót bên trong ko gỉ, vòi inox ko gỉ, bộ lọc thép ko gỉ, nắp thép ko gỉ
- Dung tích: 1,7l
- Kích thước 24,5x28.5cm
- Điện áp 220v, tần số 50hz
Ưu đãi: Tặng kèm kéo gắp gỗ
Hàng nội địa Trung ord 7-10 ngày về sẵn hàng ạ ^^', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:40:04.433' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (21, 11, N'Bếp Điện Jane', 1950000, N'- Kích thước: 24.5 x 24.5 x 6.5cm (dài x rộng x cao)
- Đường kính nhiệt: 12cm
- Chất liệu vỏ: PC
 Công suất: 220V/1250W
                                    ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:41:13.810' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (22, 11, N'Bếp Ga Cube', 2450000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (23, 11, N'Bếp Ga Mini 3 Chân', 380000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (24, 11, N'Bếp Gas Mini Bear', 950000, N'- Freeship và tặng kèm 1 kéo gắp gỗ
- Bếp Gas Mini di động, nhỏ gọn, tiện lợi
- Vòng chắn gió tích hợp, bảo vệ ngọn lửa, phù hợp với các hoạt động ngoài trởi
- Tấm dẫn nhiệt tiết kiệm năng lượng
- Lửa xanh tập trung năng lượng 2500W, đun sôi nhanh chóng
- Thiết kế nút vặn điều chỉnh nhiệt độ chắc chắn, dễ thao tác
- Bếp nhôm đúc nguyên khối, có thể tháo rời, dễ dàng vệ sinh
- Đế chống trơn trượt
- Cách lắp bình gas: Căn chỉnh cổng bình xăng với cổng van gas và ấn nhẹ để kết nối, lực hút từ tính tự động sẽ khóa lại, tiện lợi và nhanh chóng
- Nhiên liệu: Khí Butan hóa lỏng\r\n\n
- Kích Thước: 25,8x19,5x12,2cm
- Trọng Lượng: 1,08kg', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:42:06.797' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (25, 11, N'Bình Rót Dầu Olive 1000ml', 275000, N'- Chất liệu: thủy tinh
- Kích thước: như hình
- Công dụng: kiểm soát lượng dầu muốn sử dụng một cách dễ dàng', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:42:25.077' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (26, 11, N'Bình Thủy Tinh Đựng Hạt 5L', 1750000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (27, 11, N'Cắm Đũa Thủy Tinh', 395000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (28, 11, N'Cắm Đũa Trắng Trơn', 385000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (29, 11, N'Chai Mơ 500ml', 185000, N'- Chất liệu: thủy tinh
- Dung tích: 500mL', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:42:36.983' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (30, 11, N'Cốc đo thể tích nắp gỗ', 185000, N'Cốc đo thể tích nắp gỗ
Thông tin về bạn ấy: 
Chất liệu: Thủy tinh, gỗ, ron silicon
Kích thước: 350ml: 12,5*10,5*8 cm
Màu sắc: Trong suốt', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:43:13.503' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (31, 11, N'Dán Tủ Lạnh Cookie', 45000, N'- Chất Liệu: Gốm
- Gồm nam châm gắn phía sau
- Kích Thước:
+ Kem Bean: 6x5,5cm
+ Bánh Quy Đậm: 5,5x5,5cm
+ Bánh Quy Sáng Màu: 4x5,5cm
+ Bánh Quy Tàn Nhang: 5,5x5,5cm
+ Gấu bean: 6x3,8cm
+ Gấu Cơ Bắp: 6x4cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:44:04.887' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (32, 11, N'Dán Tủ Lạnh Mini', 55000, N'- Chất Liệu: Gốm
- Gồm nam châm phía sau
- Trọng Lượng: 100g', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:44:21.107' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (33, 11, N'Dao Bào Vỏ Chanh', 125000, N'- Chất liệu: PP, thép không gỉ
- Kích thước: như hình', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:44:31.680' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (34, 11, N'Dao Bếp Mini Panda', 85000, N'- Chất Liệu:
+ Thép không gỉ
+ Tay cầm thép không gỉ 420
- Công dụng: Chế biến thực phẩm nhỏ, dạy bạn nhỏ làm bếp
- Kích Thước:
+ Panda: 16.2x3.8cm (DxR)
- Góc đầu dao: 94 độ
- Phường pháp mài: Khi dao bị cùn, bán có thể cho thêm nước và mài nhẹ trên đá mài dao ở góc 15 - 20 độ vài lần, dao sẽ sắc bén như mới.
Chú ý:
- Hướng dẫn bạn nhỏ sử dụng dưới sự giám sát của người lớn
- Dao được mài sắc, chú ý an toàn khi mở hàng
- Không sử dụng với máy rửa chén, tủ khử trùng', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:45:30.700' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (35, 12, N'Dao Phết Bơ Cán Gỗ', 45000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (36, 12, N'Dĩa Gỗ Lớn', 12000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (37, 12, N'Dĩa Xanh Lá', 85000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (38, 12, N'Đũa Ghép Anh Đào', 45000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (39, 12, N'Đũa Ghép Cuốn Chỉ', 45000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (40, 12, N'Đũa Ghép Khắc Họa Tiết', 45000, N'🌊Ở đất nước Nhật Bản, dùng đũa được xem như một văn hoá, việc chọn đôi đũa ăn cơm cũng được người Nhật để tâm. Đôi đũa ngoài việc bền, đẹp còn nên nhẹ và dễ chịu khi cầm trên tay. Đũa cũng được xem là món quà may mắn, tinh tế để gửi tặng người thân!
🎋Bạn có tin một đôi đũa tốt và đẹp sẽ giúp chúng mình tận hưởng bữa ăn hơn không? Nhà Leaf có thật nhiều mẫu đũa để bạn ghé chọn đó ạ!', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:45:43.593' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (41, 12, N'Đũa Ghép Mèo', 45000, N'🌊Ở đất nước Nhật Bản, dùng đũa được xem như một văn hoá, việc chọn đôi đũa ăn cơm cũng được người Nhật để tâm. Đôi đũa ngoài việc bền, đẹp còn nên nhẹ và dễ chịu khi cầm trên tay. Đũa cũng được xem là món quà may mắn, tinh tế để gửi tặng người thân!
🎋Bạn có tin một đôi đũa tốt và đẹp sẽ giúp chúng mình tận hưởng bữa ăn hơn không? Nhà Leaf có thật nhiều mẫu đũa để bạn ghé chọn đó ạ!', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:46:02.420' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (42, 12, N'Đũa Ghép Nhật Ngắn', 45000, N'🌊Ở đất nước Nhật Bản, dùng đũa được xem như một văn hoá, việc chọn đôi đũa ăn cơm cũng được người Nhật để tâm. Đôi đũa ngoài việc bền, đẹp còn nên nhẹ và dễ chịu khi cầm trên tay. Đũa cũng được xem là món quà may mắn, tinh tế để gửi tặng người thân!
🎋Bạn có tin một đôi đũa tốt và đẹp sẽ giúp chúng mình tận hưởng bữa ăn hơn không? Nhà Leaf có thật nhiều mẫu đũa để bạn ghé chọn đó ạ!', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:46:17.540' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (43, 12, N'Đũa Ghép Ovan', 45000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (44, 12, N'Đũa Gỗ Cute', 95000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (45, 12, N'Đũa Hoa Đào', 45000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (46, 12, N'Đũa Mèo Japan', 45000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (47, 12, N'Set Thìa Dao Dĩa Anne', 570000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (48, 12, N'Set 3 Thìa Silicon Đỏ', 430000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (49, 13, N'Bát Cơm Julia (11cm)', 170000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (50, 13, N'Bát Gia Vị Lâu Đài', 115000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (51, 13, N'Bát Gỗ Thấp', 225000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (52, 13, N'Bát Gốm Daisy', 195000, N'- Chất liệu: Gốm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (53, 13, N'Bát Gốm Mèo Vàng', 330000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (54, 13, N'Bát Gốm Nhỏ Animal', 125000, N'- Chất Liệu: Sứ
- Kích Thước: 12,5x10,5x4cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:46:50.543' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (55, 13, N'Bát Gốm Nyanko', 95000, N'- Chất Liệu: Gốm
- Kích Thước: Đường kính 11cm, Chiều cao 6cm
- Trọng Lượng: 200g
- Phong cách: Nhật Bản
- Thiết kế đáy chống trượt, chất gốm sứ dày dặn,chịu nhiệt tốt
- Dùng được trong lò vi sóng, tủ khử trùng và máy rửa chén', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:47:40.407' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (56, 13, N'Bát Gốm Thủ Công Có Tai', 280000, N'Sử dụng nguyên liệu gốm thô Cảnh Đức Trấn_nơi khởi nguồn của gốm sứ Trung Quốc, nung ở nhiệt độ cao 1300 độ C nên an toàn, tốt cho sức khoẻ và thân thiện với môi trường
- Chiều cao: 8cm, đường kính miệng: 17cm, dung tích: 1400ml
- Có thể dùng trong lò vi sóng, máy rửa bát,...', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:48:00.540' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (57, 13, N'Bát Gốm Wabi Sabi', 250000, N'Được làm từ gốm sứ chất lượng cao từ Cảnh Đức Trấn_nơi khởi nguồn của gốm sứ Trung Quốc, các đốm đen không đồng đều được phân bổ tự nhiên và phù hợp với màu men mờ, mang đậm phong cách Nhật Bản
- Nung ở nhiệt độ cao 1300 độ C, làm cho sản phẩm dễ dàng làm sạch mà không có cặn
- Có thể được sử dụng để đựng súp và các món ăn, cũng có thể sử dụng như một đĩa baỳ trái cây', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:48:19.823' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (58, 13, N'Bát Hoa Nhỏ Retro', 280000, N'Góc vườn hoa nhỏ ngọt ngào, bình yên, xinh đẹp
- Chất Liệu: Gốm Sứ Tráng Men
- Màu Sắc: Trắng Sữa
- Kích Thước: 
+ Bát Mẫu 1: Đường kính 15,5cm, Chiều cao 6cm
+ Bát Mẫu 2: Đường kính 19,5, Chiều cao 3,5cm
- Trọng Lượng: 350g
* Note: Đĩa được chế tác thủ công nên có thể có lỗ kim, chấm đen nhỏ không ảnh hưởng tới chất lượng sản phẩm. Mong bạn thông cảm.', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:48:52.500' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (59, 13, N'Bát Julia (16cm)', 210000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (60, 13, N'Bát Pretty Bear', 95000, N'- Chất Liệu: Gốm tráng men
- Kích Thước:11,5x6,5cm
- Thiết kế đáy chống trượt, chịu nhiệt tốt
- Dùng được trong lò vi sóng, tủ khử trùng và máy rửa chén', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:49:12.320' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (61, 13, N'Bát Rau Củ', 125000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (62, 13, N'Bát Sứ Cinnamon', 395000, N'- Chất liệu: Sứ
- Kích thước: 18 x 9 cm (đường kính x chiều cao)', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:49:24.297' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (63, 13, N'Bát Sứ Sương Nhật Bản', 95000, N'Chất Liệu: Sứ Sương
Kích Thước: 11,4x6cm 
Dung Tích: 260ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:49:41.820' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (64, 14, N'Ấm thủy tinh (1000ml)', 240000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (65, 14, N'Ấm thủy tinh (1600ml)', 295000, N'- Chất liệu: Thủy tinh chịu nhiệt
- Dung tích: 1600ml
- Kích thước: 8 x 12.5 x 18.5cm (đường kính miệng x đường kính đáy x chiều cao)', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:50:00.927' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (66, 14, N'Ấm Thuỷ Tinh Nắp Inox (1800ml)', 290000, N'- Chất liệu: Thủy tinh, inox 304
- Dung tích: 1800ml
- Kích thước:  9 x 12.5 x 22.5 cm( đường kính miệng x đường kính đáy x chiều cao)', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:50:18.373' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (67, 14, N'Ấm Tráng Men Thỏ', 590000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (68, 14, N'Ấm Tráng Men Xanh (1,1L)', 620000, N'Pha trà, đun nước, đun sữa, làm bình đựng nước,…
Dùng trên bếp từ bếp ga, lò nướng, lò vi sóng, bếp hồng ngoại,… 
Chất liệu: tráng men', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:50:37.333' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (69, 14, N'Bình Big Size Đế Nhôm', 870000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (70, 14, N'Bình Big Size Giá Sắt', 970000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (71, 14, N'Bình Nước Cổ Điển Pháp', 550000, N'- Chất Liệu: Thủy Tinh + Nhựa ABS
- Kích Thước: Cao 24cm x Rộng 22cm
- Dung Tích: 1,6L', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:51:33.473' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (72, 14, N'Bình Thủy Tinh ( kèm ly)', 170000, N'- Chất liệu: thủy tinh', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (73, 14, N'Set Bình Smile', 310000, N'- Chất liệu: thủy tinh
- Dung tích: 850ml
- Kích thước: như hình
                                    ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:51:48.867' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (74, 2, N'Ấm Picnic', 370000, N'- Chất liệu: hợp kim nhôm
- Dung tích: 1.1 lit', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:51:59.977' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (75, 2, N'Quạt Trần Mini', 750000, N'Kích thước:17,3 x 13,6 x 9cm
Trọng lượng: 374g
Đầu vào: 5V/1.6A
Dung lượng pin: 7200mAh
Phụ kiện đi kèm: dây sạc, khiển từ xa
Quạt được trang bị công nghệ UAV động cơ giảm thanh treo từ tính. Lấy cảm hứng từ thiết kế cánh quạt của máy bay không người lái, các cánh quạt ba cánh được phát triển có thể thu thập nguồn không khí tập trung hơn và lượng gió ra lớn, không gây tiếng ồn 
Khi được sạc đầy quạt có thể chạy liên tục được khoảng 30 tiếng 
Điều khiển có thể dụng trong bán kính 5m
Đèn ngủ với 3 mức độ sáng
Ưu điểm của quạt trần mini:
- Gió mạnh 
- Không gây tiếng ồn
- Tuổi thọ pin lâu dài
- Điều khiển từ xa
- Ánh đèn vàng ấm áo
- Dễ dàng mang theo bên mình', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:53:05.910' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (76, 2, N'Thảm Picnic Vịt', 425000, N'Kích thước: 140x180cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (77, 2, N'Thảm, Chăn Cắm Trại Dã Ngoại', 390000, N'Chất liệu vải mềm mại, thoáng khi, chắc chắn, thoải mái
Trọng lượng nhẹ, dễ dàng mang theo
Đa năng, có thể dùng làm khăn trải bàn/sofa, trải niệm hoặc chăn ấm
Kích Thước: 160x130cm
Chất Liệu: Vải thêu sợi', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:00:41.933' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (78, 15, N'Bộ 2 Tách Linh Lan (kèm hộp)', 880000, N'', 96, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:02:00.647' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (79, 15, N'Bộ Cốc Đĩa Cún Viền Đỏ', 350000, N'Set gồm: 1 cốc và 1 đĩa
Chất Liệu: Gốm sứ
Kích Thước:
+ Cốc: 9x7,5 cm
+ Đĩa: 22x2,8 cm
Dung Tích cốc: 350ml
Dùng được trong lò vi sóng, máy rửa chén, tủ khử trùng', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T19:53:59.347' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (80, 15, N'Bộ Cốc Thìa Gốm Tulip', 250000, N'Bộ Gồm: 1 Cốc Gốm, 1 Thìa Gốm, 1 Hộp quà Đựng xinh xắn', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (81, 15, N'Cốc Always Love You', 165000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (82, 15, N'Cốc Bon Appétit', 95000, N'Chất liệu: thủy tinh
Dung tích: 400ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:34:01.790' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (83, 15, N'Cốc Butter Fluffy', 165000, N'- Chất Liệu: Gốm Sứ
- Dung Tích: 280ml
- Trọng Lượng: 300g 
- Kích Thước: 11,5x10,5cm ( tính cả quai cầm)
- Quy Trình: In Hình Đề Can Nhiệt Độ Cao 
- Dùng được trong lò vi sóng, máy rửa chén, tủ khử trùng    ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:34:45.617' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (84, 15, N'Cốc Capu', 130000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (85, 15, N'Cốc Chân Mèo Merie', 160000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (86, 15, N'Cốc Chú Vịt Kẻ Xanh', 195000, N'- Chất liệu: sứ
- Dung tích: 300mL', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:35:05.197' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (87, 15, N'Cốc Cún Corgi', 195000, N'', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (88, 15, N'Cốc Cún Drink More Water', 160000, N'- Chất liệu: Gốm sứ
- Kích thước: Đường kính 8,7cm x Cao 8,3cm
- Dung tích: 320ml
- Trọng lượng: 330g', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:35:39.797' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (89, 15, N'Cốc Cún Green', 150000, N'- Chất liệu: gốm \n\r\n- Dung tích: 350ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (90, 15, N'Cốc Cún Mèo Good Day', 220000, N'Chất Liệu: Gốm Sứ cao cấp
Dung Tích: 340ml
Kích Thước: Đường kính miệng 10cm x Cao 6,5cm x Rộng tổng 16,5cm
Quy trình: Tráng men mịn thủ công
Thiết kế độc đáo, phong cách nhí nhảnh tinh nghịch
Hình in nhiệt sắc nét, vòng đế không tráng chống trượt', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:36:57.377' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (91, 15, N'Cốc Cún Sinh Nhật', 165000, N'- Dung tích: 350m                                    ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:37:44.633' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (92, 15, N'Cốc Cún Strange And Cute', 220000, N'Chất Liệu: Gốm Sứ cao cấp
Dung Tích: 280ml 
Kích Thước: Đường kính 11,5cm x Cao 7,8cm
Quy trình: Tráng men mịn thủ công
Thiết kế độc đáo, phong cách nhí nhảnh tinh nghịch
Hình in nhiệt sắc nét, vòng đế không tráng chống trượt
* Note: Chưa kèm lót cốc', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:38:25.133' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (93, 15, N'Cốc Gardeners', 350000, N'- Chất liệu: sứ 
- Dung tích: 450ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:38:45.327' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (94, 15, N'Cốc Gấu Trắng', 225000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (95, 16, N'Gói Quà Ly Thủy Tinh Đôi', 935000, N'Một món quà giản dị, tinh tế, ấm áp với họa tiết sống động gửi gắm lời chúc chân thành 
- Chất Liệu:
+ Ly: Thủy tinh
+ Lót ly: Gốm
- Dung Tích: 220ml/ly đơn
- Kích Thước:
+ Ly: 157x65mm (chiều cao x đường kính)
+ Gói quà ( tổng thể): 28x25cm (dài x rộng)
Gói quà gồm:
- 2 chiếc ly thủy tinh đôi
- 2 lót ly bằng gốm
- 1 hộp quà cứng cáp, chắc chắn
- 1 tấm khăn lụa in tranh cắm trại để gói quà 
- 1 tấm thiệp \"Best Wish\"
- 1 túi nhựa mika dẻo trong suốt', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:41:00.457' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (96, 16, N'Ly Amber', 180000, N'- Chất liệu: Thủy tinh
- Dung tích: 125ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:41:19.950' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (97, 16, N'Ly Animal', 215000, N'- Chất liệu: thủy tinh
- Kích thước: như hình
- Dung tích: 150ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:41:37.617' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (98, 16, N'Ly Gốm Be Hoạt Hình', 185000, N'- Chất Liệu: Gốm tráng men
- Chất liệu dày dặn, chắc chắn
- Họa tiết hoạt hình vẽ tay dễ thương
- Kích Thước: 8x12cm
- Dung Tích: 300ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:42:06.603' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (99, 16, N'Ly Pha Lê Kim Cương', 250000, N'"Những bức ảnh không thể diễn tả hết sự tinh xảo của sản phẩm, bạn sẽ thấy được vẻ đẹp đó khi cầm trên tay" 
- Chất Liệu: Thuỷ tinh pha lê 
- Kích Thước: 20,4x7,6cm
- Dung Tích: 280ml
* Lưu ý: Chịu nhiệt 50 độ C, không dùng cho lò vi sóng, không làm lạnh và làm nóng đột ngột
Vì đồ thủ công nên sẽ không tránh khỏi việc có bong bóng hoặc tinh thể mịn, mong sẽ không ảnh hưởng tới cảm nhận của bạn ^^', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:42:40.500' AS DateTime))
GO
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (100, 16, N'Ly Serious Cat', 185000, N'- Chất Liệu: Thủy Tinh
- Màu Sắc: Trong suốt
- Dung Tích: 340ml
- Quy Trình: Chế tạo thủ công từ thủy tinh nóng chảy ở nhiệt độ cao', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:43:06.720' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (101, 16, N'Ly Thủy Tinh (200ml)', 145000, N'- Chất liệu: Thủy tinh
- Kích thước: 6.4 x 10.2 cm (đường kính x chiều cao)  ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:44:00.380' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (102, 16, N'Ly Thủy Tinh Bakeshop', 150000, N'- Chất Liệu: Thủy Tinh
- Kích Thước: 6x13cm
- Dung Tích: 340ml
', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:44:15.527' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (103, 16, N'Ly Thuỷ Tinh Blackpink', 245000, N'- Chất liệu: thuỷ tinh
- Kích thước: cao 13cm, đường kính 7cm, dung tích 300ml
*Note: Không dùng trong lò vi sóng, máy rửa chén, máy tiệt trùng, lò nướng..
', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:44:33.780' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (104, 16, N'Ly Thuỷ Tinh Champagne', 250000, N'"Ly thủy tinh hình giọt nước, giống như một nụ hoa còn đang say giấc nồng"
Thân ly được thổi nhân tạo, thành ly phải thật mỏng, miệng ly được chế tác rất tinh xảo. Đặc biệt khi làm miệng ly, các thợ thủ công lành nghề đã cắt lạnh trước, sau đó làm nóng chảy rồi tạo hình thật tỉ mỉ.
Quá trinh thủ công phức tạp này tạo nên một chiếc ly thủy tinh cao cấp, mỏng nhẹ mang tới bạn một cảm nhận tuyệt vời khi sử dụng
- Kích Thước:
+ Đường kính 6,5cm
+ Chiều cao 12,8cm
+ Đường kính đáy: 5,5cm
- Dung tính: 250ml
* Lưu ý: Chịu nhiệt 50 độ C, không dùng cho lò vi sóng, không làm lạnh và làm nóng đột ngột
Vì là đồ thủ công nên sẽ không tránh khỏi việc có bong bóng hoặc tinh thể mịn, mong sẽ không ảnh hưởng tới cảm nhận của bạn
', 97, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:00:41.930' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (105, 17, N'Bộ Trà Thỏ Trắng', 750000, N'Lấy cảm hứng từ cuộc gặp gỡ đầy thú vị của Alice và bữa tiệc trà trong "Alice in Wonderland"
Thiết kế ấm trà và tách trà, đĩa được xếp chồng lên nhau mang lại trải nghiệm khác biệt cho buổi trà chiều, làm tăng độ tinh tế từ tay nghề thủ công, hoàn chỉnh từng chi tiết
Thưởng thức trà chiều trong thế giới mộng mơ diệu kì. Biến bản thân thành những anh hùng đi khám phá miền đất mới, nắm lấy tự do cùng sự tò mò, sáng tạo dũng cảm theo đuổi ước mơ. Hãy để nguồn cảm hứng từ Alice tạo nên vẻ đẹp của bạn.
- Bộ Ấm Trà Thỏ Trắng Gồm: 1 Ấm Trà, 1 Tách Trà, 1 Đĩa , 1 Lọc trà,  1 Thiệp mời, 1 Hộp quà xinh xắn
- Kích Thước: Ấm (620ml), Tách (250ml), Đĩa (Đường kính 16cm)
- Chất liệu gốm sứ cao cấp, chất men mịn và tinh tế
- Áp dụng công nghệ tráng men màu, an toàn với sức khỏe và thân thiện với môi trường', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:46:28.820' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (106, 17, N'Tách Sứ Men Ngọc Xanh', 175000, N'- Chất Liệu: Gốm sứ tráng men
- Họa tiết được khắc nổi thủ công, tinh xảo, điều luyện
- Sản phẩm từ bàn tay các nghệ nhân truyền thống
- Sử dụng vật liểu cao cấp, đã qua tuyển chọn
- Kích Thước: Đường kính 7cm x Cao 4,8cm \n\r\n- Dung Tích: 50ml ', 97, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:01:50.947' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (107, 17, N'Bộ Trà Gốm Mây', 530000, N'Bộ Trà Gốm Mây gồm: 1 ấm trà kèm lọc gốm, 1 cốc lớn, 3 cốc nhỏ, 1 khăn vait lót, 1 túi đựng gọn
Kích Thước & Dung Tích
+ Ấm Trà: Rộng 8,5 x Cao 9,5 cm & 200ml 
+ Cốc lớn: Rộng 7,3 x Cao 4,2cm & 95ml
+ Cốc Nhỏ: Rộng 5,5 x Cao 2,8 cm & 35ml
Chất Liệu: Gốm sứ
Mang màu sắc đơn giản cùng kết cấu nhã nhặn tạo nét đẹp thanh lịch, bình yên
Sản phẩm thủ công mỹ nghệ tinh xảo
Gọn nhẹ, tiện lợi, thích hợp mang du lịch ngắm cảnh thưởng trà', 97, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:01:39.863' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (108, 17, N'Ấm Trà Nhỏ Gốm Đá Mỹ Nghệ', 580000, N'- Chất Liệu: Gốm đá tráng men cao cấp
- Kích Thước: Cao 7,2 x Rộng 8,5 x Dài 12,5 cm
- Dung Tích: 150ml
- Sản phẩm thủ công truyền thống mang nét đẹp của thời gian và quá trình
- Men nung đất sét được giữ nguyên tăng thêm nét cổ kính
- Được chế tác hoàn toàn thủ công bởi những nghệ nhân lành nghề
- Đồ gốm sứ tinh xảo, chất lượng cao', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:48:10.390' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (109, 17, N'Ấm Trà Nhỏ Thủ Công 35ml', 250000, N'- Dung Tích 35ml
- Kích Thước: Rộng 65mm x Cao 44mm
- Chất Liệu: Gốm sứ
- Sản phẩm thủ công truyền thống mang nét đẹp của thời gian và quá trình
- Được chế tác hoàn toàn thủ công bởi những nghệ nhân lành nghề
- Đồ gốm sứ tinh xảo, chất lượng cao', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:48:36.023' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (110, 17, N'Bộ Trà Sen Hồng', 780000, N'Bộ Trà Sen Hồng gồm 1 ấm trà, 3 tách trà và 1 túi du lịch
Cuộc sống luôn bận rộn nên chúng ta thường khao khát sự tĩnh lặng, giản đơn
Một tách trà và người bạn tâm tình là một loại bình yên
Nghệ thuật đồ gốm sứ truyền thống
Họa tiết vẽ tay tỉ mỉ, tinh xảo, sắc nét
Kỹ thuật men nứt 3D, bề mặt láng mịn
"Trong đầm gì đẹp bằng sen, 
Lá xanh, bông trắng lại chen nhuỵ vàng..."
Đã từ lâu hoa Sen được coi là biểu tượng của hồn Việt, một loài hoa thuần khiết, thanh cao, tượng trưng cho sự sống, thanh thiện và trí tuệ của con người
Ngoài ra, họa tiết cá trong đầm Sen hay được gọi là \"Cá chép hóa rồng\" là biểu tượng của việc vượt khó khăn để đạt đến thành công
Kích Thước:
Ấm Trà: Đường kính 9,9 x Cao 10,1 cm x 220ml
Tách Trà: Đường kính 6,5 x Cao 4,7 cm x 55ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:50:07.873' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (111, 17, N'Bộ Độc Ẩm Hoa Tử Đằng', 780000, N'Được tạo khối từ cao lanh thô, trắng sáng, láng mịn, độ bóng cao
Họa tiết vườn hoa Tử Đằng vẽ tay tinh tế, tỉ mỉ từng đường nét
Chế tác thủ công truyền thống, nung ở nhiệt độ cao, tráng men màu
Túi đựng nhỏ gọn, có thể mang theo mọi nơi, tiện lợi khi đi du lịch
Tay cầm có dây cuốn, cách nhiệt tốt và trang trí đẹp mắt
Trong phong thủy và quan niệm người phương Đông, Hoa Tử Đằng có ý nghĩa như một sự khở đầu thuận lợi cho gia chủ trong công việc, mang lại may mắn, tài lộc
Kích Thước:
Ấm Trà: Rộng 11cm x Cao 7,8cm x 200ml
Tách Trà: Rộng 8cm x Ca0 4,8cm x 150ml ', 97, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T01:59:58.990' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (112, 17, N'Bộ Trà Chùm Hồng Như Ý', 620000, N'Theo quan niệm dân gian, Quả Hồng mang ý nghĩa \"vạn sự như ý\", thức quả hình dáng tròn trịa, căng mọng được người xưa cho rằng loại trái cây này sẽ mang lại may mắn, suôn sẻ
Bộ Trà Chùm Hồng Như Ý: 1 ấm trà thủy tinh, 1 lọc trà gốm sứ, 3 chén trà, 1 hộp quà
Đường nét vẽ tay tinh tế, nghệ thuật gốm sứ truyền thống tinh xảo
Có thể xếp gọn vào ấm trà, dễ dàng mang theo
Kích Thước ( DxRxC) & Dung Tích (ml)
+ Ấm Trà: 8,3x12,3x16,7cm & 180ml
+ Chén Trà: 4,5x4,5x3,9cm & 30ml
+ Hộp Quà: 15,8x12,8x16,7cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:52:06.283' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (113, 17, N'Bộ Trà Hoa Mận Xanh', 490000, N'Quy trình làng nghề gốm thủ công: Tạo hình gốm, tráng men viền, vẽ ty họa tiết, tráng men trong suốt nhiệt cao
Sản phẩm thủ công truyền thống cao cấp, tinh xảo
Chất liệu gốm sứ mịn, các cạnh được bo tròn, đường né tinh tế
Tay cầm gỗ tre tự nhiên được chọn lọc kĩ, mang vẻ trang nhã, cách nhiệt hiệu quả
Chất Liệu: Gốm sứ
Kích Thước (RxC) & Dung Tích (ml)
+ Ấm Trà: 13,7x14,7cm (tính cả quai) & 250ml
+ Chén Trà: 5,3x4,4cm & 80ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:52:41.843' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (114, 18, N'Ấm Gốm Haru', 720000, N'', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:00:41.930' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (115, 18, N'Ấm Pha Trà Đồng Nguyên Chất', 1250000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (116, 18, N'Ấm Retro Nhật', 320000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (117, 18, N'Ấm Thuỷ Tinh Hoa Mộc', 350000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (118, 18, N'Ấm Thuỷ Tinh Hoa Mộc Quai Xanh', 350000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (119, 18, N'Ấm Trà Đất Nung Thủ Công', 780000, N'- Dung tích: 500ml
- Chất liệu: đất nung thủ công  ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:52:55.463' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (120, 18, N'Ấm Trà Dehua Vẽ Tay', 850000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (121, 18, N'Ấm Trà Lá', 740000, N'- Chất liệu: sứ
- Dung tích: 140mL
- Kích thước: 10,8x8cm (đường kính x chiều cao)
Họa tiết ấm được vẽ thủ công bằng tay', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:54:04.280' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (122, 18, N'Ấm Trà Mộc', 850000, N'- Chất liệu: Gốm sứ
- Kích thước: như hình
- Dung tích: 260mL', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:54:20.753' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (123, 18, N'Ấm Trà Mộc thương hiệu Muse Garden', 520000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (124, 18, N'Ấm Trà Ngọc Lục Bảo', 620000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (125, 19, N'Hộp Bánh Lá Nhỏ', 230000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (126, 19, N'Mật Ong Xuyến Chi Hà Giang', 220000, N'🍯Bidens Pilosa Honey🌱Mật ong hoa Xuyến chi từ cao nguyên Hà Giang
Khi nghiên cứu về Herbal Medicine (Thảo dược học) người ta thấy rằng hoa Xuyến chi thuộc họ Cúc, là một loại Wild Flower có sức sống và dược tính mạnh mẽ. Nên mật ong từ loài hoa này rất lành và tốt cho sức khoẻ.
Tại Hà Giang, người nuôi ong dùng những chú ong bản địa mang năng lượng núi rừng, tìm những cánh đồng hoa xuyến chi mọc dại để đặt ong tại đó, ngày ngày ong thu mật và tích luỹ. Đây là việc khai thác bền vững vì trong quá trình thu mật, người nuôi ong không thu toàn bộ mật và không khai thác phấn ong, mỗi khuôn mật sẽ chỉ thu một phần, phần còn lại cùng nhộng ong, ong non và phấn ong được giữ lại, trả về cho đàn để duy trì sinh trưởng tự nhiên, bền vững. Hiểu được tầm quan trọng của loài ong trong hệ sinh thái, nên A Little Leaf rất lưu tâm về điều này.
Mật ong hoa xuyến chi nhà Leaf hoàn toàn tự nhiên, có nguồn gốc nguyên liệu sạch, canh tác không hoá chất, tôn trọng sinh trưởng tự nhiên, các chỉ số về chất lượng luôn nằm trong nhóm mật tự nhiên tiêu chuẩn cao. Mật vàng ruộm màu hổ phách trong veo, nhìn thôi cũng thấy ngon miệng.
🌱Buổi sáng của bạn sẽ khoẻ và ngọt ngào hơn với cốc chanh mật ong ấm. Ly trà mỗi ngày cũng nhớ thêm chút mật ong. Ngoài ra, bạn có thể ăn mật ong kèm bánh mì, pha trà sữa dùng mật ong tạo độ ngọt dịu và healthy hơn. Khi ốm mệt, cũng đừng quên cốc trà gừng mật ong, bạn nhé!', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:55:03.473' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (127, 19, N'Rượu Haku', 280000, N'🍑Rượu Mơ nhà Leaf
🌿Haku là món rượu để thưởng thức, để nhấp từng ngụm nhỏ, nhè nhẹ, bên cạnh người thân. Với mỗi ngụm rượu, bạn sẽ thấy mình đủ đầy và trọn vẹn, thấy mình được là chính mình, thấy cả một vườn Mơ mùa xuân đang nở rộ.
🌱Rượu mơ Haku sẽ không khiến bạn say, mà khiến bạn có thêm nhiều yêu thương, thêm những khoảnh khắc hạnh phúc và ấm áp bên cạnh người thân.
Leaf chúc bạn luôn được sống trọn vẹn là chính mình. Yêu thương 🌱
Dung Tích: 380ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:55:49.050' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (128, 19, N'Set Quà Trà Chiều', 600000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (129, 19, N'Set Tách Hoa Tulip', 250000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (130, 19, N'Trà atiso đỏ (Hibiscus)', 135000, N'Cứ khi nào mình muốn tìm một loại trà để ăn bánh, thể nào lựa chọn đầu tiên cũng là Trà Atiso đỏ (Hibiscus tea), vì hương vị chua ngọt nhẹ nhàng có thể hợp với bất kì loại đồ ngọt nào, và màu đỏ ruby đặt trên bàn tiệc trà thì xinh yêu khôn tả. Và ngược lại, cứ hễ nhấp tách Trà Atiso đỏ là đầu lưỡi lại nhớ vị bánh, vị kem ngọt ngào khôn xiết. Bạn có đang tìm loại trà cho những bữa trà chiều của mình không? Chúng mình có Trà Atiso đỏ này!!!
Dung tích hũ trà: 230ml
Bảo quản: nơi khô ráo, thoáng mát
Hạn sử dụng: 12 tháng kể từ ngày mở nắp', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:56:14.197' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (131, 19, N'Trà Bạc Hà', 115000, N'Mình muốn đổi tên Trà Bạc Hà (Peppermint tea) thành Trà Bác Sĩ quá, vì những lợi ích bạn ấy đem lại cho sức khoẻ của chúng mình nhiều lắm thay. Để mình kể cho các bạn nghe nha: ví dụ như tốt cho chiếc dạ dày đang đầy và nóng của chúng mình này; hỗ trợ tiêu hoá nữa, còn chống cảm cúm, giúp hạ sốt và giảm thiểu tình trạng nôn mửa nữa. Chần chừ gì nữa, các đồng chí nhân viên A Little Leaf, mau điều một chú \"Trà Bác Sĩ\" về nhà chăm sóc sức khoẻ cả nhà ngay cho tôi!!!
Dung tích hũ trà: 230ml
Bảo quản: nơi khô ráo, thoáng mát
Hạn sử dụng: 12 tháng kể từ ngày mở nắp
                                    ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:57:22.013' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (132, 19, N'Trà buổi tối (Goodnight Tea)', 165000, N'Giấc ngủ là một trong những tiền đề quan trọng để có một cơ thể khoẻ mạnh, xin bạn hãy nâng niu thật tốt và luôn khoẻ mạnh nha. Chúng mình có Trà Buổi Tối, hay còn gọi là Trà Chúc Ngủ Ngon (Good night tea), trước khi đi ngủ, hay pha một tách, vừa nhâm nhi vừa đọc những câu truyện trẻ con nhẹ nhàng, hoặc nghe những bản nhạc du dương để có thể bình an chìm vào giấc ngủ, bạn nhé!
Dung tích hũ trà: 230ml
Bảo quản: nơi khô ráo, thoáng mát
Hạn sử dụng: 12 tháng kể từ ngày mở nắp', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:57:56.263' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (133, 19, N'Trà Cam', 115000, N'Những lúc bận rộn hay căng thẳng thường, lại cộng với những ngày mùa xuân ẩm ướt, khiến người ta vừa uể oải vừa chán ngán chẳng buồn ăn, nhưng mà bỏ bữa miết hoặc ăn qua loa sẽ khiến chúng mình dễ ốm lắm ấy! Những lúc thế này, mình thường xúc một ít Trà Cam (Orange tea) bỏ vào lọc trà xinh xinh của A Little leaf, mùi cam dịu dàng ngửi đã thấy nhẹ nhõm, vị chua chua nhẹ thể nào cũng sẽ khiến bạn đói bụng, vội thì cũng không nhịn được mà lục mấy chiếc bánh quy, mà thong thả thì phải đứng lên tìm một bữa ấm bụng ngay tức khắc!!!
Dung tích hũ trà: 230ml
Bảo quản: nơi khô ráo, thoáng mát
Hạn sử dụng: 12 tháng kể từ ngày mở nắp
', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:58:19.470' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (134, 20, N'Ấm Cổ Ngỗng 1,2L', 650000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (135, 20, N'Ấm Cổ Ngỗng 1L', 580000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (136, 20, N'Bình Cafe Cổ Ngỗng Giữ Nhiệt', 1320000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (137, 20, N'Bình cafe inox tay gỗ', 650000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (138, 20, N'Bình Thủy Tinh Silicon', 690000, N'- Chất liệu: nắp silicon, thân bình thủy tinh, lọc inox và pp
- Kích thước: 30cm x 8,3cm
- Dung tích: 1000ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:58:50.807' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (139, 20, N'Bình Xay Cafe 4.0', 380000, N'- Chất liệu: PP
- Kích thước: 15.7 x 7.2 cm (đường kính x chiều cao)', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:59:10.503' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (140, 21, N'Nhang Trầm Sả', 60000, N'Thành phần: Trầm hương, sả chanh, vỏ bời lời.
Công dụng:
- Giúp tinh thần sảng khoái giảm stress.
- Hỗ trợ tốt cho thiền định và yoga.
Hộp: 180 cây
Tép: 50 cây', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T20:59:39.933' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (141, 21, N'Bó Quế (3 Thanh)', 28000, N'Mùa đông đến, trong túi xách có vài thanh Quế xinh xinh thật thích, mùi hương ấm áp lan toả, khiến người ta nhẹ nhõm hơn.
Thi thoảng đốt một thanh Quế sẽ giúp trường năng lượng của bạn tích cực, đem lại sự dồi dào, trù phú, hấp dẫn may mắn về tiền bạc đến với mình!', 97, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:00:15.323' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (142, 21, N'Bó Tuyết Tùng (Cedar Smudge)', 350000, N'', 97, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:01:08.627' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (143, 21, N'Đốt Trầm Quýt Nhỏ', 125000, N'Kích Thước: Dài 4,3cm x Rộng 4,4cm x Cao 4,7cm
Có thể bạn chưa biết, những loại trái cây họ cam quýt thường được bày trên mâm ngũ quả với mong muốn tượng trưng cho sự giàu có và tài lộc dư dả bởi màu sắc và ngoại hình của chúng.
Ngoài ra trong tiếng hán \"Cam/quýt\" đồng âm với \"Tốt lành\" mang ý nghĩa may mắn
Với thiết kế thủ công tinh xảo, sống động như thật, đế đốt trầm sẽ mang lại không gian trà thiền thanh tịnh, nhẹ nhàng', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:00:57.633' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (144, 21, N'Gỗ Palo Santo', 150000, N'Palo santo nghĩa là “gỗ của những vị Thánh” có mùi hương tự nhiên rất thơm và ấm, nó có tác dụng xoa dịu, mang lại sự bình yên và thanh thản cho nội tâm, đưa chúng ta về trạng thái cân bằng, tích cực.
Các Shaman (pháp sư) từ nền văn minh Maya và Inca thường đốt Palo Santo trong các nghi lễ vì chúng có tác dụng thanh tẩy và khả năng kết nối mạnh mẽ với trực giác cũng như sự thông tuệ của con người.
Những thanh gỗ Palo Santo nhà Leaf đến từ Peru, được cắt thủ công bằng tay nên trông tự nhiên, mỗi thanh đều khác biệt, mùi hương gỗ tinh tế, mang một chút ngọt nhẹ, tinh dầu thấm trong các thớ gỗ sẽ giải phóng khắp không gian khi bạn đốt lên thanh gỗ thánh nhỏ xinh này!', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:01:24.847' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (145, 21, N'Hũ Gỗ Đàn Hương (50g)', 280000, N'Gỗ Đàn Hương (Sandalwood) có mùi hương đặc biệt dịu dàng, có tác dụng an thần, ổn định tâm trí, làm dịu đi những tâm trạng đang căng thẳng, quả đúng là thích hợp với những người đang ngày ngày mệt mỏi giữa đô thị phồn hoa như chúng ta.
Ngoài ra những thanh gỗ Đàn Hương của A Little Leaf rất nhỏ xinh, thích hợp để bỏ trong túi xách hoặc balo, đây là loại gỗ giúp bạn thu hút năng lượng tích cực và thanh tẩy trường không khí rất tốt. Hãy mang theo chúng để chúng mình luôn có một bầu không khí trong sạch và nhiệt thành bao quanh nhé.', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:01:41.187' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (146, 21, N'Hương Đạo Trúc Mây', 25000, N'- Chất Liệu: Bột Nén Thơm
- Kích Thước hộp: Đường kính 5cm
- Sản phẩm kèm hộp
- Nhỏ gọn, dễ dàng mang theo', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:01:57.947' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (147, 21, N'Lư Đốt Trầm Lục', 680000, N'Chất Liệu: Gốm sứ tráng men thủy tinh
Kích Thước:
+ Mẫu Cao: 6,5x10cm
+ Mẫu Thấp: 12x8cm ', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:02:15.403' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (148, 21, N'Nhang Hoa Cúc', 75000, N'Thành phần: Hoa cúc, vỏ bời lời
Công dụng:
- Giúp tinh thần sảng khoái giảm stress.
- Hỗ trợ tốt cho thiền định và yoga.
Hộp: 180cây
Tép: 50 cây', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:02:48.810' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (149, 21, N'Nhang Khuynh Diệp', 40000, N'Thành phần: Lá cây khuynh diệp, vỏ bời lời
Công dụng:
- Hương khuynh diệp là nguyên liệu để xông, đốt, giải cảm, rất tốt cho sức khỏe, hô hấp, giúp tinh thần thư giãn, giảm stress.
- Giúp xua đuổi muỗi và các loại côn trùng
Hộp: 180 cây
Tép: 50 cây', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:03:14.987' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (151, 22, N'Bếp Xông (kèm đĩa nến)', 165000, N'- Chất liệu: gốm đất nung
- Kích thước: chiều cao 10cm, đường kính 9cm ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:03:42.107' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (152, 22, N'Bếp Xông Mây', 165000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (153, 22, N'Bếp xông tinh dầu, thảo mộc điện', 650000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (154, 22, N'Chuông Đồng Nepal', 480000, N'- Chất liệu: Đồng nguyên chất
-  Kích thước: đường kính 6,5 cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:03:52.427' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (155, 22, N'Đế Sắc Màu', 155000, N'Kích thước: 9cm (đường kính)
', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:03:57.820' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (156, 22, N'Đế Sen Gỗ', 680000, N'- Chất Liệu: Gỗ Cẩm Lai
- Kích Thước: 4x12cm
- Trọng Lượng:190g
- Ý Nghĩa Kết Cấu: Đế Trầm chạm khắc Đài Sen mang ngụ ý may mắn, tốt lành
- Quá Trình: Chạm Khắc Thủ Công
+ Bền mặt được mài nhám, đánh bóng bằng tay.
+ Cánh sen được các nghệ nhân chạm khắc tỉ mỉ, kết cấu rõ ràng,sắc nét tạo nên hiệu ứng ba chiều.
+ Gỗ Cẩm Lai giữ nguyên được những đường vân gỗ và sắc hồng tự nhiên,không chất độc hại. Để lưu giữ màu sắc bền lâu, những người thợ phủ bao ngoài đế sen một lớp sáp mỏng.', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:04:47.300' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (157, 23, N'Bó Thạch Cao', 250000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (158, 23, N'Chuỗi Vòng 18 Hạt Bồ Đề', 250000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (159, 23, N'Cụm Tinh Thể Thạch Anh Trắng', 135000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (160, 23, N'Đá Bích Tủy Đen', 95000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (161, 23, N'Đá Kyanite', 95000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (162, 23, N'Đá Pha Lê Xanh', 120000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (163, 23, N'Đá Thô Thạch Anh Tím', 65000, N'- Chất Liệu: Đá Thạch Anh
- Cấu Tạo: Tinh Thể Thô Tự Nhiên
- Kích Thước: 2,5 - 3,5cm
- Trọng Lượng: Khoảng 8g
- Quá Trình: Sản xuất thủ công
- Công Dụng:Đá Thạch Anh thô có thể mang theo bên mình hoặc dùng làm đá khuếch tán mùi hương tinh dầu
- Ý Nghĩa:
Thạch anh tím được coi là biểu tượng cho tâm trí sáng suốt, điềm tĩnh, có khả năng giải độc, chữa bệnh, trừ tà và bảo vệ khỏi những tác động xấu của môi trường, mang đến cảm giác bình yên, an tâm.
Không chỉ vậy, đá Thạch Anh Tím còn được gọi là “Đá bảo hộ tình yêu”. Thạch anh tím có thể trao cho các cặp vợ chồng, những đôi yêu nhau lòng can đảm, sự trung thực và sự sâu sắc trong tình yêu.
* Note: Tùy từng viên đá sẽ có kích thước và hình dáng khác nhau vì được sản xuất tự nhiên thủ công', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:06:05.353' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (164, 24, N'Chụp Nến Thủy Tinh', 370000, N'- 12x10cm (cao x rộng)', 93, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:00:55.800' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (165, 24, N'Đế Nến Hoa Tulip', 270000, N'', 98, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T01:56:20.550' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (166, 24, N'Diêm Thủy Tinh Matches', 120000, N'- Chất Liệu: Lọ đựng bằng thủy tinh borosilicate, diêm gỗ
- Kích Thước: Chiều dài 10cm
- Thiết kế: Đầu que diêm có màu sắc, thân lọ có miếng dán quẹt lửa
- Số lượng: 150 que/lọ
- Cách dùng: Giữ que diêm và tấm quẹt trên thân lọ ở góc 45 độ, sau đó quẹt nhẹ để đánh lửa
* Note: Sau mỗi lần lấy que diêm ra khỏi lọ hãy đậy nút gỗ lại nhé, vì que diêm là từ gỗ nên sẽ bị ảnh hưởng bởi nhiệt độ và độ ẩm bên ngoài', 95, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:06:43.040' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (167, 25, N'Bếp Xông Bánh Bao', 165000, N'- Chất liệu: gốm đất nung
- Kích thước: chiều cao 10cm, đường kính 13cm ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:06:53.173' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (168, 25, N'Đế Khuếch Tán Tinh Dầu Meditation', 320000, N'- Chất liệu: Thủy tinh màu
- Kích thước: 10.5 x 7.5 x 14 cm (đường kính miệng x đường kính đáy x chiều cao) ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:07:02.760' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (169, 25, N'Khuếch Tán Tinh Dầu Địa Cầu', 1250000, N'- Chất liệu: gỗ óc chó đen giúp khóa hương tinh dầu và khuếch tán từ từ ra không gian
Sự kết hợp giữa vòng sao bằng đồng trầm lặng và gỗ óc chó đen ấm, kích thước được cân nhắc tinh xảo, thuận tiện cho việc khuếch tán hương thơm và dùng làm vật trang trí
Nhỏ 2 - 3 giọt tinh dầu có thể khuếch tán hương thơm cả ngày.', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:07:26.457' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (170, 25, N'Khuếch Tán Tinh Dầu Giọt Nước', 650000, N'- Chất liệu: PP
- Dung tích: 300ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:07:34.990' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (171, 25, N'Khuếch tán tinh dầu Reed (lọ thủy tinh)', 420000, N'- Thành phần: Tinh dầu, nước tinh khiết
- Một set gồm có: 1 lọ tinh dầu, 1 lọ thủy tinh rỗng, 12 que sậy khuếch tán
- Dung tích: 250ml
- Kích thước:
+ Lọ tinh dầu: 3 x 5.5 x 15cm (đường kính cổ lọ x đường kính đáy x chiều cao)
+ Lọ thủy tinh rỗng: 3 x 6.5 x 10.5cm (đường kính cổ lọ x đường kính đáy x chiều cao)
+ Que sậy khuếch tán: 27 cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:08:10.230' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (172, 25, N'Khuếch Tán Tinh Dầu Rose', 520000, N'- Kích thước: 12x11,6 cm (đường kính x chiều cao)
- Dung tích: 300ml
- Khối lượng: 260g
- Nguồn điện: 5V - 2A', 96, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:09:24.027' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (173, 25, N'Khuếch Tán Tinh Dầu Tròn', 650000, N'- Chất liệu: nhựa PP
- Kích thước: 16 x 18 cm (đường kính x chiều cao)
- Dung tích: 500mL
- Thời gian phun sương tối đa: 10 giờ
Máy khuếch tán phù hợp cho căn phòng có diện tích từ 10 đến 20 m2, máy có chế độ hẹn giờ và có thể sử dụng như một chiếc đèn ngủ vào ban đêm.', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:09:51.380' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (174, 26, N'Khăn Cotton Họa Tiết Cổ Điển', 65000, N'- Kích thước: 25x25cm
- Chất liệu: Cotton
- Khăn bàn ăn, khăn tay nhỏ gọn tiện lợi', 96, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T01:59:58.980' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (175, 26, N'Khăn Hier', 150000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (176, 26, N'Khăn Pain (40x50)', 125000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (177, 26, N'Khăn Trải Bàn Cat Flowers', 220000, N'- Kích Thước: 30x45cm
- Chất Liệu: Vải Cotton', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:10:21.167' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (178, 26, N'Khăn Trải Bàn Trắng Hoa Xanh', 145000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (180, 27, N'Bình Hoa Đỏ Vintage', 350000, N'- Chất Liệu: Gốm Sứ
- Kích Thước:  Cao 20cm x Đường kính miệng 7,9cm x Đường kính 11,5cm ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:10:39.293' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (181, 27, N'Chậu Gốm Sứ Hoa Xanh', 175000, N'- Chất Liệu: Gốm sứ
- Kích Thước:
- Chiều cao tổng 15,5cm
- Chiều cao thân bình: 10cm
- Đường kính miệng 12cm
- Đường kính đáy 10cm', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:11:06.770' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (182, 27, N'Đĩa Cắm Hoa Ikebana Đế Cao', 980000, N'- Chất Liệu: Gốm đá
- Kích Thước: Đường kính 20cm x Cao 12cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:11:18.963' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (183, 27, N'Bình Nứt Hoa Mai Xanh', 430000, N'Làng nghề gốm thủ công truyền thống, kỹ thuật tráng men đá nứt
Hoa văn sắc nét, trang nhã
- Chất Liệu: Gốm sứ
- Kích Thước:
+ Dài 14,5cm
+ Rộng 14,5cm
+ Cao 20cm
+ Đường kính miệng: 10cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:11:45.593' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (184, 27, N'Bình Hoa Bạc Khắc Nổi', 480000, N'- Chất Liệu: Gốm sứ + Mạ điện
- Kích Thước: 8x20cm', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:01:39.867' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (185, 27, N'Bình Hoa Sứ Chuông', 595000, N'Gió thổi, chuông ngân, hoa nở, mùa xuân đến
Lấy thiết kế cảm hứng từ chiếc chuông mang sự dịu dàng của gió, chiếc chuông còn có ý nghĩa mang lại niềm hỷ vọng, tốt lành, êm đềm cho cuộc sống
Kích Thước: Đường kính miệng 10cm, Đường kính đáy 8cm, Cao 20cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:12:13.860' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (186, 28, N'Giỏ Cói Lớn', 360000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (187, 28, N'Giỏ Đa Năng Xanh', 325000, N'', 98, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (188, 28, N'Giỏ Hoa, Cỏ', 250000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (189, 28, N'Giỏ Kim Loại Bí Ngô', 285000, N'', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T02:01:39.867' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (190, 28, N'Giỏ Lục Bình Tròn Thấp', 180000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (191, 28, N'Giỏ Mây Sáng Dài', 175000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (192, 29, N'Cún Gốm Lớn', 890000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (193, 29, N'Mèo Gốm Múp', 175000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (194, 29, N'Mèo Juhan Gốm', 180000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (195, 29, N'Tượng Gốm Rồng Thiền', 270000, N'- Chất Liệu: Gốm
- Kích Thước: Dài 4,3cm x Rộng 5,9cm x Cao 8,6cm
- Đường nét thủ công tinh xảo, bố cục cân đối
- Thiết kế tinh tế, chạm khắc ba chiều điêu luyện
Chú Rồng mang phong thái đĩnh đặc, điềm tĩnh cũng chính là lời nhắn nhủ thâm tình
Decor, trang trí không gian, bàn làm việc, bàn trà thiền, xê ô tô,...
*Note: Sản phẩm chưa kèm tấm lót ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:14:01.860' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (196, 29, N'Tượng Gốm Tiểu Long Tử', 220000, N'- Chất Liệu: Gốm sứ
- Kích Thước: Dài 7cm x Rộng 6,8cm x Cao 8cm
Sản phẩm gốm sứ thủ công tinh xảo, cao cấp
Điểm nhấn của bé Rồng là đôi mắt sáng với biểu cảm dễ thương, ngộ nghĩnh
Decor trang trí không gian, bàn trà, bàn làm việc thêm thú vị, sinh động
Bé Rồng với hành động đợi ăn đáng yêu mang lại cho cuộc sống thêm phần vui vẻ, lạc quan
\" Dù bận rộn nhưng đừng quên bé nha ^^\"', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:14:38.927' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (197, 29, N'Tượng Mèo Sứ Đeo Chuông', 240000, N'Nghệ thuật truyền thống tạo nên hương vị cao đẹp của cuộc sống
- Chất liệu gốm sứ, quy trình tạo hình, trang trí thủ công
- Kích thước: 8.9 x 3.7 x 7.6 cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:15:02.730' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (198, 30, N'Bàn Gỗ Cao', 450000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (199, 30, N'Bàn Rượu Di Động', 430000, N'- Đường kính 30cm, cao 15cm
- Chất liệu gỗ mộc chưa phủ sơn', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:15:12.530' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (200, 30, N'Bàn Tay Gốm Hoa', 285000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (201, 30, N'Bàn Trà', 1650000, N'Kích thước: 26cm x 40cm x 59cm ( cao x rộng x dài)', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
GO
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (202, 30, N'Bó Hoa Hướng Dương Đan Len', 170000, N'Kích Thước: 50x14cm ( tính cả túi đựng )', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (203, 30, N'Bó Hoa Vải', 150000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (204, 30, N'Bóng Tre (Đồ Chơi Của Boss)', 155000, N'- Chất liệu: tre đan
- Kích thước:
+ Lớn: 15cm
+ Nhỏ: 12cm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:15:38.190' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (205, 30, N'Chân Nến Cây Nấm', 285000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (206, 31, N'Bờm Hello Kitty', 70000, N'', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (207, 31, N'Dây Buộc Tóc Hoa Nhí', 38000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (208, 31, N'Dây Buộc Tóc Vải Dạ', 38000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (209, 31, N'Kẹp Basic Nhỏ', 25000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (210, 31, N'Kẹp Hoa Hồng', 45000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (211, 31, N'Kẹp Ngọc Trai Đỏ Mini', 35000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (212, 31, N'Kẹp Nhung Trái Tim Love', 45000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (213, 31, N'Kẹp Nơ Hồng Mini', 25000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (214, 31, N'Kẹp Puppy', 75000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (215, 32, N'Bàn Chải Tắm Bầu Dục (4,5inch)', 130000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (216, 32, N'Bàn Chải Tắm Dài', 165000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (217, 32, N'Đèn Đá Muối Hình Cầu', 1200000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (218, 32, N'Đèn Đá Muối Massage', 1450000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (219, 32, N'Đèn Phun Sương Jena', 230000, N'- Chất Liệu: ABS/PP/Linh kiện điện tử
- Công Dụng: Phun Sương Tạo Ẩm
- Dung Tích: 300ml
- Trọng Lượng: 150g
- Kích Thước: 11,6x 11,6x 10,2cm
- Chế độ:
+ 3 chế độ đèn, ánh sáng dịu nhẹ
+ 2 chế độ phun sương
-Tốc độ tạo ẩm: 50ml/giờ, hiệu suất tạo ẩm lên đến 99%
- Thời gian sử dụng:
+ Dùng Liên Tiếp: 4h
+ Dùng Gián Đoạn: 8h
- Cách sử dụng: Mở nắp và đổ nước trực tiếp
- Thiết kế đơn giản, tinh tế, không tạo tiếng ồn
- Thông số: DC5V; 300 - 450mA; 2,2V', 98, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T00:19:11.270' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (220, 32, N'Lược Gỗ Matxa Mèo Con', 85000, N'- Chất liệu: cây tre/trúc
- Công dụng: chăm sóc tóc', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:16:39.600' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (221, 32, N'Loa Quả Sồi', 370000, N'- Dung lượng pin: 400mAh
- Sạc đầy pin trong 1,5 tiếng.
- Dùng được 4 - 5 tiếng với âm lượng 50%', 96, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T00:19:11.257' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (222, 33, N'Bình Thủy Tinh Detox (500ml)', 135000, N'- Chất liệu: thủy tinh
- Dung tích: 500ml
- Kích thước: như hình', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:17:01.760' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (223, 33, N'Bình Thủy Tinh Hai Lớp (350ml)', 225000, N'- Chất liệu: tre, thủy tinh, inox
- Dung tích: 350ml', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:17:11.783' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (224, 33, N'Bình Thủy Tinh Hai Lớp (450ml)', 275000, N'Dung tích: 450ml
Chất liệu: Thân bình là thủy tinh 2 lớp (Borosilicat glass), nắp bình là tre và nhựa PP, loi lọc trà là inox 304
Kích thước: 6.5*7.1*22.8 cm
Khối lượng: 410 g
Mô tả: Bạn bình thuỷ tinh hai lớp có khả năng giữ nhiệt từ 6 -8 tiếng. Bạn ấy có nắp bọc tre và lọc trà được làm từ inox 304 chất lượng có thể tháo lắp dễ dàng, tiện lợi.
Ngoài ra, vì là bình thủy tinh nên nếu tụi mình bỏ vài lát hoa quả vào nước để detox thì trông cũng xịn xò lắm luôn đó.
Tụi mình còn nhận khắc chữ lên bình, với 65k cho 1 vị trí khắc ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:18:06.540' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (226, 33, N'Cốc Giữ Nhiệt Be', 250000, N'- Chất liệu: inox 304
- Dung tích 360ml
- Cao 13cm
- Đường kính miệng 8cm ', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:18:21.810' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (227, 34, N'Túi Đeo Vai Đan Thủ Công Kaki', 185000, N'-  Kích Thước:
+ Rộng: 34cm
+ Dài tổng: 55cm
+ Dày đáy: 10cm
+ Cao tay cầm: 24cm
-  Chất Liệu: Vải Đay + Da PU', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:19:00.110' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (228, 34, N'Túi Hồng Amber', 195000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (229, 34, N'Túi Màu Sắc Cổ Điển', 135000, N'- Chất Liệu: Vải cotton  + Lót poplin
-  Không gian trong rộng rãi mang theo đi học, đi làm hàng ngày, có thể làm giỏ đựng đồ khi đi mua sắm,…
-  Xách tay hoặc đeo vai đều tiện lợi
- Kích Thước: Rộng 43cm x Cao 36cm x Dây đeo 70cm (tổng)', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:19:58.533' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (230, 34, N'Túi Tote Dây Buộc', 165000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (231, 34, N'Túi Tote Enjoy', 165000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (232, 34, N'Túi Tote Hoạt Hình Mèo', 165000, N'- Chất Liệu: Vải Tote
- Kích Thước: 38x40cm
- Sức chứa: chai nước, ô, sách A4
- Có túi nhỏ ngăn nhỏ bên trong đựng điện thoại, mỹ phẩm
-  Có khóa kéo miệng túi tiện lợi', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:20:17.483' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (233, 35, N'Bookmark Cá Voi (kèm hộp)', 285000, N'- Chất liệu: hợp kim chống gỉ
-  Kích thước Cá Voi: 100x55mm
- Độ dài dây: 140mm', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:20:30.233' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (234, 35, N'Bookmark Trà Hoa', 180000, N'-  Kèm hộp đựng
-  Chất Liệu: Thép không gỉ                      ', 99, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-18T01:59:44.777' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (235, 35, N'Đèn Phi Hành Gia', 95000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (236, 35, N'Sổ Hương Thảo A5 (giấy mix)', 360000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (237, 36, N'Đế Nến Voi Gốm', 195000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (238, 36, N'Găng Tay Bông Teddy', 125000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (239, 36, N'Khăn Choàng Cổ Hình Thoi', 280000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (240, 36, N'Sạc Dự Phòng, Sưởi Ấm Animal', 550000, N'Công Dụng & Cách Dùng
+ Sưởi Ấm, Chườm  : Nhấn giữ nút nguồn 3s cho đến khi đèn trắng chuyển màu đỏ
+ Sạc Dự Phòng Điện Thoại: Ổ cắm sạc USB
+ Phun Sương Tạo Ẩm: Bơm nước vào khoang chứa nước và nhấn nút ở mặt trước để phun sương
+ Đèn pin chiếu sáng: Nhấn đúp nút nguồn
Máy sưởi ấm 2 mặt di động nhỏ gọn vào mùa đông
Dung lượng pin lớn, pin tích hợp chất lượng cao
Phun sương mịn nano dưỡng ẩm giúp làn da không khô rát
Thiết kế 2 lớp chống rò rỉ nước
Đèn pin chiếu sáng giúp tìm đồ vật nhỏ
Kích thước nhỏ gọn, dễ dàng mang theo
Dung lượng pin: 5400mAh/7800mAh
Ổ sạc: 5V2A
Kích thước: 113x73x67mm
*Lưu ý: Sản phẩm không phải đồ chơi cho trẻ dưới 14 tuổi', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:21:45.640' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (241, 36, N'Set Tất Cổ Tích Peter Rabbit', 330000, N'Bộ truyện Peter Rabbit được sáng tác bởi nữ nhà văn và hoạ sĩ người Anh - Beatrix Potter
Thống kê cách đây hơn 10 năm cho biết sách về chú thỏ Peter được dịch ra 36 ngôn ngữ và bán được 45 triệu bản, trở thành một trong những sách bán chạy nhất trong lịch sử, được ví như một tác phẩm huyền thoại kinh điển của văn học Anh
Câu chuyện kể về cuộc sống du mục vui vẻ và lý thú của chú thỏ nhỏ Peter và những người bạn động vật. Series truyện Peter Rabbit đề cao giá trị của tình bạn, lòng dũng cảm, sự trung thực và sẵn sàng giúp đỡ bạn bè trong lúc hiểm nguy
Hộp quà xinh xắn trông như một cuốn sách, là một món quà ý nghĩa
Hộp quà gồm: 6 đôi tất và sticker', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-12-15T21:22:30.640' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (242, 36, N'Set Tất Teddy', 365000, N'', 100, 1, CAST(N'2024-10-12T17:52:59.410' AS DateTime), CAST(N'2024-10-12T17:52:59.410' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (253, 6, N'Use case', 33000, N'                                                                                                                                                Mô
tả
use case
                                    
                                    
                                    ', 50, 0, CAST(N'2024-12-18T00:22:47.960' AS DateTime), CAST(N'2024-12-18T00:23:41.807' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (254, 2, N'Thử thêm sản phẩm', 333333, N'                                                                                                Đây
là
mô tả
                                    
                                    ', 30, 0, CAST(N'2024-12-18T00:50:01.843' AS DateTime), CAST(N'2024-12-18T00:50:27.703' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (255, 4, N'Thử thêm sản phẩm', 32311323, N'                                                                                                Mô tả
nè
                                    
                                    ', 392723, 0, CAST(N'2024-12-18T02:24:35.927' AS DateTime), CAST(N'2024-12-18T02:25:03.613' AS DateTime))
INSERT [dbo].[Product] ([product_id], [id_category], [product_name], [product_price], [product_description], [quantity_in_stock], [is_onSale], [created_at], [updated_at]) VALUES (256, 1, N'Test', 10000, N'test nha ae', 1000, 0, CAST(N'2025-10-21T17:38:07.547' AS DateTime), CAST(N'2025-10-21T17:38:07.547' AS DateTime))
SET IDENTITY_INSERT [dbo].[Product] OFF
GO
SET IDENTITY_INSERT [dbo].[Product_Images] ON 

INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (1, 74, N'ampicnic1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (2, 74, N'ampicnic2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (3, 74, N'ampicnic3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (4, 74, N'ampicnic4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (5, 74, N'ampicnic5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (6, 1, N'amtrangmenmickey1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (7, 1, N'amtrangmenmickey2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (8, 1, N'amtrangmenmickey3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (9, 1, N'amtrangmenmickey4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (10, 2, N'amtrangmendaisy1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (11, 2, N'amtrangmendaisy2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (12, 2, N'amtrangmendaisy3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (13, 3, N'bepnuongcontron1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (14, 3, N'bepnuongcontron2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (15, 3, N'bepnuongcontron3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (16, 3, N'bepnuongcontron4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (17, 3, N'bepnuongcontron5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (18, 4, N'beppicnicbag1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (19, 4, N'beppicnicbag2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (20, 4, N'beppicnicbag3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (21, 4, N'beppicnicbag4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (22, 4, N'beppicnicbag5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (23, 4, N'beppicnicbag6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (24, 4, N'beppicnicbag7.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (25, 4, N'beppicnicbag8.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (26, 5, N'chaodillxanhbo(195cm)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (27, 5, N'chaodillxanhbo(195cm)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (28, 5, N'chaodillxanhbo(195cm)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (29, 6, N'chaodo(24cm)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (30, 6, N'chaodo(24cm)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (31, 6, N'chaodo(24cm)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (32, 6, N'chaodo(24cm)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (33, 6, N'chaodo(24cm)5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (34, 7, N'chaogangkorea1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (35, 7, N'chaogangkorea2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (36, 7, N'chaogangkorea3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (37, 7, N'chaogangkorea4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (38, 8, N'khuonbanhvoso1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (39, 9, N'maynuongyidpu1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (40, 9, N'maynuongyidpu2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (41, 9, N'maynuongyidpu3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (42, 10, N'mayvatcam1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (43, 10, N'mayvatcam2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (44, 10, N'mayvatcam3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (45, 10, N'mayvatcam4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (46, 10, N'mayvatcam5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (47, 10, N'mayvatcam6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (48, 11, N'noidatnung1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (49, 11, N'noidatnung2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (50, 11, N'noidatnung3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (51, 11, N'noidatnung4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (52, 11, N'noidatnung5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (53, 12, N'noidillxanhbo(172cm)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (54, 12, N'noidillxanhbo(172cm)2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (55, 12, N'noidillxanhbo(172cm)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (56, 12, N'noidillxanhbo(172cm)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (57, 13, N'noihokua1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (58, 13, N'noihokua2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (59, 13, N'noihokua3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (60, 13, N'noihokua4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (61, 14, N'bancafedidongphongcachbacau1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (62, 14, N'bancafedidongphongcachbacau2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (63, 14, N'bancafedidongphongcachbacau3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (64, 14, N'bancafedidongphongcachbacau4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (65, 14, N'bancafedidongphongcachbacau5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (66, 15, N'giadia3ngan1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (67, 16, N'kedanang2tang3951.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (68, 16, N'kedanang2tang3952.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (69, 16, N'kedanang2tang3953.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (70, 17, N'kengoinhadanang1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (71, 17, N'kengoinhadanang2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (72, 17, N'kengoinhadanang3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (73, 17, N'kengoinhadanang4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (74, 18, N'kepuff1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (75, 18, N'kepuff2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (76, 18, N'kepuff3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (77, 19, N'ketrelapghep1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (78, 19, N'ketrelapghep2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (79, 19, N'ketrelapghep3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (80, 19, N'ketrelapghep4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (81, 20, N'amsieutocmickeymouse1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (82, 20, N'amsieutocmickeymouse2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (83, 20, N'amsieutocmickeymouse3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (84, 20, N'amsieutocmickeymouse4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (85, 20, N'amsieutocmickeymouse5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (86, 21, N'bepdienjane1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (87, 21, N'bepdienjane2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (88, 21, N'bepdienjane3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (89, 21, N'bepdienjane4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (90, 21, N'bepdienjane5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (91, 22, N'bepgacube1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (92, 22, N'bepgacube2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (93, 22, N'bepgacube3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (94, 22, N'bepgacube4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (95, 22, N'bepgacube5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (96, 23, N'bepgamini3chan1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (97, 23, N'bepgamini3chan2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (98, 23, N'bepgamini3chan3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (99, 24, N'bepgasminibear1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (100, 24, N'bepgasminibear2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (101, 24, N'bepgasminibear3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (102, 24, N'bepgasminibear4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (103, 24, N'bepgasminibear5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (104, 24, N'bepgasminibear6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (105, 71, N'binhnuoccodienphap1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (106, 71, N'binhnuoccodienphap2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (107, 71, N'binhnuoccodienphap3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (108, 71, N'binhnuoccodienphap4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (109, 71, N'binhnuoccodienphap5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (110, 71, N'binhnuoccodienphap6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (111, 25, N'binhrotdauolive1000ml1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (112, 25, N'binhrotdauolive1000ml2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (113, 25, N'binhrotdauolive1000ml3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (114, 25, N'binhrotdauolive1000ml4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (115, 26, N'binhthuytinhdunghat5l1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (116, 26, N'binhthuytinhdunghat5l2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (117, 27, N'camduathuytinh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (118, 27, N'camduathuytinh2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (119, 27, N'camduathuytinh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (120, 28, N'camduatrangtron1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (121, 29, N'chaimo500ml1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (122, 30, N'cocdothetichnapgo1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (123, 30, N'cocdothetichnapgo2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (124, 30, N'cocdothetichnapgo3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (125, 31, N'dantulanhcookie1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (126, 31, N'dantulanhcookie2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (127, 31, N'dantulanhcookie3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (128, 31, N'dantulanhcookie4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (129, 31, N'dantulanhcookie5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (130, 32, N'dantulanhmini1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (131, 32, N'dantulanhmini2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (132, 32, N'dantulanhmini3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (133, 32, N'dantulanhmini4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (134, 33, N'daobaovochanh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (135, 33, N'daobaovochanh2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (136, 33, N'daobaovochanh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (137, 34, N'daobepminipanda1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (138, 34, N'daobepminipanda2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (139, 34, N'daobepminipanda3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (140, 34, N'daobepminipanda4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (141, 34, N'daobepminipanda5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (142, 35, N'daophetbocango1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (143, 35, N'daophetbocango2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (144, 35, N'daophetbocango3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (145, 35, N'daophetbocango4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (146, 35, N'daophetbocango5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (147, 36, N'diagolon1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (148, 36, N'diagolon2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (149, 36, N'diagolon3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (150, 36, N'diagolon4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (151, 36, N'diagolon5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (152, 37, N'diaxanhla1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (153, 38, N'duaghepanhdao1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (154, 38, N'duaghepanhdao2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (155, 38, N'duaghepanhdao3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (156, 38, N'duaghepanhdao4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (157, 39, N'duaghepcuonchi1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (158, 39, N'duaghepcuonchi2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (159, 39, N'duaghepcuonchi3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (160, 39, N'duaghepcuonchi4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (161, 40, N'duaghepkhachoatiet1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (162, 40, N'duaghepkhachoatiet2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (163, 40, N'duaghepkhachoatiet3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (164, 40, N'duaghepkhachoatiet4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (165, 41, N'duaghepmeo1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (166, 41, N'duaghepmeo2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (167, 41, N'duaghepmeo3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (168, 41, N'duaghepmeo4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (169, 41, N'duaghepmeo5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (170, 42, N'duaghepnhatngan1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (171, 42, N'duaghepnhatngan2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (172, 42, N'duaghepnhatngan3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (173, 42, N'duaghepnhatngan4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (174, 42, N'duaghepnhatngan5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (175, 43, N'duaghepovan1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (176, 43, N'duaghepovan2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (177, 43, N'duaghepovan3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (178, 43, N'duaghepovan4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (179, 43, N'duaghepovan5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (180, 43, N'duaghepovan6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (181, 44, N'duagocute1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (182, 44, N'duagocute2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (183, 44, N'duagocute3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (184, 44, N'duagocute4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (185, 45, N'duahoadao1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (186, 45, N'duahoadao2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (187, 45, N'duahoadao3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (188, 45, N'duahoadao4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (189, 45, N'duahoadao5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (190, 46, N'duameojapan1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (191, 46, N'duameojapan2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (192, 46, N'duameojapan3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (193, 46, N'duameojapan4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (194, 46, N'duameojapan5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (195, 47, N'setthiadaodiaanne1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (196, 47, N'setthiadaodiaanne2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (197, 47, N'setthiadaodiaanne3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (198, 47, N'setthiadaodiaanne4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (199, 47, N'setthiadaodiaanne5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (200, 48, N'set3thiasilicondo1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (201, 48, N'set3thiasilicondo2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (202, 49, N'batcomjulia(11cm)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (203, 50, N'batgiavilaudai1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (204, 50, N'batgiavilaudai2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (205, 50, N'batgiavilaudai3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (206, 50, N'batgiavilaudai4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (207, 50, N'batgiavilaudai5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (208, 51, N'batgothap1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (209, 51, N'batgothap2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (210, 51, N'batgothap3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (211, 51, N'batgothap4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (212, 52, N'batgomdaisy1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (213, 53, N'batgommeovang1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (214, 53, N'batgommeovang2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (215, 53, N'batgommeovang3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (216, 53, N'batgommeovang4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (217, 54, N'batgomnhoanimal1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (218, 54, N'batgomnhoanimal2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (219, 54, N'batgomnhoanimal3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (220, 54, N'batgomnhoanimal4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (221, 54, N'batgomnhoanimal5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (222, 54, N'batgomnhoanimal6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (223, 55, N'batgomnyanko1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (224, 55, N'batgomnyanko2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (225, 55, N'batgomnyanko3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (226, 55, N'batgomnyanko4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (227, 55, N'batgomnyanko5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (228, 56, N'batgomthucongcotai1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (229, 56, N'batgomthucongcotai2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (230, 56, N'batgomthucongcotai3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (231, 56, N'batgomthucongcotai4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (232, 56, N'batgomthucongcotai5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (233, 56, N'batgomthucongcotai6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (234, 57, N'batgomwabisabi1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (235, 57, N'batgomwabisabi2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (236, 57, N'batgomwabisabi3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (237, 57, N'batgomwabisabi4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (238, 57, N'batgomwabisabi5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (239, 58, N'bathoanhoretro1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (240, 58, N'bathoanhoretro2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (241, 58, N'bathoanhoretro3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (242, 58, N'bathoanhoretro4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (243, 58, N'bathoanhoretro5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (244, 59, N'batjulia(16cm)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (245, 60, N'batprettybear1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (246, 60, N'batprettybear2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (247, 60, N'batprettybear3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (248, 60, N'batprettybear4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (249, 60, N'batprettybear5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (250, 61, N'batraucu1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (251, 61, N'batraucu2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (252, 62, N'batsucinnamon1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (253, 62, N'batsucinnamon2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (254, 63, N'batsusuongnhatban1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (255, 63, N'batsusuongnhatban2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (256, 63, N'batsusuongnhatban3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (257, 63, N'batsusuongnhatban4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (258, 63, N'batsusuongnhatban5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (259, 64, N'amthuytinh1000ml1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (260, 64, N'amthuytinh1000ml2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (261, 65, N'amthuytinh1600ml1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (262, 65, N'amthuytinh1600ml2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (263, 65, N'amthuytinh1600ml3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (264, 65, N'amthuytinh1600ml4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (265, 65, N'amthuytinh1600ml5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (266, 66, N'amthuytinhnapinox1800ml1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (267, 66, N'amthuytinhnapinox1800ml2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (268, 66, N'amthuytinhnapinox1800ml3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (269, 66, N'amthuytinhnapinox1800ml4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (270, 67, N'amtrangmentho1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (271, 67, N'amtrangmentho2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (272, 67, N'amtrangmentho3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (273, 67, N'amtrangmentho4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (274, 67, N'amtrangmentho5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (275, 68, N'amtrangmenxanh1100ml1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (276, 68, N'amtrangmenxanh1100ml2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (277, 68, N'amtrangmenxanh1100ml3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (278, 68, N'amtrangmenxanh1100ml4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (279, 68, N'amtrangmenxanh1100ml5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (280, 69, N'binhbigsizedenhom1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (281, 70, N'binhbigsizegiasat1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (282, 72, N'binhthuytinh(kemly)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (283, 72, N'binhthuytinh(kemly)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (284, 72, N'binhthuytinh(kemly)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (285, 72, N'binhthuytinh(kemly)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (286, 72, N'binhthuytinh(kemly)5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (287, 73, N'setbinhsmile1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (288, 73, N'setbinhsmile2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (289, 73, N'setbinhsmile3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (290, 73, N'setbinhsmile4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (291, 73, N'setbinhsmile5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (292, 75, N'quattranmini1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (293, 75, N'quattranmini2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (294, 75, N'quattranmini3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (295, 75, N'quattranmini4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (296, 75, N'quattranmini5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (297, 75, N'quattranmini6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (298, 76, N'thampicnicvit1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (299, 76, N'thampicnicvit2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (300, 76, N'thampicnicvit3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (301, 76, N'thampicnicvit4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (302, 77, N'thamchancamtraidangoai1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (303, 77, N'thamchancamtraidangoai2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (304, 77, N'thamchancamtraidangoai3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (305, 77, N'thamchancamtraidangoai4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (306, 77, N'thamchancamtraidangoai5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (307, 77, N'thamchancamtraidangoai6.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (308, 78, N'bo2tachlinhlan(kemhop)1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (309, 78, N'bo2tachlinhlan(kemhop)2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (310, 78, N'bo2tachlinhlan(kemhop)3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (311, 78, N'bo2tachlinhlan(kemhop)4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (312, 78, N'bo2tachlinhlan(kemhop)5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (313, 79, N'bococdiacunviendo1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (314, 79, N'bococdiacunviendo2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (315, 79, N'bococdiacunviendo3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (316, 79, N'bococdiacunviendo4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (317, 79, N'bococdiacunviendo5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (318, 80, N'bococthiagomtulip1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (319, 80, N'bococthiagomtulip2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (320, 80, N'bococthiagomtulip3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (321, 80, N'bococthiagomtulip4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (322, 81, N'cocalwaysloveyou1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (323, 81, N'cocalwaysloveyou2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (324, 81, N'cocalwaysloveyou3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (325, 81, N'cocalwaysloveyou4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (326, 82, N'cocbonappetit1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (327, 82, N'cocbonappetit2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (328, 82, N'cocbonappetit3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (329, 82, N'cocbonappetit4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (330, 82, N'cocbonappetit5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (331, 83, N'cocbutterfluffy1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (332, 83, N'cocbutterfluffy2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (333, 83, N'cocbutterfluffy3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (334, 83, N'cocbutterfluffy4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (335, 83, N'cocbutterfluffy5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (336, 84, N'coccapu1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (337, 84, N'coccapu2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (338, 84, N'coccapu3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (339, 84, N'coccapu4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (340, 84, N'coccapu5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (341, 85, N'cocchanmeomerie1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (342, 85, N'cocchanmeomerie2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (343, 85, N'cocchanmeomerie3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (344, 85, N'cocchanmeomerie4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (345, 86, N'cocchuvitkexanh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (346, 87, N'coccuncorgi1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (347, 87, N'coccuncorgi2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (348, 87, N'coccuncorgi3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (349, 87, N'coccuncorgi4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (350, 87, N'coccuncorgi5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (351, 88, N'coccundrinkmorewater1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (352, 88, N'coccundrinkmorewater2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (353, 88, N'coccundrinkmorewater3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (354, 88, N'coccundrinkmorewater4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (355, 88, N'coccundrinkmorewater5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (356, 89, N'coccungreen1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (357, 89, N'coccungreen2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (358, 89, N'coccungreen3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (359, 89, N'coccungreen4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (360, 89, N'coccungreen5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (361, 90, N'coccunmeogoodday1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (362, 90, N'coccunmeogoodday2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (363, 90, N'coccunmeogoodday3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (364, 90, N'coccunmeogoodday4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (365, 90, N'coccunmeogoodday5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (366, 91, N'coccunsinhnhat1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (367, 92, N'coccunstrangeandcute1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (368, 92, N'coccunstrangeandcute2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (369, 92, N'coccunstrangeandcute3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (370, 92, N'coccunstrangeandcute4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (371, 92, N'coccunstrangeandcute5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (372, 94, N'cocgautrang1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (373, 94, N'cocgautrang2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (374, 94, N'cocgautrang3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (375, 94, N'cocgautrang4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (376, 93, N'cocgardeners1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (377, 93, N'cocgardeners2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (378, 93, N'cocgardeners3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (379, 93, N'cocgardeners4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (380, 93, N'cocgardeners5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (381, 93, N'cocgardeners6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (382, 95, N'goiqualythuytinhdoi1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (383, 95, N'goiqualythuytinhdoi2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (384, 95, N'goiqualythuytinhdoi3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (385, 95, N'goiqualythuytinhdoi4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (386, 95, N'goiqualythuytinhdoi5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (387, 96, N'lyamber1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (388, 96, N'lyamber2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (389, 96, N'lyamber3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (390, 96, N'lyamber4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (391, 97, N'lyanimal1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (392, 97, N'lyanimal2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (393, 97, N'lyanimal3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (394, 97, N'lyanimal4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (395, 97, N'lyanimal5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (396, 98, N'lygombehoathinh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (397, 98, N'lygombehoathinh2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (398, 98, N'lygombehoathinh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (399, 98, N'lygombehoathinh4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (400, 98, N'lygombehoathinh5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (401, 99, N'lyphalekimcuong1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (402, 99, N'lyphalekimcuong2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (403, 100, N'lyseriouscat1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (404, 100, N'lyseriouscat2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (405, 100, N'lyseriouscat3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (406, 100, N'lyseriouscat4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (407, 100, N'lyseriouscat5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (408, 101, N'lythuytinh(200ml)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (409, 101, N'lythuytinh(200ml)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (410, 101, N'lythuytinh(200ml)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (411, 102, N'lythuytinhbakeshop1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (412, 102, N'lythuytinhbakeshop2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (413, 102, N'lythuytinhbakeshop3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (414, 102, N'lythuytinhbakeshop4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (415, 102, N'lythuytinhbakeshop5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (416, 103, N'lythuytinhblackpink1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (417, 103, N'lythuytinhblackpink2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (418, 103, N'lythuytinhblackpink3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (419, 103, N'lythuytinhblackpink4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (420, 104, N'lythuytinhchampagne1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (421, 104, N'lythuytinhchampagne2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (422, 104, N'lythuytinhchampagne3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (423, 104, N'lythuytinhchampagne4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (424, 104, N'lythuytinhchampagne5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (425, 105, N'botrathotrang1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (426, 105, N'botrathotrang2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (427, 105, N'botrathotrang3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (428, 105, N'botrathotrang4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (429, 105, N'botrathotrang5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (430, 105, N'botrathotrang6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (431, 106, N'tachsumenngocxanh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (432, 106, N'tachsumenngocxanh2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (433, 106, N'tachsumenngocxanh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (434, 106, N'tachsumenngocxanh4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (435, 106, N'tachsumenngocxanh5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (436, 107, N'botragommay1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (437, 107, N'botragommay2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (438, 107, N'botragommay3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (439, 107, N'botragommay4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (440, 107, N'botragommay5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (441, 108, N'amtranhogomdamynghe1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (442, 108, N'amtranhogomdamynghe2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (443, 108, N'amtranhogomdamynghe3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (444, 108, N'amtranhogomdamynghe4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (445, 109, N'amtranhothucong35ml1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (446, 109, N'amtranhothucong35ml2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (447, 109, N'amtranhothucong35ml3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (448, 109, N'amtranhothucong35ml4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (449, 110, N'botrasenhong1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (450, 110, N'botrasenhong2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (451, 110, N'botrasenhong3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (452, 110, N'botrasenhong4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (453, 111, N'bodocamhoatudang1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (454, 111, N'bodocamhoatudang2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (455, 111, N'bodocamhoatudang3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (456, 111, N'bodocamhoatudang4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (457, 111, N'bodocamhoatudang5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (458, 112, N'botrachumhongnhuy1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (459, 112, N'botrachumhongnhuy2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (460, 112, N'botrachumhongnhuy3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (461, 112, N'botrachumhongnhuy4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (462, 112, N'botrachumhongnhuy5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (463, 113, N'botrahoamanxanh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (464, 113, N'botrahoamanxanh2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (465, 113, N'botrahoamanxanh3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (466, 113, N'botrahoamanxanh4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (467, 113, N'botrahoamanxanh5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (468, 114, N'amgomharu1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (469, 114, N'amgomharu2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (470, 115, N'amphatradongnguyenchat1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (471, 115, N'amphatradongnguyenchat2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (472, 115, N'amphatradongnguyenchat3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (473, 116, N'amretronhat1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (474, 116, N'amretronhat2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (475, 116, N'amretronhat3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (476, 116, N'amretronhat4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (477, 117, N'amthuytinhhoamoc1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (478, 117, N'amthuytinhhoamoc2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (479, 117, N'amthuytinhhoamoc3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (480, 117, N'amthuytinhhoamoc4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (481, 117, N'amthuytinhhoamoc5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (482, 118, N'amthuytinhhoamocquaixanh1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (483, 118, N'amthuytinhhoamocquaixanh2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (484, 118, N'amthuytinhhoamocquaixanh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (485, 118, N'amthuytinhhoamocquaixanh4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (486, 118, N'amthuytinhhoamocquaixanh5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (487, 119, N'amtradatnungthucong1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (488, 119, N'amtradatnungthucong2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (489, 119, N'amtradatnungthucong3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (490, 119, N'amtradatnungthucong4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (491, 119, N'amtradatnungthucong5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (492, 120, N'amtradehuavetay1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (493, 120, N'amtradehuavetay2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (494, 120, N'amtradehuavetay3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (495, 120, N'amtradehuavetay4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (496, 120, N'amtradehuavetay5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (497, 121, N'amtrala1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (498, 121, N'amtrala2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (499, 121, N'amtrala3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (500, 121, N'amtrala4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (501, 121, N'amtrala5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (502, 122, N'amtramoc1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (503, 122, N'amtramoc2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (504, 122, N'amtramoc3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (505, 122, N'amtramoc4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (506, 122, N'amtramoc5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (507, 122, N'amtramoc6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (508, 123, N'amtramocthuonghieumusegarden1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (509, 123, N'amtramocthuonghieumusegarden2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (510, 124, N'amtrangoclucbao1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (511, 124, N'amtrangoclucbao2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (512, 124, N'amtrangoclucbao3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (513, 124, N'amtrangoclucbao4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (514, 124, N'amtrangoclucbao5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (515, 124, N'amtrangoclucbao6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (516, 125, N'hopbanhlanho1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (517, 125, N'hopbanhlanho2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (518, 125, N'hopbanhlanho3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (519, 125, N'hopbanhlanho4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (520, 126, N'matongxuyenchihagiang1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (521, 126, N'matongxuyenchihagiang2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (522, 126, N'matongxuyenchihagiang3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (523, 126, N'matongxuyenchihagiang4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (524, 126, N'matongxuyenchihagiang5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (525, 127, N'ruouhaku1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (526, 127, N'ruouhaku2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (527, 127, N'ruouhaku3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (528, 128, N'setquatrachieu1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (529, 128, N'setquatrachieu2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (530, 128, N'setquatrachieu3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (531, 129, N'settachhoatulip1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (532, 129, N'settachhoatulip2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (533, 129, N'settachhoatulip3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (534, 129, N'settachhoatulip4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (535, 129, N'settachhoatulip5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (536, 130, N'traatisodo(hibiscus)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (537, 131, N'trabacha1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (538, 132, N'trabuoitoi(goodnighttea)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (539, 133, N'tracam1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (540, 134, N'amcongong12l1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (541, 135, N'amcongong1l2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (542, 135, N'amcongong1l1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (543, 136, N'binhcafecongonggiunhiet1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (544, 136, N'binhcafecongonggiunhiet2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (545, 136, N'binhcafecongonggiunhiet3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (546, 136, N'binhcafecongonggiunhiet4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (547, 136, N'binhcafecongonggiunhiet5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (548, 137, N'binhcafeinoxtaygo1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (549, 137, N'binhcafeinoxtaygo2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (550, 137, N'binhcafeinoxtaygo3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (551, 137, N'binhcafeinoxtaygo4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (552, 137, N'binhcafeinoxtaygo5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (553, 138, N'binhthuytinhsilicon1.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-12-15T20:58:50.810' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (554, 139, N'binhxaycafe401.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (555, 139, N'binhxaycafe402.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (556, 139, N'binhxaycafe403.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (557, 139, N'binhxaycafe404.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (558, 139, N'binhxaycafe405.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (559, 140, N'nhangtramsa1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (560, 140, N'nhangtramsa2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (561, 141, N'boque(3thanh)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (562, 141, N'boque(3thanh)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (563, 141, N'boque(3thanh)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (564, 142, N'botuyettung(cedarsmudge)1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (565, 142, N'botuyettung(cedarsmudge)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (566, 143, N'dottramquytnho1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (567, 143, N'dottramquytnho2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (568, 143, N'dottramquytnho3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (569, 143, N'dottramquytnho4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (570, 143, N'dottramquytnho5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (571, 144, N'gopalosanto1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (572, 144, N'gopalosanto2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (573, 144, N'gopalosanto3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (574, 144, N'gopalosanto4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (575, 144, N'gopalosanto5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (576, 145, N'hugodanhuong(50g)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (577, 145, N'hugodanhuong(50g)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (578, 145, N'hugodanhuong(50g)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (579, 145, N'hugodanhuong(50g)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (580, 145, N'hugodanhuong(50g)5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (581, 146, N'huongdaotrucmay1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (582, 146, N'huongdaotrucmay2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (583, 146, N'huongdaotrucmay3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (584, 146, N'huongdaotrucmay4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (585, 146, N'huongdaotrucmay5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (586, 147, N'ludottramluc1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (587, 147, N'ludottramluc2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (588, 147, N'ludottramluc3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (589, 147, N'ludottramluc4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (590, 148, N'nhanghoacuc1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (591, 148, N'nhanghoacuc2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (592, 149, N'nhangkhuynhdiep1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (593, 149, N'nhangkhuynhdiep2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (594, 151, N'bepxong(kemdianen)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (595, 151, N'bepxong(kemdianen)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (596, 151, N'bepxong(kemdianen)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (597, 151, N'bepxong(kemdianen)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (598, 151, N'bepxong(kemdianen)5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (599, 152, N'bepxongmay1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (600, 152, N'bepxongmay2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (601, 152, N'bepxongmay3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (602, 152, N'bepxongmay4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (603, 152, N'bepxongmay5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (604, 153, N'bepxongtinhdauthaomocdien1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (605, 153, N'bepxongtinhdauthaomocdien2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (606, 154, N'chuongdongnepal1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (607, 154, N'chuongdongnepal2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (608, 154, N'chuongdongnepal3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (609, 155, N'desacmau1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (610, 155, N'desacmau2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (611, 155, N'desacmau3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (612, 156, N'desengo1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (613, 156, N'desengo2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (614, 156, N'desengo3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (615, 156, N'desengo4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (616, 156, N'desengo5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (617, 157, N'bothachcao1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (618, 158, N'chuoivong18hatbode1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (619, 158, N'chuoivong18hatbode2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (620, 158, N'chuoivong18hatbode3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (621, 158, N'chuoivong18hatbode4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (622, 159, N'cumtinhthethachanhtrang1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (623, 160, N'dabichtuyden1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (624, 160, N'dabichtuyden2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (625, 160, N'dabichtuyden3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (626, 161, N'dakyanite1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (627, 161, N'dakyanite2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (628, 161, N'dakyanite3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (629, 162, N'daphalexanh1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (630, 163, N'dathothachanhtim1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (631, 163, N'dathothachanhtim2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (632, 163, N'dathothachanhtim3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (633, 163, N'dathothachanhtim4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (634, 163, N'dathothachanhtim5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (635, 164, N'chupnenthuytinh1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (636, 164, N'chupnenthuytinh2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (637, 164, N'chupnenthuytinh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (638, 164, N'chupnenthuytinh4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (639, 164, N'chupnenthuytinh5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (640, 165, N'denenhoatulip1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (641, 165, N'denenhoatulip2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (642, 165, N'denenhoatulip3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (643, 165, N'denenhoatulip4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (644, 165, N'denenhoatulip5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (645, 166, N'diemthuytinhmatches1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (646, 166, N'diemthuytinhmatches2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (647, 167, N'bepxongbanhbao1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (648, 167, N'bepxongbanhbao2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (649, 167, N'bepxongbanhbao3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (650, 167, N'bepxongbanhbao4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (651, 167, N'bepxongbanhbao5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (652, 168, N'dekhuechtantinhdaumeditation1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (653, 168, N'dekhuechtantinhdaumeditation2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (654, 169, N'khuechtantinhdaudiacau1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (655, 169, N'khuechtantinhdaudiacau2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (656, 169, N'khuechtantinhdaudiacau3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (657, 169, N'khuechtantinhdaudiacau4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (658, 169, N'khuechtantinhdaudiacau5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (659, 170, N'khuechtantinhdaugiotnuoc1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (660, 170, N'khuechtantinhdaugiotnuoc2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (661, 171, N'khuechtantinhdaureed(lothuytinh)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (662, 171, N'khuechtantinhdaureed(lothuytinh)2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (663, 172, N'khuechtantinhdaurose1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (664, 172, N'khuechtantinhdaurose2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (665, 172, N'khuechtantinhdaurose3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (666, 172, N'khuechtantinhdaurose4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (667, 172, N'khuechtantinhdaurose5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (668, 173, N'khuechtantinhdautron1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (669, 173, N'khuechtantinhdautron2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (670, 173, N'khuechtantinhdautron3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (671, 173, N'khuechtantinhdautron4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (672, 173, N'khuechtantinhdautron5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (673, 174, N'khancottonhoatietcodien1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (674, 174, N'khancottonhoatietcodien2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (675, 174, N'khancottonhoatietcodien3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (676, 174, N'khancottonhoatietcodien4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (677, 175, N'khanhier1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (678, 175, N'khanhier2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (679, 175, N'khanhier3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (680, 175, N'khanhier4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (681, 176, N'khanpain(40x50)1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (682, 176, N'khanpain(40x50)2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (683, 176, N'khanpain(40x50)3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (684, 176, N'khanpain(40x50)4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (685, 176, N'khanpain(40x50)5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (686, 177, N'khantraibancatflowers1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (687, 177, N'khantraibancatflowers2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (688, 178, N'khantraibantranghoaxanh1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (689, 178, N'khantraibantranghoaxanh2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (690, 178, N'khantraibantranghoaxanh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (691, 178, N'khantraibantranghoaxanh4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (692, 178, N'khantraibantranghoaxanh5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (693, 180, N'binhhoadovintage1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (694, 180, N'binhhoadovintage2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (695, 180, N'binhhoadovintage3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (696, 181, N'chaugomsuhoaxanh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (697, 181, N'chaugomsuhoaxanh2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (698, 181, N'chaugomsuhoaxanh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (699, 181, N'chaugomsuhoaxanh4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (700, 181, N'chaugomsuhoaxanh5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (701, 182, N'diacamhoaikebanadecao1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (702, 182, N'diacamhoaikebanadecao2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (703, 182, N'diacamhoaikebanadecao3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (704, 182, N'diacamhoaikebanadecao4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (705, 182, N'diacamhoaikebanadecao5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (706, 183, N'binhnuthoamaixanh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (707, 183, N'binhnuthoamaixanh2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (708, 183, N'binhnuthoamaixanh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (709, 184, N'binhhoabackhacnoi1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (710, 184, N'binhhoabackhacnoi2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (711, 184, N'binhhoabackhacnoi3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (712, 184, N'binhhoabackhacnoi4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (713, 184, N'binhhoabackhacnoi5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (714, 185, N'binhhoasuchuong1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (715, 185, N'binhhoasuchuong2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (716, 185, N'binhhoasuchuong3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (717, 185, N'binhhoasuchuong4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (718, 185, N'binhhoasuchuong5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (719, 186, N'giocoilon1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (720, 186, N'giocoilon2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (721, 186, N'giocoilon3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (722, 186, N'giocoilon4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (723, 186, N'giocoilon5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (724, 186, N'giocoilon6.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (725, 187, N'giodanangxanh1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (726, 187, N'giodanangxanh2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (727, 187, N'giodanangxanh3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (728, 187, N'giodanangxanh4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (729, 187, N'giodanangxanh5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (730, 188, N'giohoaco1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (731, 188, N'giohoaco2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (732, 188, N'giohoaco3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (733, 188, N'giohoaco4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (734, 189, N'giokimloaibingo1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (735, 189, N'giokimloaibingo2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (736, 189, N'giokimloaibingo3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (737, 189, N'giokimloaibingo4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (738, 189, N'giokimloaibingo5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (739, 190, N'giolucbinhtronthap1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (740, 190, N'giolucbinhtronthap2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (741, 190, N'giolucbinhtronthap3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (742, 191, N'giomaysangdai1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (743, 191, N'giomaysangdai2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (744, 191, N'giomaysangdai3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (745, 191, N'giomaysangdai4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (746, 191, N'giomaysangdai5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (747, 192, N'cungomlon1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (748, 192, N'cungomlon2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (749, 192, N'cungomlon3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (750, 192, N'cungomlon4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (751, 192, N'cungomlon5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (752, 193, N'meogommup1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (753, 193, N'meogommup2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (754, 193, N'meogommup3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (755, 194, N'meojuhangom1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (756, 194, N'meojuhangom2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (757, 194, N'meojuhangom3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (758, 194, N'meojuhangom4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (759, 194, N'meojuhangom5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (760, 194, N'meojuhangom6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (761, 195, N'tuonggomrongthien1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (762, 195, N'tuonggomrongthien2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (763, 195, N'tuonggomrongthien3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (764, 195, N'tuonggomrongthien4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (765, 195, N'tuonggomrongthien5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (766, 196, N'tuonggomtieulongtu1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (767, 196, N'tuonggomtieulongtu2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (768, 196, N'tuonggomtieulongtu3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (769, 196, N'tuonggomtieulongtu4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (770, 197, N'tuongmeosudeochuong1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (771, 197, N'tuongmeosudeochuong2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (772, 197, N'tuongmeosudeochuong3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (773, 197, N'tuongmeosudeochuong4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (774, 197, N'tuongmeosudeochuong5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (775, 198, N'bangocao1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (776, 198, N'bangocao2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (777, 198, N'bangocao3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (778, 198, N'bangocao4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (779, 198, N'bangocao5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (780, 198, N'bangocao6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (781, 199, N'banruoudidong1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (782, 199, N'banruoudidong2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (783, 199, N'banruoudidong3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (784, 199, N'banruoudidong4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (785, 199, N'banruoudidong5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (786, 200, N'bantaygomhoa1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (787, 200, N'bantaygomhoa2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (788, 200, N'bantaygomhoa3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (789, 200, N'bantaygomhoa4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (790, 200, N'bantaygomhoa5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (791, 200, N'bantaygomhoa6.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (792, 201, N'bantra1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (793, 201, N'bantra2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (794, 201, N'bantra3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (795, 201, N'bantra4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (796, 201, N'bantra5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (797, 202, N'bohoahuongduongdanlen1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (798, 202, N'bohoahuongduongdanlen2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (799, 202, N'bohoahuongduongdanlen3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (800, 202, N'bohoahuongduongdanlen4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (801, 202, N'bohoahuongduongdanlen5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (802, 203, N'bohoavai1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (803, 203, N'bohoavai2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (804, 203, N'bohoavai3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (805, 203, N'bohoavai4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (806, 203, N'bohoavai5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (807, 204, N'bongtre(dochoicuaboss)1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (808, 204, N'bongtre(dochoicuaboss)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (809, 204, N'bongtre(dochoicuaboss)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (810, 204, N'bongtre(dochoicuaboss)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (811, 204, N'bongtre(dochoicuaboss)5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (812, 205, N'channencaynam1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (813, 205, N'channencaynam2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (814, 205, N'channencaynam3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (815, 205, N'channencaynam4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (816, 205, N'channencaynam5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (817, 206, N'bomhellokitty1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (818, 206, N'bomhellokitty2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (819, 206, N'bomhellokitty3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (820, 206, N'bomhellokitty4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (821, 206, N'bomhellokitty5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (822, 207, N'daybuoctochoanhi1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (823, 207, N'daybuoctochoanhi2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (824, 208, N'daybuoctocvaida1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (825, 208, N'daybuoctocvaida2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (826, 208, N'daybuoctocvaida3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (827, 208, N'daybuoctocvaida4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (828, 208, N'daybuoctocvaida5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (829, 209, N'kepbasicnho1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (830, 209, N'kepbasicnho2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (831, 210, N'kephoahong1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (832, 210, N'kephoahong2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (833, 210, N'kephoahong3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (834, 211, N'kepngoctraidomini1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (835, 211, N'kepngoctraidomini2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (836, 211, N'kepngoctraidomini3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (837, 211, N'kepngoctraidomini4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (838, 211, N'kepngoctraidomini5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (839, 212, N'kepnhungtraitimlove1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (840, 212, N'kepnhungtraitimlove2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (841, 212, N'kepnhungtraitimlove3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (842, 212, N'kepnhungtraitimlove4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (843, 212, N'kepnhungtraitimlove5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (844, 213, N'kepnohongmini1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (845, 213, N'kepnohongmini2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (846, 213, N'kepnohongmini3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (847, 213, N'kepnohongmini4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (848, 214, N'keppuppy1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (849, 214, N'keppuppy2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (850, 215, N'banchaitambauduc(45inch)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (851, 215, N'banchaitambauduc(45inch)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (852, 216, N'banchaitamdai1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (853, 216, N'banchaitamdai2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (854, 216, N'banchaitamdai3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (855, 217, N'dendamuoihinhcau1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (856, 217, N'dendamuoihinhcau2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (857, 217, N'dendamuoihinhcau3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (858, 217, N'dendamuoihinhcau4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (859, 217, N'dendamuoihinhcau5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (860, 218, N'dendamuoimassage1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (861, 218, N'dendamuoimassage2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (862, 218, N'dendamuoimassage3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (863, 219, N'denphunsuongjena1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (864, 219, N'denphunsuongjena2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (865, 219, N'denphunsuongjena3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (866, 219, N'denphunsuongjena4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (867, 219, N'denphunsuongjena5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (868, 219, N'denphunsuongjena6.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (869, 220, N'luocgomatxameocon1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (870, 220, N'luocgomatxameocon2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (871, 220, N'luocgomatxameocon3.jpeg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (872, 220, N'luocgomatxameocon4.jpeg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (873, 221, N'loaquasoi1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (874, 221, N'loaquasoi2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (875, 222, N'binhthuytinhdetox(500ml)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (876, 222, N'binhthuytinhdetox(500ml)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (877, 222, N'binhthuytinhdetox(500ml)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (878, 222, N'binhthuytinhdetox(500ml)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (879, 222, N'binhthuytinhdetox(500ml)5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (880, 223, N'binhthuytinhhailop(350ml)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (881, 223, N'binhthuytinhhailop(350ml)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (882, 223, N'binhthuytinhhailop(350ml)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (883, 223, N'binhthuytinhhailop(350ml)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (884, 223, N'binhthuytinhhailop(350ml)5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (885, 224, N'binhthuytinhhailop(450ml)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (886, 224, N'binhthuytinhhailop(450ml)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (887, 224, N'binhthuytinhhailop(450ml)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (888, 224, N'binhthuytinhhailop(450ml)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (889, 138, N'binhthuytinhsilicon1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (890, 138, N'binhthuytinhsilicon2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (891, 226, N'cocgiunhietbe1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (892, 226, N'cocgiunhietbe2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (893, 226, N'cocgiunhietbe3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (894, 226, N'cocgiunhietbe4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (895, 226, N'cocgiunhietbe5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (896, 227, N'tuideovaidanthucongkaki1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (897, 227, N'tuideovaidanthucongkaki2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (898, 227, N'tuideovaidanthucongkaki3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (899, 227, N'tuideovaidanthucongkaki4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
GO
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (900, 227, N'tuideovaidanthucongkaki5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (901, 227, N'tuideovaidanthucongkaki6.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (902, 227, N'tuideovaidanthucongkaki7.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (903, 227, N'tuideovaidanthucongkaki8.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (904, 228, N'tuihongamber1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (905, 228, N'tuihongamber2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (906, 228, N'tuihongamber3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (907, 228, N'tuihongamber4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (908, 228, N'tuihongamber5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (909, 229, N'tuimausaccodien1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (910, 229, N'tuimausaccodien2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (911, 229, N'tuimausaccodien3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (912, 229, N'tuimausaccodien4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (913, 229, N'tuimausaccodien5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (914, 229, N'tuimausaccodien6.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (915, 230, N'tuitotedaybuoc1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (916, 230, N'tuitotedaybuoc2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (917, 230, N'tuitotedaybuoc3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (918, 230, N'tuitotedaybuoc4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (919, 230, N'tuitotedaybuoc5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (920, 231, N'tuitoteenjoy1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (921, 231, N'tuitoteenjoy2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (922, 231, N'tuitoteenjoy3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (923, 231, N'tuitoteenjoy4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (924, 231, N'tuitoteenjoy5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (925, 232, N'tuitotehoathinhmeo1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (926, 232, N'tuitotehoathinhmeo2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (927, 232, N'tuitotehoathinhmeo3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (928, 232, N'tuitotehoathinhmeo4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (929, 232, N'tuitotehoathinhmeo5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (930, 233, N'bookmarkcavoi(kemhop)1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (931, 233, N'bookmarkcavoi(kemhop)2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (932, 233, N'bookmarkcavoi(kemhop)3.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (933, 233, N'bookmarkcavoi(kemhop)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (934, 234, N'bookmarktrahoa1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (935, 235, N'denphihanhgia1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (936, 235, N'denphihanhgia2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (937, 235, N'denphihanhgia3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (938, 236, N'sohuongthaoa5(giaymix)1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (939, 236, N'sohuongthaoa5(giaymix)2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (940, 236, N'sohuongthaoa5(giaymix)3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (941, 236, N'sohuongthaoa5(giaymix)4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (942, 236, N'sohuongthaoa5(giaymix)5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (943, 237, N'denenvoigom1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (944, 237, N'denenvoigom2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (945, 237, N'denenvoigom3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (946, 237, N'denenvoigom4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (947, 237, N'denenvoigom5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (948, 238, N'gangtaybongteddy1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (949, 238, N'gangtaybongteddy2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (950, 238, N'gangtaybongteddy3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (951, 238, N'gangtaybongteddy4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (952, 239, N'khanchoangcohinhthoi1.jpg', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (953, 239, N'khanchoangcohinhthoi2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (954, 240, N'sacduphongsuoiamanimal1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (955, 240, N'sacduphongsuoiamanimal2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (956, 240, N'sacduphongsuoiamanimal3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (957, 240, N'sacduphongsuoiamanimal4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (958, 240, N'sacduphongsuoiamanimal5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (959, 241, N'settatcotichpeterrabbit1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (960, 241, N'settatcotichpeterrabbit2.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (961, 241, N'settatcotichpeterrabbit3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (962, 241, N'settatcotichpeterrabbit4.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (963, 241, N'settatcotichpeterrabbit5.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (964, 242, N'settatteddy1.webp', 1, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:22:12.947' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (965, 242, N'settatteddy2.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (966, 242, N'settatteddy3.webp', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (967, 242, N'settatteddy4.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (968, 242, N'settatteddy5.jpg', 0, CAST(N'2024-10-12T18:20:48.740' AS DateTime), CAST(N'2024-10-12T18:20:48.740' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (988, 253, N'2f6ab9b1-8227-44db-b0ea-99198938aa68.jpg', 0, CAST(N'2024-12-18T00:22:47.977' AS DateTime), CAST(N'2024-12-18T00:23:41.810' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (990, 253, N'2dd50c15-a184-4eda-bc99-833ec489c937.png', 1, CAST(N'2024-12-18T00:23:38.283' AS DateTime), CAST(N'2024-12-18T00:23:41.810' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (991, 254, N'dunno_square.png', 0, CAST(N'2024-12-18T00:50:01.880' AS DateTime), CAST(N'2024-12-18T00:50:27.707' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (992, 254, N'7313907e-b599-4ac0-bb9d-39df359aa0ab.png', 0, CAST(N'2024-12-18T00:50:23.220' AS DateTime), CAST(N'2024-12-18T00:50:23.220' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (993, 254, N'6a24ded5-7e85-44d3-8a20-75805805e6d9.jpg', 1, CAST(N'2024-12-18T00:50:23.220' AS DateTime), CAST(N'2024-12-18T00:50:27.707' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (994, 255, N'2f6ab9b1-8227-44db-b0ea-99198938aa68_94bf852c-2936-4a78-a4eb-65dc3dd7b673.jpg', 0, CAST(N'2024-12-18T02:24:35.967' AS DateTime), CAST(N'2024-12-18T02:25:03.617' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (995, 255, N'78e1a7ad-aec3-446d-807c-01b403c4765a.png', 1, CAST(N'2024-12-18T02:25:00.413' AS DateTime), CAST(N'2024-12-18T02:25:03.620' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (996, 8, N'89fe68ad-597e-4caa-b63b-f045e6422495.png', 0, CAST(N'2024-12-18T09:50:14.433' AS DateTime), CAST(N'2024-12-18T09:50:14.433' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (997, 256, N'Screenshot 2025-10-21 031451.png', 1, CAST(N'2025-10-21T17:38:07.570' AS DateTime), CAST(N'2025-10-21T17:38:07.570' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (998, 256, N'Screenshot (60).png', 0, CAST(N'2025-10-21T17:38:07.573' AS DateTime), CAST(N'2025-10-21T17:38:07.573' AS DateTime))
INSERT [dbo].[Product_Images] ([img_id], [id_product], [img_name], [is_primary], [created_at], [updated_at]) VALUES (999, 256, N'Screenshot (61).png', 0, CAST(N'2025-10-21T17:38:07.577' AS DateTime), CAST(N'2025-10-21T17:38:07.577' AS DateTime))
SET IDENTITY_INSERT [dbo].[Product_Images] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (35, N'hoanhnghiep2704@gmail.com', N'AQAAAAIAAYagAAAAEOs8tqEp3SoAaMIi5HG5mzt1weDLrGuMeNciO1neSUdZM8DXkIuIY6HVp78VMOn8Pw==', N'La Hoành Nghiệp', 0, CAST(N'2004-04-27' AS Date), 1, CAST(N'2024-12-18T01:30:30.737' AS DateTime), CAST(N'2024-12-18T09:49:34.407' AS DateTime), N'admin')
INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (36, N'nguyenxuanthuong@gmail.com', N'AQAAAAIAAYagAAAAEG+dlHhjGxhJdKQiYEzhjPW6pCw/d727kzcJ0nKPNgcXeHYCeHanQjTOz6ips+4rNg==', N'Nguyễn Xuân Thương', 0, CAST(N'2004-03-22' AS Date), 1, CAST(N'2024-12-18T01:32:17.130' AS DateTime), CAST(N'2024-12-18T01:34:04.907' AS DateTime), N'admin')
INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (37, N'haphungviet359@google.com', N'AQAAAAIAAYagAAAAEHhGb4GnBM2cuatEsd7GaWKqYTZmIV6H38kh4zNg0TAs0Tp8Fqqbi/P9PMdCXtgIDQ==', N'Hạ Phụng Việt', 0, CAST(N'1999-02-22' AS Date), 1, CAST(N'2024-12-18T01:32:52.377' AS DateTime), CAST(N'2024-12-18T01:32:52.377' AS DateTime), N'customer')
INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (38, N'hophuongquyen470@gmail.com', N'AQAAAAIAAYagAAAAEMTHS7pwqRENqyLIpkMAI+R8AKbJTeAhl416m+G87sW5LfElUfiWAcpMfT9YAiTJ3w==', N'Hồ Phương Quyên', 1, CAST(N'2011-11-22' AS Date), 1, CAST(N'2024-12-18T01:33:37.853' AS DateTime), CAST(N'2024-12-18T01:34:22.070' AS DateTime), N'customer')
INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (39, N'cobichha953@icloud.com', N'AQAAAAIAAYagAAAAENM/viFRJwrbCLoI+G7hFs7KDyduFy9PAV0i8Jzv2UH/9ZjiyLzxrHCTAhCshtuP1g==', N'Cồ Bích Hà', 1, CAST(N'2001-03-20' AS Date), 1, CAST(N'2024-12-18T01:35:07.243' AS DateTime), CAST(N'2024-12-18T01:35:07.243' AS DateTime), N'customer')
INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (40, N'nhathi0102@gmail.com', N'AQAAAAIAAYagAAAAEEdrvQeDL71HFUq7ykhN9ToO+CXPaeyOmT8l1Oxcb1rFMKGIG3OdL+oxpdis+0rtBQ==', N'Nhã Thi', 1, CAST(N'2010-02-03' AS Date), 1, CAST(N'2024-12-18T02:22:23.257' AS DateTime), CAST(N'2024-12-18T02:22:23.257' AS DateTime), N'customer')
INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (41, N'lanhale0201@gmail.com', N'AQAAAAIAAYagAAAAEG7c7vXIcraO1to1Scwro8PXKsy5BcywNFtMZpOQshqDP2ZRDb9IsdiCAd9W2pM3pQ==', N'Nhã Lệ', 1, CAST(N'2002-01-02' AS Date), 1, CAST(N'2024-12-18T02:23:32.120' AS DateTime), CAST(N'2024-12-18T02:24:02.057' AS DateTime), N'admin')
INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (42, N'phamminhtoan@gmail.com', N'AQAAAAIAAYagAAAAEAxlj5QtXoJ7fJKwXcOkGCLrFJ2MbQ6KbLolkdpyxI5GkuVAAbpJEG1aJhQYQ1tvng==', N'Phạm Minh Toàn', 0, CAST(N'2004-09-22' AS Date), 1, CAST(N'2024-12-18T02:27:47.130' AS DateTime), CAST(N'2024-12-18T02:27:47.133' AS DateTime), N'customer')
INSERT [dbo].[User] ([user_id], [user_email], [user_password], [user_fullname], [user_sex], [user_birthday], [user_isActive], [created_at], [updated_at], [user_role]) VALUES (43, N'email@email.com', N'AQAAAAIAAYagAAAAEKmuQYcijOUKUofMvgHmULgNpuJYtNkprc+Mkzv1LBosNCtT1dTggZvupMrCvw6teg==', N'Hồ Phương Quyên', 1, CAST(N'2006-02-01' AS Date), 1, CAST(N'2024-12-18T09:47:07.613' AS DateTime), CAST(N'2024-12-18T09:47:07.613' AS DateTime), N'customer')
SET IDENTITY_INSERT [dbo].[User] OFF
GO
ALTER TABLE [dbo].[Address_List] ADD  DEFAULT ((0)) FOR [adrs_isDefault]
GO
ALTER TABLE [dbo].[Address_List] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Address_List] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[Bill] ADD  DEFAULT ((0)) FOR [is_confirmed]
GO
ALTER TABLE [dbo].[Bill] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[Bill_Detail] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Bill_Detail] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[Category] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Category] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF__Product__is_onSa__4CA06362]  DEFAULT ((0)) FOR [is_onSale]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF__Product__created__4D94879B]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF__Product__updated__4E88ABD4]  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[Product_Images] ADD  DEFAULT ((0)) FOR [is_primary]
GO
ALTER TABLE [dbo].[Product_Images] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Product_Images] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__user_isAct__38996AB5]  DEFAULT ((1)) FOR [user_isActive]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__created_at__398D8EEE]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__updated_at__3A81B327]  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__user_role__7D439ABD]  DEFAULT ('customer') FOR [user_role]
GO
ALTER TABLE [dbo].[Address_List]  WITH CHECK ADD  CONSTRAINT [FK__Address_L__id_us__403A8C7D] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([user_id])
GO
ALTER TABLE [dbo].[Address_List] CHECK CONSTRAINT [FK__Address_L__id_us__403A8C7D]
GO
ALTER TABLE [dbo].[Bill]  WITH CHECK ADD FOREIGN KEY([id_adrs])
REFERENCES [dbo].[Address_List] ([adrs_id])
GO
ALTER TABLE [dbo].[Bill]  WITH CHECK ADD  CONSTRAINT [FK__Bill__updated_at__44FF419A] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([user_id])
GO
ALTER TABLE [dbo].[Bill] CHECK CONSTRAINT [FK__Bill__updated_at__44FF419A]
GO
ALTER TABLE [dbo].[Bill_Detail]  WITH CHECK ADD FOREIGN KEY([id_bill])
REFERENCES [dbo].[Bill] ([bill_id])
GO
ALTER TABLE [dbo].[Bill_Detail]  WITH CHECK ADD  CONSTRAINT [FK__Bill_Deta__id_pr__5535A963] FOREIGN KEY([id_product])
REFERENCES [dbo].[Product] ([product_id])
GO
ALTER TABLE [dbo].[Bill_Detail] CHECK CONSTRAINT [FK__Bill_Deta__id_pr__5535A963]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK__Product__id_cate__4F7CD00D] FOREIGN KEY([id_category])
REFERENCES [dbo].[Category] ([category_id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK__Product__id_cate__4F7CD00D]
GO
ALTER TABLE [dbo].[Product_Images]  WITH CHECK ADD  CONSTRAINT [FK__Product_I__id_pr__5AEE82B9] FOREIGN KEY([id_product])
REFERENCES [dbo].[Product] ([product_id])
GO
ALTER TABLE [dbo].[Product_Images] CHECK CONSTRAINT [FK__Product_I__id_pr__5AEE82B9]
GO

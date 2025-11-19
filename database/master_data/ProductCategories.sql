USE [MetaKing]
GO

-- Tạo biến lưu Id danh mục cha
DECLARE @PhoneId UNIQUEIDENTIFIER = NEWID()
DECLARE @LaptopId UNIQUEIDENTIFIER = NEWID()
DECLARE @PCId UNIQUEIDENTIFIER = NEWID()
DECLARE @TabletId UNIQUEIDENTIFIER = NEWID()
DECLARE @HeadphoneId UNIQUEIDENTIFIER = NEWID()

-- Thêm danh mục cha
INSERT INTO [dbo].[AppProductCategories]
           ([Id],[Name],[Code],[Slug],[SortOrder],[CoverPicture],[Visibility],[IsActive],[ParentId],
            [SeoMetaDescription],[ExtraProperties],[ConcurrencyStamp],[CreationTime],[CreatorId])
VALUES
(@PhoneId, N'Điện Thoại', 'PHONE', 'dien-thoai', 1, N'/images/logo/apple.webp', 1, 1, NULL, 
 N'Danh mục các sản phẩm điện thoại thông minh từ Apple, Samsung, Xiaomi...', N'', N'', SYSDATETIME(), NULL),
(@LaptopId, N'Laptop', 'LAPTOP', 'laptop', 2, N'/images/logo/apple.webp', 1, 1, NULL, 
 N'Danh mục laptop hiệu năng cao cho học tập, làm việc và chơi game.', N'', N'', SYSDATETIME(), NULL),
(@PCId, N'PC', 'PC', 'pc', 3, N'/images/logo/apple.webp', 1, 1, NULL, 
 N'Danh mục pc, PC gaming, văn phòng.', N'', N'', SYSDATETIME(), NULL),
(@TabletId, N'Tablet', 'TABLET', 'tablet', 4, N'/images/logo/apple.webp', 1, 1, NULL, 
 N'Danh mục tablet iPad, Samsung Tab, Xiaomi Pad...', N'', N'', SYSDATETIME(), NULL),
(@HeadphoneId, N'Tai Nghe', 'HEADPHONE', 'tai-nghe', 5, N'/images/logo/apple.webp', 1, 1, NULL, 
 N'Danh mục tai nghe Bluetooth, có dây, chụp tai...', N'', N'', SYSDATETIME(), NULL)

-- Thêm các danh mục con cho từng danh mục cha
INSERT INTO [dbo].[AppProductCategories]
           ([Id],[Name],[Code],[Slug],[SortOrder],[CoverPicture],[Visibility],[IsActive],[ParentId],
            [SeoMetaDescription],[ExtraProperties],[ConcurrencyStamp],[CreationTime],[CreatorId])
VALUES
-- Điện thoại
(NEWID(), N'iPhone', 'IPHONE', 'iphone', 1, N'/images/logo/apple.webp', 1, 1, @PhoneId, N'Danh mục iPhone', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'Samsung Galaxy', 'SAMSUNG_PHONE', 'samsung-galaxy', 2, N'/images/logo/apple.webp', 1, 1, @PhoneId, N'Danh mục Samsung Galaxy', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'Xiaomi', 'XIAOMI_PHONE', 'xiaomi', 3, N'/images/logo/apple.webp', 1, 1, @PhoneId, N'Danh mục Xiaomi', N'', N'', SYSDATETIME(), NULL),

-- Laptop
(NEWID(), N'MacBook', 'MACBOOK', 'macbook', 1, N'/images/logo/apple.webp', 1, 1, @LaptopId, N'Danh mục MacBook', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'Dell', 'DELL_LAPTOP', 'dell-laptop', 2, N'/images/logo/apple.webp', 1, 1, @LaptopId, N'Danh mục Dell Laptop', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'HP', 'HP_LAPTOP', 'hp-laptop', 3, N'/images/logo/apple.webp', 1, 1, @LaptopId, N'Danh mục HP Laptop', N'', N'', SYSDATETIME(), NULL),

-- PC
(NEWID(), N'Gaming PC', 'GAMING_PC', 'gaming-pc', 1, N'/images/logo/apple.webp', 1, 1, @PCId, N'Danh mục Gaming PC', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'Office PC', 'OFFICE_PC', 'office-pc', 2, N'/images/logo/apple.webp', 1, 1, @PCId, N'Danh mục Office PC', N'', N'', SYSDATETIME(), NULL),

-- Tablet
(NEWID(), N'iPad', 'IPAD', 'ipad', 1, N'/images/logo/apple.webp', 1, 1, @TabletId, N'Danh mục iPad', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'Samsung Tab', 'SAMSUNG_TAB', 'samsung-tab', 2, N'/images/logo/apple.webp', 1, 1, @TabletId, N'Danh mục Samsung Tab', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'Xiaomi Pad', 'XIAOMI_PAD', 'xiaomi-pad', 3, N'/images/logo/apple.webp', 1, 1, @TabletId, N'Danh mục Xiaomi Pad', N'', N'', SYSDATETIME(), NULL),

-- Tai Nghe
(NEWID(), N'Bluetooth', 'BLUETOOTH_HEADPHONE', 'bluetooth-headphone', 1, N'/images/logo/apple.webp', 1, 1, @HeadphoneId, N'Danh mục tai nghe Bluetooth', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'Có dây', 'WIRED_HEADPHONE', 'wired-headphone', 2, N'/images/logo/apple.webp', 1, 1, @HeadphoneId, N'Danh mục tai nghe có dây', N'', N'', SYSDATETIME(), NULL),
(NEWID(), N'Chụp tai', 'OVER_EAR_HEADPHONE', 'over-ear-headphone', 3, N'/images/logo/apple.webp', 1, 1, @HeadphoneId, N'Danh mục tai nghe chụp tai', N'', N'', SYSDATETIME(), NULL)
GO



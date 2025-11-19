USE [MetaKing]
GO

INSERT INTO [dbo].[AppManufacturers]
           ([Id]
           ,[Name]
           ,[Code]
           ,[Slug]
           ,[CoverPicture]
           ,[Visibility]
           ,[IsActive]
           ,[Country]
           ,[ExtraProperties]
           ,[ConcurrencyStamp]
           ,[CreationTime]
           ,[CreatorId])
    VALUES
	-- Apple
	(NEWID(), N'Apple', 'APPLE', 'apple', N'/images/logo/apple.webp', 1, 1, N'Mỹ', N'', N'', SYSDATETIME(), NULL),

	-- Samsung
	(NEWID(), N'Samsung', 'SAMSUNG', 'samsung', N'/images/logo/samsung.webp', 1, 1, N'Hàn Quốc', N'', N'', SYSDATETIME(), NULL),

	-- Dell
	(NEWID(), N'Dell', 'DELL', 'dell', N'/images/logo/dell.webp', 1, 1, N'Mỹ', N'', N'', SYSDATETIME(), NULL),

	-- HP
	(NEWID(), N'HP', 'HP', 'hp', N'/images/logo/hp.webp', 1, 1, N'Mỹ', N'', N'', SYSDATETIME(), NULL),

	-- Lenovo
	(NEWID(), N'Lenovo', 'LENOVO', 'lenovo', N'/images/logo/lenovo.webp', 1, 1, N'Trung Quốc', N'', N'', SYSDATETIME(), NULL)

	GO

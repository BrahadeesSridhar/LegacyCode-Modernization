-- Create database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Shop')
BEGIN
    CREATE DATABASE Shop;
END
GO

USE Shop;
GO

-- Create Products table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products](
        [Id] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Stock] [int] NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Create Orders table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Orders](
        [Id] [uniqueidentifier] NOT NULL,
        [CustomerId] [uniqueidentifier] NOT NULL,
        [ProductId] [uniqueidentifier] NOT NULL,
        [Quantity] [int] NOT NULL,
        [OrderDate] [datetime2](7) NOT NULL,
        [Express] [bit] NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Orders_Products] FOREIGN KEY([ProductId]) REFERENCES [dbo].[Products] ([Id])
    );
END
GO

-- Insert sample product
IF NOT EXISTS (SELECT * FROM [dbo].[Products])
BEGIN
    INSERT INTO [dbo].[Products] ([Id], [Name], [Stock])
    VALUES (NEWID(), 'Sample Product', 100);
END
GO 
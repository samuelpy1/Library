-- =============================================
-- Library System Database Schema
-- =============================================

-- Create EF Migrations History Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] NVARCHAR(150) NOT NULL,
        [ProductVersion] NVARCHAR(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END
GO

-- =============================================
-- Members Table (Library Members)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Members')
BEGIN
    CREATE TABLE [Members] (
        [MemberId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Name] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [Password] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(20) NOT NULL,
        [RegistrationDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [UK_Members_Email] UNIQUE ([Email])
    );

    CREATE INDEX [IX_Members_Email] ON [Members] ([Email]);
    CREATE INDEX [IX_Members_IsActive] ON [Members] ([IsActive]);
END
GO

-- =============================================
-- Books Table (Library Books)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Books')
BEGIN
    CREATE TABLE [Books] (
        [BookId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [ISBN] NVARCHAR(20) NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Author] NVARCHAR(100) NOT NULL,
        [Publisher] NVARCHAR(100) NOT NULL,
        [PublicationYear] INT NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [TotalCopies] INT NOT NULL DEFAULT 1,
        [AvailableCopies] INT NOT NULL DEFAULT 1,
        [Status] INT NOT NULL DEFAULT 0,
        CONSTRAINT [UK_Books_ISBN] UNIQUE ([ISBN]),
        CONSTRAINT [CK_Books_Copies] CHECK ([AvailableCopies] >= 0 AND [AvailableCopies] <= [TotalCopies])
    );

    CREATE INDEX [IX_Books_ISBN] ON [Books] ([ISBN]);
    CREATE INDEX [IX_Books_Title] ON [Books] ([Title]);
    CREATE INDEX [IX_Books_Author] ON [Books] ([Author]);
    CREATE INDEX [IX_Books_Category] ON [Books] ([Category]);
    CREATE INDEX [IX_Books_Status] ON [Books] ([Status]);
END
GO

-- =============================================
-- Loans Table (Book Loans)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Loans')
BEGIN
    CREATE TABLE [Loans] (
        [LoanId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [BookId] UNIQUEIDENTIFIER NOT NULL,
        [MemberId] UNIQUEIDENTIFIER NOT NULL,
        [LoanDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [DueDate] DATETIME2 NOT NULL,
        [ReturnDate] DATETIME2 NULL,
        [Status] INT NOT NULL DEFAULT 0,
        [LateFee] DECIMAL(18,2) NULL,
        [Notes] NVARCHAR(500) NULL,
        CONSTRAINT [FK_Loans_Books] FOREIGN KEY ([BookId]) REFERENCES [Books]([BookId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Loans_Members] FOREIGN KEY ([MemberId]) REFERENCES [Members]([MemberId]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Loans_BookId] ON [Loans] ([BookId]);
    CREATE INDEX [IX_Loans_MemberId] ON [Loans] ([MemberId]);
    CREATE INDEX [IX_Loans_Status] ON [Loans] ([Status]);
    CREATE INDEX [IX_Loans_LoanDate] ON [Loans] ([LoanDate]);
    CREATE INDEX [IX_Loans_DueDate] ON [Loans] ([DueDate]);
END
GO

-- =============================================
-- Insert Sample Data
-- =============================================

-- Sample Members
INSERT INTO [Members] ([MemberId], [Name], [Email], [Password], [Phone], [RegistrationDate], [IsActive])
VALUES
    (NEWID(), 'João Silva', 'joao.silva@email.com', 'Senha@123', '11987654321', GETUTCDATE(), 1),
    (NEWID(), 'Maria Santos', 'maria.santos@email.com', 'Senha@456', '11987654322', GETUTCDATE(), 1),
    (NEWID(), 'Pedro Oliveira', 'pedro.oliveira@email.com', 'Senha@789', '11987654323', GETUTCDATE(), 1);
GO

-- Sample Books
INSERT INTO [Books] ([BookId], [ISBN], [Title], [Author], [Publisher], [PublicationYear], [Category], [TotalCopies], [AvailableCopies], [Status])
VALUES
    (NEWID(), '978-8535902774', 'Clean Code', 'Robert C. Martin', 'Alta Books', 2009, 'Technology', 3, 3, 0),
    (NEWID(), '978-8550801407', 'Domain-Driven Design', 'Eric Evans', 'Alta Books', 2016, 'Technology', 2, 2, 0),
    (NEWID(), '978-8575225264', 'Refactoring', 'Martin Fowler', 'Novatec', 2019, 'Technology', 2, 2, 0),
    (NEWID(), '978-8535909555', '1984', 'George Orwell', 'Companhia das Letras', 2009, 'Fiction', 5, 5, 0),
    (NEWID(), '978-8535930498', 'Sapiens', 'Yuval Noah Harari', 'L&PM', 2020, 'History', 4, 4, 0);
GO

-- Record migration
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251007000000_InitialLibrarySchema', N'8.0.3');
GO

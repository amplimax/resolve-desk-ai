IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920100150_InitialCreate'
)
BEGIN
    CREATE TABLE [SupportTickets] (
        [Id] uniqueidentifier NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(4000) NOT NULL,
        [RequesterName] nvarchar(120) NOT NULL,
        [RequesterEmail] nvarchar(254) NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [Priority] nvarchar(16) NOT NULL,
        [Category] nvarchar(32) NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [UpdatedAtUtc] datetimeoffset NOT NULL,
        [ResolvedAtUtc] datetimeoffset NULL,
        CONSTRAINT [PK_SupportTickets] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920100150_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SupportTickets_Category] ON [SupportTickets] ([Category]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920100150_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SupportTickets_CreatedAtUtc] ON [SupportTickets] ([CreatedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920100150_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SupportTickets_Status] ON [SupportTickets] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920100150_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920100150_InitialCreate', N'9.0.20');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920121715_AddTicketAiAnalysis'
)
BEGIN
    CREATE TABLE [TicketAiAnalyses] (
        [Id] uniqueidentifier NOT NULL,
        [SupportTicketId] uniqueidentifier NOT NULL,
        [Summary] nvarchar(1000) NOT NULL,
        [SuggestedCategory] nvarchar(32) NOT NULL,
        [SuggestedPriority] nvarchar(16) NOT NULL,
        [SuggestedResponse] nvarchar(4000) NOT NULL,
        [MissingInformationJson] nvarchar(max) NOT NULL,
        [Provider] nvarchar(50) NOT NULL,
        [Model] nvarchar(100) NOT NULL,
        [AnalyzedAtUtc] datetimeoffset NOT NULL,
        CONSTRAINT [PK_TicketAiAnalyses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TicketAiAnalyses_SupportTickets_SupportTicketId] FOREIGN KEY ([SupportTicketId]) REFERENCES [SupportTickets] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920121715_AddTicketAiAnalysis'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TicketAiAnalyses_SupportTicketId] ON [TicketAiAnalyses] ([SupportTicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920121715_AddTicketAiAnalysis'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920121715_AddTicketAiAnalysis', N'9.0.20');
END;

COMMIT;
GO
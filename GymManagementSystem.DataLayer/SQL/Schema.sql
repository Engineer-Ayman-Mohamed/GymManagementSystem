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
IF SCHEMA_ID(N'Gym') IS NULL EXEC(N'CREATE SCHEMA [Gym];');

CREATE TABLE [Gym].[Categories] (
    [Id] int NOT NULL IDENTITY,
    [CategoryName] varchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] varchar(50) NULL DEFAULT 'System',
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);

CREATE TABLE [Gym].[Members] (
    [Id] int NOT NULL IDENTITY,
    [Photo] varchar(500) NULL,
    [JoinDate] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] varchar(50) NULL DEFAULT 'System',
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [Name] varchar(50) NOT NULL,
    [Email] varchar(100) NOT NULL,
    [Phone] varchar(11) NOT NULL,
    [DateOfBirth] date NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [BuildingNumber] int NOT NULL,
    [Street] varchar(30) NOT NULL,
    [City] varchar(30) NOT NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
);

CREATE TABLE [Gym].[Plans] (
    [Id] int NOT NULL IDENTITY,
    [Name] varchar(50) NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    [DurationDays] int NOT NULL,
    [Price] decimal(10,2) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] varchar(50) NULL DEFAULT 'System',
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Plans] PRIMARY KEY ([Id]),
    CONSTRAINT [PlanDurationCheck] CHECK (DurationDays BETWEEN 1 AND 365)
);

CREATE TABLE [Gym].[Trainers] (
    [Id] int NOT NULL IDENTITY,
    [Specialty] nvarchar(max) NOT NULL,
    [HireDate] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] varchar(50) NULL DEFAULT 'System',
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [Name] varchar(50) NOT NULL,
    [Email] varchar(100) NOT NULL,
    [Phone] varchar(11) NOT NULL,
    [DateOfBirth] date NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [BuildingNumber] int NOT NULL,
    [Street] varchar(30) NOT NULL,
    [City] varchar(30) NOT NULL,
    CONSTRAINT [PK_Trainers] PRIMARY KEY ([Id])
);

CREATE TABLE [Gym].[HealthRecords] (
    [Id] int NOT NULL IDENTITY,
    [Height] decimal(5,2) NOT NULL,
    [Weight] decimal(5,2) NOT NULL,
    [BloodType] varchar(5) NOT NULL,
    [Note] nvarchar(500) NULL,
    [MemberId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] varchar(50) NULL DEFAULT 'System',
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_HealthRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HealthRecords_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Gym].[Members] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Gym].[Memberships] (
    [Id] int NOT NULL IDENTITY,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [MemberId] int NOT NULL,
    [PlanId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Memberships] PRIMARY KEY ([Id]),
    CONSTRAINT [MembershipDatesCheck] CHECK (EndDate > StartDate),
    CONSTRAINT [FK_Memberships_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Gym].[Members] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Memberships_Plans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [Gym].[Plans] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Gym].[Sessions] (
    [Id] int NOT NULL IDENTITY,
    [Description] nvarchar NOT NULL,
    [Capacity] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [TrainerId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] varchar(50) NULL DEFAULT 'System',
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
    CONSTRAINT [SessionCapacityCheck] CHECK (Capacity > 0),
    CONSTRAINT [SessionDatesCheck] CHECK (EndDate > StartDate),
    CONSTRAINT [FK_Sessions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Gym].[Categories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Sessions_Trainers_TrainerId] FOREIGN KEY ([TrainerId]) REFERENCES [Gym].[Trainers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Gym].[Bookings] (
    [Id] int NOT NULL IDENTITY,
    [BookingDate] datetime2 NOT NULL,
    [IsAttended] bit NOT NULL DEFAULT CAST(0 AS bit),
    [MemberId] int NOT NULL,
    [SessionId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] varchar(50) NULL DEFAULT 'System',
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Bookings_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Gym].[Members] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Bookings_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Gym].[Sessions] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Bookings_BookingDate] ON [Gym].[Bookings] ([BookingDate]);

CREATE INDEX [IX_Bookings_MemberId] ON [Gym].[Bookings] ([MemberId]);

CREATE INDEX [IX_Bookings_SessionId] ON [Gym].[Bookings] ([SessionId]);

CREATE UNIQUE INDEX [IX_HealthRecords_MemberId] ON [Gym].[HealthRecords] ([MemberId]);

CREATE UNIQUE INDEX [IX_Members_Email] ON [Gym].[Members] ([Email]);

CREATE INDEX [IX_Members_JoinDate] ON [Gym].[Members] ([JoinDate]);

CREATE UNIQUE INDEX [IX_Members_Phone] ON [Gym].[Members] ([Phone]);

CREATE INDEX [IX_Memberships_EndDate] ON [Gym].[Memberships] ([EndDate]);

CREATE INDEX [IX_Memberships_MemberId] ON [Gym].[Memberships] ([MemberId]);

CREATE INDEX [IX_Memberships_PlanId] ON [Gym].[Memberships] ([PlanId]);

CREATE INDEX [IX_Memberships_StartDate] ON [Gym].[Memberships] ([StartDate]);

CREATE INDEX [IX_Sessions_CategoryId] ON [Gym].[Sessions] ([CategoryId]);

CREATE INDEX [IX_Sessions_EndDate] ON [Gym].[Sessions] ([EndDate]);

CREATE INDEX [IX_Sessions_StartDate] ON [Gym].[Sessions] ([StartDate]);

CREATE INDEX [IX_Sessions_TrainerId] ON [Gym].[Sessions] ([TrainerId]);

CREATE UNIQUE INDEX [IX_Trainers_Email] ON [Gym].[Trainers] ([Email]);

CREATE INDEX [IX_Trainers_HireDate] ON [Gym].[Trainers] ([HireDate]);

CREATE UNIQUE INDEX [IX_Trainers_Phone] ON [Gym].[Trainers] ([Phone]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260514132637_DatabaseSchema', N'11.0.0-preview.4.26230.115');

COMMIT;
GO


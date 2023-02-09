CREATE TABLE [Event] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    [Name] VARCHAR(100) NOT NULL,
    [Description] VARCHAR(2000) NOT NULL
);

CREATE TABLE [EventRecurrence] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    [EventId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Event]([Id]),
    [DayOfWeek] INTEGER NOT NULL,
    [StartTime] TIME(0) NOT NULL,
    [EndTime] TIME(0) NOT NULL
);

CREATE TABLE [NewsletterSubscription] (
    [Email] VARCHAR(100) PRIMARY KEY
);

CREATE TABLE [StockedProducts] (
    [LicenceNo] VARCHAR(8) PRIMARY KEY
);

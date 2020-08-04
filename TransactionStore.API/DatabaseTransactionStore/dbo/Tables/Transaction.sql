CREATE TABLE [dbo].[Transaction] (
    [Id]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [LeadId]     BIGINT        NOT NULL,
    [TypeId]     TINYINT       NOT NULL,
    [CurrencyId] TINYINT       NOT NULL,
    [Amount]     MONEY         NOT NULL,
    [Timestamp]  DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_TRANSACTION] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [Transaction_fk0] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[Type] ([Id]),
    CONSTRAINT [Transaction_fk1] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currency] ([Id])
);


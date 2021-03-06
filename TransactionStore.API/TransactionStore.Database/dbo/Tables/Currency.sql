﻿CREATE TABLE [dbo].[Currency] (
    [Id]   TINYINT       IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (30) NOT NULL,
    [Code] NVARCHAR (3)  NOT NULL,
    CONSTRAINT [PK_CURRENCY] PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Code] ASC)
);


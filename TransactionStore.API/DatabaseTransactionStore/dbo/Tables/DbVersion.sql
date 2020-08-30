CREATE TABLE [dbo].[DbVersion] (
    [Id]   INT   IDENTITY  NOT NULL,
    [Created] DATETIME2(0) NOT NULL,
    [DbVersion] NVARCHAR (8)  NOT NULL
  
);


﻿CREATE TABLE [dbo].[FileStatus] (
    [Id]   UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWID(),
    [Name] NVARCHAR (20) NULL, 
    CONSTRAINT [PK_FileStatus] PRIMARY KEY ([Id])
);

ALTER TABLE dbo.GlobomanticsUser
ADD [AccessFailedCount] [SMALLINT] NOT NULL DEFAULT 0,
	[LockoutEnd] [DATETIMEOFFSET](7) NULL,
	[LockoutEnabled] BIT NOT NULL DEFAULT 0;

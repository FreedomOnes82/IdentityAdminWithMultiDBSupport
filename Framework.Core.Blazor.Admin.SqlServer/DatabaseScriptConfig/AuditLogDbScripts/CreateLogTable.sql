IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[$schemaname$].[AuditLogs]') AND type in (N'U'))
BEGIN
CREATE TABLE [$schemaname$].[AuditLogs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](256) NOT NULL,
	[Status] [int] NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](450) NOT NULL,
	[LastModifiedBy] [nvarchar](450) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[LastModifiedDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
ALTER TABLE [$schemaname$].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_AspNetUsers_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [$schemaname$].[AspNetUsers] ([Id])
ON DELETE CASCADE
END
GO

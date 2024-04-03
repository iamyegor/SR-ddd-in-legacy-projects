CREATE TABLE Synchronization (
                                 Name NVARCHAR(50) PRIMARY KEY,
                                 IsSyncRequired BIT NOT NULL DEFAULT 0
);
GO

ALTER TABLE PRD_TBL
    ADD IsSyncNeeded BIT NOT NULL DEFAULT 0;
GO

INSERT INTO [Synchronization] (Name, IsSyncRequired)
VALUES ('Product', 0);
GO

ALTER TABLE [Synchronization]
    ADD RowVersion rowversion;
GO

CREATE TRIGGER ProductSync ON [dbo].[PRD_TBL]
    AFTER UPDATE, INSERT
    AS
BEGIN
    SET NOCOUNT ON;

    IF (UPDATE(NM_CLM) OR UPDATE(WT) OR UPDATE(WT_KG))
        BEGIN
            UPDATE [dbo].[PRD_TBL]
            SET	IsSyncNeeded = 1
            WHERE EXISTS (SELECT 1 FROM inserted WHERE [dbo].[PRD_TBL].NMB_CM = inserted.NMB_CM)

            UPDATE [dbo].[Synchronization]
            SET	IsSyncRequired = 1
            WHERE Name = 'Product'
        END
END
GO

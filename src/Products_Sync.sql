ALTER TABLE [dbo].[PRD_TBL]
ADD IsSyncNeeded bit NOT NULL DEFAULT 0
GO

DELETE FROM [dbo].[Synchronization]
GO

ALTER TABLE [dbo].[Synchronization]
ADD Name VARCHAR(50)
GO

INSERT INTO [dbo].[Synchronization] (Name, IsSyncRequired)
VALUES ('Delivery', 0), ('Product', 0)
GO

CREATE TRIGGER [dbo].[ProductSync] ON [dbo].[PRD_TBL]
AFTER INSERT, UPDATE
AS 
BEGIN 
	SET NOCOUNT ON;

	IF (UPDATE(NM_CLM) OR UPDATE(WT) OR UPDATE(WT_KG))
	BEGIN
		UPDATE [dbo].[PRD_TBL]
		SET IsSyncNeeded = 1
		WHERE EXISTS (SELECT 1 FROM inserted WHERE [dbo].[PRD_TBL].NMB_CM = inserted.NMB_CM)
		
		UPDATE [dbo].[Synchronization]
		SET IsSyncRequired = 1
		WHERE Name = 'Product'
	END
END 
GO

ALTER TRIGGER [dbo].[AddressSync] ON [dbo].[ADDR_TBL]
AFTER UPDATE, INSERT
AS 
BEGIN
    SET NOCOUNT ON;

	IF(UPDATE(STR) OR UPDATE(CT_ST) OR UPDATE(ZP))
	BEGIN
		UPDATE [dbo].[DLVR_TBL]
		SET IsSyncNeeded = 1
		WHERE [dbo].[DLVR_TBL].NMB_CLM = (SELECT DLVR FROM inserted)
		
		UPDATE [dbo].[Synchronization]
		SET IsSyncRequired = 1
		WHERE Name = 'Delivery'
	END
END




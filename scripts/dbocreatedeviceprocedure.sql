CREATE PROCEDURE dbo.CreateDevice @Id NVARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled INT,
    @Type NVARCHAR(2),
    @Battery INT = NULL,
    @OS NVARCHAR(100) = NULL,
    @IP NVARCHAR(100) = NULL,
    @Network NVARCHAR(100) = NULL
AS
BEGIN
    SET
NOCOUNT ON;

BEGIN TRY
BEGIN
TRANSACTION;

INSERT INTO Device (Id, Name, IsEnabled)
VALUES (@Id, @Name, @IsEnabled);

IF
@Type = 'SW'
BEGIN
            DECLARE
@SwId INT = (SELECT ISNULL(MAX(Id), 0) + 1 FROM Smartwatch);
INSERT INTO Smartwatch (Id, BatteryPercentage, DeviceId)
VALUES (@SwId, @Battery, @Id);
END
ELSE IF @Type = 'P-'
BEGIN
            DECLARE
@PcId INT = (SELECT ISNULL(MAX(Id), 0) + 1 FROM PersonalComputer);
INSERT INTO PersonalComputer (Id, OperationSystem, DeviceId)
VALUES (@PcId, @OS, @Id);
END
ELSE IF @Type = 'ED'
BEGIN
            DECLARE
@EdId INT = (SELECT ISNULL(MAX(Id), 0) + 1 FROM Embedded);
INSERT INTO Embedded (Id, IpAddress, NetworkName, DeviceId)
VALUES (@EdId, @IP, @Network, @Id);
END

COMMIT;
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW; -- rethrow the original error
END CATCH
END

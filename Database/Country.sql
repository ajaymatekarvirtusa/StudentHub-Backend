/* =====================================================================
   dbo.Country  -  master table (SQL Server 2014 compatible)

   This is the same DDL that EF Core generates from CountryConfiguration.
   Code-first: normally create it with the migration instead:
       Add-Migration AddCountry  -Project Repositories -StartupProject API
       Update-Database           -Project Repositories -StartupProject API
   Use this script only when a DBA must create the table by hand
   (do NOT run both, or the migration will fail because the table exists).
   ===================================================================== */

IF OBJECT_ID(N'dbo.Country', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Country
    (
        Id            INT            IDENTITY(1,1) NOT NULL,
        Name          NVARCHAR(100)  NOT NULL,
        Code          VARCHAR(3)     NOT NULL,          -- ISO 3166: IN / IND
        IsActive      BIT            NOT NULL,
        IsDeleted     BIT            NOT NULL,          -- soft delete
        CreatedDate   DATETIME2(0)   NOT NULL CONSTRAINT DF_Country_CreatedDate DEFAULT (SYSUTCDATETIME()),
        CreatedBy     NVARCHAR(100)  NOT NULL,
        ModifiedDate  DATETIME2(0)   NULL,
        ModifiedBy    NVARCHAR(100)  NULL,

        CONSTRAINT PK_Country PRIMARY KEY CLUSTERED (Id)
    );

    -- Unique only among non-deleted rows, so a deleted name/code can be reused.
    CREATE UNIQUE NONCLUSTERED INDEX IX_Country_Name ON dbo.Country (Name) WHERE IsDeleted = 0;
    CREATE UNIQUE NONCLUSTERED INDEX IX_Country_Code ON dbo.Country (Code) WHERE IsDeleted = 0;
END
GO

/* ---------------------------- OPTIONAL SEED ---------------------------- */
--INSERT INTO dbo.Country (Name, Code, IsActive, IsDeleted, CreatedBy)
--VALUES 
--    (N'India',         'IN', 1, 0, N'system'),
--    (N'Nepal',         'NP', 1, 0, N'system'),
--    (N'Sri Lanka',     'LK', 1, 0, N'system'),
--    (N'Bangladesh',    'BD', 1, 0, N'system'),
--    (N'Bhutan',        'BT', 1, 0, N'system'),
--    (N'United States', 'US', 1, 0, N'system'),
--    (N'United Kingdom','GB', 1, 0, N'system'),
--    (N'Australia',     'AU', 1, 0, N'system'),
--    (N'Canada',        'CA', 1, 0, N'system'),
--    (N'Germany',       'DE', 1, 0, N'system'),
--    (N'France',        'FR', 1, 0, N'system'),
--    (N'Japan',         'JP', 1, 0, N'system'

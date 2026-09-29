/* =====================================================================
   StudentHub - StudentDetail table + sp_StudentDetail
   Matches Repositories/StudentRepository.cs and Data/Models/StudentDetail.cs

   Actions used by the repository:
     INSERT   @Name, @Dob, @MobileNo, @CreatedBy      -> returns new Id
     GETBYID  @StudentId                              -> returns 1 row
     GETALL   (none)                                  -> returns all active rows
     UPDATE   @Id, @Name, @Dob, @MobileNo, @ModifiedBy -> returns Id (0 if not found)
     DELETE   @StudentId, @ModifiedBy                 -> returns 1 if deleted, else 0 (soft delete)
   ===================================================================== */

-- CREATE DATABASE StudentHubDb;
-- GO
-- USE StudentHubDb;
-- GO

/* ----------------------------- TABLE -------------------------------- */
IF OBJECT_ID(N'dbo.StudentDetail', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StudentDetail
    (
        Id            INT            IDENTITY(1,1) NOT NULL,
        Name          NVARCHAR(150)  NOT NULL,
        Dob           DATE           NULL,
        MobileNo      VARCHAR(15)    NULL,
        CreatedDate   DATETIME2(0)   NOT NULL CONSTRAINT DF_StudentDetail_CreatedDate DEFAULT (SYSDATETIME()),
        CreatedBy     INT            NULL,
        ModifiedDate  DATETIME2(0)   NULL,
        ModifiedBy    INT            NULL,
        IsDeleted     BIT            NOT NULL CONSTRAINT DF_StudentDetail_IsDeleted DEFAULT (0),

        CONSTRAINT PK_StudentDetail PRIMARY KEY CLUSTERED (Id)
    );

    CREATE NONCLUSTERED INDEX IX_StudentDetail_IsDeleted
        ON dbo.StudentDetail (IsDeleted) INCLUDE (Name, MobileNo);
END
GO

/* ------------------------- STORED PROCEDURE ------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_StudentDetail
    @Action      VARCHAR(20),
    @Id          INT            = NULL,   -- used by UPDATE
    @StudentId   INT            = NULL,   -- used by GETBYID / DELETE
    @Name        NVARCHAR(150)  = NULL,
    @Dob         VARCHAR(10)    = NULL,   -- 'yyyy-MM-dd' (model property is string)
    @MobileNo    VARCHAR(15)    = NULL,
    @CreatedBy   INT            = NULL,
    @ModifiedBy  INT            = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DobDate DATE = TRY_CONVERT(DATE, NULLIF(LTRIM(RTRIM(@Dob)), ''));

    /* ---------- INSERT ---------- */
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO dbo.StudentDetail (Name, Dob, MobileNo, CreatedDate, CreatedBy, IsDeleted)
        VALUES (@Name, @DobDate, @MobileNo, SYSDATETIME(), @CreatedBy, 0);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
        RETURN;
    END

    /* ---------- GET BY ID ---------- */
    IF @Action = 'GETBYID'
    BEGIN
        SELECT  Id,
                Name,
                CONVERT(VARCHAR(10), Dob, 23) AS Dob,   -- yyyy-MM-dd
                MobileNo,
                CreatedDate,
                CreatedBy,
                ModifiedDate,
                ModifiedBy,
                CAST(IsDeleted AS INT)        AS IsDeleted
        FROM    dbo.StudentDetail
        WHERE   Id = @StudentId
          AND   IsDeleted = 0;
        RETURN;
    END

    /* ---------- GET ALL ---------- */
    IF @Action = 'GETALL'
    BEGIN
        SELECT  Id,
                Name,
                CONVERT(VARCHAR(10), Dob, 23) AS Dob,
                MobileNo,
                CreatedDate,
                CreatedBy,
                ModifiedDate,
                ModifiedBy,
                CAST(IsDeleted AS INT)        AS IsDeleted
        FROM    dbo.StudentDetail
        WHERE   IsDeleted = 0
        ORDER BY Id DESC;
        RETURN;
    END

    /* ---------- UPDATE ---------- */
    IF @Action = 'UPDATE'
    BEGIN
        UPDATE  dbo.StudentDetail
        SET     Name         = @Name,
                Dob          = @DobDate,
                MobileNo     = @MobileNo,
                ModifiedDate = SYSDATETIME(),
                ModifiedBy   = @ModifiedBy
        WHERE   Id = @Id
          AND   IsDeleted = 0;

        SELECT CASE WHEN @@ROWCOUNT > 0 THEN @Id ELSE 0 END AS Id;
        RETURN;
    END

    /* ---------- DELETE (soft) ---------- */
    IF @Action = 'DELETE'
    BEGIN
        UPDATE  dbo.StudentDetail
        SET     IsDeleted    = 1,
                ModifiedDate = SYSDATETIME(),
                ModifiedBy   = @ModifiedBy
        WHERE   Id = @StudentId
          AND   IsDeleted = 0;

        SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Result;
        RETURN;
    END

    RAISERROR('Invalid @Action value: %s', 16, 1, @Action);
END
GO

/* --------------------------- QUICK TEST ----------------------------- */
-- EXEC dbo.sp_StudentDetail @Action='INSERT', @Name=N'Ravi Kumar', @Dob='2002-05-14', @MobileNo='9876543210', @CreatedBy=1;
-- EXEC dbo.sp_StudentDetail @Action='GETALL';
-- EXEC dbo.sp_StudentDetail @Action='GETBYID', @StudentId=1;
-- EXEC dbo.sp_StudentDetail @Action='UPDATE', @Id=1, @Name=N'Ravi K', @Dob='2002-05-14', @MobileNo='9000000000', @ModifiedBy=1;
-- EXEC dbo.sp_StudentDetail @Action='DELETE', @StudentId=1, @ModifiedBy=1;

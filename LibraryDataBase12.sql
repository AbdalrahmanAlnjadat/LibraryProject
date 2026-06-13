IF DB_ID('LibraryDB') IS NULL
BEGIN
    CREATE DATABASE LibraryDB;
END
GO

USE LibraryDB;
GO

--------------------------------------------------
-- Roles
--------------------------------------------------
CREATE TABLE Roles
(
    RoleID INT PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

INSERT INTO Roles VALUES (1, 'Librarian');
INSERT INTO Roles VALUES (2, 'Member');

--------------------------------------------------
-- Users
--------------------------------------------------
CREATE TABLE Users
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,

    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleID)
        REFERENCES Roles(RoleID)
);

-- Default librarian login:
-- Email: admin@library.local
-- Password: Admin123!
INSERT INTO Users (FullName, Email, Password, RoleID)
VALUES
(
    'System Librarian',
    'admin@library.local',
    '$2a$11$CXu7PANb5rqG8ZtOvBD5rODTQ/O2CDdNnKdJXFJgBJNxcS5kAeQRO',
    1
);

--------------------------------------------------
-- Books
--------------------------------------------------
CREATE TABLE Books
(
    BookID INT IDENTITY(1,1) PRIMARY KEY,

    Title NVARCHAR(300) NOT NULL,
    Author NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100) NOT NULL,

    Quantity INT NOT NULL,
    AvailableQuantity INT NOT NULL,

    Price DECIMAL(10,2) NOT NULL,
    DailyLateFee DECIMAL(10,2) NOT NULL CONSTRAINT DF_Books_DailyLateFee DEFAULT 1.00,
    PurchasePrice DECIMAL(10,2) NOT NULL CONSTRAINT DF_Books_PurchasePrice DEFAULT 0.00,

    CONSTRAINT CK_Books_Quantity CHECK (Quantity >= 0),
    CONSTRAINT CK_Books_AvailableQuantity CHECK (AvailableQuantity >= 0 AND AvailableQuantity <= Quantity),
    CONSTRAINT CK_Books_Price CHECK (Price >= 0),
    CONSTRAINT CK_Books_DailyLateFee CHECK (DailyLateFee >= 0),
    CONSTRAINT CK_Books_PurchasePrice CHECK (PurchasePrice >= 0)
);

--------------------------------------------------
-- Borrow Records
--------------------------------------------------
CREATE TABLE BorrowRecords
(
    BorrowID INT IDENTITY(1,1) PRIMARY KEY,

    UserID INT NOT NULL,
    BookID INT NOT NULL,

    BorrowDate DATETIME NOT NULL,
    DueDate DATETIME NOT NULL,
    ReturnDate DATETIME NULL,

    Status NVARCHAR(50) NOT NULL,
    IsPurchased BIT NOT NULL CONSTRAINT DF_BorrowRecords_IsPurchased DEFAULT 0,

    CONSTRAINT FK_BorrowRecords_Users
        FOREIGN KEY (UserID)
        REFERENCES Users(UserID),

    CONSTRAINT FK_BorrowRecords_Books
        FOREIGN KEY (BookID)
        REFERENCES Books(BookID)
);

--------------------------------------------------
-- Payments
--------------------------------------------------
CREATE TABLE Payments
(
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,

    BorrowID INT NOT NULL,
    UserID INT NOT NULL,

    Amount DECIMAL(10,2) NOT NULL,

    PaymentStatus NVARCHAR(50) NOT NULL,
    PaymentType NVARCHAR(50) NOT NULL,

    CreatedDate DATETIME NOT NULL,

    CONSTRAINT FK_Payments_BorrowRecords
        FOREIGN KEY (BorrowID)
        REFERENCES BorrowRecords(BorrowID),

    CONSTRAINT FK_Payments_Users
        FOREIGN KEY (UserID)
        REFERENCES Users(UserID)
);
GO

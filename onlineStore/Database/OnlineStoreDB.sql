-- ========================================================= 
-- CREATE DATABASE 
-- ========================================================= 
CREATE DATABASE OnlineStoreDB; 
USE OnlineStoreDB; 

--CREATE TABLE 

-- ========================================================= 
-- Roles Table 
-- ========================================================= 
CREATE TABLE Roles (
	Id INT		PRIMARY KEY IDENTITY(1,1),
	Name		NVARCHAR(50) NOT NULL UNIQUE, 
); 
-- ========================================================= 
-- Users Table 
-- ========================================================= 
CREATE TABLE Users ( 
	Id				INT PRIMARY KEY IDENTITY(1,1),
	FullName		NVARCHAR(100) NOT NULL,
	Email			NVARCHAR(150) NOT NULL UNIQUE,
	PasswordHash	NVARCHAR(355) NOT NULL,
	IsApproved		BIT NOT NULL DEFAULT 0,
	IsBlocked		BIT NOT NULL DEFAULT 0,
	RoleId			INT NOT NULL,

	CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId)
	REFERENCES Roles(Id) 
); 

-- PasswordHash:
-- يتم تخزينها في قاعدة البيانات بدل كلمة المرور الأصلية
-- بدل ما نخزن: MyPassword123 
-- نخزن: a$11$N9qo8uLOickgx2ZMRZoMye... 

-- ========================================================= 
-- Categories Table 
-- ========================================================= 
CREATE TABLE Categories ( 
	Id			INT IDENTITY(1,1) PRIMARY KEY,
	Name		NVARCHAR(100) NOT NULL UNIQUE 
);

-- ========================================================= 
-- Products Table 
-- ========================================================= 
CREATE TABLE Products (
	Id				INT IDENTITY(1,1) PRIMARY KEY,
	Name			NVARCHAR(150) NOT NULL,
	Description		NVARCHAR(500) NULL,
	Price			DECIMAL(10,2) NOT NULL,
	ImageUrl		NVARCHAR(300) NULL,
	CategoryId		INT NOT NULL,
	
	CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId)
	REFERENCES Categories(Id)
);

-- ========================================================= 
-- Cart Table (One-to-One with Users) 
-- ========================================================= 
CREATE TABLE Cart ( 
	Id				INT IDENTITY(1,1) PRIMARY KEY, 
	UserId			INT NOT NULL UNIQUE, -- UNIQUE => One-to-One relationship 

	CONSTRAINT FK_Cart_Users FOREIGN KEY (UserId) 
	REFERENCES Users(Id) ON DELETE CASCADE 
); 

-- ========================================================= 
-- CartItems Table (Junction table between Cart & Products) 
-- ========================================================= 
CREATE TABLE CartItems ( 
	Id				INT IDENTITY(1,1) PRIMARY KEY, CartId INT NOT NULL,
	ProductId		INT NOT NULL,
	Quantity	INT NOT NULL DEFAULT 1,

	CONSTRAINT FK_CartItems_Cart FOREIGN KEY (CartId)
	REFERENCES Cart(Id) ON DELETE CASCADE,

	CONSTRAINT FK_CartItems_Products FOREIGN KEY (ProductId)
	REFERENCES Products(Id) 
); 

-- ========================================================= 
-- Default Roles 
-- ========================================================= 

INSERT INTO Roles (Name) 
VALUES ('Admin'),('User'); 

SELECT * FROM ROLES
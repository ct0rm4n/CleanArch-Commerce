
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FastCommerce')
BEGIN
    CREATE DATABASE [FastCommerce];
END


USE [FastCommerce];

	
CREATE TABLE FastCommerce.dbo.[User] (
	Id int IDENTITY(1,1) NOT NULL,
	Email varchar(255) COLLATE Latin1_General_CI_AS NOT NULL,
	Password varchar(255) COLLATE Latin1_General_CI_AS NOT NULL,
	FirstName varchar(100) COLLATE Latin1_General_CI_AS NOT NULL,
	LastName varchar(100) COLLATE Latin1_General_CI_AS NOT NULL,
	PhoneNumber varchar(20) COLLATE Latin1_General_CI_AS NOT NULL,
	InsertedDate datetime DEFAULT getdate() NOT NULL,
	Status int NULL,
	CONSTRAINT PK__Users__3214EC076114C424 PRIMARY KEY (Id)
);



	

CREATE TABLE FastCommerce.dbo.Product (
	InsertedDate datetime NULL,
	LastModifiedDate datetime NULL,
	Status varchar(50) COLLATE Latin1_General_CI_AS NULL,
	Name varchar(50) COLLATE Latin1_General_CI_AS NULL,
	Title nchar(10) COLLATE Latin1_General_CI_AS NULL,
	ShortDescription varchar(200) COLLATE Latin1_General_CI_AS NULL,
	FullDescription varchar(MAX) COLLATE Latin1_General_CI_AS NULL,
	Mpn varchar(20) COLLATE Latin1_General_CI_AS NULL,
	Type varchar(100) COLLATE Latin1_General_CI_AS NULL,
	Price decimal(10,0) NULL,
	Html varchar(MAX) COLLATE Latin1_General_CI_AS NULL,
	Seo varchar(50) COLLATE Latin1_General_CI_AS NULL,
	Publish bit NULL,
	Deleted bit NULL,
	DateStart datetime  NULL,
	DateEnd datetime  NULL,
	Id int IDENTITY(1,1) NOT NULL,
	CONSTRAINT PK_Product PRIMARY KEY (Id)
);


CREATE TABLE FastCommerce.dbo.ProductImage (
	Id int IDENTITY(1,1) NOT NULL,
	InsertedDate datetime NULL,
	ProductId int NOT NULL,
	LastModifiedDate datetime NULL,
	[Binary] ntext COLLATE Latin1_General_CI_AS NULL,
	Status int NOT NULL,
	Publish bit NULL,
	CONSTRAINT PK_ProductImage PRIMARY KEY (Id)
);

ALTER TABLE FastCommerce.dbo.ProductImage ADD CONSTRAINT FK__ProductImage__Image FOREIGN KEY (ProductId) REFERENCES FastCommerce.dbo.Product(Id);

CREATE TABLE FastCommerce.dbo.OrderItem (
	Id int IDENTITY(1,1) NOT NULL,
	OrderId int NOT NULL,
	ProductId int NOT NULL,
	Quantity int NOT NULL,
	UnitPrice decimal(10,2) NOT NULL,
	TotalPrice decimal(10,2) NOT NULL,
	CreatedDate datetime DEFAULT getdate() NOT NULL,
	LastModifiedDate datetime DEFAULT getdate() NOT NULL,
	Status int NOT NULL,
	CONSTRAINT PK__OrderIte__3214EC077C547BAC PRIMARY KEY (Id)
);

	
ALTER TABLE FastCommerce.dbo.OrderItem ADD CONSTRAINT FK__OrderItem__Produ__1332DBDC FOREIGN KEY (ProductId) REFERENCES FastCommerce.dbo.Product(Id);



CREATE TABLE FastCommerce.dbo.[Order] (
	Id int IDENTITY(1,1) NOT NULL,
	UserId int NOT NULL,
	OrderDate datetime DEFAULT getdate() NOT NULL,
	TotalPrice decimal(10,2) NOT NULL,
	Status int NOT NULL,
	DeliveryAddress varchar(255) COLLATE Latin1_General_CI_AS NOT NULL,
	DeliveryCity varchar(100) COLLATE Latin1_General_CI_AS NOT NULL,
	DeliveryState varchar(50) COLLATE Latin1_General_CI_AS NOT NULL,
	DeliveryZipCode varchar(20) COLLATE Latin1_General_CI_AS NOT NULL,
	CreatedDate datetime DEFAULT getdate() NOT NULL,
	LastModifiedDate datetime DEFAULT getdate() NOT NULL,
	CONSTRAINT PK__Order3__3214EC0735352361 PRIMARY KEY (Id)
);

ALTER TABLE FastCommerce.dbo.OrderItem ADD CONSTRAINT OrderItem_Order_FK FOREIGN KEY (OrderId) REFERENCES FastCommerce.dbo.[Order](Id);


ALTER TABLE FastCommerce.dbo.[Order] ADD CONSTRAINT FK__Order3__UserId__0E6E26BF FOREIGN KEY (UserId) REFERENCES FastCommerce.dbo.[User](Id);

CREATE TABLE FastCommerce.dbo.ShoppingCartItem (
	Id int IDENTITY(1,1) NOT NULL,
	UserId int NOT NULL,
	ProductId int NOT NULL,
	Quantity int NOT NULL,
	UnitPrice decimal(10,2) NOT NULL,
	TotalPrice decimal(10,2) NOT NULL,
	InsertionDate datetime DEFAULT getdate() NOT NULL,
	CreatedDate datetime DEFAULT getdate() NOT NULL,
	LastModifiedDate datetime DEFAULT getdate() NOT NULL,
	Status varchar(15) COLLATE Latin1_General_CI_AS NOT NULL,
	InsertedDate datetime DEFAULT getdate() NOT NULL,
	CONSTRAINT PK__Shopping__3214EC07B0E71BB4 PRIMARY KEY (Id)
);

ALTER TABLE FastCommerce.dbo.ShoppingCartItem ADD CONSTRAINT FK__ShoppingCartItem__UserId FOREIGN KEY (UserId) REFERENCES FastCommerce.dbo.[User](Id);
ALTER TABLE FastCommerce.dbo.ShoppingCartItem ADD CONSTRAINT FK__ShoppingCartItem__ProductId FOREIGN KEY (ProductId) REFERENCES FastCommerce.dbo.[Product](Id);

CREATE TABLE FastCommerce.dbo.Category (
	Id int IDENTITY(1,1) NOT NULL,
	InsertedDate datetime NULL,
	LastModifiedDate datetime NULL,
	Name varchar(100) COLLATE Latin1_General_CI_AS NULL,
	Html text COLLATE Latin1_General_CI_AS NULL,
	Seo varchar(100) COLLATE Latin1_General_CI_AS NULL,
	Publish bit NULL,
	Deleted bit NULL,
	DateStart datetime NULL,
	DateEnd datetime NULL,
	Status nvarchar(50) COLLATE Latin1_General_CI_AS NULL,
	CONSTRAINT PK__Category__3214EC0725816E84 PRIMARY KEY (Id)
);



CREATE TABLE FastCommerce.dbo.BlogPost (
	InsertedDate datetime NULL,
	LastModifiedDate datetime NULL,
	Status nvarchar(50) COLLATE Latin1_General_CI_AS NULL,
	Name nvarchar(50) COLLATE Latin1_General_CI_AS NULL,
	Url nvarchar(50) COLLATE Latin1_General_CI_AS NULL,
	Publish bit NULL,
	Deleted bit NULL,
	DateStart datetime NOT NULL,
	DateEnd datetime NOT NULL,
	Id int IDENTITY(1,1) NOT NULL,
	UserId int NULL,
	Html nvarchar(MAX) COLLATE Latin1_General_CI_AS NULL,
	CONSTRAINT PK__BlogPost__3214EC073F25D042 PRIMARY KEY (Id)
);


CREATE TABLE FastCommerce.dbo.Banner (
	Id int IDENTITY(1,1) NOT NULL,
	InsertedDate datetime NULL,
	LastModifiedDate datetime NULL,
	Title varchar(50) COLLATE Latin1_General_CI_AS NULL,
	Url varchar(50) COLLATE Latin1_General_CI_AS NULL,
	[Binary] ntext COLLATE Latin1_General_CI_AS NULL,
	Publish bit NULL,
	DateStart datetime NOT NULL,
	DateEnd datetime NOT NULL,
	Status varchar(30) COLLATE Latin1_General_CI_AS NULL,
	CONSTRAINT PK_Banner PRIMARY KEY (Id)
);




CREATE TABLE FastCommerce.dbo.AuthUser (
	Id int IDENTITY(1,1) NOT NULL,
	UserId int NOT NULL,
	Token nvarchar(255) COLLATE Latin1_General_CI_AS NOT NULL,
	Expiration datetime NOT NULL,
	IsActive bit NOT NULL,
	InsertedDate datetime DEFAULT getdate() NOT NULL,
	LastModifiedDate datetime DEFAULT getdate() NOT NULL,
	Status varchar(20) COLLATE Latin1_General_CI_AS NOT NULL,
	CONSTRAINT PK__UserAuth__3214EC078ECA7ED7 PRIMARY KEY (Id)
);



ALTER TABLE FastCommerce.dbo.AuthUser ADD CONSTRAINT AuthUser_User_FK FOREIGN KEY (UserId) REFERENCES FastCommerce.dbo.[User](Id);



INSERT INTO FastCommerce.dbo.Banner
(InsertedDate, LastModifiedDate, Title, Url, [Binary], Publish, DateStart, DateEnd, Status)
VALUES('2024-12-05 11:48:30.000', '2024-12-05 11:48:30.000', N'dsasadsda', N'dasdsasdasda', N'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', 0, '2024-12-05 11:48:15.000', '2026-12-05 11:48:15.000', N'Inserted');


INSERT INTO FastCommerce.dbo.Product
(InsertedDate, LastModifiedDate, Status, Name, Title, ShortDescription, FullDescription, Mpnn, Price, Html, Seo, Publish, Deleted, DateStart, DateEnd)
VALUES('2024-12-05 12:38:13.123', NULL, N'10', N'Product1', N'Title1    ', N'ShortDesc', N'FullDesc', N'MPNN001', 100, N'<html>', N'SEO Content', 1, 0, '2024-12-01 00:00:00.000', '2024-12-31 00:00:00.000');


INSERT INTO FastCommerce.dbo.[User]
(Email, Password, FirstName, LastName, PhoneNumber, InsertedDate, Status)
VALUES(N'torman@tormantech.com', N'123456', N'João', N'Silva', N'11987654321', '2024-12-14 13:13:38.040', 1);

INSERT INTO FastCommerce.dbo.BlogPost
(InsertedDate, LastModifiedDate, Status, Name, Url, Publish, Deleted, DateStart, DateEnd, UserId, Html)
VALUES('2024-12-05 11:11:01.000', NULL, N'Inserted', N'asddsaasdsad', NULL, 1, 0, '2024-12-05 11:10:56.000', '2026-12-05 11:10:56.000', NULL, N'<p>sdaaaaaaaaaaaaaaaaaaaaa</p>');


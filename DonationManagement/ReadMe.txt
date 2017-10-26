CREATE TABLE IF NOT EXISTS Expenses(ID INTEGER PRIMARY KEY AUTOINCREMENT,ExpenseNo INTEGER,Edate DATETIME,Name Varchar(200),Amount Varchar(200),Reason Varchar(1000),Comment Varchar(1000),Created DATETIME,Modified DATETIME,CreatedBy Varchar(200),ModifiedBy Varchar(200));

CREATE TABLE IF NOT EXISTS Donations(ReceiptNo Varchar(200) PRIMARY KEY ,Ddate DATETIME,Name Varchar(200),Gender Varchar(200),Email Varchar(200),Phone Varchar(200),Place Varchar(200),FundType Varchar(200),PaymentType Varchar(200),BTNo Varchar(200),Amount DECIMAL(10,10),AmountType Varchar(200),Comment Varchar(200),Address Varchar(200),Created DATETIME,Modified DATETIME,CreatedBy Varchar(200),ModifiedBy Varchar(200)); 

CREATE TABLE IF NOT EXISTS Users(ID INTEGER PRIMARY KEY AUTOINCREMENT,UName Varchar(200),Password Varchar(200),Role Varchar(200))
Insert Into SequenceNumber(TabName,NextNumber)Values('Donations'00001);
CREATE Table SequenceNumber(TabName varchar(100), NextNumber INTEGER, Category varchaR(100) )

INSERT INTO Users(UName,Password,Role)values('Admin','Admin@123','Admin');

INSERT INTO Users(UName,Password,Role)values('Karki','Karki@123','User');

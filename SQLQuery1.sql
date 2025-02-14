create database Acceloka
Go
Use Acceloka
Go

Create Table Tickets(
	TicketCode varchar(5) NOT NULL
	Constraint PK_Tickets Primary Key,
	CategoryName varchar(255) NOT NULL,
	TicketName varchar(255) NOT NULL,
	EventDate DateTime NOT NULL,
	Price INT NOT NULL,
	SeatNumber INT NOT NULL,
	Quota INT NOT NULL
)

Create Table BookTickets(
	BookTicketID INT Constraint PK_BookTickets primary key identity NOT NULL,
	TicketCode varchar(5) Constraint FK_BookTickets_Tickets foreign key References Tickets(TicketCode) NOT NULL,
	CategoryName varchar(255) NOT NULL,
	TicketName varchar(255) NOT NULL,
	EventDate DateTime NOT NULL,
	Price INT NOT NULL,
	Qty INT NOT NULL
)

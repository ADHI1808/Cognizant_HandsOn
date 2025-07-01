--Step1: Create table Employees
CREATE TABLE Employees (
  EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
  FirstName VARCHAR(50),
  LastName VARCHAR(50),
  DepartmentID INT FOREIGN KEY REFERENCES Departments(DepartmentID),
  Salary DECIMAL(10,2),
  JoinDate DATE
);

--Step2: Create Insert Procedure
CREATE PROCEDURE sp_InsertEmployee
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @DepartmentID INT,
    @Salary DECIMAL(10,2),
    @JoinDate DATE
AS
BEGIN
    INSERT INTO Employees (FirstName, LastName, DepartmentID, Salary, JoinDate)
    VALUES (@FirstName, @LastName, @DepartmentID, @Salary, @JoinDate);
END;

--Step3: Execute Procedure
EXEC sp_InsertEmployee
    @FirstName = 'Adhi',
    @LastName = 'S',
    @DepartmentID = 3,
    @Salary = 50000.00,
    @JoinDate = '2024-08-18';

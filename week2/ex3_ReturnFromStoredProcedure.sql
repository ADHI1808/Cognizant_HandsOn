--Step1: create Get Employees by Department Procedure
CREATE PROCEDURE sp_GetEmployeesByDepartment
    @DepartmentID INT
AS
BEGIN
    SELECT 
        e.EmployeeID,
        e.FirstName,
        e.LastName,
        e.Salary,
        e.JoinDate,
        d.DepartmentName
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
    WHERE e.DepartmentID = @DepartmentID;
END;

--Step2: Execute Procedure
EXEC sp_GetEmployeesByDepartment @DepartmentID = 3;

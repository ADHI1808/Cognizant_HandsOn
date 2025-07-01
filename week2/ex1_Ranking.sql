--step1:create tables
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY,
    CategoryName VARCHAR(100)
);

CREATE TABLE Products (
    ProductID INT PRIMARY KEY,
    ProductName VARCHAR(100),
    Price DECIMAL(10, 2),
    CategoryID INT,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

--step2: insert values
INSERT INTO Categories (CategoryID, CategoryName) VALUES
(1, 'Electronics'),
(2, 'Books'),
(3, 'Clothing');

INSERT INTO Products (ProductID, ProductName, Price, CategoryID) VALUES
(1, 'Smartphone', 1800.00, 1),
(2, 'Laptop', 2000.00, 1),
(3, 'Tablet', 650.00, 1),
(4, 'Smartwatch', 620.00, 1),
(5, 'Novel A', 10.00, 2),
(6, 'Novel B', 20.00, 2),
(7, 'Collector Edition', 22.00, 2),
(8, 'T-shirt', 21.00, 3),
(9, 'Jacket', 50.00, 3),
(10, 'Sneakers', 40.00, 3),
(11, 'Cap', 15.00, 3);

--step3: ranking query
SELECT
    c.CategoryName,
    p.ProductName,
    p.Price,
    ROW_NUMBER() OVER (PARTITION BY c.CategoryID ORDER BY p.Price DESC) AS RowNum,
    RANK() OVER (PARTITION BY c.CategoryID ORDER BY p.Price DESC) AS RankNum,
    DENSE_RANK() OVER (PARTITION BY c.CategoryID ORDER BY p.Price DESC) AS DenseRankNum
FROM Products p
JOIN Categories c ON p.CategoryID = c.CategoryID;


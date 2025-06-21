using System;
class Product
{
    public int ProductId;
    public string ProductName;
    public string Category;
    public Product(int id, string name, string category)
    {
        ProductId = id;
        ProductName = name;
        Category = category;
    }
}
class SearchFunctionExample
{
    static void Main()
    {
        Product[] products = {
            new Product(101, "Laptop", "Electronics"),
            new Product(102, "Shoes", "Fashion"),
            new Product(103, "Phone", "Electronics"),
            new Product(104, "T-shirt", "Fashion"),
            new Product(105, "Book", "Education")
        };
        Console.WriteLine("Searching for 'Phone' using Linear Search:");
        int index1 = LinearSearch(products, "Phone");
        Console.WriteLine(index1 == -1 ? "Not found." : $"Found at index {index1}");
        Console.WriteLine("\nSearching for 'Phone' using Binary Search:");
        Array.Sort(products, (p1, p2) => p1.ProductName.CompareTo(p2.ProductName));
        int index2 = BinarySearch(products, "Phone");
        Console.WriteLine(index2 == -1 ? "Not found." : $"Found at index {index2}");
    }
    static int LinearSearch(Product[] products, string searchName)
    {
        for (int i = 0; i < products.Length; i++)
        {
            if (products[i].ProductName.Equals(searchName, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }
    static int BinarySearch(Product[] products, string searchName)
    {
        int left = 0, right = products.Length - 1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            int comparison = string.Compare(products[mid].ProductName, searchName, StringComparison.OrdinalIgnoreCase);
            if (comparison == 0) return mid;
            else if (comparison < 0) left = mid + 1;
            else right = mid - 1;
        }
        return -1;
    }
}

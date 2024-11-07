namespace Ed.Analytics.Models;

public sealed record ThreadCategories(
    ThreadCategory Category, 
    ThreadCategory Subcategory, 
    ThreadCategory Subsubcategory
) 
{
    public ThreadCategories(string? category, string? subcategory, string? subsubcategory)
        : this(
            ThreadCategory.Create(category), 
            ThreadCategory.Create(subcategory), 
            ThreadCategory.Create(subsubcategory)
        ) {}
}
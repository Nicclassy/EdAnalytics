namespace EdAnalytics.Models;

public sealed record ThreadCategories(
    ThreadCategory Category, 
    ThreadCategory Subcategory, 
    ThreadCategory Subsubcategory
) 
{
    public ThreadCategories(string? category, string? subcategory, string? subsubcategory)
        : this(
            ThreadCategory.Parse(category), 
            ThreadCategory.Parse(subcategory), 
            ThreadCategory.Parse(subsubcategory)
        ) {}
}
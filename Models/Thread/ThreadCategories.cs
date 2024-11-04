namespace EdAnalytics.Models;

public sealed class ThreadCategories(ThreadCategory category, ThreadCategory subcategory, ThreadCategory subsubcategory) 
{
    public ThreadCategory Category { get; } = category;
    public ThreadCategory Subcategory { get; } = subcategory;
    public ThreadCategory Subsubcategory { get; } = subsubcategory;

    public ThreadCategories(string? category, string? subcategory, string? subsubcategory)
        : this(ThreadCategory.Parse(category), ThreadCategory.Parse(subcategory), ThreadCategory.Parse(subsubcategory)) {}
}
# Ed Analytics
Perform some data analysis on data downloaded from an Edstem discussion.

## Requirements
1. .NET 8.0, which was used to develop this project
2. The appropriate JSON/CSV files from an Ed discussion, which are only
downloadable if you are an administrator on that discussion.

## Example usage
Start by creating the two required objects: an `Analytics` instance and a `Threads` instance.
```cs
using EdAnalytics.Deserialization;
using EdAnalytics.EdDiscussion;
using EdAnalytics.Models;

Threads threads = ThreadsJSON.Deserialize("threads.json");
Analytics analytics = AnalyticsCSV.Parse("analytics.csv");
```

Now you can analyse the data as you wish. The following code demonstrates how to 
output the list of users who have received more than twenty hearts in the discussion:
```cs
var query = 
    from row in analytics
    where row.Reactions.Hearts > 20 && row.User.Role == Role.Student
    orderby row.Reactions.Hearts descending
    select new 
    {
        row.User.Name, 
        row.Reactions.Hearts
    };

foreach ((int i, var user) in query.Enumerate(start: 1))
    Console.WriteLine($"{i}. {user.Name}: {user.Hearts} hearts");
```
Using LINQ is highly advantageous for extracting relevant query data.
Here, keyword syntax is particularly helpful.

In some cases you may want to just reuse some existing/common statistics.
Here's how you can find the 5 most endorsed users in a discussion:
```cs
var topEndorsed = analytics.TopEndorsed(count: 5);
```
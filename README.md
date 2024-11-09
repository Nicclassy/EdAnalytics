# Ed Analytics
Perform some data analysis on data downloaded from an Edstem discussion.

## Requirements
1. .NET 8.0, which was used to develop this project
2. The appropriate JSON/CSV files from an Ed discussion, which are only
downloadable if you are an administrator on that discussion

## Example Usage
Start by creating the two required objects: an `Analytics` instance and a `Threads` instance.
```cs
using Ed.Analytics.Deserialization;
using Ed.Analytics.Discussion;
using Ed.Analytics.Models;

Threads threads = ThreadsJSON.Deserialize("threads.json");
UserAnalytics analytics = AnalyticsCSV.Parse("analytics.csv");
```

Now you can analyse the data as you wish. Two code examples are provided
below to demonstrate how data analysis can be performed.

The following code demonstrates how to 
output the list of users who have received more than twenty hearts in the discussion:
```cs
var query = 
    from user in analytics
    where user.Reactions.Hearts > 20 && user.Account.Role == DiscussionRole.Student
    orderby user.Reactions.Hearts descending
    select new 
    {
        user.Account.Name, 
        user.Reactions.Hearts
    };

foreach ((int i, var user) in query.Enumerate(start: 1))
    Console.WriteLine($"{i}. {user.Name}: {user.Hearts} hearts");
```
Using LINQ is highly advantageous for extracting relevant query data.
Here, keyword syntax is particularly helpful.

The code below determines the total number of hearts
of students of each tutor and sorts them in a descending order
```cs
var queryTutorGroups =
    from user in analytics
    where user.Account.Role == DiscussionRole.Student
    group user by user.Tutorial.Tutor;

var heartsOfTutor =
    from grouping in queryTutorGroups
    let hearts = grouping.Sum(student => student.Reactions.Hearts)
    orderby hearts descending
    select new
    {
        Tutor = grouping.Key,
        Hearts = hearts
    };
```

In some cases you may want to just reuse some existing/common statistics.
Here's how you can find the 5 most endorsed users in a discussion:
```cs
var topEndorsed = analytics.TopEndorsed(count: 5);
```
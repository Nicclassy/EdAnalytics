# Ed Analytics
Perform some data analysis on data downloaded from an Edstem discussion.

## Requirements
1. .NET 8.0, which was used to develop this project
2. The appropriate JSON/CSV files from an Ed discussion, which are only
downloadable if you are an administrator on that discussion

## Example Usage
Start by creating the three required objects: an `Analytics`, a `Threads` instance and a `DiscussionStatistics` instance.
```cs
using Ed.Analytics.Deserialization;
using Ed.Analytics.Discussion;
using Ed.Analytics.Models;

Threads threads = ThreadsJSON.Deserialize("threads.json");
UserAnalytics analytics = AnalyticsCSV.Parse("analytics.csv");
DiscussionStatistics discussion = new DiscussionStatistics(analytics, threads);
```

Now you can analyse the data as you wish. 

### Analysing data directly
Three code examples are provided
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

LINQ method syntax can also be used along with the provided extension methods.
For example, getting five threads with the highest number of answers descending:
```cs
threads
    .OrderByDescending(thread => thread.Answers.Length)
    .Take(5)
    .Enumerate(start: 1)
    .Select(pair => $"{pair.Item1}. {pair.Item2.Title.Text} ({pair.Item2.Answers.Length})")
    .ForEachWriteLine();
```
### Using existing statistics
In some cases you may want to just reuse some existing/common statistics. 
These are provided in the `DiscussionStatistics` class.
Here's how you can find the users with the most active days on a discussion in descending order:
```cs
discussion.MostDaysActive();
```
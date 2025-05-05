# BlazorDexie: Accessing IndexedDB in Blazor Applications with Dexie.js 

BlazorDexie provides an easy way to access the browsers IndexedDb in Blazor applications. It is a wrapper around the well-known javascript library Dexie.js. The code can be written in C# with few exceptions.

nuget package: https://www.nuget.org/packages/BlazorDexie

## Get Started

Install Nuget package by:

```
dotnet add package BlazorDexie
```

Reference dexie.js in wwwroot/index.html:

```
<script src="https://unpkg.com/dexie/dist/dexie.js"></script>
```

Register services in Program.cs:

```
builder.Services.AddBlazorDexie();
```

### Example
The following example is similar to the "Hello World" from Dexie.js, which you can find at the following link: https://dexie.org/docs/Tutorial/Hello-World 

The Friend class represents the object to be stored in the IndexedDb
```
public class Friend
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string NotIndexedProperty { get; set; } = string.Empty;
}
```
MyDb with single table "friends" with primary key "id" and
indices on properties "name" and "age"
```
public class MyDb : Db<MyDb>
{
    public Store<Friend, int> Friends { get; set; } = new(nameof(Friend.Id), nameof(Friend.Name), nameof(Friend.Age));

    public MyDb(BlazorDexieOptions blazorDexieOptions)
        : base("FriendDatabase", 1, new IDbVersion[] { }, blazorDexieOptions)
    {
    }
}
```
#### Usage in Blazor
```
    public partial class Index
    {
        [Inject] public BlazorDexieOptions BlazorDexieOptions { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            var db = new MyDb(BlazorDexieOptions);

            await db.Friends.BulkPut(new Friend[]
            {
                new Friend(){ Id = 1, Name = "Josephine", Age = 21 },
                new Friend(){ Id = 2, Name = "Per", Age = 75 },
                new Friend(){ Id = 3, Name = "Simon", Age = 5 },
                new Friend(){ Id = 4, Name = "Sara", Age = 50, NotIndexedProperty= "foo" }
            });

            var youngFriends =  await db.Friends
                .Where("age")
                .Between(0, 25)
                .ToArray();

            Console.WriteLine("Found young friends: " + string.Join(", ", youngFriends.Select(f => f.Name)));

            var friendsInReverseOrder = await db.Friends
                .OrderBy(nameof(Friend.Age))
                .Reverse()
                .ToArray();

            Console.WriteLine("Friends in reverse age order: " + 
                string.Join(", ", friendsInReverseOrder.Select(f => f.Name + " " + f.Age)));

            var friendStartsWithS = await db.Friends
                .Where(nameof(Friend.Name))
                .StartsWith("S")
                .ToArray();
            
            Console.WriteLine("Friends on 'S': " + string.Join(", ", friendStartsWithS.Select(f => f.Name)));
        }
    }
```
### Auto Increment

```
public class Friend
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }
    ...
}
```

```
public class MyDb : Db<MyDb>
{
    public Store<Friend, int> Friends { get; set; } = new($"++{nameof(Friend.Id)}", nameof(Friend.Name), nameof(Friend.Age));
    ...
}
```

```
 await db.Friends.Put(new Friend(){ Name = "Josephine", Age = 21 });
```

### Transactions

```
try
{
    await db.Transaction("rw", [nameof(MyDb.Friends)], 60000, async () =>
    {
        var friend = await db.Friends.Get(key) ?? throw new InvalidOperationException();
        friend.Age = 30;
        await db.Friends.Put(friend);
    });
}
catch (Exception e)
{
    // handle error
}
```
### Database Versioning

**Note:** Starting with Dexie 3.0, you only need to keep versions that include an upgrade function or upgrade module. If a version doesn’t include either, you can simply increase the version number without keeping the previous version definitions. For more info, see: [Dexie Database Versioning](https://dexie.org/docs/Tutorial/Design#database-versioning)

**Version 1**

- The database is defined in in the class MyDb

```
public class MyDb : Db<MyDb>
{
    public Store<Friend1, int> Friends { get; set; } = new("++" + nameof(Friend.Id), nameof(Friend.Name), nameof(Friend.Age));

    public MyDb(BlazorDexieOptions blazorDexieOptions)
        : base("TestDb", 1, new IDbVersion[0], blazorDexieOptions)
    {
    }
}
```

**Version 2**

- Create a a new class DbVersion1 and move the Properties and the VersionNumber from MyDb to it.
- Maybe the nameofs should be replaced by string literals because the properties used in the nameof can be changed.

```
public class Version1 : DbVersion<Version1>
{
    public Store<Friend1, int> Friends { get; set; } = new("++id", "name" + "age");

    public Version1() : base(1)
    {
    }
}
```
- Change the Properties in MyDb
- Increase the VersionNumber.
- Add an instance of Version1 to parameter ```previousVersions``` passed to the base constructor
- An upgrade function can be pass to the base constructor if needed. The uprade function is a string with JavaScript code. The parameter tx (transaction) will be pass to the function from the framework.

```
public class MyDb : Db<MyDb>
{
    public Store<Friend, int> Friends { get; set; } = new("++" + nameof(Friend.Id), nameof(Friend.Name), nameof(Friend.BirthDate));

    public MyDb(BlazorDexieOptions blazorDexieOptions)
        : base("TestDb", 2, new IDbVersion[] { new Version1() }, blazorDexieOptions, GetUpgrade())
    {
    }

    private static string GetUpgrade()
    {
        return
            "var YEAR = 365 * 24 * 60 * 60 * 1000; " +
            "return tx.table(\"Friends\").toCollection().modify(friend => { " +
            "    friend.birthdate = new Date(Date.now() - (friend.age * YEAR)); " +
            "    delete friend.age; " +
            "}); ";
    }
}
```
Note: Depending on whether the camelCaseStoreNames parameter is set to true or false (default) when creating a Db instance, either 
tx.table(\"Friends\") ... or tx.table(\"friends\") ...
must be written.

**Alternative:**

- Instead of pass the code as string it is also possible to create a ES-Module with the upgrade function an pass the path of the module to the constructor

```
public class MyDb : Db<MyDb>
{
    public Store<Friend, int> Friends { get; set; } = new("++" + nameof(Friend.Id), nameof(Friend.Name), nameof(Friend.BirthDate));

    public MyDb(BlazorDexieOptions blazorDexieOptions)
        : base("TestDb", 2, new IDbVersion[] { new V1.Version1() }, blazorDexieOptions, upgradeModule: "dbUpgrade2.js")
    {
    }
}
```
dbUpgrade2.js
```
export default function update(tx) {
    var YEAR = 365 * 24 * 60 * 60 * 1000; 
    return tx.table(\"Friends\").toCollection().modify(friend => { 
        friend.birthdate = new Date(Date.now() - (friend.age * YEAR)); 
        delete friend.age; 
    });
}
```
Note: Depending on whether the camelCaseStoreNames parameter is set to true or false (default) when creating a Db instance, either 
tx.table(\"Friends\") ... or tx.table(\"friends\") ...
must be written.

For release history, see the [CHANGELOG](./CHANGELOG.md)

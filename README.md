# BlazorDexie: Accessing IndexedDB in Blazor Applications with Dexie.js 

BlazorDexie provides an easy way to access the browsers IndexedDb in Blazor applications. It is a wrapper around the well-known javascript library Dexie.js. The code can be written in C# with few exceptions.

nuget package: https://www.nuget.org/packages/BlazorDexie

## Usage

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
public class MyDb : Db
{
    public Store<Friend, int> Friends { get; set; } = new(nameof(Friend.Id), nameof(Friend.Name), nameof(Friend.Age));

    public MyDb(IModuleFactory moduleFactory)
        : base("FriendDatabase", 1, new DbVersion[] { }, moduleFactory)
    {
    }
}
```
Usage in Blazor
```
    public partial class Index
    {
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            var moduleFactory = new EsModuleFactory(JSRuntime);
            var db = new MyDb(moduleFactory);

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
public class MyDb : Db
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

**Version 1**

- The database is defined in in the class MyDb

```
public class MyDb : Db
{
    public Store<Friend1, int> Friends { get; set; } = new("++" + nameof(Friend.Id), nameof(Friend.Name), nameof(Friend.Age));

    public MyDb(IModuleFactory jsModuleFactory)
        : base("TestDb", 1, new DbVersion[0], jsModuleFactory)
    {
    }
}
```

**Version 2**

- Create a a new class DbVersion1 and move the Properties and the VersionNumber from MyDb to it.
- Maybe the nameofs should be replaced by string literals because the properties used in the nameof can be changed.

```
public class Version1 : DbVersion
{
    public Store<Friend1, int> Friends { get; set; } = new("++id", "name" + "age");

    public Version1() : base(1)
    {
    }
}
```
- Change the Properties in MyDb
- Increase the VersionNumber.
- Add an instance of Version1 to DbVersion Array passed to the base contructor
- An upgrade function can be pass to the base constructor if needed. The uprade function is a string with JavaScript code. The parameter tx (transaction) will be pass to the function from the framework.

```
public class MyDb : Db
{
    public Store<Friend, int> Friends { get; set; } = new("++" + nameof(Friend.Id), nameof(Friend.Name), nameof(Friend.BirthDate));

    public MyDb(IModuleFactory jsModuleFactory)
        : base("TestDb", 2, new DbVersion[] { new Version1() }, jsModuleFactory, GetUpgrade())
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
**Alternative:**

- Instead of pass the code as string it is also possible to create a ES-Module with the upgrade function an pass the path of the module to the constructor

```
public class MyDb : Db
{
    public Store<Friend, int> Friends { get; set; } = new("++" + nameof(Friend.Id), nameof(Friend.Name), nameof(Friend.BirthDate));

    public MyDb(IModuleFactory jsModuleFactory)
        : base("TestDb", 2, new DbVersion[] { new V1.Version1() }, jsModuleFactory, upgradeModule: "dbUpgrade2.js")
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
## Version 1.5.0
- Add support for transactions
  
## Version 1.4.0
- Add methods ToCollecton on Store

## Version 1.3.0
- Add methods SortBy and SortByToList (same as SortBy but returns a list instead of an array) to Collection.

## Version 1.2.0
- In previous versions the store name was written to the IndexedDB in pascal case. Now it is possible to write it in camel case, as it is common. To be backward compatible the default behaviour is like in the previous versions. To use camel case for store names pass the following optional parameter to the constuctor of the class Db.
```camelCaseStoreNames : true```
- Add support for .NET 7.0 and .NET 8.0

## Version 1.1.0
- Add parameters upgrade and upgradeModule to constructor of classes Db and Version to call Version.upgrade in Dexie.js.

## Version 1.0.0 

- Rename project to BlazorDexie (Nosthy.Blazor.DexieWrapper before)

## Version 0.7.0

### Breaking Changes

- Remove optional Parameter databaseName in several methods of Collection and Store. 

### Enhancements

- Add method Delete to class Db
- Add class Dexie with the static methods (static for Dexie not in C#) Delete and Exits
- Add the following methods to to class Store to work with blobs:
   - AddBlob: Add byte array as blob to the db
   - PutBlob: Put byte array as blob to the db
   - GetBlob: Get blob from the DB as byte array
   - AddObjecUrl:  Add ObjectUrl as blob to the db
   - PutObjectUrl: Put ObjectUrl as blob to the db
   - GetObjectUrl: Get blob from the DB as ObjectUrl
    

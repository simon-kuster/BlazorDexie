# BlazorDexie (Nosthy.Blazor.DexieWrapper before)

BlazorDexie provides an easy way to access the browers IndexedDb for Blazor applications.
It is a wrapper around the well-known javascript library Dexie.js.

## Usage

Friend represent object to store in table
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
indexes on properties "name" and "age"
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

For more Information have look to: https://dexie.org/docs/Tutorial/Hello-World

### 

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
    

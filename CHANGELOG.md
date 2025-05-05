## Version 2.0.0

### Breaking Changes:
#### 1. **Service Registration Method Renamed**

Old:
```
AddDexieWrapper(IServiceCollection services, string userModuleBasePath = "")
```
New:
```
AddBlazorDexie(IServiceCollection services, bool camelCaseStoreNames = false)
```
Impact: The method name has changed, and the new method now includes the parameter ```camelCaseStoreNames``` and the parameter ```userModuleBasePath``` has been removed. 

#### 2. **Update Class Db**

The new ```Db``` class is generic, and its generic parameter ```TConcrete``` represents the concrete class that extends it. For example:

``` public class MyDb : Db<MyDb> {}```

Old Constructor: 
```
protected Db(
    string databaseName,
    int currentVersionNumber,
    IEnumerable<IDbVersion> previousVersions,
    IModuleFactory moduleFactory,
    string? upgrade = null,
    string? upgradeModule = null,
    bool camelCaseStoreNames = false)
```
New Constructor:
```
protected Db(
    string databaseName,
    int currentVersionNumber,
    IEnumerable<IDbVersion> previousVersions,
    BlazorDexieOptions blazorDexieOptions,
    string? upgrade = null,
    string? upgradeModule = null)
```
Impact: The parameter ```moduleFactory``` and ```camelCaseStoreNames``` have been replaced with the parameter ```blazorDexieOptions```, which includes both functionalities. The parameter ```previousVersions``` has new the type ```IEnumerable<IDbVersion>```

#### 3. **Update Class Dexie**

Old Constructor:  
```
public Dexie(BlazorDexieOptions blazorDexieOptions)
```
New Constructor:
```
public Dexie(IModuleFactory jsModuleFactory)
```
Impact: The parameter ```jsModuleFactory``` has been replaced with the parameter ```blazorDexieOptions```, which includes the ```jsModuleFactory``` functionality.

#### 3. **Update Class DbVersion**

The new ```DbVersion``` class is generic, and its generic parameter ```TConcrete``` represents the concrete class that extends it. For example:

``` public class Version1 : DbVersion<Version1> {}```

## Version 1.6.0
- Add support for .NET 9.0
- Remove support for .NET 6.0 and .NET 7.0
- Add console log

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
    

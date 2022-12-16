export function initDbAndExecute(databaseName, versions, storeName, commands) {
    var db = initDb(databaseName, versions);
    return execute(db, storeName, commands);
}

export async function execute(db, storeName, commands) {
    var result = await executeNonQuery(db, storeName, commands);
    if (result === undefined) {
        return null;
    }

    return result;
}

export function initDbAndExecuteNonQuery(databaseName, versions, storeName, commands) {
    var db = initDb(databaseName, versions);
    return executeNonQuery(db, storeName, commands);
}

export async function executeNonQuery(db, storeName, commands) {
    var query = db[storeName];

    for (const c of commands) {
        switch (c.cmd) {
            case "filter":
                var filterFunction = new Function('i', 'p', c.parameters[0]);
                query = query.filter((i) => filterFunction(i, c.parameters[1]));
                break;

            case "filterModule":
                var cust = await import(c.parameters[0]);
                query = query.filter((i) => cust.default(i, c.parameters[1]));
                break;

            default:
                query = query[c.cmd](...c.parameters);
                break;
        }
    }

    return query;
}

export function initDb(databaseName, versions) {
    const db = new Dexie(databaseName);

    versions.forEach(version => {
        var storeDefinitions = {};

        version.stores.forEach(store => {
            storeDefinitions[store.storeName] = store.indices;
        }); 

        db.version(version.versionNumber).stores(storeDefinitions);
    });

    return db;
}

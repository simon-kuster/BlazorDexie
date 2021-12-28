export function execute(dbDefinition, storeName, commands) {
    return executeNonQuery(dbDefinition, storeName, commands).then(result => {
        if (!result) {
            return null;
        }

        return result;
    });
}

export function executeNonQuery(dbDefinition, storeName, commands) {
    var db = initDb(dbDefinition);
    var query = db[storeName];

    commands.forEach(c => {
        query = query[c.cmd](...c.parameters);
    });

    return query;
}

function initDb(dbDefinition) {
    const db = new Dexie(dbDefinition.databaseName);

    dbDefinition.versions.forEach(version => {
        var storeDefinitions = {};

        version.stores.forEach(store => {
            storeDefinitions[store.storeName] = store.indices;
        }); 

        db.version(version.versionNumber).stores(storeDefinitions);
    });

    return db;
}

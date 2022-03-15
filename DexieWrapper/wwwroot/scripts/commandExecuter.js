export function initDbAndExecute(dbDefinition, storeName, commands) {
    var db = initDb(dbDefinition);
    return execute(db, storeName, commands);
}

export function execute(db, storeName, commands) {
    return executeNonQuery(db, storeName, commands).then(result => {
        if (!result) {
            return null;
        }

        return result;
    });
}

export function initDbAndExecuteNonQuery(dbDefinition, storeName, commands) {
    var db = initDb(dbDefinition);
    return executeNonQuery(db, storeName, commands);
}

export function executeNonQuery(db, storeName, commands) {
    var query = db[storeName];

    commands.forEach(c => {
        if (c.cmd === 'filter') {
            var filterFunction = new Function('i', 'p', c.parameters[0]);
            query = query[c.cmd]((i) => filterFunction(i, c.parameters[1]));
        } else {
            query = query[c.cmd](...c.parameters);
        }
    });

    return query;
}

export function initDb(dbDefinition) {
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

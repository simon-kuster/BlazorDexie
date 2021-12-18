import Dexie from './lib/dexie.mjs';

export function execute(databaseDefinition, command) {
    var db = initDb(databaseDefinition);

    return db[command.storeName][command.cmd](...command.parameters).then(result => {
        if (!result) {
            return null;
        }

        return result;
    });
}

export function executeNonQuery(dbDefinition, command) {

    var db = initDb(dbDefinition);
    return db[command.storeName][command.cmd](...command.parameters);
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

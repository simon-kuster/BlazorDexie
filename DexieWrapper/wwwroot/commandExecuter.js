import Dexie from './lib/dexie.mjs';

export function execute(dbDefinition, command) {
    return executeNonQuery(dbDefinition, command).then(result => {
        if (!result) {
            return null;
        }

        return result;
    });
}

export function executeNonQuery(dbDefinition, command) {
    var db = initDb(dbDefinition);
    var query = db[command.storeName][command.cmd](...command.parameters);

    command.subCommands.forEach(subCommand => {
        query = query[subCommand.cmd](...subCommand.parameters);
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

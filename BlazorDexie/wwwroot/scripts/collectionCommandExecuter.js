import { createObjectUrl, fetchObjectUrl, revokeObjectUrl } from './objectUrl.js';
const runInBrowser = typeof window !== 'undefined';
let _userModuleBasePath = '';

export function setUserModuleBasePath(userModuleBasePath) {
    _userModuleBasePath = userModuleBasePath;
}

export async function initDbAndExecute(databaseName, versions, storeName, commands) {
    const db = await initDb(databaseName, versions);
    return await execute(db, storeName, commands);
}

export async function execute(db, storeName, commands) {
    const result = await executeNonQuery(db, storeName, commands);
    if (result === undefined) {
        return null;
    }

    return result;
}

export async function initDbAndExecuteNonQuery(databaseName, versions, storeName, commands) {
    const db = await initDb(databaseName, versions);
    return await executeNonQuery(db, storeName, commands);
}

export async function executeNonQuery(db, storeName, commands) {
    let query = db[storeName];

    for (const c of commands) {
        switch (c.cmd) {
            case "filter":
                const filterFunction = new Function('i', 'p', c.parameters[0]);
                query = await query.filter((i) => filterFunction(i, c.parameters[1]));
                break;

            case "filterModule":
                const filterModulePath = combinePathes(_userModuleBasePath, c.parameters[0]);
                const filterModule = await import(filterModulePath);
                query = await query.filter((i) => filterModule.default(i, c.parameters[1]));
                break;

            case "addBlob":
                if (runInBrowser) {
                    query = await query.add(new Blob([c.parameters[0]], { type: c.parameters[2] }), c.parameters[1]);
                } else {
                    query = await query.add({ data: c.parameters[0], type: c.parameters[2] }, c.parameters[1]);
                }
                break;

            case "bulkAddBlob":
                if (runInBrowser) {
                    query = await query.bulkAdd(c.parameters[0].map(d => new Blob([d], { type: c.parameters[2] })), c.parameters[1]);
                } else {
                    query = await query.bulkAdd(c.parameters[0].map(d => ({ data: d, type: c.parameters[2] })), c.parameters[1]);
                }
                break;

            case "putBlob":
                if (runInBrowser) {
                    query = await query.put(new Blob([c.parameters[0]], { type: c.parameters[2] }), c.parameters[1]);
                } else {

                    query = await query.put({ data: c.parameters[0], type: c.parameters[2] }, c.parameters[1]);
                }
                break;

            case "getBlob":
                const blob = await query.get(c.parameters[0]);
                if (runInBrowser) {
                    query = new Uint8Array(await new Response(blob).arrayBuffer());
                } else {
                    query = blob.data;
                }
                break;

            case "addObjectUrl":
                query = await query.add(await ObjectUrlToBlob(c.parameters[0]), c.parameters[1]);
                break;

            case "putObjectUrl":
                query = await query.put(await ObjectUrlToBlob(c.parameters[0]), c.parameters[1]);
                break;

            case "getObjectUrl":
                const blob2 = await query.get(c.parameters[0])
                query = createObjectUrl(blob2);
                break;

            default:         
                query = await query[c.cmd](...c.parameters);
                break;
        }
    }

    return query;
}

export async function initDb(databaseName, versions) {
    const db = new Dexie(databaseName);

    for (const version of versions) {
        const storeDefinitions = {};

        version.stores.forEach(store => {
            storeDefinitions[store.storeName] = store.indices;
        });

        const dbWithVersion = db.version(version.versionNumber).stores(storeDefinitions);

        if (version.upgrade) {
            const upgradeFunction = new Function('tx', version.upgrade);
            dbWithVersion.upgrade(tx => upgradeFunction(tx));
        }

        if (version.upgradeModule) {
            const upgradeModulePath = combinePathes(_userModuleBasePath, version.upgradeModule);
            const upgradeModule = await import(upgradeModulePath);
            dbWithVersion.upgrade(tx => upgradeModule.default(tx));
        }
    }

    return db;
}

export function close(db) {
    db.close();
}

async function ObjectUrlToBlob(objectUrl) {
    const blob = await fetchObjectUrl(objectUrl);
    revokeObjectUrl(objectUrl);
    return blob;
}

function combinePathes(pathA, pathB) {
    if (!pathA && !pathB) {
        return '';
    }

    if (!pathA) {
        return pathB;
    }

    if (!pathB) {
        return pathA;
    }

    return pathA.replace(/\/+$/g, '') + '/' + pathB.replace(/^\/+/g, '')
}

import { createObjectUrl, fetchObjectUrl, revokeObjectUrl } from './objectUrl.js';

export async function initDbAndExecute(databaseName, versions, storeName, commands) {
    const db = initDb(databaseName, versions);
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
    const db = initDb(databaseName, versions);
    return await executeNonQuery(db, storeName, commands);
}

export async function executeNonQuery(db, storeName, commands) {
    let query = db[storeName];

    for (const c of commands) {

        if (c.commandParameters.blobDataConvert && c.commandParameters.blobDataConvert.convertType === 0) {
            await replaceObjectUrlWithBlob(c.dexieParameters, c.commandParameters.blobDataConvert.pathes)
        }

        switch (c.cmd) {
            case "filter":
                var filterFunction = new Function('i', 'p', c.dexieParameters[0]);
                query = await query.filter((i) => filterFunction(i, c.dexieParameters[1]));
                break;

            case "filterModule":
                var cust = await import(c.dexieParameters[0]);
                query = await query.filter((i) => cust.default(i, c.dexieParameters[1]));
                break;

            default:
                query = await query[c.cmd](...c.dexieParameters);
                break;
        }

        if (c.commandParameters.blobDataConvert && c.commandParameters.blobDataConvert.convertType === 1) {
            query = await replaceBlobWithByteArray(query, c.commandParameters.blobDataConvert.pathes)
        }

        if (c.commandParameters.blobDataConvert && c.commandParameters.blobDataConvert.convertType === 2) {
            query = await replaceBlobWithObjectUrl(query, c.commandParameters.blobDataConvert.pathes)
        }
    }

    return query;
}

async function replaceObjectUrlWithBlob(dexieParameters, blobDataPathes) {
    await ForeachBlobDataPath(dexieParameters, blobDataPathes, async (getFunction, setFunction, base) => {
        const blobData = getFunction(base);

        let blob;
        if (blobData.objectUrl) {
            blob = await fetchObjectUrl(blobData.objectUrl);
            revokeObjectUrl(blobData.objectUrl);
        } else {
            blob = blobData.byteArray;
        }

        setFunction(base, blob);
    });
}

async function replaceBlobWithObjectUrl(item, blobDataPathes) {

    await ForeachBlobDataPath(item, blobDataPathes, async (getFunction, setFunction, base) => {
        const blob = getFunction(base);
        const blobData = {};
        blobData.objectUrl = createObjectUrl(blob);
        blobData.byteArray = null;
        setFunction(base, blobData);
    });
    return item;
}

async function replaceBlobWithByteArray(item, blobDataPathes) {
    await ForeachBlobDataPath(item, blobDataPathes, async (getFunction, setFunction, base) => {
        const blob = getFunction(base);
        const blobData = {};
        blobData.objectUrl = null;
        blobData.byteArray = blob;
        setFunction(base, blobData);
    });
    return item;
}

async function ForeachBlobDataPath(parameters, blobDataPathes, replaceFunction) {

    for (const path of blobDataPathes) {
        const indexOfArrayStart = path.indexOf("*");
        let getFunction;
        let setFunction;

        if (indexOfArrayStart > -1) {

            if (indexOfArrayStart > 0) {
                var arrayPath = path.substring(0, indexOfArrayStart);
                var getArrayFunction = new Function('parameters', "return parameters" + arrayPath + ";");
                var array = getArrayFunction(parameters);
            } else {
                array = parameters;
            }

            var subPath = path.substring(indexOfArrayStart + 1);
            getFunction = new Function('item', "return item" + subPath + ";");
            setFunction = new Function('item', 'value', "item" + subPath + "=value;");

            for (const item of array) {
                await replaceFunction(getFunction, setFunction, item);
            }

        } else {
            getFunction = new Function('parameters', "return parameters" + path + ";");
            setFunction = new Function('parameters', 'value', "parameters" + path + "=value;");
            await replaceFunction(getFunction, setFunction, parameters);
        }
    }
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

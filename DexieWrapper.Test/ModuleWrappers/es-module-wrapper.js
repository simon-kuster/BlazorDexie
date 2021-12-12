require("fake-indexeddb/auto");
const IDBKeyRange = require("fake-indexeddb/lib/FDBKeyRange");
const FDBFactory = require("fake-indexeddb/lib/FDBFactory");

module.exports = async (fileName, method, arg1, arg2, arg3, arg4) => {
    indexedDB = new FDBFactory();

    const module2 = await import(fileName);
    return module2[method](arg1, arg2, arg3, arg4);
}
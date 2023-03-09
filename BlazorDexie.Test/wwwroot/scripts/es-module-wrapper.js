require("fake-indexeddb/auto");
const FDBFactory = require("fake-indexeddb/lib/FDBFactory");

module.exports = async (modulPath, method, arg1, arg2, arg3, arg4) => {
    indexedDB = new FDBFactory();
    var dexieModule = await import("./lib/dexie.mjs");
    Dexie = dexieModule.Dexie;
    const module = await import(modulPath);
    return module[method](arg1, arg2, arg3, arg4);
}
export async function executeNonQuery(c) {
    const startTime = performance.now();

    switch (c.cmd) {
        case "transaction":
            console.log(`BlazorDexie: Beginn transaction`);
            await executeTransaction(c.parameters[0], c.parameters[1], c.parameters[2], c.parameters[3], c.parameters[4]);
            console.log(`BlazorDexie: End transaction`);
            break;

        default:
            Dexie[c.cmd](...c.parameters);
            const endTime = performance.now();
            console.log(`BlazorDexie: Dexie.${c.cmd}(${c.parameters})} [${(endTime - startTime).toFixed(0)}ms]`);

            break;
    }
}

export async function execute(c) {
    const result = await Dexie[c.cmd](...c.parameters);;
    if (result === undefined) {
        return null;
    }

    return result;
}

async function executeTransaction(db, mode, storeNames, timeout, transactionBodyWrapper) {
    var stores = storeNames.map(storeName => db[storeName]);

    await db.transaction(mode, ...stores, async () => {
        await Dexie.waitFor(transactionBodyWrapper.invokeMethodAsync('CallTransactionBody'), timeout);
    });
}
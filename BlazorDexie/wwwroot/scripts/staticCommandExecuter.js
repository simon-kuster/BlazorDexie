import { execute as collectionExecute, executeNonQuery as collectionExecuteNonQuery } from './collectionCommandExecuter.js';

export async function executeNonQuery(c) {
    switch (c.cmd) {
        case "transaction":
            await executeTransaction(c.parameters[0], c.parameters[1], c.parameters[2], c.parameters[3], c.parameters[4]);
            break;

        default:
            Dexie[c.cmd](...c.parameters);
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
        transactionBodyWrapper.invokeMethod('StartTransactionBody');

        let lastCommandResult;
        let lastCommandId;
        let done = false;
        const startTime = Date.now();

        while (!done) {

            if (Date.now() - startTime > timeout) {
                throw new Error("Transaction timed out");
            }

            const response = transactionBodyWrapper.invokeMethod('Next', {
                lastCommandId: lastCommandId,
                lastCommandResult: lastCommandResult
            });

            lastCommandId = null;
            lastCommandResult = null;

            switch (response.transactionBodyStatus) {

                case 0:
                    if (response.nextCommand) {
                        lastCommandResult = await executeTransactionCommand(db, response.nextCommand);
                        lastCommandId = response.nextCommand.id;
                    }

                    await Dexie.waitFor(Promise.resolve());
                    break;

                case 1:
                    done = true;
                    break;

                case 2:
                    throw new Error(`Error in transaction body: ${response.transactionBodyErrorMessage}`);

                case 3:
                    throw new Error("Transaction canceled");
                    break;
            }
        }
    });
}

async function executeTransactionCommand(db, transactionCommand) {
    switch (transactionCommand.type) {
        case 0:
            let result = await collectionExecute(db, transactionCommand.storeName, transactionCommand.commands);
            return result;
            break;

        case 1:
            await collectionExecuteNonQuery(db, transactionCommand.storeName, transactionCommand.commands);
            break;
    }
}
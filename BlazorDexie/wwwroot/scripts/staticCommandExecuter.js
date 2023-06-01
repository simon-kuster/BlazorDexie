export async function executeNonQuery(c) {
    Dexie[c.cmd](...c.parameters);
}

export async function execute(c) {
    const result = await Dexie[c.cmd](...c.parameters);;
    if (result === undefined) {
        return null;
    }

    return result;
}
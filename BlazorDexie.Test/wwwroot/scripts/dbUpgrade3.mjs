export default function update(tx) {
    var YEAR = 365 * 24 * 60 * 60 * 1000;
    return tx.table("Friends").toCollection().modify(friend => {
        if (Date.now() - friend.birthdate > 18 * YEAR) {
            friend.isAdult = 1;
        }
    });
}
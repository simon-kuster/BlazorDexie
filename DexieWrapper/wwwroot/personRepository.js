import { db } from "./db.js";

export function getAll() {
    return db.persons.toArray();
}

export function getById(id) {
    return db.persons.get(id).then(person => {
        if (!!person) {
            return person;
        } else {
            return null;
        }
    });
}

export function createOrUpdate(person) {
    db.persons.put(person);
    return person;
}

export function remove(person) {
    db.persons.delete(person.id);
}

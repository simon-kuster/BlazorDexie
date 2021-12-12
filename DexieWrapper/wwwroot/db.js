import Dexie from './dexie.mjs';

export const db = new Dexie('MyDatabase');

db.version(1).stores({
	persons: 'id'
});

// Built-in filters for use
const all = (_) => true;
const id = id => doc => doc.$id === id;

// The root variable will contain all the databases in the application
const root = {};

// A utility function that pretty prints JSONs
function prettyPrint(obj) {
    return JSON.stringify(obj, null, 2);
}

// A function that converts C# lists to Javascript arrays
function listToJSArray(list) {
    const arr = [];
    for (let i = 0; i < list.Count; i++)
        arr.push(list[i]);
    return arr;
}

// A function that lists the current databases or collections in the application
function ls(collectionName) {
    if (collectionName === undefined)
        return listToJSArray(manager.ListDatabases());
    else
        return listToJSArray(manager.ListCollections(collectionName));
}

// A function that creates a new database or a new collection
function create(databaseName, collectionName, schema) {
    if (databaseName === undefined)
        return "The function 'create' accepts at least one argument (the database name)";
    if (collectionName === undefined) {
        manager.CreateDatabase(databaseName);
        root[databaseName] = new Database(databaseName);
        return {created: databaseName};
    }
    else if (schema !== undefined) {
        if (typeof(schema) === 'string')
            schema = load(schema);
        manager.CreateCollection(databaseName, collectionName, JSON.stringify(schema));
        root[databaseName] = new Database(databaseName);
        root[databaseName][collectionName] = new Collection(databaseName, collectionName);
        return {created: `${databaseName}/${collectionName}`}
    }
    else {
        return "A schema must be provided when creating a new collection";
    }
}

// A function that deletes database or collection, depending on the number of arguments
function remove(databaseName, collectionName) {
    if (databaseName === undefined)
        return "The function 'remove' accepts at least one argument (the database name)"
    if (collectionName === undefined) {
        manager.DeleteDatabase(databaseName);
        delete root[databaseName];
        return {deleted: databaseName};
    }
    manager.DeleteCollection(databaseName, collectionName);
    delete root[databaseName][collectionName];
    return {deleted: `${databaseName}/${collectionName}`}
}

class Collection {
    constructor(databaseName, collectionName) {
        this.databaseName = databaseName;
        this.collectionName = collectionName;
    }

    add(...documents) {
        documents.forEach(document => manager.WriteDocument(this.databaseName, this.collectionName, JSON.stringify(document)));
        return {
            added: documents.length
        }
    }

    read(predicate) {
        return listToJSArray(manager.ReadDocuments(
            this.databaseName,
            this.collectionName,
            utils.ConvertPredicate(documentString => predicate(JSON.parse(documentString)))
        )).map(JSON.parse);
    }

    delete(predicate) {
        return {
            deleted: manager.DeleteDocuments(
                this.databaseName,
                this.collectionName,
                utils.ConvertPredicate(documentString => predicate(JSON.parse(documentString)))
            )
        }
    }

    update(predice, updater) {
        return {
            updated: manager.ReplaceDocuments(
                this.databaseName,
                this.collectionName,
                utils.ConvertPredicate(documentString => predice(JSON.parse(documentString))),
                utils.ConvertUpdater(documentString => JSON.stringify(updater(JSON.parse(documentString))))
            )
        }
    }
}

class Database {
    constructor(databaseName) {
        this.databaseName = databaseName;
        // Load all existing collections
        const collections = manager.ListCollections(databaseName);
        for (let i = 0; i < collections.Count; i++) {
            this[collections[i]] = new Collection(databaseName, collections[i]);
        }
    }
}

// Load all existing databases
let databases = manager.ListDatabases()
for (let i = 0; i < databases.Count; i++) {
    root[databases[i]] = new Database(databases[i]);
}
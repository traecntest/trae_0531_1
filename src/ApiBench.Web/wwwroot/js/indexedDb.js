(function () {
  let db = null;

  function init(dbName, version, storeNames) {
    return new Promise(function (resolve, reject) {
      const request = indexedDB.open(dbName, version);

      request.onupgradeneeded = function (e) {
        const database = e.target.result;
        storeNames.forEach(function (name) {
          if (!database.objectStoreNames.contains(name)) {
            database.createObjectStore(name, { keyPath: 'id' });
          }
        });
      };

      request.onsuccess = function (e) {
        db = e.target.result;
        resolve();
      };

      request.onerror = function (e) {
        reject(e.target.error);
      };
    });
  }

  function put(storeName, item) {
    return new Promise(function (resolve, reject) {
      const tx = db.transaction(storeName, 'readwrite');
      const store = tx.objectStore(storeName);
      const request = store.put(item);

      request.onsuccess = function () {
        resolve(request.result);
      };

      request.onerror = function (e) {
        reject(e.target.error);
      };
    });
  }

  function get(storeName, id) {
    return new Promise(function (resolve, reject) {
      const tx = db.transaction(storeName, 'readonly');
      const store = tx.objectStore(storeName);
      const request = store.get(id);

      request.onsuccess = function () {
        resolve(request.result);
      };

      request.onerror = function (e) {
        reject(e.target.error);
      };
    });
  }

  function getAll(storeName) {
    return new Promise(function (resolve, reject) {
      const tx = db.transaction(storeName, 'readonly');
      const store = tx.objectStore(storeName);
      const request = store.getAll();

      request.onsuccess = function () {
        resolve(request.result);
      };

      request.onerror = function (e) {
        reject(e.target.error);
      };
    });
  }

  function deleteItem(storeName, id) {
    return new Promise(function (resolve, reject) {
      const tx = db.transaction(storeName, 'readwrite');
      const store = tx.objectStore(storeName);
      const request = store.delete(id);

      request.onsuccess = function () {
        resolve();
      };

      request.onerror = function (e) {
        reject(e.target.error);
      };
    });
  }

  function clear(storeName) {
    return new Promise(function (resolve, reject) {
      const tx = db.transaction(storeName, 'readwrite');
      const store = tx.objectStore(storeName);
      const request = store.clear();

      request.onsuccess = function () {
        resolve();
      };

      request.onerror = function (e) {
        reject(e.target.error);
      };
    });
  }

  window.apiBenchIndexedDb = {
    init: init,
    put: put,
    get: get,
    getAll: getAll,
    delete: deleteItem,
    clear: clear
  };
})();

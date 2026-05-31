(function () {
  const editors = {};
  let nextId = 1;
  let loaderLoaded = false;

  function loadMonaco() {
    return new Promise(function (resolve) {
      if (loaderLoaded && window.monaco) {
        resolve();
        return;
      }

      if (loaderLoaded && !window.monaco) {
        const check = setInterval(function () {
          if (window.monaco) {
            clearInterval(check);
            resolve();
          }
        }, 50);
        return;
      }

      loaderLoaded = true;
      const script = document.createElement('script');
      script.src = 'https://cdn.jsdelivr.net/npm/monaco-editor@0.45.0/min/vs/loader.js';
      script.onload = function () {
        require.config({
          paths: { vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.45.0/min/vs' }
        });
        require(['vs/editor/editor.main'], function () {
          resolve();
        });
      };
      document.head.appendChild(script);
    });
  }

  function create(containerId, language, value, readOnly) {
    return loadMonaco().then(function () {
      const container = document.getElementById(containerId);
      if (!container) return -1;

      const id = nextId++;
      const editor = monaco.editor.create(container, {
        value: value || '',
        language: language || 'plaintext',
        readOnly: !!readOnly,
        automaticLayout: true
      });

      editors[id] = editor;
      return id;
    });
  }

  function getValue(editorId) {
    const editor = editors[editorId];
    return editor ? editor.getValue() : null;
  }

  function setValue(editorId, value) {
    const editor = editors[editorId];
    if (editor) {
      editor.setValue(value || '');
    }
  }

  function setLanguage(editorId, language) {
    const editor = editors[editorId];
    if (editor) {
      const model = editor.getModel();
      if (model) {
        monaco.editor.setModelLanguage(model, language);
      }
    }
  }

  function dispose(editorId) {
    const editor = editors[editorId];
    if (editor) {
      editor.dispose();
      delete editors[editorId];
    }
  }

  function onValueChanged(editorId, dotNetRef, callbackName) {
    const editor = editors[editorId];
    if (editor) {
      editor.onDidChangeModelContent(function () {
        dotNetRef.invokeMethodAsync(callbackName, editor.getValue());
      });
    }
  }

  window.apiBenchMonaco = {
    create: create,
    getValue: getValue,
    setValue: setValue,
    setLanguage: setLanguage,
    dispose: dispose,
    onValueChanged: onValueChanged
  };
})();

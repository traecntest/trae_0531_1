(function () {
  const containers = {};

  function render(containerId, jsonString) {
    const container = document.getElementById(containerId);
    if (!container) return;

    container.innerHTML = '';
    let parsed;
    try {
      parsed = JSON.parse(jsonString);
    } catch (e) {
      container.textContent = 'Invalid JSON';
      return;
    }

    const tree = buildNode(parsed);
    container.appendChild(tree);
    containers[containerId] = container;
  }

  function buildNode(data) {
    const wrapper = document.createElement('div');
    wrapper.className = 'jv-node';

    if (data === null) {
      wrapper.textContent = 'null';
      wrapper.className += ' jv-null';
      return wrapper;
    }

    if (typeof data === 'object' && !Array.isArray(data)) {
      const keys = Object.keys(data);
      const header = document.createElement('span');
      header.className = 'jv-toggle';
      header.textContent = '\u25BC {' + keys.length + '}';
      header.style.cursor = 'pointer';

      const children = document.createElement('div');
      children.className = 'jv-children';

      keys.forEach(function (key) {
        const row = document.createElement('div');
        row.className = 'jv-row';
        row.style.marginLeft = '16px';
        const label = document.createElement('span');
        label.className = 'jv-key';
        label.textContent = JSON.stringify(key) + ': ';
        row.appendChild(label);
        row.appendChild(buildNode(data[key]));
        children.appendChild(row);
      });

      header.onclick = function () {
        const isHidden = children.style.display === 'none';
        children.style.display = isHidden ? '' : 'none';
        header.textContent = isHidden ? '\u25BC {' + keys.length + '}' : '\u25B6 {' + keys.length + '}';
      };

      wrapper.appendChild(header);
      wrapper.appendChild(children);
    } else if (Array.isArray(data)) {
      const header = document.createElement('span');
      header.className = 'jv-toggle';
      header.textContent = '\u25BC [' + data.length + ']';
      header.style.cursor = 'pointer';

      const children = document.createElement('div');
      children.className = 'jv-children';

      data.forEach(function (item, i) {
        const row = document.createElement('div');
        row.className = 'jv-row';
        row.style.marginLeft = '16px';
        const label = document.createElement('span');
        label.className = 'jv-index';
        label.textContent = i + ': ';
        row.appendChild(label);
        row.appendChild(buildNode(item));
        children.appendChild(row);
      });

      header.onclick = function () {
        const isHidden = children.style.display === 'none';
        children.style.display = isHidden ? '' : 'none';
        header.textContent = isHidden ? '\u25BC [' + data.length + ']' : '\u25B6 [' + data.length + ']';
      };

      wrapper.appendChild(header);
      wrapper.appendChild(children);
    } else {
      const span = document.createElement('span');
      span.className = typeof data === 'string' ? 'jv-string' : typeof data === 'number' ? 'jv-number' : typeof data === 'boolean' ? 'jv-boolean' : 'jv-null';
      span.textContent = typeof data === 'string' ? JSON.stringify(data) : String(data);
      wrapper.appendChild(span);
    }

    return wrapper;
  }

  function search(containerId, regex) {
    const container = containers[containerId] || document.getElementById(containerId);
    if (!container) return;

    const allKeys = container.querySelectorAll('.jv-key');
    const allValues = container.querySelectorAll('.jv-string, .jv-number, .jv-boolean, .jv-null');

    allKeys.forEach(function (el) {
      el.classList.remove('jv-highlight');
      try {
        if (regex && new RegExp(regex).test(el.textContent)) {
          el.classList.add('jv-highlight');
        }
      } catch (e) {}
    });

    allValues.forEach(function (el) {
      el.classList.remove('jv-highlight');
      try {
        if (regex && new RegExp(regex).test(el.textContent)) {
          el.classList.add('jv-highlight');
        }
      } catch (e) {}
    });
  }

  function expandAll(containerId) {
    const container = containers[containerId] || document.getElementById(containerId);
    if (!container) return;

    const children = container.querySelectorAll('.jv-children');
    children.forEach(function (el) {
      el.style.display = '';
    });

    const toggles = container.querySelectorAll('.jv-toggle');
    toggles.forEach(function (el) {
      const text = el.textContent;
      if (text.startsWith('\u25B6')) {
        el.textContent = '\u25BC' + text.substring(1);
      }
    });
  }

  function collapseAll(containerId) {
    const container = containers[containerId] || document.getElementById(containerId);
    if (!container) return;

    const children = container.querySelectorAll('.jv-children');
    children.forEach(function (el) {
      el.style.display = 'none';
    });

    const toggles = container.querySelectorAll('.jv-toggle');
    toggles.forEach(function (el) {
      const text = el.textContent;
      if (text.startsWith('\u25BC')) {
        el.textContent = '\u25B6' + text.substring(1);
      }
    });
  }

  window.apiBenchJsonViewer = {
    render: render,
    search: search,
    expandAll: expandAll,
    collapseAll: collapseAll
  };
})();

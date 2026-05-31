(function () {
  function readFileAsBase64(fileInputId) {
    return new Promise(function (resolve, reject) {
      const input = document.getElementById(fileInputId);
      if (!input || !input.files || input.files.length === 0) {
        reject(new Error('No file selected'));
        return;
      }

      const file = input.files[0];
      const reader = new FileReader();

      reader.onload = function () {
        const base64 = reader.result.split(',')[1];
        resolve({
          base64: base64,
          fileName: file.name
        });
      };

      reader.onerror = function () {
        reject(new Error('Failed to read file'));
      };

      reader.readAsDataURL(file);
    });
  }

  function clickElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
      element.click();
    }
  }

  window.apiBenchFileUpload = {
    readFileAsBase64: readFileAsBase64
  };

  window.clickElement = clickElement;
})();

// based on https://forum.unity.com/threads/how-do-i-let-the-user-load-an-image-from-their-harddrive-into-a-webgl-app.380985/
var TapeUpdownPlugin = {
  TapeUpdownInit: function() {
    var div = document.createElement('div');
    var uploadLabel = document.createElement('p')
    var fileInput = document.createElement('input');
    var downloadLabel = document.createElement('p')

    div.id = 'updownDiv';
    div.style.alignItems = 'flex-end';
    uploadLabel.innerText = 'Upload...';

    fileInput.type = 'file';
    fileInput.onclick = function(event) {
      this.value = null;
    };
    fileInput.onchange = function(event) {
      SendMessage('TapeLibrary', 'UploadName', event.target.files[0].name);
      SendMessage('TapeLibrary', 'Upload', URL.createObjectURL(event.target.files[0]));
    };

    downloadLabel.innerText = 'Download...';

    document.body.appendChild(div);
    div.appendChild(uploadLabel);
    div.appendChild(fileInput);
    div.appendChild(downloadLabel);
  }, 

  // based on "traditional way" from https://web.dev/browser-fs-access/

  AddTapeDownload: function(name, data) {
    var div = document.getElementById('updownDiv');
    var a = document.createElement('a');
    var p = document.createElement('p');

    a.innerText = UTF8ToString(name);
    a.download = UTF8ToString(name);
    a.href = URL.createObjectURL(new Blob([UTF8ToString(data)], {type: 'text/plain'}));
    a.addEventListener('click', function (e) {
        setTimeout(function () {URL.revokeObjectURL(a.href);}, 30 * 1000);
    }); 
    div.appendChild(p);
    p.appendChild(a);
  }, 

  AddPngDownload: function(name, data, size) {
    var div = document.getElementById('updownDiv');
    var a = document.createElement('a');
    var p = document.createElement('p');

    var bytes = new Uint8Array(size);
    for (var i = 0; i < size; i++)
    {
      bytes[i] = HEAPU8[data + i];
    }

    a.innerText = UTF8ToString(name);
    a.download = UTF8ToString(name);
    a.href = URL.createObjectURL(new Blob([bytes.buffer], {type: 'image/png'}));
    a.addEventListener('click', function (e) {
        setTimeout(function () {URL.revokeObjectURL(a.href);}, 30 * 1000);
    }); 
    div.appendChild(p);
    p.appendChild(a);
  }
};

mergeInto(LibraryManager.library, TapeUpdownPlugin);

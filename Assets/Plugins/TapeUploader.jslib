var TapeUploaderPlugin = {
  TapeUploaderInit: function() {
    var div = document.createElement('div');
    var uploadLabel = document.createElement('p')
    var fileInput = document.createElement('input');
    var downloadLabel = document.createElement('p')

    div.setAttribute('id', 'fileDiv');

    uploadLabel.innerText = 'Upload...';

    fileInput.setAttribute('type', 'file');
    fileInput.onclick = function (event) {
      this.value = null;
    };
    fileInput.onchange = function (event) {
      SendMessage('TapeLibrary', 'UploadName', URL.createObjectURL(event.target.files[0].name));
      SendMessage('TapeLibrary', 'Upload', URL.createObjectURL(event.target.files[0]));
    }

    downloadLabel.innerText = 'Download...';

    document.body.appendChild(div);
    div.appendChild(uploadLabel);
    div.appendChild(fileInput);
    div.appendChild(downloadLabel);
  }
};
mergeInto(LibraryManager.library, TapeUploaderPlugin);

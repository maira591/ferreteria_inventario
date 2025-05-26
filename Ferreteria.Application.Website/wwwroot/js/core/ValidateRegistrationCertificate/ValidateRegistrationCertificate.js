async function AJAXSubmit(oFormElement) {
    var urlBase = document.getElementById("baseUrl").value;
    showLoading('Verificando código...')
    const formData = new FormData(oFormElement);
    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        })
            .then(function (res) { return res.json(); })
            .then(function (data) {
                if (data.Status) {

                    // Convertir la cadena base64 a un ArrayBuffer
                    let byteCharacters = atob(data.BytesFile.FileContents);
                    let byteNumbers = new Array(byteCharacters.length);
                    for (let i = 0; i < byteCharacters.length; i++) {
                        byteNumbers[i] = byteCharacters.charCodeAt(i);
                    }
                    let byteArray = new Uint8Array(byteNumbers);

                    // Crear un Blob
                    let blob = new Blob([byteArray], { type: 'application/pdf' });

                    // Hacer algo con el Blob, por ejemplo, abrirlo en una nueva ventana
                    let blobUrl = URL.createObjectURL(blob);
                    // Crear un enlace <a> para descargar el Blob
                    let a = document.createElement('a');
                    a.href = blobUrl;
                    a.download = 'Certificado de inscripción.pdf'; // Establecer el nombre del archivo
                    a.style.display = 'none'; // Ocultar el enlace

                    document.body.appendChild(a);
                    a.click();
                    hideLoading();

                    showAlert("Certificado descargado correctamente.", 'success');
                } else {
                    showAlert(data.Message, 'danger');
                }
            });
    } catch (error) {
        hideLoading();
        console.error(error);
    }
}
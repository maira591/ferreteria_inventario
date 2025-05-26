//Clase encargada del crud de validaciones.
var Validations = (function () {
    function Validations() {

    }
       
    //Método encargado de abrir la ventana para editar
    Validations.OpenEdit = function (urlEdit, id) {
        var form = document.createElement("form");
        form.method = 'post';
        form.action = urlEdit;
        var input = document.createElement('input');
        input.type = "hidden";
        input.name = "id";
        input.value = id;
        form.appendChild(input);
        $('body').append(form);
        form.submit();
    };

    //Método encargado de recargar la información de las validaciones
    Validations.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    /**
     * Exporta las validaciones en formato Excel
     * @param {any} url
     */
    Validations.ExportValidations = function (url) {
        var fileName = 'Validaciones.xlsx';
        $.ajax({
            url: url,
            method: 'post',
            beforeSend: function () {
                showLoading('Cargando...')
            },
            xhrFields: { responseType: 'blob' },
            data: { fileName },
            success: function (data) {
                var blob = new Blob([data], { type: 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                var downloadUrl = URL.createObjectURL(blob);
                var a = document.createElement("a");
                a.href = downloadUrl;
                a.download = fileName;
                a.target = '_blank';
                a.click();
            },
            error: function (jqXHR, exception) {
                showAlert("Unexpected error", "warning");
            },
            complete: function () {
                $("#modal-loading").modal("hide");
                $("#modal-loading-back").hide();
            }
        });
    };

    return Validations;
})();
//Clase encargada del crud de validaciones.
var MessageError = (function () {
    function MessageError() {

    }
       

    //Método encargado de abrir la ventana para editar
    MessageError.OpenEdit = function (urlEdit, id) {
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
    MessageError.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    MessageError.Save = function () {
        var messageError = getObject("formMessageCrud");

        showLoading("Validando...");
        if (formValidation("formMessageCrud")) {
            hideLoading();
            ExecuteAjax("MessageError/SaveInfo", "Post", messageError, "MessageError.ResultSave");
        }
    };

    MessageError.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        MessageError.Reload();
    };

    //Método encargado de recargar la información
    MessageError.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    MessageError.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    MessageError.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    MessageError.ExportMessageErrors = function (url) {
        var fileName = 'Mensajes de Error.xlsx';
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

    return MessageError;
})();
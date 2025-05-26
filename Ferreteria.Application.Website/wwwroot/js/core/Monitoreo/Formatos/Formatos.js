var Formatos = (function () {
    function Formatos() {

    }

    //Método encargado de abrir la ventana para editar
    Formatos.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    Formatos.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    Formatos.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Formatos.Save = function () {
        if ($("#Circular").val() != '' && $("#Circular").val() <= 0) {
            showAlert("El campo Circular, debe ser mayor a cero.", 'danger');
            return;
        }
        var format = getObject("formconstantescrud");
        if (formValidation("formconstantescrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoFormatos/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoFormatos/CreateOrUpdate", "Post", format, "Formatos.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    debugger;

                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Formatos.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Formatos.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return Formatos;
})();
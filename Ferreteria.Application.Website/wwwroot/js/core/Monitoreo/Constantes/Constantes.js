var Constantes = (function () {
    function Constantes() {

    }

    //Método encargado de abrir la ventana para editar
    Constantes.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    Constantes.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    Constantes.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Constantes.Save = function () {
        var format = getObject("formconstantescrud");
        if (formValidation("formconstantescrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoConstantes/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoConstantes/CreateOrUpdate", "Post", format, "Constantes.ResultSaveFormat");
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

    Constantes.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Constantes.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return Constantes;
})();
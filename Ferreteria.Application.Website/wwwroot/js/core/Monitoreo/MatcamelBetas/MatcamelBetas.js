var MatcamelBetas = (function () {
    function MatcamelBetas () {

    }

    //Método encargado de abrir la ventana para editar
    MatcamelBetas.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            $(".inputPositiveInput2d").numeric({ negative: true, decimalPlaces: 10 });
            $('#Anio').attr('readonly', 'readonly');
            $('#TipoEntidad').attr('readonly', 'readonly');
            $('#IdBeta').attr('readonly', 'readonly');
        });
    };

    //Método encargado de recargar la información de los formatos
    MatcamelBetas.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    MatcamelBetas.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            $(".inputPositiveInput2d").numeric({ negative: true, decimalPlaces: 10 });
        });
    };

    MatcamelBetas.Save = function () {
        var format = getObject("formatcamelbetascrud");
        if (formValidation("formatcamelbetascrud")) {

            if (!isValidInteger("Valor", 10, "Valor")) {
                return;
            }
           
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoMatcamelBetas/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoMatcamelBetas/CreateOrUpdate", "Post", format, "MatcamelBetas.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    MatcamelBetas.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            MatcamelBetas.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return MatcamelBetas;
})();
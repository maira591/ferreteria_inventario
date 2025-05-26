var Indicator = (function () {
    function Indicator() {

    }

    //Método encargado de abrir la ventana para editar
    Indicator.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información
    Indicator.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    Indicator.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Periodicity.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };


    //Método encargado de realizar el borrado de un tipo de carga.
    Indicator.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Indicator.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    Indicator.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Indicator.Save = function () {
        var periodicity = getObject("formIndicatorCrud");

        if (formValidation("formIndicatorCrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'Indicator/Validations',
                type: 'POST',
                data: periodicity,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("Indicator/CreateOrUpdate", "Post", periodicity, "Indicator.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del indicador.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Indicator.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        Indicator.Reload();
    };


    Indicator.ExportIndicator = function (url) {
        var fileName = 'Indicadores.xlsx';
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

    return Indicator;
})();
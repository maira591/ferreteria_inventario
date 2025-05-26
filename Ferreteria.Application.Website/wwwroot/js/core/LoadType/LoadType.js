var LoadType = (function () {
    function LoadType() {

    }

    //Método encargado de abrir la ventana para editar
    LoadType.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información
    LoadType.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    LoadType.Delete = function (url) {
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
    LoadType.DisabledLoadType = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    LoadType.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    LoadType.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    LoadType.Save = function () {
        var periodicity = getObject("formloadtypecrud");

        if (formValidation("formloadtypecrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'LoadType/Validations',
                type: 'POST',
                data: periodicity,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("LoadType/CreateOrUpdate", "Post", periodicity, "LoadType.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del tipo de carga.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    LoadType.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        LoadType.Reload();
    };

    /**
     * Exporta las validaciones en formato Excel
     * @param {any} url
     */
    LoadType.ExportLoadTypes = function (url) {
        var fileName = 'Tipos de Carga.xlsx';
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

    return LoadType;
})();
var RangosValores = (function () {
    function RangosValores () {

    }

    //Método encargado de abrir la ventana para editar
    RangosValores.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            $('#TipoRango').attr('readonly', 'readonly');
            $('#RangoHasta').attr('readonly', 'readonly');
        });
    };

    RangosValores.LoadLastRangoDesde = function (value) {
        showLoading("Cargando valores...");

        $.ajaxSetup({ cache: false });
        $.ajax({
            ContentType: "application/json",
            url: 'MonitoreoRangosValores/GetLastValues',
            type: 'POST',
            data: { rangeType: value },
            success: function (data) {
                hideLoading();
                $('#RangoDesde').val(data);
            },
            error: function (jqXHR, exception) {
                showAlert("No fue posible cargar el último valor para el rango seleccionado.", 'danger');
                hideLoading();
            }
        });
    };

    //Método encargado de recargar la información de los formatos
    RangosValores.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    RangosValores.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    RangosValores.Save = function () {
        var format = getObject("formatrangosvalores");
        if (formValidation("formatrangosvalores")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoRangosValores/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoRangosValores/CreateOrUpdate", "Post", format, "RangosValores.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    RangosValores.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            RangosValores.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return RangosValores;
})();
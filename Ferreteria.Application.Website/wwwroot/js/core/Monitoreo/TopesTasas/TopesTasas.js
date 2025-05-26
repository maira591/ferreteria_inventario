var TopesTasas = (function () {
    function TopesTasas () {

    }

    TopesTasas.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            $("#FecIni").val($("#hdf_FechaInicio").val());
            $("#FecFin").val($("#hdf_FechaFin").val());
            $('#FecFin').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
            $('#FecIni, #FecFin').mask('00/00/0000', { placeholder: "__/__/____" });
            showModalResult();
        });
    };

    TopesTasas.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            $("#FecIni").val($("#hdf_FechaInicio").val());
            $("#FecFin").val($("#hdf_FechaFin").val());
            $('#FecFin').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
            $('#FecIni, #FecFin').mask('00/00/0000', { placeholder: "__/__/____" });
            TopesTasas.InitControls();
            showModalResult();
            $('#FecIni').attr('readonly', 'readonly');
            $('#Clasificacion').attr('readonly', 'readonly');
        });
    };

    TopesTasas.InitControls = function () {
        if ($("#FecIni").val() === "") {
            $('#FecIni').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
        }
    };

    TopesTasas.GetFecFin = function (value) {
        $.ajaxSetup({ cache: false });
        $.ajax({
            ContentType: "application/json",
            url: 'MonitoreoTopesTasas/GetLastDayOfMonth',
            type: 'POST',
            data: { fecIni: value },
            success: function (data) {
                hideLoading();
                $('#FecFin').val(data);
            },
            error: function (jqXHR, exception) {
                showAlert("No fue posible cargar el último día para la Fecha seleccionada.", 'danger');
            }
        });
    };

    TopesTasas.LoadLastEndDate = function (value) {
        showLoading("Cargando Fecha...");

        $.ajaxSetup({ cache: false });
        $.ajax({
            ContentType: "application/json",
            url: 'MonitoreoTopesTasas/GetLastDayOfMonthByClasification',
            type: 'POST',
            data: { idClasificacion: value },
            success: function (data) {
                hideLoading();
                $('#FecFin').val(data.fecFin);
                $('#FecIni').val(data.fecIni);
                if ($('#FecIni').val() !== "") {
                    $('#FecIni').attr('readonly', 'readonly');
                }
                else {
                    $('#FecIni').removeAttr('readonly', 'readonly');
                    TopesTasas.InitControls();
                }
            },
            error: function (jqXHR, exception) {
                showAlert("No fue posible cargar el último valor para la Fecha seleccionada.", 'danger');
                hideLoading();
            }
        });

    };

    //Método encargado de recargar la información de los formatos
    TopesTasas.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    
    TopesTasas.Save = function () {
        var format = getObject("formatrangosvalores");
        if (formValidation("formatrangosvalores")) {

            if (!isValidInteger("TasaUsura", 4, "Tasa de Usura")) {
                return;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoTopesTasas/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoTopesTasas/CreateOrUpdate", "Post", format, "TopesTasas.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    TopesTasas.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            TopesTasas.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return TopesTasas;
})();
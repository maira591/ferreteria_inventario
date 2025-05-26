var PsdControlFecha = (function () {
    function PsdControlFecha() {

    }

    //Método encargado de abrir la ventana para editar
    PsdControlFecha.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    PsdControlFecha.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    PsdControlFecha.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    PsdControlFecha.Save = function () {

        var format = getObject("formentity");

        if (formValidation("formentity")) {
            if (PsdControlFecha.ValidateDates()) {
                return;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'RecaudoPsdControlFecha/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("RecaudoPsdControlFecha/CreateOrUpdate", "Post", format, "PsdControlFecha.ResultSave");
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

    PsdControlFecha.InitControls = function () {
        $("#FechaInicio").val($("#hdf_FechaInicio").val());
        $("#FechaFinal").val($("#hdf_FechaFinal").val());
        $("#FechaCorte").val($("#hdf_FechaCorte").val());
        $("#FechaMaxPago").val($("#hdf_FechaMaxPago").val());

        $('#FechaMaxPago').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
        $('#FechaMaxPago').mask('00/00/0000', { placeholder: "__/__/____" });

        if ($("#IsNew").val() == '0') {
            $('#FechaCorte').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
            $('#FechaCorte').mask('00/00/0000', { placeholder: "__/__/____" });
        } else {
            $('#FechaCorte').attr("readonly", true)

        }

        $('#FechaCorte').change(() => {
            PsdControlFecha.GetCalculationDates();
        });
        $('#NumeroMeses').change(() => {
            PsdControlFecha.GetCalculationDates();
        });
    }

    PsdControlFecha.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            PsdControlFecha.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    PsdControlFecha.GetCalculationDates = function () {

        if ($('#FechaCorte').val() != '' && $('#NumeroMeses').val() != '') {
            var format = getObject("formentity");
            showLoading("Calculando Fechas...");

            $.ajax({
                ContentType: "application/json",
                url: 'RecaudoPsdControlFecha/GetParamsByCutoffDate',
                type: 'POST',
                data: format,
                success: function (data) {
                    $("#FechaFinal").val(data.FechaFinal)
                    $("#FechaInicio").val(data.FechaInicio)
                    $("#NumeroDias").val(data.NumeroDias)
                    $("#FechaMaxPago").val(data.FechaMaxPago)
                    hideLoading();
                },
                error: function (jqXHR, exception) {
                    showAlert("No obtener la fechas.", 'danger');
                    hideLoading();
                }
            });
        }

    }

    PsdControlFecha.ValidateDates = function () {
        var maxPaymentDate = ConvertStringToDate($("#FechaMaxPago").val());
        var cutoffDate = ConvertStringToDate($("#FechaCorte").val());
        if (maxPaymentDate < cutoffDate) {
            $("#FechaMaxPago").addClass("errorValidate");
            $("#FechaMaxPago").attr("data-errormessage", "Debe ser mayor a la Fecha de Corte.");
            showAlert("El campo Fecha Maxima Pago debe ser mayor al campo Fecha Corte.", 'danger');
            showTooltip();
            return true;
        }

        return false;
    }

    

    return PsdControlFecha;
})();
var PucFinanciera = (function () {
    function PucFinanciera() {

    }

    //Método encargado de abrir la ventana para editar
    PucFinanciera.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información 
    PucFinanciera.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    PucFinanciera.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    PucFinanciera.Save = function () {
        var format = getObject("formentity");
        if (formValidation("formentity")) {

            if (PucFinanciera.ValidateDates()) {
                return;
            }
            if (PucFinanciera.ValidateLengthAccount()) {
                return;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoPucFinanciera/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoPucFinanciera/CreateOrUpdate", "Post", format, "PucFinanciera.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    PucFinanciera.InitControls = function () {
        $("#FechaInicio").val($("#hdf_FechaInicio").val());
        $("#FechaFin").val($("#hdf_FechaFin").val());

        if ($("#FechaFin").val() != '') {
            $("#FechaFin").attr("readonly", true);
        } else {
            $('#FechaFin').mask('00/00/0000', { placeholder: "__/__/____" });
            $('#FechaFin').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
        }

        if ($("#IsNew").val() == '') {
            $("#FechaInicio").attr("readonly", true);
            $("#NombreCuenta").attr("readonly", true);
            $("#CuentaTxt").attr("readonly", true);
        }

        $('#FechaInicio').mask('00/00/0000', { placeholder: "__/__/____" });
        

        $("#CuentaTxt").on("input", function () {
            if ($("#CuentaTxt").val() != '') {
                Petitions.Get('MonitoreoPucFinanciera/GetLastRecordByAccount?account=' + $("#CuentaTxt").val(), function (response) {

                    $("#FechaInicio").val(response.FechaInicio);

                    if (response.Exists) {
                        $("#FechaInicio").attr("readonly", true);
                    } else {
                        $("#FechaInicio").attr("readonly", false);
                        $('#FechaInicio').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
                    }
                });
            } else {
                $("#FechaInicio").val('')
            }
        });
    };

    PucFinanciera.ValidateDates = function () {
        var dateEnd;
        var dateStartSplit = $("#FechaInicio").val().split("/");
        var dateStart = new Date(`${dateStartSplit[2]}/${dateStartSplit[1]}/${dateStartSplit[0]}`);

        if ($("#FechaInicio").val() != "") {
            var dateEndSplit = $("#FechaFin").val().split("/");
            dateEnd = new Date(`${dateEndSplit[2]}/${dateEndSplit[1]}/${dateEndSplit[0]}`);
        }

        if (dateEnd.getFullYear() < 2000) {
            $("#FechaFin").addClass("errorValidate");
            $("#FechaFin").attr("data-errormessage", "No debe ser menor al año 2000.");
            showAlert("El campo Fecha Fin no debe ser menor al año 2000.", 'danger');
            showTooltip();
            return true;
        }

        if (dateStart.getFullYear() < 2000) {
            $("#FechaInicio").addClass("errorValidate");
            $("#FechaInicio").attr("data-errormessage", "No debe ser menor al año 2000.");
            showAlert("El campo Fecha Inicio no debe ser menor al año 2000.", 'danger');
            showTooltip();
            return true;
        }

        if (dateEnd < dateStart) {
            $("#FechaFin").addClass("errorValidate");
            $("#FechaFin").attr("data-errormessage", "Debe ser mayor a la Fecha de Inicio.");
            showAlert("El campo Fecha Fin debe ser mayor al campo Fecha Inicio.", 'danger');
            showTooltip();
            return true;
        }

        return false;
    };

    PucFinanciera.ValidateLengthAccount = function () {
        if (($("#CuentaTxt").val()).length < 6) {
            $("#CuentaTxt").addClass("errorValidate");
            $("#CuentaTxt").attr("data-errormessage", "Debe tener 6 caracteres.");
            showAlert("El campo Cuenta debe tener 6 caracteres.", 'danger');
            showTooltip();
            return true;
        }

        if (($("#CuentaTxt").val()).substring(0, 1) == "0") {
            $("#CuentaTxt").addClass("errorValidate");
            $("#CuentaTxt").attr("data-errormessage", "No debe comenzar con 0.");
            showAlert("El campo Cuenta no debe comenzar con 0", 'danger');
            showTooltip();
            return true;
        }

        return false;
    };

    PucFinanciera.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            PucFinanciera.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return PucFinanciera;
})();
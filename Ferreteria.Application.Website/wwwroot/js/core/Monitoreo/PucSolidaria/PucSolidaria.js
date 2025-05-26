var PucSolidaria = (function () {
    function PucSolidaria() {

    }

    //Método encargado de abrir la ventana para editar
    PucSolidaria.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información 
    PucSolidaria.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    PucSolidaria.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    PucSolidaria.Save = function () {
        var format = getObject("formentity");
        if (formValidation("formentity")) {

            if (PucSolidaria.ValidateDates()) {
                return;
            }
            if (PucSolidaria.ValidateLengthAccount()) {
                return;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoPucSolidaria/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoPucSolidaria/CreateOrUpdate", "Post", format, "PucSolidaria.ResultSaveFormat");
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

    PucSolidaria.InitControls = function () {
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
                Petitions.Get('MonitoreoPucSolidaria/GetLastRecordByAccount?account=' + $("#CuentaTxt").val(), function (response) {

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

    PucSolidaria.ValidateDates = function () {
        var dateEnd;
        var dateStart = ConvertStringToDate($("#FechaInicio").val());
        
        if ($("#FechaInicio").val() != "") {
            dateEnd = ConvertStringToDate($("#FechaFin").val());
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

    PucSolidaria.ValidateLengthAccount = function () {
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

    PucSolidaria.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            PucSolidaria.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return PucSolidaria;
})();
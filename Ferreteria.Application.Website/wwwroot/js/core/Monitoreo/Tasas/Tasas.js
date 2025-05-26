var Tasas = (function () {
    function Tasas() {

    }

    //Método encargado de abrir la ventana para editar
    Tasas.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    Tasas.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    Tasas.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Tasas.Save = function () {

        var format = getObject("formtasascrud");
        if (formValidation("formtasascrud")) {

            if (Tasas.ValidarFechaTasa()) {
                return;
            }

            if ($("#ValorTasa").val() != '' && $("#ValorTasa").val() <= 0) {
                $("#ValorTasa").addClass("errorValidate");
                $("#ValorTasa").attr("data-errormessage", "Debe ser mayor a cero.");
                showAlert("El campo Valor Tasa, debe ser mayor a cero.", 'danger');
                showTooltip();
                return;
            }

            if (!isValidInteger("ValorTasa", 3, "Valor Tasa")) {
                return;
            }

            $.ajaxSetup({ cache: false });

            showLoading("Guardando...");
            ExecuteAjax("MonitoreoTasas/CreateOrUpdate", "Post", format, "Tasas.ResultSaveFormat");
            hideLoading();
        }
    };

    Tasas.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Tasas.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    Tasas.InitControls = function () {
        if ($('#Id').val() == '0') {
            $('#NombreTasa').select2();
        }
        $("#FechaTasa").val($("#hdf_FechTasa").val());

        $("#NombreTasa").change(function () {
            Petitions.Get('MonitoreoTasas/GetLastRecordByNameRates?nameRates=' + $("#NombreTasa").val(), function (response) {
                $("#FechaTasa").val(response.FechaTasa);

                var splitDate = $("#FechaTasa").val().split("/");
                var year = parseInt(splitDate[2]);
                var month = parseInt(splitDate[1]);
               
                var primerDia = new Date(year, month >= 12 ? month - 1 : month < 12 ? month - 1 : month, 1);
                var ultimoDia = new Date(year, primerDia.getMonth() + 1, 0);
                
                if ($("#NombreTasa").val() == "PUNTOS" || $("#NombreTasa").val() == "DTF") {
                    $("#FechaTasa").val(primerDia.toJSON().slice(0, 10).split('-').reverse().join('/'));
                } else if ($("#NombreTasa").val() == '') {
                    $("#FechaTasa").val('');
                } else {
                    $("#FechaTasa").val(ultimoDia.toJSON().slice(0, 10).split('-').reverse().join('/'));
                }

            });
        });
    };

    Tasas.ValidarFechaTasa = function () {
        var splitDate = $("#FechaTasa").val().split("/");
        var year = splitDate[2];
        var month = splitDate[1];

        var date = new Date();
        var dateNextMonth = new Date(year, month, 0)
        var dateNow = new Date(date.getFullYear(), date.getMonth() + 1, 0)

        if (dateNextMonth > dateNow) {

            $("#FechaTasa").addClass("errorValidate");
            $("#FechaTasa").attr("data-errormessage", "No puede ser un mes futuro.");
            showAlert("La fecha no puede ser de un mes futuro o mayor al actual.", 'danger');
            showTooltip();
        }

        return dateNextMonth > dateNow;
    }

    return Tasas;
})();
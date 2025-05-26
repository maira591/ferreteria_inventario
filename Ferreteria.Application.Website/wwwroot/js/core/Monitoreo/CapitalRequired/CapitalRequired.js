var CapitalRequired = (function () {
    function CapitalRequired() {

    }

    //Método encargado de abrir la ventana para editar
    CapitalRequired.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            $('#Anio').prop('readonly', 'readonly');
            $('#CapitalFinanciera').prop('readonly', 'readonly');
            $('#CapitalSolidaria').prop('readonly', 'readonly');
        });
    };

    //Método encargado de recargar la información de los formatos
    CapitalRequired.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    CapitalRequired.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            $(".inputPositiveInput2d").numeric({ negative: false, decimalPlaces: 2 });
        });
    };

    CapitalRequired.Save = function () {
        var format = getObject("formcapitalrequiredcrud");
        if (formValidation("formcapitalrequiredcrud")) {

            if (!isValidInteger("Ipc", 4, "IPC")) {
                return;
            }
            if (!isValidInteger("CapitalFinanciera", 15, "Capital Financiera")) {
                return;
            }
            if (!isValidInteger("CapitalSolidaria", 15, "Capital Solidaria")) {
                return;
            }
            
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoCapitalRequired/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoCapitalRequired/CreateOrUpdate", "Post", format, "CapitalRequired.ResultSaveFormat");
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

    CapitalRequired.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            CapitalRequired.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return CapitalRequired;
})();
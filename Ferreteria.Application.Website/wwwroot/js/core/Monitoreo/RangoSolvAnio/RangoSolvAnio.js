var RangoSolvAnio = (function () {
    function RangoSolvAnio() {

    }

    //Método encargado de abrir la ventana para editar
    RangoSolvAnio.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            $('#Anio').attr('readonly', 'readonly');
            $(".inputPositiveInput2d").numeric({ negative: false, decimalPlaces: 2 });
            RangoSolvAnio.AdjustInterface();
        });
    };

    //Método encargado de recargar la información de los formatos
    RangoSolvAnio.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    RangoSolvAnio.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            $(".inputPositiveInput2d").numeric({ negative: false, decimalPlaces: 2 });
            RangoSolvAnio.AdjustInterface();
        });
    };

    RangoSolvAnio.AdjustInterface = function () {
        var pattect1 = $('#PreviousPattec1').val();
        var pattect2 = $('#PreviousPattec2').val();
        var pattect3 = $('#PreviousPattec3').val();
        if (pattect1 !== "") {
            $('#Pattec1').attr('readonly', 'readonly');
        }
        if (pattect2 !== "") {
            $('#Pattec2').attr('readonly', 'readonly');
        }
        if (pattect3 !== "") {
            $('#Pattec3').attr('readonly', 'readonly');
        }

        $('#Ipc').on('input', function () {
            var ipc = this.value;
            var previousPattec1 = $('#PreviousPattec1').val();
            var previousPattec2 = $('#PreviousPattec2').val();
            var previousPattec3 = $('#PreviousPattec3').val();
            if (ipc !== "" && previousPattec1 !== "" && previousPattec2 !== "" && previousPattec3 !== "") {
                var params = {
                    Ipc: this.value,
                    PreviousPattec1: previousPattec1,
                    PreviousPattec2: previousPattec2,
                    PreviousPattec3: previousPattec3
                };
                $.ajax({
                    url: 'MonitoreoRangoSolvAnio/CalculatePattec',
                    method: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(params),
                    success: function (data) {

                        $('#Pattec1').val(data.Pattec1);
                        $('#Pattec2').val(data.Pattec2);
                        $('#Pattec3').val(data.Pattec3);
                    }
                });
            }
        });
    };

    RangoSolvAnio.Save = function () {
        var format = getObject("formrangosolvaniocrud");
        if (formValidation("formrangosolvaniocrud")) {

            if (!isValidInteger("Ipc", 3, "Ipc")) {
                return;
            }
            if (!isValidInteger("Pattec1", 15, "Patrimonio Técnico 1")) {
                return;
            }
            if (!isValidInteger("Pattec2", 15, "Patrimonio Técnico 2")) {
                return;
            }
            if (!isValidInteger("Pattec3", 15, "Patrimonio Técnico 3")) {
                return;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoRangoSolvAnio/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoRangoSolvAnio/CreateOrUpdate", "Post", format, "RangoSolvAnio.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    RangoSolvAnio.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            RangoSolvAnio.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return RangoSolvAnio;
})();
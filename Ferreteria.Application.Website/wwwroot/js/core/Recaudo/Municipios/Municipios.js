var Municipios = (function () {
    function Municipios() {

    }

    Municipios.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            $('#Municipio').attr('readonly', 'readonly');
            $('#PrevCodMunicipio').val($('#CodMunicipio').val());
            showModalResult();
        });
    };

    Municipios.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    Municipios.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            $('.inputPositiveInput2d').numeric({ negative: false, decimalPlaces: 0 })
            showModalResult();
        });
    };

    Municipios.Save = function () {

        var format = getObject("formMunicipioscrud");

        if (formValidation("formMunicipioscrud")) {

            if (!isValidInteger("Departamento", 2, "Departamento")) {
                return;
            }

            if (!isValidInteger("Municipio", 3, "Código Municipio Dane")) {
                return;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            showLoading("Guardando...");
            $.ajax({
                ContentType: "application/json",
                url: 'RecaudoMunicipios/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("RecaudoMunicipios/CreateOrUpdate", "Post", format, "Municipios.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Municipios.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Municipios.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    Municipios.CalculateCodMunicipio = function () {
        $.ajax({
            url: 'RecaudoMunicipios/CalculateCodMunicipio',
            type: 'POST',
            data: {
                departamento : $('#Departamento').val(),
                municipio : $('#Municipio').val()
            },
            success: function (data) {
                $('#CodMunicipio').val(data);
            }
        });
    };

    return Municipios;
})();
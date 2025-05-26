var MaximunDate = (function () {
    function MaximunDate() {

    }

    //Método encargado de abrir la ventana para editar
    MaximunDate.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información
    MaximunDate.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    MaximunDate.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    MaximunDate.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    MaximunDate.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };




    MaximunDate.Save = function () {
        var maximunDate = getObject("formmaximundatecrud");

        if (formValidation("formmaximundatecrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MaximunDate/Validations',
                type: 'POST',
                data: maximunDate,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MaximunDate/CreateOrUpdate", "Post", maximunDate, "MaximunDate.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación de la Fecha Maxima.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    MaximunDate.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("MaximunDate/GetEntityList", "GET", { id: entityTypeId }, "MaximunDate.LoadEntities", null);
        } else {
            $('#GuidLoadTypeId').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };

    MaximunDate.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#GuidLoadTypeId').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#GuidLoadTypeId').append(listitems);
    };

    MaximunDate.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        MaximunDate.Reload();
    };

    MaximunDate.InitControls = function () {
        $("#GuidCooperativeTypeId").change(function () {
            MaximunDate.GetEntities($(this).val());
        });
        $("#GuidLoadTypeId").change(function () {
            $("#Name").val($(this).select2('data')[0].text);
        });
    };

    MaximunDate.ExportData = function (url) {
        var fileName = 'Fechas máximas de transmisión.xlsx';
        $.ajax({
            url: url,
            method: 'post',
            beforeSend: function () {
                showLoading('Exportando...')
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

    return MaximunDate;
})();
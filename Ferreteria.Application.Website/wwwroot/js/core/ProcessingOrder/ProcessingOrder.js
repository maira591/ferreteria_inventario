var ProcessingOrder = (function () {
    function ProcessingOrder() {

    }

    //Método encargado de abrir la ventana para editar
    ProcessingOrder.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información
    ProcessingOrder.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    ProcessingOrder.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    ProcessingOrder.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    ProcessingOrder.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    ProcessingOrder.Save = function () {
        $("#EntityName").val($("#EntityCode").select2('data')[0].text);
        var periodicity = getObject("formqueuemessageprioritycrud");

        if (formValidation("formqueuemessageprioritycrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'ProcessingOrder/Validations',
                type: 'POST',
                data: periodicity,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("ProcessingOrder/CreateOrUpdate", "Post", periodicity, "ProcessingOrder.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación de la priorización.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    ProcessingOrder.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        ProcessingOrder.Reload();
    };


    ProcessingOrder.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("ProcessingOrder/GetEntityList", "GET", { entityType: entityTypeId }, "ProcessingOrder.LoadEntities", null);
        } else {
            $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };
    
    ProcessingOrder.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };

    ProcessingOrder.InitControls = function () {
        $("#EntityType").change(function () {
            ProcessingOrder.GetEntities($(this).val());
        });
        $("#EntityCode").change(function () {
            $("#EntityName").val($(this).select2('data')[0].text);
        });
    };

    ProcessingOrder.ExportProcessingOrders = function (url) {
        var fileName = 'Priorización Cargue.xlsx';
        $.ajax({
            url: url,
            method: 'post',
            beforeSend: function () {
                showLoading('Cargando...')
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

    return ProcessingOrder;
})();
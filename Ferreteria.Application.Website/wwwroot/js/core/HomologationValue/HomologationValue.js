var HomologationValue = (function () {
    function HomologationValue() {

    }

    //Método encargado de abrir la ventana para editar
    HomologationValue.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();            
        });
    };

    //Método encargado de recargar la información los valores
    HomologationValue.Reload = function (idHomologation) {
        
        var homologationId = "";
        if (idHomologation !== undefined) {
            homologationId = idHomologation;
        }
        else {
            homologationId = $("#HomologationId").val();
        }           

        var urlReload = $("#urlHomologationValue").val().replace('0', homologationId);

        Petitions.Get(urlReload, function (response) {
            $(`#divHomologationValue_${homologationId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de realizar el borrado un valor
    HomologationValue.Delete = function (url, idHomologation) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este valor?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    HomologationValue.Reload(idHomologation);
                } else {
                    showAlert("Ocurrió un error eliminando el valor, verifique que no tenga datos asociados.");
                }
            });
        });
    };

    HomologationValue.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    HomologationValue.Save = function () {        
        var homologationValue = getObject("formhomologationvaluecrud");

        if (formValidation("formhomologationvaluecrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'HomologationValue/Validations',
                type: 'POST',
                data: homologationValue,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("HomologationValue/CreateOrUpdate", "Post", homologationValue, "HomologationValue.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del valor.", 'danger');
                    hideLoading();
                }
            });
        }
        
    };

    HomologationValue.ResultSave = function (response) {
        hideLoading();        
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        HomologationValue.Reload();
    };

    return HomologationValue;
})();



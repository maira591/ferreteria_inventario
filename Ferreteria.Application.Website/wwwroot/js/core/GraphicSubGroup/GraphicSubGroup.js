var GraphicSubGroup = (function () {
    function GraphicSubGroup() {

    }

    //Método encargado de abrir la ventana para editar
    GraphicSubGroup.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();            
        });
    };

    //Método encargado de recargar la información los valores
    GraphicSubGroup.Reload = function (idGraphicGroup) {
        
        var graphicGroupId = "";
        if (idGraphicGroup !== undefined) {
            graphicGroupId = idGraphicGroup;
        }
        else {
            graphicGroupId = $("#GraphicGroupId").val();
        }           

        var urlReload = $("#urlGraphicSubGroup").val().replace('0', graphicGroupId);

        Petitions.Get(urlReload, function (response) {
            $(`#divChildren_${graphicGroupId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de realizar el borrado un valor
    GraphicSubGroup.Delete = function (url, idGraphicGroup) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este valor?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    GraphicSubGroup.Reload(idGraphicGroup);
                } else {
                    showAlert("Ocurrió un error eliminando el valor, verifique que no tenga datos asociados.");
                }
            });
        });
    };

    GraphicSubGroup.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    GraphicSubGroup.Save = function () {        
        var graphicSubGroup = getObject("formgraphicsubgroupcrud");

        if (formValidation("formgraphicsubgroupcrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'GraphicSubGroup/Validations',
                type: 'POST',
                data: graphicSubGroup,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("GraphicSubGroup/CreateOrUpdate", "Post", graphicSubGroup, "GraphicSubGroup.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
        
    };

    GraphicSubGroup.ResultSave = function (response) {
        hideLoading();        
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        GraphicSubGroup.Reload();
    };

    return GraphicSubGroup;
})();



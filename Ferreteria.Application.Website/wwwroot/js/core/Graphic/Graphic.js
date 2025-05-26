var columnsTable;
var filters;
var tables;
var selectColumn;
var queryBase = "SELECT {columns} FROM {tableName} ";

var Graphic = (function () {

    function Graphic() {

    }

    Graphic.GetListTablesWithType = function () {
        Petitions.Get($('#baseUrl').val() + 'Graphic/GetListTablesWithType', function (response) {
            tables = response;
        });
    };

    //Método encargado de abrir la ventana para editar
    Graphic.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de abrir la ventana para detalles
    Graphic.OpenGraphicDetails = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información
    Graphic.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    Graphic.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea inactivar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Graphic.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    Graphic.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Graphic.Save = function (urlValidations, urlSave) {


        if (formValidation("form-graphic-crud")) {
            GraphicIndicator.GetGraphicIndicatorObjects("tbDetail", "ListGraphicIndicator");
            var parameter = getObject("form-graphic-crud");

            var listGraphicIndicator = getObject("graphic-indicator *");

            if (!($("#UseAccount").is(":checked") || $("#ShowIndicator").is(":checked") || $("#TypeGraphic").val() == "Bigotes" || $("#TypeGraphic").val() == "Radar")) {
                if (listGraphicIndicator.length <= 1) {
                    $('#graphic-indicator-tab').click();
                    showAlert("Debe agregar al menos un indicador para poder guardar la gráfica.", 'danger');
                    return;
                }
            }


            showLoading("Validando...");
            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: urlValidations,
                type: 'POST',
                data: parameter,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax(urlSave, "Post", parameter, "Graphic.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del gráfico.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Graphic.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            setTimeout(function () {
                var baseUrl = $("#baseUrl").val();
                window.location.href = baseUrl + "Graphic";
            }, 1000);

        } else {
            showAlert(response.Message, 'danger');
        }
    };

    Graphic.OpenEdit = function (urlEdit, id) {
        var form = document.createElement("form");
        form.method = 'post';
        form.action = urlEdit;
        var input = document.createElement('input');
        input.type = "hidden";
        input.name = "id";
        input.value = id;
        form.appendChild(input);
        $('body').append(form);
        form.submit();
    };

    Graphic.GetColumnsEdit = function (tableName) {
        $.ajax({
            url: $('#baseUrl').val() + "Graphic/GetColumns",
            data: { tableName: tableName },
            method: 'GET',
            success: function (data) {
                columnsTable = data;
                hideLoading();
            }
        });
    };

    Graphic.SubGroups = function (data) {

        columnsTable = data;
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#GraphicSubGroupId').find('option').remove().end().val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        listitems += '<option value>' + 'Seleccione' + '</option>';
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Id + '>' + value.Name + '</option>';
        });
        $('#GraphicSubGroupId').append(listitems);
        //Graphic.DestroyMultiSelect();
        $('.select2').select2();
        CreateAllTooltips();
    };

    Graphic.SetGraphicPermissionSelection = function () {
        var split = $("#GraphicPermissions").val().split(',');

        for (var i = 0; i < split.length; i++) {
            $('#GraphicPermissionId option').each(function () {
                if ($(this).val() === split[i]) {
                    $(this).prop('selected', true);
                    return;
                }
            });
        };

        $('#GraphicPermissionId').multiselect('refresh');
    };

    Graphic.GetSelectedGraphicPermission = function () {
        var selectedOptions = $('#GraphicPermissionId option:selected');
        var ids = "";
        selectedOptions.each(function (index, element) {
            if (ids == "") {
                ids = element.value;
            }
            else {
                ids = ids + "," + element.value;
            }
        });

        $("#GraphicPermissions").val(ids);
        return ids;
    };

    Graphic.InitControls = function () {
        $("#GraphicGroupId").change(function () {
            Graphic.GetSubGroups($(this).val());
        });

        $(".inputPositiveInput").numeric({ negative: false, decimal: false });

        
        $("#TypeGraphic").change(function () {
            if ($(this).val() == "Bigotes" || $(this).val() == "Radar") {
                $('#graphic-indicator-tab').addClass('disabled');
                $('#graphic-indicator-tab').attr('aria-disabled', 'true');
                $('#graphic-indicator-tab').removeAttr('data-toggle');
                $('#graphic-indicator').removeClass('show active');
            } else {
                $('#graphic-indicator-tab').removeClass('disabled');
                $('#graphic-indicator-tab').attr('aria-disabled', 'false');
                $('#graphic-indicator-tab').attr('data-toggle', 'tab');
            }
        });


        $("#UseAccount").change(function () {
            if ($('#UseAccount').is(':checked') || $('#ShowIndicator').is(':checked')) {
                $('#graphic-indicator-tab').addClass('disabled');
                $('#graphic-indicator-tab').attr('aria-disabled', 'true');
                $('#graphic-indicator-tab').removeAttr('data-toggle');
                $('#graphic-indicator').removeClass('show active');
            } else {
                $('#graphic-indicator-tab').removeClass('disabled');
                $('#graphic-indicator-tab').attr('aria-disabled', 'false');
                $('#graphic-indicator-tab').attr('data-toggle', 'tab');
            }
        });

        $("#ShowIndicator").change(function () {
            if ($('#UseAccount').is(':checked') || $('#ShowIndicator').is(':checked')) {
                $('#graphic-indicator-tab').addClass('disabled');
                $('#graphic-indicator-tab').attr('aria-disabled', 'true');
                $('#graphic-indicator-tab').removeAttr('data-toggle');
                $('#graphic-indicator').removeClass('show active');
            } else {
                $('#graphic-indicator-tab').removeClass('disabled');
                $('#graphic-indicator-tab').attr('aria-disabled', 'false');
                $('#graphic-indicator-tab').attr('data-toggle', 'tab');
            }
        });
       
    };

    Graphic.GetSubGroups = function (groupId) {
        ExecuteAjax($('#baseUrl').val() + "Graphic/GetSubGroups", "GET", { groupId: groupId }, "Graphic.SubGroups", null);
    };

    Graphic.ExportData = function (url) {
        var fileName = 'Gráficas.xlsx';
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


    return Graphic;
})();


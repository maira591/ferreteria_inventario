var ReportParameter = (function () {
    function ReportParameter() {

    }

    //Método encargado de abrir la ventana para editar
    ReportParameter.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información
    ReportParameter.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    ReportParameter.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    ReportParameter.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    ReportParameter.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    ReportParameter.Save = function () {
        var result = formValidation("form-report-parameter-crud");
        var required = $("#SQLSentence").hasClass("required");
        
        if (required && result && $("#SQLSentence").val() == "" || $("#SQLSentence").val() == '') {

            $(".ace_scroller").css("border", "1px #dd3f3f solid");
            $(".ace_scroller").attr("data-errormessage", "Este campo es obligatorio");

            if ($("#DataType").val().toUpperCase() === "LISTA" && result) {
                showAlert("Hay inconsistencias en el formulario, revise los campos demarcados con color rojo.", "warning");
                return;
            }
            
        } else {
            $(".ace_scroller").css("border", "none");
        }
        var parameter = getObject("form-report-parameter-crud");

        if (result) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'ReportParameter/Validations',
                type: 'POST',
                data: parameter,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("ReportParameter/CreateOrUpdate", "Post", parameter, "ReportParameter.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del parametro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    ReportParameter.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        ReportParameter.Reload();
    };

    ReportParameter.ChangeDataType = function () {
        if ($("#DataType").val().toUpperCase() === "LISTA") {
            $(".tipoLista").show();
            $("#DataType").addClass("required");
            $("#SQLSentence").addClass("required");
        } else {
            $("#DataType").removeClass("required");
            $("#SQLSentence").removeClass("required");
            $("#SQLSentence").val(null);
            $(".tipoLista").fadeOut();
        }
    };

    ReportParameter.ExportData = function (url) {
        var fileName = 'Parámetros Reportes.xlsx';
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

    return ReportParameter;
})();
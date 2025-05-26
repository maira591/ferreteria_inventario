var columnsTable;
var filters;
var tables;
var selectColumn;
var queryBase = "SELECT {columns} FROM {tableName} ";

var Report = (function () {

    function Report() {

    }

    Report.GetListTablesWithType = function () {
        Petitions.Get($('#baseUrl').val() + 'Report/GetListTablesWithType', function (response) {
            tables = response;
        });
    };

    //Método encargado de abrir la ventana para editar
    Report.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de abrir la ventana para detalles
    Report.OpenReportDetails = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información
    Report.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    Report.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Report.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    Report.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Report.Save = function (urlValidations, urlSave) {
        Report.ValidateReportColoumnsSelection();

        $('#SQLSentence').val($('#SQLSentence').data('ace').editor.ace.getValue());
        if (!$("#IsOnlySQL").is(":checked")) {
            $("#ColumnReportId").addClass("required")
            $("#CodeTable").addClass("required")
            $("#IsOnlySQL").addClass("required")
        } else {
            $("#IsOnlySQL").removeClass("required");
            $("#ColumnReportId").removeClass("required")
            $("#CodeTable").removeClass("required")
        }

        if ($("#IsOnlySQL").is(":checked")) {
            $('#SQLSentence').val($('#SQLSentence').data('ace').editor.ace.getValue());
        } else {
            $('#SQLSentence').val(selectColumn);
        }

        var result = formValidation("form-report-crud");

        if ($('#SQLSentence').data('ace').editor.ace.getValue() == "") {
            $(".ace_scroller").css("border", "1px #dd3f3f solid");
            $(".ace_scroller").attr("data-errormessage", "Este campo es obligatorio");

            return;
        } else {
            $(".ace_scroller").css("border", "none");
        }

        if (!$("#IsOnlySQL").is(":checked")) {
            var filterquery = GetFiltersQuery();
            if (filterquery.length > 0) {
                $('#SQLSentence').val(selectColumn + filterquery)
            } else {
                $('#ReportParameters').val('')
            }
        }
    

        var parameter = getObject("form-report-crud");

        if (result) {
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
                        ExecuteAjax(urlSave, "Post", parameter, "Report.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del reporte.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Report.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            location.href = $("#baseUrl").val() + "Report"
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    Report.GetSelectedReportPermission = function () {
        var selectedOptions = $('#ReportPermissionId option:selected');
        var ids = "";
        selectedOptions.each(function (index, element) {
            if (ids == "") {
                ids = element.value;
            }
            else {
                ids = ids + "," + element.value;
            }
        });

        $("#ReportPermissions").val(ids);
        return ids;
    };

    Report.ValidateReportPermissionSelection = function () {
        return Report.GetSelectedReportPermission().length > 0;
    };

    Report.SetReportPermissionSelection = function () {
        var split = $("#ReportPermissions").val().split(',');

        for (var i = 0; i < split.length; i++) {
            $('#ReportPermissionId option').each(function () {
                if ($(this).val() === split[i]) {
                    $(this).prop('selected', true);
                    return;
                }
            });
        };

        $('#ReportPermissionId').multiselect('refresh');
    };


    Report.GetSelectedReportParameter = function (option, checked) {
        var ids = "";

        if ($("#ReportParameters").val() != "") {

            var elements = $("#ReportParameters").val().split(',')

            if (elements.find(x => x == option.val()) != undefined) {
                if (!checked) {
                    elements.splice($.inArray(option.val(), elements), 1);                    
                }
            }
            else {
                if (checked) {
                    elements.push(option.val())
                }
            }

            elements.forEach(function (element) {
                if (ids == "") {
                    ids = element;
                }
                else {
                    ids = ids + "," + element;
                }
            });

            $("#ReportParameters").val(ids);
        } else {
            $("#ReportParameters").val(option.val());
        }
    };

    Report.SetReportParameterSelection = function () {
        var split = $("#ReportParameters").val().split(',');

        for (var i = 0; i < split.length; i++) {
            $('#ReportParameterId option').each(function () {
                if ($(this).val() === split[i]) {
                    $(this).prop('selected', true);
                    return;
                }
            });
        };

        $('#ReportParameterId').multiselect('refresh');
    };

    Report.OpenEdit = function (urlEdit, id) {
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

    Report.SetReplaceKey = function (option, checked) {
        var idParameter = option.val();
        var parameter = parameters.filter(x => x.Id == idParameter)[0]

        if (checked) {
            $('#SQLSentence').val($('#SQLSentence').val() + parameter.ReplaceKey);
        } else {
            $('#SQLSentence').val($('#SQLSentence').val().replace(parameter.ReplaceKey, ''));
        }

        $('#SQLSentence').data('ace').editor.ace.setValue($('#SQLSentence').val());
    }

    Report.GetColumns = function (tableName) {
        if (tableName != '') {
            ExecuteAjax($('#baseUrl').val() + "Report/GetColumns", "GET", { tableName: tableName }, "Report.Columns", null);
        } else {
            $('#ColumnReportId').find('option').remove().end().val('');
            Report.DestroyMultiSelect();
        }

        if ($('#SQLSentence').val() != '') {
            $('#SQLSentence').val('')
            $('#SQLSentence').data('ace').editor.ace.setValue('');
        }
    };

    Report.GetColumnsEdit = function (tableName) {
        $.ajax({
            url: $('#baseUrl').val() + "Report/GetColumns",
            data: { tableName: tableName },
            method: 'GET',
            success: function (data) {
                columnsTable = data;
                hideLoading();
            }
        });
    };

    Report.Columns = function (data) {

        columnsTable = data;
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#ColumnReportId').find('option').remove().end().val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Id + '>' + value.Name + '</option>';
        });
        $('#ColumnReportId').append(listitems);
        Report.DestroyMultiSelect();
        $("#FormulaCont").html(FormulatorParameter.GetInitialParameters());
        $('.select2').select2();
        CreateAllTooltips();
    };

    Report.InitControls = function () {
        $("#CodeTable").change(function () {
            Report.GetColumns($(this).val());
        });
    };

    Report.ValidateReportColoumnsSelection = function () {
        return Report.GetSelectedReportColumns(true).length > 0;
    };

    Report.GetSelectedReportColumns = function (toSave) {

        var selectedOptions = $('#ColumnReportId option:selected');
        var ids = "";
        var idsToShow = "";

        selectedOptions.each(function (index, element) {
            if (ids == "") {
                idsToShow = `"${element.value}"`;
                ids = element.value
            }
            else {
                idsToShow += ", " + `"${element.value}"`;
                ids = ids + "," + element.value;
            }
        });

        var replacedQuery = queryBase.replace("{columns}", idsToShow);

        if ($("#CodeTable").val().includes(".")) {
            var parts = $("#CodeTable").val().split(".")
            replacedQuery = replacedQuery.replace("{tableName}", `"${parts[0]}"."${parts[1]}"`);
        } else {
            replacedQuery = replacedQuery.replace("{tableName}", `"${$('#CodeTable').val()}"`);
        }

        selectColumn = replacedQuery;
        $('#SQLSentence').val(replacedQuery);

        if (toSave == false) {
            if (selectedOptions.length == 0) {
                $('#SQLSentence').val('');
                $('#SQLSentence').data('ace').editor.ace.setValue('');
            } else {
                $('#SQLSentence').val(replacedQuery);
                $('#SQLSentence').data('ace').editor.ace.setValue(replacedQuery);
            }
        }

        $("#ReportColumns").val(ids);
        return ids;
    };

    Report.DestroyMultiSelect = function () {
        $("#ColumnReportId").multiselect("destroy").multiselect({
            includeSelectAllOption: false,
            enableCaseInsensitiveFiltering: true,
            filterPlaceholder: 'Buscar ...',
            maxHeight: 150,
            onChange: function (option, checked) {
                Report.GetSelectedReportColumns(false);
            },
            buttonTextAlignment: 'left',
            buttonWidth: '100%'
        });
    }

    Report.SetReportColumnSelection = function () {
        var split = $("#ReportColumns").val().split(',');
        for (var i = 0; i < split.length; i++) {
            $('#ColumnReportId option').each(function () {
                if ($(this).val() === split[i]) {
                    $(this).prop('selected', true);
                    return;
                }
            });
        };

        $('#ColumnReportId').multiselect('refresh');
    };

    Report.GetListParaterSelect = function () {
        var html = "";
        html += `<div class="col-sm-2"> <label class="col-form-label">Parámetros</label></div>`
        html += `<div class="col-sm-12">`
        html += `<select class="form-control" multiple="multiple" id="ReportParameterId">`;

        parameters.forEach(item => {
            html += `<option value="${item.Id}" ${($('#ReportParameters').val().includes(item.Id) ? "selected" : "")}>${item.Name}</option>`;
        })

        html += '</select>';
        html += `<div class="col-sm-12">`;

        $(".contParameters").html(html)

        $('#ReportParameterId').multiselect({
            includeSelectAllOption: false,
            enableCaseInsensitiveFiltering: true,
            filterPlaceholder: 'Buscar ...',
            maxHeight: 150,
            onChange: function (option, checked) {
                Report.GetSelectedReportParameter(option, checked);
                Report.SetReplaceKey(option, checked);
            },
            buttonTextAlignment: 'left',
            buttonWidth: '100%'
        });
    }

    Report.ChangeOnlySQL = function (element) {
        $('#SQLSentence').data('ace').editor.ace.setReadOnly(!$(element).is(":checked"));
        if (element.checked) {
            $('#ReportParameters').val('')
            Report.GetListParaterSelect()

            if ($("#CodeTable").val() != "") {
                Report.GetSelectedReportColumns();
                $('#SQLSentence').val(selectColumn);
                $('#SQLSentence').data('ace').editor.ace.setValue(selectColumn);
            }

            $(".contParameters").show();
            $("#tapParameters").addClass("disabledTab");
            $("#sql-tab").click()
        } else {
            $("#FormulaCont").html(FormulatorParameter.GetInitialParameters());
            $('.select2').select2();
            $(".contParameters").hide();
            $(".contParameters").html('')
            $("#formulator-tab").click()
            $("#tapParameters").removeClass("disabledTab");
        }
    }

    //Methodo para exportar las plantillas de los reportes a Excel
    Report.ExportData = function (url) {
        var fileName = 'Reportes.xlsx';
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

    return Report;
})();


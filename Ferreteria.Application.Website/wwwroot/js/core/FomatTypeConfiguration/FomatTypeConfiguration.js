var extensionsFormat;
var listIdsExtensionFormat = [];

var FomatTypeConfiguration = (function () {
    function FomatTypeConfiguration() {

    }

    FomatTypeConfiguration.OpenFomatTypeConfiguration = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };


    FomatTypeConfiguration.SaveFormatLine = function () {

        if (formValidation("formformarextension")) {
            if (FomatTypeConfiguration.Validation()) {
                FomatTypeConfiguration.GetFormatLineObjects("tbDetail", "listFormatLine")

                var formatLineList = getObject("formformarextension *");

                if (formatLineList.length <= 1) {
                    showAlert("No hay datos para guardar.", 'danger');
                } else {
                    ExecuteAjax("FomatTypeConfiguration/SaveFormatLine", "Post", formatLineList, "FomatTypeConfiguration.ResultSave");
                }
            }
        }
    };

    FomatTypeConfiguration.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    //Load the format line data in the table.
    FomatTypeConfiguration.InitForm = function (formatLineList, extensionsFormatList) {
        extensionsFormat = extensionsFormatList
        var FormatId = FomatTypeConfiguration.GetFormatId();

        $("#btnAddRecord").click(function () {
            var count = $("#tbDetail tbody tr").length + 1;
            var dropDownCaptureUnit = "";
            $.each(extensionsFormatList, function (key, value) {
                dropDownCaptureUnit += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
            });

            var tableRow = "<tr>" +
                "<td><select style='width:400px;' class='form-control required formatExtension select2' id='IdCaptureUnit_" + count + "' name='FomatTypeConfiguration.FormatExtensionTypeId' ><option value=''>Seleccione</option>" + dropDownCaptureUnit + "</select></td>" +
                "<td><input type='text' style='width:200px;'maxlength='2' id='NumeroFila_" + count + "' class='form-control required inputPositiveInput' name='FomatTypeConfiguration.RowNumber' /></input></td>" +
                "<td><input type='text' style='width:300px;resize:none;' rows='1' id='ColumnaExcel_" + count + "' name='FomatTypeConfiguration.ExcelColumn' maxlength='2' class='form-control required'></input></td>" +
                "<td><a href='javascript:void(0)' onclick='FomatTypeConfiguration.RemoveRow(this)' data-tippy-content='Remover Registro'><i class='fa fa-trash'></i></a> <input type='hidden' id='FormatId_" + count + "' name='FomatTypeConfiguration.FormatId' value='" + FormatId + "'</input></td > " +
                "</tr>";
            $("#tbDetail tbody").append(tableRow);

            CreateAllTooltips();
            setNumericInput();
            setSelect2();
        });


        $.each(formatLineList, function (key, item) {
            var tableRow = "";
            var count = $("#tbDetail tbody tr").length + 1;
            var dropDownCaptureUnit = "";

            $.each(extensionsFormatList, function (key, value) {
                if (value.Value === item.FormatExtensionTypeId) {

                    dropDownCaptureUnit += '<option value=' + value.Value + ' data-id=' + value.Value + ' selected="selected"' + ' >' + value.Text + '</option>';
                }
                else {
                    dropDownCaptureUnit += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
                }
            });

            tableRow = "<tr>" +
                "<td><select style='width:400px;' class='form-control required formatExtension select2' id='IdCaptureUnit_" + count + "' name='FomatTypeConfiguration.FormatExtensionTypeId' ><option value=''>Seleccione</option>" + dropDownCaptureUnit + "</select></td>" +
                "<td><input type='text' style='width:200px;' maxlength='2' id='NumeroFila_" + count + "' class='form-control required inputPositiveInput' name='FomatTypeConfiguration.RowNumber' value=" + item.RowNumber + "></input></td>" +
                "<td><input type='text' style='width:300px;resize:none;' id='ColumnaExcel_" + count + "' rows='1' name='FomatTypeConfiguration.ExcelColumn' maxlength='2' class='form-control required inputPositiveInput' value='" + item.ExcelColumn + "'></input></td>" +
                "<td><a href='javascript:void(0)' onclick='FomatTypeConfiguration.RemoveRow(this)' data-tippy-content='Remover Registro'><i class='fa fa-trash'></i></a> <input type='hidden' id='FormatId_" + count + "' name='FomatTypeConfiguration.FormatId' value='" + FormatId + "'></input></td > " +
                "<td style='display:none;'>" + "<input type='hidden' id='Id_" + count + "' name='FomatTypeConfiguration.Id' value='" + item.Id + "'></input>" + "</td>" +
                "</tr>";
            $("#tbDetail tbody").append(tableRow);
        });
    };

    //Show confirm to remove row.
    FomatTypeConfiguration.RemoveRow = function (row) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            HideModalQuestion();
            FomatTypeConfiguration.RemoveFormatLine(row)
        });
    };


    //Remove the selected row from the table.
    FomatTypeConfiguration.RemoveFormatLine = function (element) {
        var tbody = $(element).parent().parent().parent();
        $(element).parent().parent().remove();

        var IdFormatLine = $(element).parent().parent().find("input[name*='FomatTypeConfiguration.Id']").val();

        ExecuteAjax("FomatTypeConfiguration/Delete", "GET", { id: IdFormatLine }, "FomatTypeConfiguration.SuccessDelete", null);

    };

    FomatTypeConfiguration.SuccessDelete = function (response) {
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message);
        }
    };

    //Create the correct structure to get json object to send and to save.
    FomatTypeConfiguration.GetFormatLineObjects = function (table, propertyname) {
        var countRows = 0;
        var FieldName = "";

        $.each($("#" + table + " tbody tr"), function (i, tr) {
            var countColumns = 0;
            $.each($(tr).find("td"), function (j, td) {
                var input = $(td).find("input");

                var select = $(td).find("select");

                var textarea = $(td).find("textarea");

                if (input.length > 0) {
                    FieldName = $(input[0]).attr("name");
                    var split = FieldName.split('.');
                    $(input[0]).attr("name", propertyname + "[" + countRows + "]." + split[(split.length - 1)]);
                }

                if (select.length > 0) {
                    FieldName = $(select[0]).attr("name");
                    var split = FieldName.split('.');
                    $(select[0]).attr("name", propertyname + "[" + countRows + "]." + split[(split.length - 1)]);
                }

                if (textarea.length > 0) {
                    FieldName = $(textarea[0]).attr("name");
                    var split = FieldName.split('.');
                    $(textarea[0]).attr("name", propertyname + "[" + countRows + "]." + split[(split.length - 1)]);
                }

                countColumns++;
            });
            countRows++;
        });
    };

    //Validates that the capture unit and the row are not repeated in a record.
    FomatTypeConfiguration.Validation = function () {
        var save = true;
        var counter = 0;

        $(".formatExtension").each(function (index, element) {
            counter = 0;
            $(element).removeClass("errorValidate");
            $(element).attr("data-errormessage", "");

            var objFormatExtensionType = $(element).parent().parent().find("select[name*='FormatExtensionTypeId']");
            var objCode = $(element).parent().parent().find("input[name*='RowNumber']");

            $(".formatExtension").each(function (index, elementvalidate) {
                var objFormatExtensionTypeValidate = $(elementvalidate).parent().parent().find("select[name*='FormatExtensionTypeId']");
                var objRowNumberValidate = $(elementvalidate).parent().parent().find("input[name*='RowNumber']");

                if (objFormatExtensionType.val() === objFormatExtensionTypeValidate.val() && objCode.val() === objRowNumberValidate.val()) {
                    counter = counter + 1
                }
                if (counter > 1) {
                    var objFormatExtension = $(objFormatExtensionTypeValidate).next().children().eq(0).children().eq(0);
                    $(objFormatExtension).addClass("errorValidate");
                    $(objFormatExtension).attr("data-errormessage", "Ya hay una extensión igual seleccionada.");


                    save = false;
                }
            });

        });

        if (!save) {
            showAlert("Hay inconsistencias en el formulario, revise los campos demarcados con color rojo.", 'danger');
            showTooltip();
        } else {
            $(".tooltipError").hide();
            removeTooltip();
        }

        return save;
    };

    FomatTypeConfiguration.GetFormatId = function () {
        return $("#hdfFormatId").val();
    }

    return FomatTypeConfiguration;
})();

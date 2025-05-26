var FormatLine = (function () {
    function FormatLine() {

    }

    FormatLine.OpenFormatLine = function (urlFormatLine) {
        Petitions.Get(urlFormatLine, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };


    FormatLine.SaveFormatLine = function () {

        if (formValidation("formformatline")) {
            if (FormatLine.Validation()) {
                FormatLine.GetFormatLineObjects("tbDetail", "listFormatLine")

                var formatLineList = getObject("formformatline *");

                if (formatLineList.length <= 1) {
                    showAlert("No hay datos para guardar.", 'danger');
                } else {
                    ExecuteAjax("FormatLine/SaveFormatLine", "Post", formatLineList, "FormatLine.ResultSave");
                }               
            }
        }
    };

    FormatLine.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();            
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    //Load the format line data in the table.
    FormatLine.InitForm = function (formatLineList, captureUnitList) {

        var FormatId = FormatLine.GetFormatId();

        $("#btnAddRecord").click(function () {
            var count = $("#tbDetail tbody tr").length + 1;
            var dropDownCaptureUnit = "";
            $.each(captureUnitList, function (key, value) {
                dropDownCaptureUnit += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
            });

            var tableRow = "<tr>" +
                "<td><select style='width:200px;' class='form-control required captureunit select2' id='IdCaptureUnit_" + count + "' name='FormatLine.CaptureUnitId' ><option value=''>Seleccione</option>" + dropDownCaptureUnit + "</select></td>" +
                "<td><input type='text' style='width:200px;'maxlength='5' id='CodigoRenglon_" + count + "' class='form-control required inputPositiveInput' name='FormatLine.Code' /></input></td>" +
                "<td><input type='text' style='width:300px;resize:none;' rows='1' name='FormatLine.Description' maxlength='250' class='form-control required'></input></td>" +
                "<td><a href='javascript:void(0)' onclick='FormatLine.RemoveRow(this)' data-tippy-content='Remover Registro'><i class='fa fa-trash'></i></a> <input type='hidden' id='FormatId_" + count + "' name='FormatLine.FormatId' value='" + FormatId + "'</input></td > " +
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
            
            $.each(captureUnitList, function (key, value) {
                if (value.Value === item.CaptureUnitId) {
                    dropDownCaptureUnit += '<option value=' + value.Value + ' data-id=' + value.Value + ' selected="selected"' + ' >' + value.Text + '</option>';
                }
                else {
                    dropDownCaptureUnit += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
                }                
            });

            tableRow = "<tr>" +
                "<td><select style='width:200px;' class='form-control required captureunit select2' id='IdCaptureUnit_" + count + "' name='FormatLine.CaptureUnitId' ><option value=''>Seleccione</option>" + dropDownCaptureUnit + "</select></td>" +
                "<td><input type='text' style='width:200px;' maxlength='2' id='CodigoRenglon_" + count + "' class='form-control required inputPositiveInput' name='FormatLine.Code' value=" + item.Code + "></input></td>" +
                "<td><input type='text' style='width:300px;resize:none;' rows='1' name='FormatLine.Description' maxlength='250' class='form-control required' value='" + item.Description + "'></input></td>" +
                "<td><a href='javascript:void(0)' onclick='FormatLine.RemoveRow(this)' data-tippy-content='Remover Registro'><i class='fa fa-trash'></i></a> <input type='hidden' id='FormatId_" + count + "' name='FormatLine.FormatId' value='" + FormatId + "'></input></td > " +
                "<td style='display:none;'>" + "<input type='hidden' id='Id_" + count + "' name='FormatLine.Id' value='" + item.Id + "'></input>" + "</td>" + 
                "</tr>";

            $("#tbDetail tbody").append(tableRow);
        });


    };

    //Show confirm to remove row.
    FormatLine.RemoveRow = function (row) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            HideModalQuestion();
            FormatLine.RemoveFormatLine(row)
        });
    };

    //Remove the selected row from the table.
    FormatLine.RemoveFormatLine = function (element) {
        var tbody = $(element).parent().parent().parent();
        $(element).parent().parent().remove();

        var IdFormatLine = $(element).parent().parent().find("input[name*='FormatLine.Id']").val();

        ExecuteAjax("FormatLine/Delete", "GET", { id: IdFormatLine }, "FormatLine.SuccessDelete", null);

    };

    FormatLine.SuccessDelete = function (response) {
        if (response.Success) {
            showAlert(response.Message, 'success');            
        } else {
            showAlert(response.Message);
        }
    };

    //Create the correct structure to get json object to send and to save.
    FormatLine.GetFormatLineObjects = function (table, propertyname) {
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
    FormatLine.Validation = function () {
        var save  = true;
        var counter = 0;

        $(".captureunit").each(function (index, element) {
            counter = 0;
            $(element).removeClass("errorValidate");
            $(element).attr("data-errormessage", "");

            var objFormatLine = $(element).parent().parent().find("select[name*='CaptureUnitId']");
            var objCode = $(element).parent().parent().find("input[name*='Code']");

            $(".captureunit").each(function (index, elementvalidate) {
                var objFormatLineValidate = $(elementvalidate).parent().parent().find("select[name*='CaptureUnitId']");
                var objCodeValidate = $(elementvalidate).parent().parent().find("input[name*='Code']");

                if (objFormatLine.val() === objFormatLineValidate.val() && objCode.val() === objCodeValidate.val()) {
                    counter = counter + 1
                }
                if (counter > 1) {
                    $(objFormatLineValidate).addClass("errorValidate");
                    $(objFormatLineValidate).attr("data-errormessage", "Unidad de captura y código renglón se encuentran repetidos.");

                    $(objCodeValidate).addClass("errorValidate");
                    $(objCodeValidate).attr("data-errormessage", "Unidad de captura y código renglón se encuentran repetidos.");

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
        
    FormatLine.GetFormatId = function ()
    {
        return $("#hdfFormatId").val();
    }

    return FormatLine;
})();

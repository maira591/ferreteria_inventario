var GraphicIndicator = (function () {
    function GraphicIndicator() {

    }



    GraphicIndicator.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    GraphicIndicator.GetNextInputId = function () {

        var lastBodyTr = $("#tbDetail tbody tr:last td").find('select').attr('id');

        if (lastBodyTr) {

            return parseInt(lastBodyTr.split('_')[1]) + 1;

        } else {
            return $("#tbDetail tbody tr").length + 1;
        }

    };
    
    GraphicIndicator.InitForm = function (graphicIndicatorList, indicatorList, typeFieldList, typeEjeList, indicatorAggregateList, signIndicatorList) {

        var booleanValues = ["true", "false"];
        var GraphicId = GraphicIndicator.GetGraphicId();
        $("#btnAddRecord").click(function () {
            var count = GraphicIndicator.GetNextInputId();

            var dropDownIndicatorList = "";
            var dropDownTypeFieldList = "";
            var dropDownTypeEjeList = "";
            var dropDownIndicatorAggregateList = "";
            var dropDownSignIndicatorList = "";

            $.each(indicatorList, function (key, value) {
                dropDownIndicatorList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
            });

            $.each(typeFieldList, function (key, value) {
                dropDownTypeFieldList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
            });

            $.each(typeEjeList, function (key, value) {
                dropDownTypeEjeList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
            });

            $.each(indicatorAggregateList, function (key, value) {
                dropDownIndicatorAggregateList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
            });

            $.each(signIndicatorList, function (key, value) {
                dropDownSignIndicatorList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
            });

            var tableRow = "<tr>" +
                "<td><input type='text' style='width:70px;'maxlength='3' id='Order_" + count + "' class='form-control required inputPositiveInput' name='ListGraphicIndicator.Order' /></input></td>" +
                "<td><select style='width:200px;' class='form-control required indicatorid select2' id='IdIndicator_" + count + "' name='ListGraphicIndicator.IndicatorId' ><option value=''>Seleccione</option>" + dropDownIndicatorList + "</select></td>" +
                "<td><select style='width:60px;' class='form-control required captureunit select2' id='TypeEje_" + count + "' name='ListGraphicIndicator.Axis' ><option value=''>Seleccione</option>" + dropDownTypeEjeList + "</select></td>" +
                "<td><select style='width:100%;' class='form-control required captureunit select2' id='TypeField_" + count + "' name='ListGraphicIndicator.TypeField' ><option value=''>Seleccione</option>" + dropDownTypeFieldList + "</select></td>" +
                "<td><input type='text' style='width:100%;'maxlength='60' id='Legend_" + count + "' class='form-control required' name='ListGraphicIndicator.DisplayText' /></input></td>" +
                "<td><input type='color' style='width:70px;'maxlength='60' id='Color_" + count + "' class='form-control required' name='ListGraphicIndicator.Color' /></input></td>" +
                "<td><select style='width:100px;' class='form-control captureunit select2' id='Sign_" + count + "' name='ListGraphicIndicator.Sign' ><option value=''>Seleccione</option>" + dropDownSignIndicatorList + "</select></td>" +
                "<td><select style='width:200px;' class='form-control captureunit select2' id='IndicatorAggregateId_" + count + "' name='ListGraphicIndicator.IndicatorAggregateId' ><option value=''>Seleccione</option>" + dropDownIndicatorAggregateList + "</select></td>" +
                "<td><a href='javascript:void(0)' onclick='GraphicIndicator.RemoveRow(this)' data-tippy-content='Remover Registro'><i class='fa fa-trash'></i></a> <input type='hidden' id='GraphicId_" + count + "' name='ListGraphicIndicator.GraphicId' value='" + GraphicId + "'</input></td > " +
                "</tr>";
            $("#tbDetail tbody").append(tableRow);

            CreateAllTooltips();
            setNumericInput();
            $('.select2').select2();
        });

        $.each(graphicIndicatorList, function (key, item) {
            var tableRow = "";
            var count = $("#tbDetail tbody tr").length + 1;
            var dropDownIndicatorList = "";
            var dropDownTypeEjeList = "";
            var dropDownTypeFieldList = "";
            var dropDownIndicatorAggregateList = "";
            var dropDownSignIndicatorList = "";

            $.each(indicatorList, function (key, value) {
                if (value.Value === item.IndicatorId) {
                    dropDownIndicatorList += '<option value=' + value.Value + ' data-id=' + value.Value + ' selected="selected"' + ' >' + value.Text + '</option>';
                }
                else {
                    dropDownIndicatorList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
                }
            });

            $.each(typeFieldList, function (key, value) {
                if (value.Value === item.TypeField) {
                    dropDownTypeFieldList += '<option value=' + value.Value + ' data-id=' + value.Value + ' selected="selected"' + ' >' + value.Text + '</option>';
                }
                else {
                    dropDownTypeFieldList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
                }
            });

            $.each(typeEjeList, function (key, value) {
                if (value.Value === item.Axis) {
                    dropDownTypeEjeList += '<option value=' + value.Value + ' data-id=' + value.Value + ' selected="selected"' + ' >' + value.Text + '</option>';
                }
                else {
                    dropDownTypeEjeList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
                }
            });

            $.each(indicatorAggregateList, function (key, value) {
                if (value.Value === item.IndicatorAggregateId) {
                    dropDownIndicatorAggregateList += '<option value=' + value.Value + ' data-id=' + value.Value + ' selected="selected"' + ' >' + value.Text + '</option>';
                }
                else {
                    dropDownIndicatorAggregateList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
                }
            });
            
            $.each(signIndicatorList, function (key, value) {
                if (value.Value === item.Sign) {
                    dropDownSignIndicatorList += '<option value=' + value.Value + ' data-id=' + value.Value + ' selected="selected"' + ' >' + value.Text + '</option>';
                }
                else {
                    dropDownSignIndicatorList += '<option value=' + value.Value + ' data-id=' + value.Value + ' >' + value.Text + '</option>';
                }
            });


            var tableRow = "<tr>" +
                "<td><input type='text' style='width:70px;'maxlength='3' id='Order_" + count + "' class='form-control required inputPositiveInput' name='ListGraphicIndicator.Order' value='" + item.Order +"'/></input></td>" +
                "<td><select style='width:200px;' class='form-control required indicatorid select2' id='IdIndicator_" + count + "' name='ListGraphicIndicator.IndicatorId' ><option value=''>Seleccione</option>" + dropDownIndicatorList + "</select></td>" +
                "<td><select style='width:60px;' class='form-control required captureunit select2' id='TypeEje_" + count + "' name='ListGraphicIndicator.Axis' ><option value=''>Seleccione</option>" + dropDownTypeEjeList + "</select></td>" +
                "<td><select style='width:100px;' class='form-control required captureunit select2' id='TypeField_" + count + "' name='ListGraphicIndicator.TypeField' ><option value=''>Seleccione</option>" + dropDownTypeFieldList + "</select></td>" +
                "<td><input type='text' style='width:200px;'maxlength='60' id='Legend_" + count + "' class='form-control required' name='ListGraphicIndicator.DisplayText' value='" + item.DisplayText +"'/></input></td>" +
                "<td><input type='color' style='width:70px;'maxlength='5' id='Color_" + count + "' class='form-control required' name='ListGraphicIndicator.Color' value='" + item.Color +"'/></input></td>" +
                "<td><select style='width:100px;' class='form-control captureunit select2' id='Sign_" + count + "' name='ListGraphicIndicator.Sign' ><option value=''>Seleccione</option>" + dropDownSignIndicatorList + "</select></td>" +
                "<td><select style='width:200px;' class='form-control captureunit select2' id='IndicatorAggregateId_" + count + "' name='ListGraphicIndicator.IndicatorAggregateId' ><option value=''>Seleccione</option>" + dropDownIndicatorAggregateList  + "</select></td>" +
                "<td><a href='javascript:void(0)' onclick='GraphicIndicator.RemoveRow(this)' data-tippy-content='Remover Registro'><i class='fa fa-trash'></i></a> <input type='hidden' id='GraphicId_" + count + "' name='ListGraphicIndicator.GraphicId' value='" + GraphicId + "'</input></td > " +
                "<td style='display:none;'>" + "<input type='hidden' id='Id_" + count + "' name='ListGraphicIndicator.Id' value='" + item.Id + "'></input>" + "</td>" + 
                "</tr>";

            $(".inputPositiveInput").numeric({ negative: false, decimal: false });
            $("#tbDetail tbody").append(tableRow);
            CreateAllTooltips();
            setNumericInput();
            $('.select2').select2();
        });


    };

    //Show confirm to remove row.
    GraphicIndicator.RemoveRow = function (row) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            HideModalQuestion();
            GraphicIndicator.RemoveGraphicIndicator(row)
        });
    };

    //Remove the selected row from the table.
    GraphicIndicator.RemoveGraphicIndicator = function (element) {
        var tbody = $(element).parent().parent().parent();
        $(element).parent().parent().remove();

        var IdGraphicIndicator = $(element).parent().parent().find("input[name*='ListGraphicIndicator.Id']").val();

        ExecuteAjax("DeleteIndicator", "GET", { id: IdGraphicIndicator }, "GraphicIndicator.SuccessDelete", null);

    };

    GraphicIndicator.SuccessDelete = function (response) {
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message);
        }
    };

    //Create the correct structure to get json object to send and to save.
    GraphicIndicator.GetGraphicIndicatorObjects = function (table, propertyname) {
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

    GraphicIndicator.GetGraphicId = function () {
        return $("#hdfGraphicId").val();
    }

    return GraphicIndicator;
})();

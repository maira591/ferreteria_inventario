var FormatColumn = (function () {
    function FormatColumn() {

    }

    //Método encargado de abrir la ventana para editar
    FormatColumn.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            if ($("#TipoDato").val().toUpperCase() === "NUMBER") {
                $("#LongitudDato").removeClass("inputPositiveInput");
                $("#LongitudDato").addClass("inputPositiveInput2d");
            } else if ($("#TipoDato").val().toUpperCase() === "DATE") {
                $("#LongitudDato").removeClass("inputPositiveInput2d");
                $("#LongitudDato").addClass("inputPositiveInput");                
            } else if ($("#TipoDato").val().toUpperCase() === "VARCHAR2") {
                $("#LongitudDato").removeClass("inputPositiveInput2d");
                $("#LongitudDato").addClass("inputPositiveInput");
            }
            setNumericInput();

            
            if ($("#IsCalculated").is(':checked')) {
                $("#divFormula").show();
                $("#Formula").addClass("required");
            }

        });
    };

    //Método encargado de recargar la información de las columnas del formato
    FormatColumn.Reload = function (idFormat) {
        var formatId = "";
        if (idFormat !== undefined) {
            formatId = idFormat;
        }
        else {
            formatId = $("#FormatId").val();
        }

        var urlReload = $("#urlFormatColumns").val().replace('0', formatId);

        Petitions.Get(urlReload, function (response) {
            $(`#divFormatColumns_${formatId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de realizar el borrado de un cátalogo.
    FormatColumn.DeleteColumn = function (url, idFormat) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar esta columna del formato?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    FormatColumn.Reload(idFormat);
                } else {
                    showAlert("Ocurrió un error eliminando el columna del formato, verifique que no tenga datos asociados.");
                }
            });
        });
    };

    FormatColumn.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    FormatColumn.ChangeDataType = function () {
        $("#LongitudDato").val("");
        if ($("#TipoDato").val().toUpperCase() === "NUMBER") {
            $("#LongitudDato").removeClass("inputPositiveInput");
            $("#LongitudDato").addClass("inputPositiveInput2d");
        } else if ($("#TipoDato").val().toUpperCase() === "DATE")
        {
            $("#LongitudDato").removeClass("inputPositiveInput2d");
            $("#LongitudDato").addClass("inputPositiveInput");
            $("#LongitudDato").val("10");
        } else if ($("#TipoDato").val().toUpperCase() === "VARCHAR2")
        {
            $("#LongitudDato").removeClass("inputPositiveInput2d");
            $("#LongitudDato").addClass("inputPositiveInput");
        }
        setNumericInput();
    };


    FormatColumn.SaveFormatColumn = function () {        
        var formatColumn = getObject("formformatcolumncrud");

        if (formValidation("formformatcolumncrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'FormatColumn/Validations',
                type: 'POST',
                data: formatColumn,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("FormatColumn/CreateOrUpdate", "Post", formatColumn, "FormatColumn.ResultSaveFormatColumn");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación de la columna.", 'danger');
                    hideLoading();
                }
            });
        }
        
    };

    FormatColumn.ResultSaveFormatColumn = function (response) {
        hideLoading();        
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        FormatColumn.Reload();
    };

    FormatColumn.SetEventIsCalculate = function () {
        $("#IsCalculated").change(function () {

            if (this.checked) { 
                $("#divFormula").show();
                $("#Formula").addClass("required");
            }
            else {
                $("#divFormula").hide();
                $("#Formula").val('');
                $("#Formula").removeClass("required");
            }
       }); 

    };  

    return FormatColumn;
})();



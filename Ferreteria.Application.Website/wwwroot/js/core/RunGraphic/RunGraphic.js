var elementsGraphic = "";
var indicatorsInfo;
var showIndicatorUseAccount = false;

var callbackObject =
{
    callbacks: {
        label: function (context) {
            let chartType = context.chart.config.type;
            let label = context.dataset.label || '';
            if (chartType == 'boxplot') {

                const mean = context.parsed.mean
                const boxplotValues = [
                    `Valor maxímo: ${context.parsed.whiskerMax}`,
                    `Q3 (75% de los datos): ${context.parsed.q3}`,
                    `Mediana (50% de los datos): ${context.parsed.median}`,
                    `Punto promedio: ${mean}`,
                    `Q1 (25% de los datos): ${context.parsed.q1}`,
                    `Valor mínimo: ${context.parsed.whiskerMin}`
                ];
                return boxplotValues;

            } else if (chartType == 'bar' || chartType == 'line') {

                let graphicTitle = context.chart.config.options.plugins.title.text;
                let siymbol = RunGraphic.GetSimbolBySigla(graphicTitle, label);

                if (context.parsed.y.toString().length > 5 && (siymbol == '' || siymbol == null) && showIndicatorUseAccount) {
                    siymbol = "$";
                }

                if (context.parsed.y !== null) {
                    if (siymbol == "$") {
                        label += ": " + siymbol + "" + new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP' }).format(context.parsed.y).replace('$', '');

                    } else if (siymbol == "%") {
                        label += ": " + new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP' }).format(context.parsed.y).replace('$', '') + siymbol;
                    } else if (siymbol == "#") {
                        label += ": " + new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP' }).format(context.parsed.y).replace('$', '').split(",")[0];
                    }
                    else {
                        label += ": " + new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP' }).format(context.parsed.y).replace('$', '');
                    }
                }
                return label;
            }
        }
    }
}

var RunGraphic = (function () {

    var callbackObjectYAxis =
    {
        callback: function (value, index, values) {
            if (value.toString().length > 3) {
                return new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP' }).format(value).replace('$', '').replace(',00', '')
            } else {
                return value;
            }
        }
    }


    function RunGraphic() {

    }

    RunGraphic.Init = function () {

        $('#StartDate').mask('00/00/0000', { placeholder: "__/__/____" });
        $('#EndDate').mask('00/00/0000', { placeholder: "__/__/____" });


        var options = { language: 'esp', format: "dd/mm/yyyy" };
        $('.dp-date input').datepicker(options);

        $('.select2').select2();

        $("#GraphicGroupId").change(function () {
            RunGraphic.GetSubGroups($(this).val());
            $("#container-graphics").html('');
            $("#container-indicators").html('');
            $("#IndicatorsSiglas").val(null);
        });

        $("#GraphicSubGroupId").change(function () {
            $("#container-graphics").html('');
            $("#container-indicators").html('');
            $("#IndicatorsSiglas").val(null)
        });

        $("#btnRunGraphicThatUseIndicators").click(function () {
            $("#btnConsultar").click();
        });

        if ($("#validateCoop").val() == "S") {
            $("#EntityType").change(function () {
                RunGraphic.GetEntities($(this).val());
            });
        }
    };

    RunGraphic.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("RunGraphic/GetEntityList", "GET", { entityType: entityTypeId }, "RunGraphic.LoadEntities", null);
        } else {
            $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };


    RunGraphic.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };

    RunGraphic.GetSubGroups = function (groupId) {
        ExecuteAjax($('#baseUrl').val() + "RunGraphic/GetSubGroups", "GET", { groupId: groupId }, "RunGraphic.SubGroups", null);
    };

    RunGraphic.ExcecuteGraphics = function (urlSave) {
        if (createDate($('#StartDate').val()).getTime() > createDate($('#EndDate').val()).getTime()) {
            showAlert('La fecha de inicio no puede ser mayor que la fecha de fin', 'danger');
            return;
        }

        if (formValidation("paramsForm")) {
            var parameter = getObject("paramsForm");
            showLoading("Generando gráficas...");
            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: urlSave,
                type: 'POST',
                data: parameter,
                success: function (data) {
                    if (data.Valid != undefined) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Cargando...");
                        RunGraphic.RenderGraphics(data);
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue ejecutar las gráficas, comuniquese con el administrador del sistema.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    RunGraphic.SubGroups = function (data) {

        columnsTable = data;
        $('#GraphicSubGroupId').find('option').remove().end().val('');

        var listitems = "";
        listitems += '<option value>' + 'Seleccione' + '</option>';
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Id + '>' + value.Name + '</option>';
        });
        $('#GraphicSubGroupId').append(listitems);

        $('.select2').select2();
        CreateAllTooltips();
    };


    RunGraphic.RenderGraphics = function (graphics) {
        $("#container-graphics").html('');
        showLoading();

        if (graphics.ListResponseCharJs.length > 0) {
            indicatorsInfo = JSON.parse(graphics.ListIndicatorGraphicInfoToTooltip);

            for (var i = 0; i < graphics.ListResponseCharJs.length; i++) {
                var graphicId = "graphic_" + i;
                RunGraphic.GenerateGraphic(graphicId, graphics.ListResponseCharJs[i]);
            }
        } else {
            $("#container-indicators").html('');

            if (graphics.ShowIndicator || graphics.UseAccount) {
                showIndicatorUseAccount = true;
                RunGraphic.GenerateIndicatorGraphic(JSON.parse(graphics.ListIndicatorGraphic), JSON.parse(graphics.ListAccountGraphic), graphics.ShowIndicator, graphics.UseAccount);
            }
            else {
                showAlert("Este grupo o subgrupo no contiene ninguna gráfica parametrizada.", "warning")
            }

        }
        CreateAllTooltips();
        hideLoading();
    };

    RunGraphic.GenerateIndicatorGraphic = function (dataIndicators, dataAccount, showIndicator, useAccount) {
        var contentHtml = "";

        if (showIndicator) {
            contentHtml = contentHtml + `<p>Seleccione los indicadores a gráficar:</p> <p>Se recomienda seleccionar un máximo de 8 variables para una mejor lectura de la información</p><select class="form-control multiple required" id="select-indicators" name="select-indicators" multiple = "multiple">`;
        } else if (useAccount) {
            contentHtml = contentHtml + `<p>Seleccione las cuentas a gráficar:</p><select class="form-control multiple required" id="select-indicators" name="select-indicators" multiple = "multiple">`;
        }

        if (showIndicator) {
            for (var i = 0; i < dataIndicators.length; i++) {
                contentHtml = contentHtml + `<option value="${dataIndicators[i].Sigla}">${dataIndicators[i].ValueCatalogGroupIndicator.Name} - ${dataIndicators[i].Name}</option>`
            }
        } else if (useAccount) {
            for (var i = 0; i < dataAccount.length; i++) {
                contentHtml = contentHtml + `<option value="${dataAccount[i].CuentaTxt}">${dataAccount[i].CuentaTxt} - ${dataAccount[i].NombreCuenta}</option>`
            }
        }


        contentHtml = contentHtml + `</select>
        </br>
        </br>
        <div>
               <button class="btn btn-success" data-tippy-content="Pulse para gráficar" onclick="$('#btnConsultar').click();"> 
               <span>Consultar</span>
               </button>
            </div>
        `;

        $("#container-indicators").html(contentHtml);


        $('#select-indicators').multiselect({
            includeSelectAllOption: false,
            enableCaseInsensitiveFiltering: true,
            filterPlaceholder: 'Buscar ...',
            maxHeight: 180,
            onChange: function (option, checked) {
                RunGraphic.GetSelectedIndicators();
            },
            buttonTextAlignment: 'left',
            buttonWidth: '100%'
        });
        CreateAllTooltips();
    }


    RunGraphic.GetSelectedIndicators = function () {
        var selectedOptions = $('#select-indicators option:selected');
        var ids = "";
        selectedOptions.each(function (index, element) {
            if (ids == "") {
                ids = element.value;
            }
            else {
                ids = ids + "," + element.value;
            }
        });
        $("#IndicatorsSiglas").val(ids);
        return ids;
    };

    RunGraphic.GenerateGraphic = function (canvasId, jsonData) {
        var contentButtonExport = "";
        var contentButtonExportComplementary = "";
        var inforGrapchi = JSON.parse(jsonData.Graphic)
        RunGraphic.UnSetGraphic(canvasId);
        var dataInfoTable = JSON.parse(jsonData.DataSource);

        if (inforGrapchi.IsDownloadable && dataInfoTable.length > 0) {
            contentButtonExport = `
            <div>
               <button onclick="RunGraphic.ExportToExcel('dataTable_${canvasId}', 'xlsx', 'Gráfica - ${inforGrapchi.Title.toUpperCase()}')" class="btn btn-success" data-tippy-content="Pulse para exportar a Excel."> 
               <i class='fa fa-file-excel fa-lg'></i>
               <span>Exportar</span>
               </button>
            </div>
            <br>`;
        }

        if (inforGrapchi.IsDownloadable && JSON.parse(jsonData.DataSourceComplementary).length > 0) {
            contentButtonExportComplementary = `
            <div>
               <button onclick="RunGraphic.ExportToExcel('dataTableComplementary_${canvasId}', 'xlsx', 'Gráfica - ${inforGrapchi.Title.toUpperCase()}')" class="btn btn-success" data-tippy-content="Pulse para exportar a Excel."> 
               <i class='fa fa-file-excel fa-lg'></i>
               <span>Exportar</span>
               </button>
            </div>
            <br>`;
        }
        var newGraphicHTML = `
        <br>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="card">
                    <div class="card-header text-center">
                        <strong><strong>
                    </div>
                    <div class="card-body">
                        <div class="text-center hiddenscroll" style="height: 450px; width: 100%; overflow-x: scroll;">
                            <canvas id="${canvasId}" style="display: block; box-sizing: border-box; height: 450px; width: 100%;"></canvas>
                        </div>
                    </div>
                </div>
            </div>
        </div><br>
        ${contentButtonExport}
        <div class="row">
            <div style="text-align: center; width: 100%;margin-bottom: 20px;background: #dfd7d7;padding: 7px; display:none;" id="infoTitleIndicator_${canvasId}"> INFORMACIÓN - ${inforGrapchi.Title + ` <br> ${$("#EntityCode option:selected").text()} / Generado: ${getCurrentDate()}`} </div>
            <br>
            <table class="table table-hover table-bordered" id="dataTable_${canvasId}" style="overflow: auto; height:auto; width:100%;">
                <thead>
                </thead>
                <tbody></tbody>
            </table>
        </div>
        <br>
        ${contentButtonExportComplementary}
        <div class="row">
            <div style="text-align: center; width: 100%;margin-bottom: 20px;background: #dfd7d7;padding: 7px; display:none;" id="infoTitleIndicatorComplementary_${canvasId}"> INFORMACIÓN COMPLENTARIA - ${inforGrapchi.Title}   <br> ${$("#EntityCode option:selected").text()} / Generado: ${getCurrentDate()}</div>
            <br>
            <table class="table table-hover table-bordered" id="dataTableComplementary_${canvasId}" style="overflow: auto; height:auto; width:100%;">
                <thead>
                </thead>
                <tbody></tbody>
            </table>
        </div>
         `;

        // Agrega el nuevo bloque de HTML al final del contenedor sin reemplazar el contenido existente
        var container = document.getElementById('container-graphics');
        container.insertAdjacentHTML('beforeend', newGraphicHTML);

        var ctx = document.getElementById(canvasId);
        var chartData = JSON.parse(jsonData.ChartJson);
        chartData.options.plugins.title.text = [`${chartData.options.plugins.title.text}`, `${$("#EntityCode option:selected").text()} (${getCurrentDate()})`];
        RunGraphic.GenerateTableGraphic(canvasId, jsonData)
        RunGraphic.GenerateTableComplementaryGraphic(canvasId, jsonData)
        chartData.options.plugins.tooltip = callbackObject;

        if (chartData.type != 'radar' && chartData.type != 'boxplot') {
            chartData.options.scales.y.ticks = callbackObjectYAxis;
        }

        var myChart = new Chart(ctx, chartData);
    };


    RunGraphic.UnSetGraphic = function (canvasId) {
        let chartStatus = Chart.getChart(canvasId);
        if (chartStatus != undefined) {
            chartStatus.destroy();
        }
    };

    RunGraphic.GetSimbolBySigla = function (graphic, sigla) {
        var graphciTitle = graphic[0];
        var simbol = indicatorsInfo.filter(x => x.GraphicTitle.toLowerCase() == graphciTitle.toLowerCase() && x.DisplayText.toLowerCase() == sigla.toLowerCase())[0];
        if (simbol != undefined) {
            return simbol.Sign;
        }
        else {
            return "";
        }
    }

    RunGraphic.ExportToExcel = function (idTable, type, nameFile) {
        let table = document.getElementById(`${idTable}`);
        TableToExcel.convert(table, {
            name: `${nameFile}_${formatDate(new Date(), 'DD_MM_yyyy')} - ${$('#EntityCode option:selected').text()}.xlsx`,
            sheet: {
                name: `${nameFile}`
            }
        });
    }

    RunGraphic.GenerateTableGraphic = function (canvasId, jsonData) {
        var dataInfoTable = JSON.parse(jsonData.DataSource);

        if (dataInfoTable.length > 0) {

            var table = $(`#dataTable_${canvasId}`);
            $(`#infoTitleIndicator_${canvasId}`).show();
            var thead = table.find("thead");
            var tbody = table.find("tbody");

            // Crear encabezados de columna a partir de las claves del primer objeto
            var columns = Object.keys(dataInfoTable[0]);
            var theadRow = $(`<tr class="text-primary"></tr>`);
            $.each(columns, function (index, column) {
                theadRow.append(`<th style="color:#143d8d;font-weight: 780; text-align: center;">` + column + "</th>");
            });
            thead.append(theadRow);

            // Crear filas de datos
            $.each(dataInfoTable, function (index, rowData) {
                var tr = $(`<tr style="color:#858796;font-weight: 100;height: 20px; border-bottom: 1px solid #e3e6f0; text-align: right;"></tr>`);
                $.each(columns, function (index, column) {
                    if (index != 0) {
                        tr.append(`<td>` + ConvertNullValue(rowData[column]) + "</td>");
                    } else {
                        tr.append(`<td style="text-align: left;">` + ConvertNullValue(rowData[column]) + "</td>");
                    }
                });
                tbody.append(tr);

            });
        }
    };


    RunGraphic.GenerateTableComplementaryGraphic = function (canvasId, jsonData) {
        var dataInfoTable = JSON.parse(jsonData.DataSourceComplementary);

        if (dataInfoTable.length > 0) {
            $(`#infoTitleIndicatorComplementary_${canvasId}`).show();
            var table = $(`#dataTableComplementary_${canvasId}`);
            var thead = table.find("thead");
            var tbody = table.find("tbody");

            // Crear encabezados de columna a partir de las claves del primer objeto
            var columns = Object.keys(dataInfoTable[0]);
            var theadRow = $(`<tr class="text-primary"></tr>`);
            $.each(columns, function (index, column) {
                theadRow.append(`<th style="color:#143d8d;font-weight: 780; text-align: center;">` + column + "</th>");
            });
            thead.append(theadRow);

            // Crear filas de datos
            $.each(dataInfoTable, function (index, rowData) {
                var tr = $(`<tr style="color:#858796;font-weight: 100;height: 20px; border-bottom: 1px solid #e3e6f0; text-align: right;"></tr>`);
                $.each(columns, function (index, column) {

                    if (index != 0) {
                        tr.append(`<td>` + ConvertNullValue(rowData[column]) + "</td>");
                    } else {
                        tr.append(`<td style="text-align: left;">` + ConvertNullValue(rowData[column]) + "</td>");
                    }

                });
                tbody.append(tr);

            });
        }
    };



    return RunGraphic;
})();
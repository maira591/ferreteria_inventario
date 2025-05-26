var Dashboard = (function () {
    function Dashboard() {

    }

    Dashboard.Init = function () {

        $('#cutoffDate').mask('00/00/0000', { placeholder: "__/__/____" });

        var options = { language: 'esp', format: "dd/mm/yyyy" };
        $('.dp-date input').datepicker(options);
               

        if ($("#validateCoop").val() == "S") {
            $("#EntityType").change(function () {
                Dashboard.GetEntities($(this).val());                
            });
        }
        
    };

    Dashboard.Search = function () {

        if ($('#cutoffDate').val() == '') {
            showAlert("Debe ingresar la fecha de corte.");
            return;
        }

        if ($("#validateCoop").val() == "S" && $("#EntityCode").val() == "") {
            showAlert("Debe seleccionar la cooperativa.");
            return;
        }

        ExecuteAjax("DashBoard/GetDashboard", "GET", { cutOffDate: $('#cutoffDate').val(), entityId: $("#EntityCode").val(), entityType: $("#EntityType").val() }, "Dashboard.SetDashboard", null);

    };

    Dashboard.SetDashboard = function (data) {
        if (data != null) {
            $("#div_dashboard").html(data)
        } else {
            $("#div_dashboard").html("");
        }
    };

    Dashboard.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("DashBoard/GetEntityList", "GET", { entityType: entityTypeId }, "Dashboard.LoadEntities", null);
        } else {
            $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };

    Dashboard.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };
       

    Dashboard.RenderGraphics = function (graphics) {
        let positionRadarDataInArray = 0;
        let positionGrossPortfolioLineGraphicDataInArray = 1;
        let depositsLineGraphicDataInArray = 2;

        if (graphics != null) {
            Dashboard.GenerateGraphic("radarGraphic", graphics.Graphics[positionRadarDataInArray])
            Dashboard.GenerateGraphic("grossPortfolioLineGraphic", graphics.Graphics[positionGrossPortfolioLineGraphicDataInArray])
            Dashboard.GenerateGraphic("depositsLineGraphic", graphics.Graphics[depositsLineGraphicDataInArray])
        }
        else {
            Dashboard.UnSetGraphic("radarGraphic");
        }
    };

    Dashboard.GenerateGraphic = function (canvasId, jsonData)
    {
        Dashboard.UnSetGraphic(canvasId);

        var ctx = document.getElementById(canvasId);
        var myChart = new Chart(ctx, JSON.parse(jsonData.ChartJson));
    };

    Dashboard.UnSetGraphic = function (canvasId)
    {
        let chartStatus = Chart.getChart(canvasId);
        if (chartStatus != undefined) {
            chartStatus.destroy();
        } 
    };



    return Dashboard;
})();
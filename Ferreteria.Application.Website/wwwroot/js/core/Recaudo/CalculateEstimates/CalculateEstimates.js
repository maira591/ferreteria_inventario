var baseUrl, baseDirectory

var CalculateEstimates = (function () {

    function CalculateEstimates() {

    }

    CalculateEstimates.Init = function () {
        $('#DateCalculation').mask('0000-00-00-', { placeholder: "____-__-__" });
        $('.select2').select2();

        var options = {
            language: 'esp',
            format: "yyyy-mm-dd",
            startView: 'year',
            minViewMode: 'months',
            beforeShowMonth: function (date) {
                var currentMonth = date.getMonth() + 1; 
                return (currentMonth % 3 === 0);  
            },
            validateOnBlur: false,
            forceParse: false
        };
        $('.dp-date input').datepicker(options);
        
        baseUrl = $('#UrlBase').val();

        $("#EntityType").change(function () {
            CalculateEstimates.GetEntities($(this).val());
        });
        $("#DateCalculation").change(function () {
            $(this).val(ConvertStringToDate($(this).val()));
        });
    };

    CalculateEstimates.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("RecaudoCalculateEstimates/GetEntityList", "GET", { entityType: entityTypeId }, "CalculateEstimates.LoadEntities", null);
        } else {
            $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };

    CalculateEstimates.VerifyValuesFilters = function () {
        if (formValidation("form")) {
            showLoading("Validando...")
            ExecuteAjax("RecaudoCalculateEstimates/ValidateEstimatedCalculation", "GET", {
                entityType: $('#EntityType').val(),
                entityCode: $('#EntityCode').val(),
                dateCalculation: $('#DateCalculation').val()
            }, "CalculateEstimates.ResulValidationEstimatedCalculation", null);
        }
    };

    CalculateEstimates.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };

    CalculateEstimates.ResulValidationEstimatedCalculation = function (data) {
        hideLoading()
        if (data.Message == null) {
            CalculateEstimates.RunEstimatedCalculation();
        } else {
            ShowModalQuestion('Advertencia', data.Message, function () {
                HideModalQuestion()
                CalculateEstimates.RunEstimatedCalculation();
            }, undefined, undefined, 'Calcular');
        }
    };

    CalculateEstimates.RunEstimatedCalculation = function () {

        $.ajax({
            beforeSend: function () { showLoading('Calculando Estimados...'); },
            url: baseUrl + 'RunEstimatedCalculation',
            method: 'POST',
            data: {
                entityType: $('#EntityType').val(),
                entityCode: $('#EntityCode').val(),
                dateCalculation: $('#DateCalculation').val()
            },
            success: function (data) {
                showAlert(`Estimados calculados correctamente`, "success");
            },
            error: function (jqXHR, exception) {
                showAlert("No terminó satisfactoriamente los cálculos.", "warning");
                HideModalQuestion()
            },
            complete: function () {
                hideLoading()
            }
        });
    };


    return CalculateEstimates;
})();
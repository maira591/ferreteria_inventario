var CertificateRequestAdmin = (function () {
    function CertificateRequestAdmin() {

    }
    CertificateRequestAdmin.Init = function () {

        $(".select2").select2();
        $("#EntityType").change(function () {
            $("#container-generate").css("display", "none");
            $("#response-div").html("");
            CertificateRequestAdmin.GetEntities($(this).val());
        });

        $("#EntityCode").change(function () {
            $("#response-div").html("");
        });
    }

    CertificateRequestAdmin.VerifyFilters = function () {
            
        if ($("#EntityType").val() != '' && $("#EntityCode").val() != '') {
            $("#container-generate").show()
        } else {
            $("#container-generate").css("display", "none");
            $("#response-div").html("");
        }
    }
    CertificateRequestAdmin.GenerateCertificate = function () {
        var periodicity = getObject("formCertificate");
        showLoading("Cargando...")
        if (formValidation("formCertificate")) {
            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'GenerateCertificateCoopAdmin/CreateOrUpdate',
                type: 'POST',
                data: periodicity,
                success: function (data) {
                    hideLoading();
                    if (data.Status) {

                        showAlert("Certificado generado exitosamente.", "success")
                        var html = "";
                        html = `Se ha generado el certificado No. ${data.Consecutive} con el código de verificación: <b>${data.ValidateCode}</b>, para descargar su certificado haga 
                        clic en "Descargar certificado"

                        <br><br> 
                        <button onclick="ajaxCallPDF('GenerateCertificateCoopAdmin/GenerateCertificateFile', 'POST',{validateCode:'${data.ValidateCode}'}, 'Certificado de inscripción.pdf')" class="btn btn-success" data-tippy-content="Pulse para descargar el certificado.">
                            <i class='fa fa-file-pdf fa-lg'></i>
                            <span>Descargar certificado</span>
                        </button>
                        <br><br>
                        `;

                        $('#response-div').html(html);
                    }
                    else {
                        showAlert(data.Message);
                    }
                },
                error: function (jqXHR, exception) {
                    hideLoading();
                    showAlert("No fue posible realizar generar el certificado.", 'danger');
                }
            });
        }
    };


    CertificateRequestAdmin.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("GenerateCertificateCoopAdmin/GetEntityList", "GET", { entityType: entityTypeId }, "CertificateRequestAdmin.LoadEntities", null);
        } else {
            $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };

    CertificateRequestAdmin.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };

    return CertificateRequestAdmin;
})();
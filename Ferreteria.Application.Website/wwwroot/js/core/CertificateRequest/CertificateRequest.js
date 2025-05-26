var CertificateRequest = (function () {
    function CertificateRequest() {

    }

    CertificateRequest.GenerateCertificate = function () {
        var periodicity = getObject("formCertificate");
        showLoading("Cargando...")
        if (formValidation("formCertificate")) {
            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'GenerateCertificateCooperative/CreateOrUpdate',
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
                        <button onclick="ajaxCallPDF('GenerateCertificateCooperative/GenerateCertificateFile', 'POST',{validateCode:'${data.ValidateCode}'}, 'Certificado de inscripción.pdf')" class="btn btn-success" data-tippy-content="Pulse para descargar el certificado.">
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

    return CertificateRequest;
})();
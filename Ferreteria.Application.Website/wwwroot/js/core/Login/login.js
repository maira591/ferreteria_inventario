function rememberPassword() {
    showLoading('Cargando...')
    var data = getObject("formRememberPassword");
    ExecuteAjax("RememberPassword", "Post", data, "resultRememberPassword");
}

function resultRememberPassword(response) {
    var urlBase = document.getElementById("baseUrl").value;
    if (response.Status == 500) {
        showAlert(response.Message, undefined, false, 20, 10);
    } else {
        location.href = urlBase + `Login/VerifyToken?UserId=${response.UserId}&TimeToLive=${response.TimeToLive}&UserName=${response.UserName}`;
    }
    hideModalPartial();
}


function verifyToken() {
    showLoading('Verificando...')
    var data = getObject("formVerifyToken");
    ExecuteAjax("VerifyTokenn", "Post", data, "resultVerifyToken");
}
function initValidations() {
    setNumericInput()
}

function updateInfoUser() {
    var userInformation = getObject("formUpdateInfoUser");
    if (formValidation("formUpdateInfoUser") && validateEmail($("#Email").val())) {
        ExecuteAjax("UpdateInfoUser", "Post", userInformation, "resultUpdateInfoUser");
    }
}
function resultUpdateInfoUser(response) {
    if (response.Status == 500) {
        showAlert(response.Message, 'danger', false, 20, 10);
    } else {
        showAlert(response.Message, 'success', false, 20, 10);
        showLoading("Aplicando cambios...")
        setTimeout(function () {
            location.reload();
        }, 2000);
    }
}

function validateEmail(email) {
    var expresion = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (expresion.test(email)) {
        $("#Email").removeClass("errorValidate");
        $("#Email").css("border", "none");
        return true;
    } else {
        $("#Email").addClass("errorValidate");
        $("#Email").css("border", "1px #dd3f3f solid");
        showAlert("El correo ingresado deber ser un correo válido","warning")
        return false;
    }

    showTooltip();
}

function resultVerifyToken(response) {
    var urlBase = document.getElementById("baseUrl").value;
    if (response.Status == 500) {
        showAlert(response.Message, 'danger', false, 20, 10);
    } else {
        location.href = urlBase + 'Login/PasswordChange';
    }
    hideModalPartial();
}

async function AJAXSubmit(oFormElement) {
    var urlBase = document.getElementById("baseUrl").value;
    showLoading('Cargando...')
    const formData = new FormData(oFormElement);
    try {
        const response = await fetch(oFormElement.action, {
            method: 'POST',
            body: formData
        })
            .then(function (res) { return res.json(); })
            .then(function (data) {

                if (data.Status != 200) {
                    showAlert(data.Message, undefined, false, 20, 10);
                } else {
                    checkLoginResult(data);
                }
            });
    } catch (error) {
        hideLoading();
        console.error(error);
    }
}

function checkLoginResult(data) {
    if (data.Status !== 200) {
        showAlert(data.Message, "danger", true);
        var captcha = $('#hfIsEnableCaptcha').val();
        if (captcha === 'true') { grecaptcha.reset(); } return;
    }

    if (!data.Body.AcceptedHabeasData || !data.Body.AcceptedTermsAndConditions) {
        mostrarTerminosYCondiciones();
    }
    else {
        establecerLogin();
    }
}

/** Muestra la ventana para aceptar terminos y condiciones */
function mostrarTerminosYCondiciones() {
    var urlTerminos = $('#hfTerminosUrl').val();
    var urlHabeasData = $('#hfHabeasDataUrl').val();
    var captcha = $('#hfIsEnableCaptcha').val();

    hideLoading();
    $('#btnSiModalQuestion').html('Acepto');
    $('#btnNoModalQuestion').html('Cancelar');
    $("#btnNoModalQuestion").on("click", function () {
        HideModalQuestion();
    });
    ShowModalQuestion('Fogacoop SIDCORE',
        `<p>Se deben aceptar los términos y condiciones.</p>
        <div class="form-group">
            <div class="form-check">
                <input type="checkbox" class="form-check-input" id="chkTerminosyCondiciones">
                <label class="form-check-label" for="chkTerminosyCondiciones">
					Consiento y autorizo de manera inequívoca que mis datos sean tratados conforme a lo previsto en la política de tratamiento de datos personales de Fogacoop (<a href="${urlTerminos}" target="_blank">Ver política de tratamiento de datos personales</a>)
				</label>
            </div>
			<br/>
            <div class="form-check">
                <input type="checkbox" class="form-check-input" id="chkHabeasData">
                <label class="form-check-label" for="chkHabeasData">
					He leído y estoy de acuerdo con las políticas de privacidad y condiciones de uso establecidas por el Fogacoop para este (<a href="${urlHabeasData}" target="_blank">Ver políticas de privacidad y condiciones de uso</a>)
				</label>
            </div>
        </div>`,
        validarTerminosYCondiciones, function () {
            $('#frm-login').trigger('reset');
            if (captcha === 'true') {
                grecaptcha.reset();
            }
            HideModalQuestion();
        });
}
/**Funcion que verifica que se acepten los terminos y condiciones */
function validarTerminosYCondiciones() {
    if (!$('#chkTerminosyCondiciones').is(':checked') || !$('#chkHabeasData').is(':checked')) {
        showAlert('Se deben aceptar los <b>Términos y Condiciones</b> y <b>Habeas Data</b> para continuar.', 'warning');
    }
    else {
        aceptarTerminosYCondiciones();
    }
}

/** Establece el usuario logeado */
function aceptarTerminosYCondiciones() {
    HideModalQuestion().then(function () {
        showLoading('Actualizando...');
        $.ajax({
            url: $('#baseUrl').val() + 'Login/AgreeTerms',
            method: 'POST',
            global: false,
            success: function (data) {
                if (data.Status === 200) {
                    establecerLogin();
                } else {
                    hideLoading();
                    showAlert(data.Message, 'danger', true);
                }
            },
            error: function () { showAlert("Ocurrió un error en el proceso.", 'danger', true); }
        });
    });
}

function establecerLogin() {
    showLoading('Estableciendo sesión.');
    $.ajax({
        url: $('#baseUrl').val() + 'Login/SetUSer',
        method: 'POST',
        global: false,
        success: function (data) {
            if (data.Status === 200) {
                showLoading(data.Message);
                window.location.href = $('#baseUrl').val() + 'Home/Index';
            } else {
                hideLoading();
                showAlert(data.Message, 'danger', true);
            }
        },
        error: function () { showAlert("Ocurrió un error en el proceso.", 'danger', true); }
    });
}